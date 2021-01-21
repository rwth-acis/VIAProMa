using ExitGames.Client.Photon;
using HoloToolkit.Unity;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

namespace i5.VIAProMa.Multiplayer.Poll
{
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
            yield return new WaitForSeconds(seconds);
            PollHandler.Instance.EndPoll();
            yield return new WaitForSeconds(1);
            PollHandler.Instance.DisplayPoll();
        }

        public void StopCountdown() 
        {
            StopCoroutine(currentCountdown);
        }

        public void StartPoll(string question, string[] answers, PollOptions flags, DateTime end)
        {
            currentPoll = new Poll(question, answers, flags);
            // Send out poll message
            int syncedEndTime = PhotonNetwork.ServerTimestamp + (end - DateTime.Now).Milliseconds;
            photonView.RPC("PollStartedReceived", RpcTarget.All, question, answers, syncedEndTime, flags);
            // Setup timer on host
            if (flags.HasFlag(PollOptions.Countdown))
            {
                Debug.Log("Starting Countdown!");
                TimeSpan timeToGo = end - DateTime.Now;
                if (timeToGo > TimeSpan.Zero)
                {
                    Debug.Log("Is Valid! Time to go: " + timeToGo.Seconds);
                    currentCountdown = Countdown(timeToGo.Seconds);
                    StartCoroutine(currentCountdown);
                }
            }
        }

        public void RespondPoll(bool[] selection, Player leader)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {TargetActors = new int[]{leader.ActorNumber}};
            PhotonNetwork.RaiseEvent(PollRespondEventCode, selection, raiseEventOptions, SendOptions.SendReliable);
        }

        public void EndPoll()
        {
            photonView.RPC("PollEndReceived", RpcTarget.All);
        }

        public void DisplayPoll(short id, PollDisplayEventArgs.DisplayType type)
        {
            photonView.RPC("PollDisplayReceived", RpcTarget.All, id, type);
        }

        public void DisplayPoll()
        {
            if (currentPoll.SerializeableSelection.Count == 0)
            {
                Debug.Log("No participants, discarding poll!");
            }
            // TODO: Send to database and continue with ID
            // DisplayPoll(id, DisplayType.Bar);
            photonView.RPC("PollDisplayReceived", RpcTarget.All, currentPoll.Answers, currentPoll.AccumulatedResults, PollDisplayEventArgs.DisplayType.Bar);
        }
        
        public void SendNAK(Player leader)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {TargetActors = new int[]{leader.ActorNumber}};
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
                DisplayPoll();
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
            PollBarVisiualization pollViz = barChartObj.GetComponent<PollBarVisiualization>();
            pollViz.Setup(answers, results);
        }

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