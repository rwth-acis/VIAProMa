﻿using ExitGames.Client.Photon;
using HoloToolkit.Unity;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;
using System.Collections;
using i5.VIAProMa.Visualizations.Poll;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using System.Linq;
using i5.VIAProMa.ResourceManagagement;
using i5.VIAProMa.Utilities;

namespace i5.VIAProMa.Multiplayer.Poll
{
    /**
     * Handles poll networking, poll host state management and local events for UI
     */
    [RequireComponent(typeof(PhotonView))]
    public class PollHandler : Singleton<PollHandler>, IOnEventCallback, IInRoomCallbacks, IMatchmakingCallbacks
    {
        private PhotonView photonView;
        public event EventHandler<PollStartEventArgs> PollStarted;
        public event EventHandler<PollEndEventArgs> PollEnded;
        public event EventHandler<Player> PollDiscardByPlayer;
        public event EventHandler<int> PollToDisplayRecieved;
        public const byte PollRespondEventCode = 2;
        public const byte PollAcknowledgedEventCode = 3;
        public const byte PollSavedPollSyncEventCode = 4;

        [Header("Poll Visualizations")]
        [SerializeField] private GameObject barChartVisualizationPrefab;
        [SerializeField] private GameObject pollShelfPrefab;
        [SerializeField] private float barDistance;
        [SerializeField] private float shelfDistance;

        private Poll currentPoll;
        private IEnumerator currentCountdown;
        private List<int> pollsToDisplay;

        [Header("Project Poll Serialization")]
        [SerializeField] private GameObject pollSerializerPrefab;
        public List<SerializablePoll> savedPolls;
        private List<SerializablePoll> tempSavedPolls;

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }
        
        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        protected override void Awake()
        {
            base.Awake();
            if (barChartVisualizationPrefab == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(barChartVisualizationPrefab));
            }
            if (pollShelfPrefab == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(pollShelfPrefab));
            }
            if (pollSerializerPrefab == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(pollSerializerPrefab));
            }

            photonView = GetComponent<PhotonView>();
            if (!PhotonPeer.RegisterType(typeof(SerializablePoll), SerializablePoll.SerializeablePollCode, SerializablePoll.Serialize, SerializablePoll.Deserialize))
            {
                Debug.LogError("couldn't register SerializeablePoll");
            }
            savedPolls = new List<SerializablePoll>();
            tempSavedPolls = new List<SerializablePoll>();
            pollsToDisplay = new List<int>();
			Instantiate(pollSerializerPrefab);
        }

        private IEnumerator Countdown(int seconds) 
        {
            yield return new WaitForSecondsRealtime(seconds);
            EndPoll();
            yield return new WaitForSecondsRealtime(1);
            FinalizePoll();
        }

        private void StopCountdown() 
        {
             if (!(currentCountdown is null))
                StopCoroutine(currentCountdown);
        }

        public void PollShelfDisplay()
        {
            GameObject pollShelf = Instantiate(pollShelfPrefab);

			Vector3 position = CameraCache.Main.transform.position;
            position.y = 0f;
            position += shelfDistance * new Vector3(CameraCache.Main.transform.forward.x, 0, CameraCache.Main.transform.forward.z).normalized;
            pollShelf.transform.position = position;
        }

        private void GenerateSynchronizedPollDisplay(int pollIndex)
        {
            Vector3 position = CameraCache.Main.transform.position;
            position.y = 0.5f;
            position += barDistance * new Vector3(CameraCache.Main.transform.forward.x, 0, CameraCache.Main.transform.forward.z).normalized;
            GameObject barChartObj = ResourceManager.Instance.NetworkInstantiate(barChartVisualizationPrefab, position, new Quaternion(0,0,0,0));

            barChartObj.GetComponent<PollBarVisualization>()?.SetupPoll(pollIndex);
        }

        /**
         * Starts a poll with this player as host
         */
        public void StartPoll(string question, string[] answers, PollOptions flags, DateTime end)
        {
            currentPoll = new Poll(question, answers, flags);
            // Send out poll message
            int syncedEndTime = PhotonNetwork.ServerTimestamp + (int)(end - DateTime.Now).TotalMilliseconds;
            photonView.RPC("PollStartedReceived", RpcTarget.All, question, answers, syncedEndTime, flags);
            // Setup timer on host
            if (flags.HasFlag(PollOptions.Countdown))
            {
                Debug.Log("Starting Countdown!");
                TimeSpan timeToGo = end - DateTime.Now;
                if (timeToGo > TimeSpan.Zero)
                {
                    Debug.Log("Is Valid! Time to go: " + (int)timeToGo.TotalSeconds);
                    currentCountdown = Countdown((int)timeToGo.TotalSeconds);
                    StartCoroutine(currentCountdown);
                }
            }
        }

        /**
         * Responds to poll as user with specified poll host
         */
        public void RespondPoll(bool[] selection, Player host)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {TargetActors = new int[]{host.ActorNumber}};
            PhotonNetwork.RaiseEvent(PollRespondEventCode, selection, raiseEventOptions, SendOptions.SendReliable);
        }

        /**
         * Force Poll End by poll host
         */
        public void EndPoll()
        {
            if (currentPoll == null) return;
            if (currentPoll.IsEnded) return;
            currentPoll.IsEnded = true;
            photonView.RPC("PollEndReceived", RpcTarget.All);
        }

        /**
         * Finalize ended poll, save and display if desired
         */
        public void FinalizePoll()
        {
            if (currentPoll.IsFinalized) return;
            currentPoll.IsFinalized = true;
            if (currentPoll.SerializeableSelection.Count == 0)
            {
                Debug.Log("No participants, discarding poll!");
                currentPoll = null;
            }
            else
            {
                SerializablePoll poll = SerializablePoll.FromPoll(currentPoll);
                photonView.RPC("PollSaveRequestReceived", RpcTarget.MasterClient, poll, currentPoll.Flags.HasFlag(PollOptions.SaveResults));
                // if (currentPoll.Flags.HasFlag(PollOptions.SaveResults))
                // {
                //     Debug.Log("Sending poll save request!");
                //     SerializablePoll poll = SerializablePoll.FromPoll(currentPoll);
                //     photonView.RPC("PollSaveRequestReceived", RpcTarget.MasterClient, poll);
                //     // This will later broadcast PollDisplayStoredReceived 
                // }
                // else
                // {
                //     Debug.Log("Sending poll display immediate!");
                //     photonView.RPC("PollDisplayImmediateReceived", RpcTarget.All, SerializablePoll.FromPoll(currentPoll));
                //     currentPoll = null;
                // }
            }
        }

        /**
         * Unregister this client from the current poll hosted by host 
         */
        public void SendStatus(bool ack, Player host)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {TargetActors = new int[]{host.ActorNumber}};
            PhotonNetwork.RaiseEvent(PollAcknowledgedEventCode, ack, raiseEventOptions, SendOptions.SendReliable);
        }

        public void DisplayPollAtIndex(int index)
        {
            if (index >= savedPolls.Count)
            {
                Debug.LogWarning("tried to display poll out of bounds");
                return;
            }
            GenerateSynchronizedPollDisplay(index);
        }

        public SerializablePoll GetPollAtIndex(int index)
        {
            if (index >= 0)
            {
                if (index < savedPolls.Count)
                {
                    return savedPolls[index];
                }
                else
                {
                    if (!pollsToDisplay.Contains(index)) 
                        pollsToDisplay.Add(index);
                    return null;
                }
            }
            else
            {
                int tIndex = -index-1;
                if (tIndex < tempSavedPolls.Count)
                {
                    return tempSavedPolls[tIndex];
                }
                else
                {
                    if (!pollsToDisplay.Contains(index)) 
                        pollsToDisplay.Add(index);
                    return null;
                }
            }
        }

        [PunRPC]
        private void PollStartedReceived(string question, string[] answers, int syncedEndTime, PollOptions flags, PhotonMessageInfo messageInfo)
        {
            int milliseconds = syncedEndTime < PhotonNetwork.ServerTimestamp? (int.MaxValue-PhotonNetwork.ServerTimestamp + syncedEndTime) : (syncedEndTime-PhotonNetwork.ServerTimestamp);
            if (PollStarted != null)
            {
                PollStartEventArgs args = new PollStartEventArgs(question, answers, DateTime.Now.AddMilliseconds(milliseconds), messageInfo.Sender, flags);
                PollStarted?.Invoke(this, args);
            }
        }

        [PunRPC]
        private void PollEndReceived(PhotonMessageInfo messageInfo)
        {
            Debug.Log("Poll end requested!!");
            PollEndEventArgs args = new PollEndEventArgs(messageInfo.Sender);
            PollEnded?.Invoke(this,args);
        }
        
        [PunRPC]
        private void PollSaveRequestReceived(SerializablePoll poll, bool save, PhotonMessageInfo messageInfo)
        {
            int index = 0;
            Debug.Log("Poll save request received!");
            if (save)
            {
                index = savedPolls.Count;
                savedPolls.Add(poll);
            }
            else
            {
                index = -1-tempSavedPolls.Count;
                tempSavedPolls.Add(poll);
            }
            photonView.RPC("PollUpdateReceived", RpcTarget.All, index, poll);
        }
        
        [PunRPC]
        private void PollUpdateReceived(int index, SerializablePoll poll, PhotonMessageInfo messageInfo)
        {
            Debug.Log("Poll update received!");
            if(index < 0)
            {
                int tIndex = -index -1;
                if (tempSavedPolls.Count <= tIndex)
                {
                    if(tIndex >= tempSavedPolls.Capacity)
                        tempSavedPolls.Capacity = tIndex+1;
                    tempSavedPolls.AddRange(Enumerable.Repeat<SerializablePoll>(null, tIndex-tempSavedPolls.Count+1));
                }
                tempSavedPolls[tIndex] = poll;
                
            }
            else
            {
                if (savedPolls.Count <= index)
                { // Make sure we can access index
                    if (index >= savedPolls.Capacity)
                        savedPolls.Capacity = index+1;
                    savedPolls.AddRange(Enumerable.Repeat<SerializablePoll>(null, index-savedPolls.Count+1));
                }
                savedPolls[index] = poll;
            }
            if (currentPoll != null && currentPoll.Question == poll.Question && currentPoll.IsFinalized) // should be enough
            {
                Debug.Log("Poll display stored send!");
                currentPoll = null;
                GenerateSynchronizedPollDisplay(index);        
            }

            if (pollsToDisplay.Contains(index))
            {
                PollToDisplayRecieved?.Invoke(this, index);
                pollsToDisplay.Remove(index);
            }
        }

        /**
         * Handles events from user to poll host
         */
        public void OnEvent(EventData photonEvent)
        {
            bool? finished = false;
            switch (photonEvent.Code)
            {
                case PollRespondEventCode:
                    finished = currentPoll?.OnResponse(PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender), (bool[])photonEvent.CustomData);
                    break;
                case PollAcknowledgedEventCode:
                    finished = currentPoll?.OnStatus(PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender), (bool)photonEvent.CustomData);
                    break;
                case PollSavedPollSyncEventCode:
                    object[] data = (object[])photonEvent.CustomData;
                    savedPolls = ((SerializablePoll[])data[0]).ToList();
                    tempSavedPolls = ((SerializablePoll[])data[1]).ToList();
                    break;
            }
            if (finished == true)
            {
                StopCountdown();
                EndPoll();
                FinalizePoll();
            }
        }

        /**
         * Implementations for IInRoomCallbacks and IMatchmakingCallbacks
         */

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient && newPlayer.UserId != PhotonNetwork.LocalPlayer.UserId)
            {
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions {TargetActors = new int[]{newPlayer.ActorNumber}};
                PhotonNetwork.RaiseEvent(PollSavedPollSyncEventCode, new object[] {savedPolls.ToArray(), tempSavedPolls.ToArray()}, raiseEventOptions, SendOptions.SendReliable);
            }
        }
        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            currentPoll?.OnStatus(otherPlayer, false);
			PollDiscardByPlayer?.Invoke(this, otherPlayer);
        }
        public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps){}
        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged){}
        public void OnMasterClientSwitched (Player newMasterClient){}
        public void OnFriendListUpdate(List<FriendInfo> friendList){}
        public void OnCreatedRoom(){}
        public void OnCreateRoomFailed(short returnCode, string message){}
        public void OnJoinedRoom()
        {
            savedPolls = new List<SerializablePoll>();
            pollsToDisplay = new List<int>();
        }
        public void OnJoinRoomFailed(short returnCode, string message){}
        public void OnJoinRandomFailed(short returnCode, string message){}
        public void OnLeftRoom()
		{
			if (currentPoll != null)
				PollDiscardByPlayer?.Invoke(this, PhotonNetwork.LocalPlayer);
		}
    }
}