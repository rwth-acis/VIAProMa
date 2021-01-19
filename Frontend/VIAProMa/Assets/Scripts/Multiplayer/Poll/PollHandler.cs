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
        
        public const byte PollRespondEventCode = 2;

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

        public async Task<byte> StartPoll(string question, string[] answers, DateTime end, PollOptions flags){
            photonView.RPC("PollStartedReceived", RpcTarget.All, question, answers, end, flags);
            return PhotonNetwork.CurrentRoom.PlayerCount;
        }

        public async void RespondPoll(bool[] selection, Player leader){
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {TargetActors = new int[]{leader.ActorNumber}};
            PhotonNetwork.RaiseEvent(PollRespondEventCode, selection, raiseEventOptions, SendOptions.SendReliable);
        }

        public async void EndPoll(){
            photonView.RPC("PollEndReceived", RpcTarget.All);
        }

        public async void DisplayPoll(short id, PollDisplayEventArgs.DisplayType type){
            photonView.RPC("PollDisplayReceived", RpcTarget.All, id, type);
        }

        [PunRPC]
        private async void PollStartedReceived(string question, string[] answers, DateTime end, PollOptions flags, PhotonMessageInfo messageInfo){
            PollStartEventArgs  args = new PollStartEventArgs(question, answers, end, messageInfo.Sender, flags);
            PollStarted?.Invoke(this, args);
        }

        [PunRPC]
        private async void PollEndReceived(PhotonMessageInfo messageInfo){
            PollEndEventArgs args = new PollEndEventArgs(messageInfo.Sender);
            PollEnded?.Invoke(this,args);
        }

        public void OnEvent(EventData photonEvent)
        {
            if(photonEvent.Code == PollRespondEventCode){
                bool[] selection = (bool[])photonEvent.CustomData;
                PollRespondEventArgs args = new PollRespondEventArgs(selection, PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender));
                PollRespond?.Invoke(this, args);
            }
        }

        [PunRPC]
        private async void PollDisplayReceived(short id, PollDisplayEventArgs.DisplayType type, PhotonMessageInfo messageInfo){
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