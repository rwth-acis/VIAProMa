using ExitGames.Client.Photon;
using HoloToolkit.Unity;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Poll{
    [RequireComponent(typeof(PhotonView))]
    public class PollHandler : Singleton<PollHandler>, IOnEventCallback, IInRoomCallbacks
    {
        private PhotonView photonView;
        public event EventHandler<PollStartEventArgs> PollStarted;
        public event EventHandler<PollRespondEventArgs> PollRespond;
        public event EventHandler<PollEndEventArgs> PollEnded;
        public event EventHandler<PollDisplayEventArgs> PollDisplayed;
        public event EventHandler<PollAcknowledgedEventArgs> PollAcknowledged;
        public const byte PollRespondEventCode = 2;
        public const byte PollAcknowledgedEventCode = 3;

        private void OnEnable(){
            PhotonNetwork.AddCallbackTarget(this);
        }
        
        private void OnDisable(){
            PhotonNetwork.RemoveCallbackTarget(this);
        }
        protected override void Awake()
        {
            base.Awake();
            photonView = GetComponent<PhotonView>();
        }

        public byte StartPoll(string question, string[] answers, DateTime end, PollOptions flags){
			int syncedEndTime = PhotonNetwork.ServerTimestamp + (end - DateTime.Now).Milliseconds;
            photonView.RPC("PollStartedReceived", RpcTarget.All, question, answers, syncedEndTime, flags);
            return PhotonNetwork.CurrentRoom.PlayerCount;
        }

        public void RespondPoll(bool[] selection, Player leader){
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {TargetActors = new int[]{leader.ActorNumber}};
            PhotonNetwork.RaiseEvent(PollRespondEventCode, selection, raiseEventOptions, SendOptions.SendReliable);
        }

        public void EndPoll(){
            photonView.RPC("PollEndReceived", RpcTarget.All);
        }

        public void DisplayPoll(short id, PollDisplayEventArgs.DisplayType type){
            photonView.RPC("PollDisplayReceived", RpcTarget.All, id, type);
        }
        
        public void SendNAK(Player leader){
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {TargetActors = new int[]{leader.ActorNumber}};
            PhotonNetwork.RaiseEvent(PollAcknowledgedEventCode, false, raiseEventOptions, SendOptions.SendReliable);
        }

        [PunRPC]
        private void PollStartedReceived(string question, string[] answers, int syncedEndTime, PollOptions flags, PhotonMessageInfo messageInfo){
            bool state = false;
			int milliseconds = syncedEndTime < PhotonNetwork.ServerTimestamp? (int.MaxValue-PhotonNetwork.ServerTimestamp + syncedEndTime) : (syncedEndTime-PhotonNetwork.ServerTimestamp);
            if(PollStarted != null){
                PollStartEventArgs  args = new PollStartEventArgs(question, answers, DateTime.Now.AddMilliseconds(milliseconds), messageInfo.Sender, flags);
                PollStarted?.Invoke(this, args);
                state = true;
            }
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {TargetActors = new int[]{messageInfo.Sender.ActorNumber}};
            PhotonNetwork.RaiseEvent(PollAcknowledgedEventCode, state, raiseEventOptions, SendOptions.SendReliable);
        }

        [PunRPC]
        private void PollEndReceived(PhotonMessageInfo messageInfo){
            PollEndEventArgs args = new PollEndEventArgs(messageInfo.Sender);
            PollEnded?.Invoke(this,args);
        }

        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case PollRespondEventCode:
                    bool[] selection = (bool[])photonEvent.CustomData;
                    PollRespondEventArgs argsResp = new PollRespondEventArgs(selection, PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender));
                    PollRespond?.Invoke(this, argsResp);
                    break;
                case PollAcknowledgedEventCode:
                    bool state = (bool)photonEvent.CustomData;
                    PollAcknowledgedEventArgs argsAck = new PollAcknowledgedEventArgs(state, PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender));
                    PollAcknowledged?.Invoke(this, argsAck);
                    break;
            }
        }

        [PunRPC]
        private void PollDisplayReceived(short id, PollDisplayEventArgs.DisplayType type, PhotonMessageInfo messageInfo){
            PollDisplayEventArgs args = new PollDisplayEventArgs(id, type);
            PollDisplayed?.Invoke(this,args);

        }

        public void OnPlayerEnteredRoom(Player newPlayer){}
        public void OnPlayerLeftRoom(Player otherPlayer){
            bool[] selection = new bool[0];
            PollRespondEventArgs args = new PollRespondEventArgs(selection, otherPlayer); //ignoring case of user joining after poll start and leaving before poll end
            PollRespond?.Invoke(this,args);
        }
        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps){}
        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged){}
        public void OnMasterClientSwitched (Player newMasterClient){}



    }
}