using ExitGames.Client.Photon;
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
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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
        public const byte PollRespondEventCode = 2;
        public const byte PollAcknowledgedEventCode = 3;
        public const byte PollSyncResponseEventCode = 4;

        public GameObject barChartVisualizationPrefab;
        public GameObject PollSerializerPrefab;

        private Poll currentPoll;
        private IEnumerator currentCountdown;

        // distance of the position of the Camera and the created visualization
        [SerializeField] private float barDistance; 
        public List<SerializeablePoll> SavedPolls 
        {
            get 
            {
                syncSavedPolls();
                return savedPolls;
            }
            set 
            {
                savedPolls = value;
            }
        }
        private List<SerializeablePoll> savedPolls;
        private bool synced;
        private byte[] savedHash;

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
            photonView = GetComponent<PhotonView>();
            if(!PhotonPeer.RegisterType(typeof(SerializeablePoll), SerializeablePoll.SerializeablePollCode, SerializeablePoll.Serialize, SerializeablePoll.Deserialize))
            {
                Debug.LogError("couldn't register SerializeablePoll");
            }
            savedPolls = new List<SerializeablePoll>();
            GameObject pollSerializer = Instantiate(PollSerializerPrefab);
        }

        private IEnumerator Countdown(int seconds) 
        {
            yield return new WaitForSecondsRealtime(seconds);
            EndPoll();
            yield return new WaitForSecondsRealtime(1);
            DisplayPoll();
            currentPoll = null;
        }

        private void StopCountdown() 
        {
             if(!(currentCountdown is null))
                StopCoroutine(currentCountdown);
        }
        private byte[] hashSavedPolls(){
            var binaryFormatter = new BinaryFormatter();
            using (var md5 = MD5.Create())
            using (var stream = new MemoryStream())
            {
                binaryFormatter.Serialize(stream, savedPolls);
                return md5.ComputeHash(stream);
            }
        }

        private void PollDisplay(SerializeablePoll poll, PhotonMessageInfo messageInfo)
        {
            var results = poll.AccumulatedResult;
            var answers = poll.Answers;

            GameObject barChartObj = Instantiate(barChartVisualizationPrefab);

            Vector3 position = CameraCache.Main.transform.position;
            position.y = 0.5f;
            position += barDistance * new Vector3(CameraCache.Main.transform.forward.x, 0, CameraCache.Main.transform.forward.z).normalized;
            barChartObj.transform.position = position;
            PollBarVisualization pollViz = barChartObj.GetComponent<PollBarVisualization>();
            pollViz.Setup(answers, results);
        }

        private async void syncSavedPolls()
        {
            if(PhotonNetwork.IsMasterClient)
            {
                return;
            }
            synced = false;
            // send sync request
            while(!synced)
            {
                await Task.Delay(25);
            }
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
         * Initiate Poll Display of the currently hosted poll on all clients
         */
        public void DisplayPoll()
        {
            if (currentPoll == null) return;
            if (currentPoll.IsDisplayed) return;
            currentPoll.IsDisplayed = true;
            if (currentPoll.SerializeableSelection.Count == 0)
            {
                Debug.Log("No participants, discarding poll!");
            }
            SerializeablePoll poll = SerializeablePoll.FromPoll(currentPoll);
            photonView.RPC("PollDisplayReceived", RpcTarget.All, poll);
        }

        public void DisplayPoll(int i)
        {
            photonView.RPC("PollDisplayByIndexReceived", RpcTarget.All, i);
        }

        /**
         * Unregister this client from the current poll hosted by host 
         */
        public void SendNAK(Player host)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {TargetActors = new int[]{host.ActorNumber}};
            PhotonNetwork.RaiseEvent(PollAcknowledgedEventCode, false, raiseEventOptions, SendOptions.SendReliable);
        }

        [PunRPC]
        private void PollSyncRequestReceived(byte[] hash, PhotonMessageInfo messageInfo)
        {
            object[] responseObject;
            if(savedHash.SequenceEqual(hash))
            {
                responseObject = new object[] {true};
                
            }else
            {
                responseObject = new object[] {false, savedPolls.ToArray()};
            }
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {TargetActors = new int[]{messageInfo.Sender.ActorNumber}};
            PhotonNetwork.RaiseEvent(PollSyncResponseEventCode, responseObject, raiseEventOptions, SendOptions.SendReliable);
        }

        [PunRPC]
        private void PollStartedReceived(string question, string[] answers, int syncedEndTime, PollOptions flags, PhotonMessageInfo messageInfo)
        {
            bool state = false;
            int milliseconds = syncedEndTime < PhotonNetwork.ServerTimestamp? (int.MaxValue-PhotonNetwork.ServerTimestamp + syncedEndTime) : (syncedEndTime-PhotonNetwork.ServerTimestamp);
            if (PollStarted != null)
            {
                PollStartEventArgs  args = new PollStartEventArgs(question, answers, DateTime.Now.AddMilliseconds(milliseconds), messageInfo.Sender, flags);
                PollStarted?.Invoke(this, args);
                state = true;
            }
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {TargetActors = new int[]{messageInfo.Sender.ActorNumber}};
            PhotonNetwork.RaiseEvent(PollAcknowledgedEventCode, state, raiseEventOptions, SendOptions.SendReliable);
        }

        [PunRPC]
        private void PollEndReceived(PhotonMessageInfo messageInfo)
        {
            Debug.Log("Poll end requested!!");
            PollEndEventArgs args = new PollEndEventArgs(messageInfo.Sender);
            PollEnded?.Invoke(this,args);
        }

        [PunRPC]
        private void PollDisplayReceived(SerializeablePoll poll, PhotonMessageInfo messageInfo)
        {   
            if(poll.Flags.HasFlag(PollOptions.SaveResults))
            {
                SavedPolls.Add(poll);
                if(PhotonNetwork.IsMasterClient)
                {
                    savedHash = hashSavedPolls();
                }
            }
            PollDisplay(poll, messageInfo);
        }

        [PunRPC]
        private void PollDisplayByIndexReceived(int i, PhotonMessageInfo messageInfo)
        {
            SerializeablePoll poll = SavedPolls[i];
            PollDisplay(poll, messageInfo);
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
                case PollSyncResponseEventCode:
                    if(!synced)
                    {
                        if(!((bool)((object[]) photonEvent.CustomData)[0]))
                        {
                            savedPolls = ((SerializeablePoll[])((object[]) photonEvent.CustomData)[1]).ToList();
                        }
                        synced = true;
                    }
                    break;
            }
            if (finished == true)
            {
                StopCountdown();
                EndPoll();
                DisplayPoll();
                currentPoll = null;
            }
        }

        /**
         * Implementations for IInRoomCallbacks
         */

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            if(PhotonNetwork.IsMasterClient && newPlayer.UserId != PhotonNetwork.LocalPlayer.UserId)
            {
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions {TargetActors = new int[]{newPlayer.ActorNumber}};
                PhotonNetwork.RaiseEvent(PollSyncResponseEventCode, new object[] {false, savedPolls.ToArray()}, raiseEventOptions, SendOptions.SendReliable);
            }
        }
        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            currentPoll?.OnStatus(otherPlayer, false);
        }
        public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps){}
        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged){}
        public void OnMasterClientSwitched (Player newMasterClient){}
        public void OnFriendListUpdate(List<FriendInfo> friendList){}
        public void OnCreatedRoom(){}
        public void OnCreateRoomFailed(short returnCode, string message){}
        public void OnJoinedRoom()
        {
            savedPolls = new List<SerializeablePoll>();
        }
        public void OnJoinRoomFailed(short returnCode, string message){}
        public void OnJoinRandomFailed(short returnCode, string message){}
        public void OnLeftRoom()
        {}
    }
}