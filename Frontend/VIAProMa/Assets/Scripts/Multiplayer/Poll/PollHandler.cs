using ExitGames.Client.Photon;
using HoloToolkit.Unity;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using i5.VIAProMa.Visualizations.Poll;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace i5.VIAProMa.Multiplayer.Poll
{
    /**
     * Handles poll networking, poll host state management and local events for UI
     */
    [RequireComponent(typeof(PhotonView))]
    public class PollHandler : Singleton<PollHandler>, IOnEventCallback, IInRoomCallbacks
    {
        private PhotonView photonView;
        public event EventHandler<PollStartEventArgs> PollStarted;
        public event EventHandler<PollEndEventArgs> PollEnded;
        public event EventHandler<PollDisplayEventArgs> PollDisplayed;
        public const byte PollRespondEventCode = 2;
        public const byte PollAcknowledgedEventCode = 3;

        public GameObject barChartVisualizationPrefab;

        private Poll currentPoll;
        private IEnumerator currentCountdown;

        // distance of the position of the Camera and the created visualization
        [SerializeField] private float barDistance; 

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
            // TODO: Send to database and continue with ID
            // photonView.RPC("PollDisplayReceived", RpcTarget.All, id, DisplayType.Bar);
            photonView.RPC("PollDisplayReceived", RpcTarget.All, currentPoll.Answers, currentPoll.AccumulatedResults, PollDisplayEventArgs.DisplayType.Bar);
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
            }
            if (finished == true)
            {
                StopCountdown();
                EndPoll();
                DisplayPoll();
                currentPoll = null;
            }
        }

/*        [PunRPC]
        private void PollDisplayReceived(short id, PollDisplayEventArgs.DisplayType type, PhotonMessageInfo messageInfo){
            PollDisplayEventArgs args = new PollDisplayEventArgs(id, type);
            PollDisplayed?.Invoke(this,args);

            // TODO: Display properly, load from database
        }*/

        [PunRPC]
        private void PollDisplayReceived(string[] answers, int[] results, PollDisplayEventArgs.DisplayType type, PhotonMessageInfo messageInfo)
        {
            PollDisplayEventArgs args = new PollDisplayEventArgs(results, type);
            PollDisplayed?.Invoke(this,args);

            GameObject barChartObj = Instantiate(barChartVisualizationPrefab);

            Vector3 position = CameraCache.Main.transform.position;
            position.y = 0.5f;
            position += barDistance * new Vector3(CameraCache.Main.transform.forward.x, 0, CameraCache.Main.transform.forward.z).normalized;
            barChartObj.transform.position = position;
            PollBarVisualization pollViz = barChartObj.GetComponent<PollBarVisualization>();
            pollViz.Setup(answers, results);
        }

        /**
         * Implementations for IInRoomCallbacks
         */

        public void OnPlayerEnteredRoom(Player newPlayer){}
        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            currentPoll?.OnStatus(otherPlayer, false);
        }
        public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps){}
        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged){}
        public void OnMasterClientSwitched (Player newMasterClient){}
    }
}