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
using i5.VIAProMa.ResourceManagagement;
using i5.VIAProMa.Utilities;

namespace i5.VIAProMa.Multiplayer.Poll
{
    [RequireComponent(typeof(PhotonView))]
    /// <summary>
    /// Handles poll networking, poll host state manageement and local events for UI
    /// </summary>
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
        public const byte PollJoinSyncEventCode = 5;

        [Header("Poll Visualizations")]
        [SerializeField] private GameObject barChartVisualizationPrefab;
        [SerializeField] private GameObject pollShelfPrefab;
        [SerializeField] private float barDistance;
        [SerializeField] private float shelfDistance;

        private Poll currentPoll;
        private int realtimeVizIndex;
        private PollBarVisualization realtimeViz;
        private IEnumerator currentCountdown;

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
                Debug.LogError($"Couldn't register SerializeablePoll, is there another Serializer registered at {SerializablePoll.SerializeablePollCode}?");
            }
            savedPolls = new List<SerializablePoll>();
            tempSavedPolls = new List<SerializablePoll>();
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

        /// <summary>
        /// Instatiates Poll Shelf
        /// </summary>
        public void PollShelfDisplay()
        {
            GameObject pollShelf = Instantiate(pollShelfPrefab);

            Vector3 position = CameraCache.Main.transform.position;
            position.y = 0f;
            position += shelfDistance * new Vector3(CameraCache.Main.transform.forward.x, 0, CameraCache.Main.transform.forward.z).normalized;
            pollShelf.transform.position = position;
        }

        /// <summary>
        /// Instaniates Result Visualization
        /// </summary>
        /// <param name="pollIndex">Index of the poll data &gt 0 for permantly saved polls &lt 0 for temporary polls</param>
        /// <returns>PollBarVisualization component of the instantiated object</returns>
        public PollBarVisualization GenerateSynchronizedPollDisplay(int pollIndex)
        {
            Vector3 position = CameraCache.Main.transform.position;
            position.y = 0.5f;
            position += barDistance * new Vector3(CameraCache.Main.transform.forward.x, 0, CameraCache.Main.transform.forward.z).normalized;
            GameObject barChartObj = ResourceManager.Instance.NetworkInstantiate(barChartVisualizationPrefab, position, new Quaternion(0, 0, 0, 0));

            PollBarVisualization viz = barChartObj.GetComponent<PollBarVisualization>();
            viz?.SetupPoll(pollIndex);
            return viz;
        }

        /// <summary>
        /// Creates and starts a poll
        /// </summary>
        /// <param name="question"></param>
        /// <param name="answers"></param>
        /// <param name="flags"></param>
        /// <param name="end"></param>
        public void StartPoll(string question, string[] answers, PollOptions flags, DateTime end)
        {
            // Send out poll message
            int syncedEndTime = PhotonNetwork.ServerTimestamp + (int)(end - DateTime.Now).TotalMilliseconds;
            currentPoll = new Poll(question, answers, flags, syncedEndTime);
            photonView.RPC("PollStartedReceived", RpcTarget.All, question, answers, syncedEndTime, flags);
            // Setup timer on host
            if (flags.HasFlag(PollOptions.Countdown))
            {
                TimeSpan timeToGo = end - DateTime.Now;
                if (timeToGo > TimeSpan.Zero)
                {
                    currentCountdown = Countdown((int)timeToGo.TotalSeconds);
                    StartCoroutine(currentCountdown);
                }
            }
            // Setup timer on host
            if (flags.HasFlag(PollOptions.RealtimeViz))
            {
                photonView.RPC("PollSaveRequestReceived", RpcTarget.MasterClient, SerializablePoll.FromPoll(currentPoll));
            }
        }

        /// <summary>
        /// Responds to poll as user
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="host">the creator of the poll</param>
        public void RespondPoll(bool[] selection, Player host)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { host.ActorNumber } };
            PhotonNetwork.RaiseEvent(PollRespondEventCode, selection, raiseEventOptions, SendOptions.SendReliable);
        }

        /// <summary>
        /// Force Poll End by poll host
        /// </summary>
        public void EndPoll()
        {
            if (currentPoll == null) return;
            if (currentPoll.IsEnded) return;
            currentPoll.IsEnded = true;
            photonView.RPC("PollEndReceived", RpcTarget.All);
        }

        private void FinalizePoll()
        {
            if (currentPoll.IsFinalized) return;
            currentPoll.IsFinalized = true;
            if (currentPoll.ResponseCount == 0)
            {
                currentPoll = null;
            }
            else
            {
                if (currentPoll.Flags.HasFlag(PollOptions.RealtimeViz) && realtimeVizIndex != 0)
                { // Update existing entry one last time
                    photonView.RPC("PollUpdateRequestReceived", RpcTarget.MasterClient, SerializablePoll.FromPoll(currentPoll), realtimeVizIndex);
                }
                else
                { // Create final entry
                    photonView.RPC("PollSaveRequestReceived", RpcTarget.MasterClient, SerializablePoll.FromPoll(currentPoll));
                }
            }
        }

        /// <summary>
        /// Register and unregister from poll
        /// </summary>
        /// <param name="ack"></param>
        /// <param name="host">the creator of the poll</param>
        public void SendStatus(bool ack, Player host)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { host.ActorNumber } };
            PhotonNetwork.RaiseEvent(PollAcknowledgedEventCode, ack, raiseEventOptions, SendOptions.SendReliable);
        }

        /// <summary>
        /// Looks up the saved or temporary poll with the given ID
        /// </summary>
        /// <param name="id">ID of the poll data &gt 0 for permantly saved polls &lt 0 for temporary polls</param>
        /// <returns></returns>
        public SerializablePoll GetPollByID(int id)
        {
            if (id > 0)
                return id - 1 < savedPolls.Count ? savedPolls[id - 1] : null;
            else
                return -id - 1 < tempSavedPolls.Count ? tempSavedPolls[-id - 1] : null;
        }

        [PunRPC]
        private void PollStartedReceived(string question, string[] answers, int syncedEndTime, PollOptions flags, PhotonMessageInfo messageInfo)
        {
            int milliseconds = syncedEndTime < PhotonNetwork.ServerTimestamp ? (int.MaxValue - PhotonNetwork.ServerTimestamp + syncedEndTime) : (syncedEndTime - PhotonNetwork.ServerTimestamp);
            PollStartEventArgs args = new PollStartEventArgs(question, answers, DateTime.Now.AddMilliseconds(milliseconds), messageInfo.Sender, flags);
            PollStarted?.Invoke(this, args);
        }

        [PunRPC]
        private void PollEndReceived(PhotonMessageInfo messageInfo)
        {
            PollEndEventArgs args = new PollEndEventArgs(messageInfo.Sender);
            PollEnded?.Invoke(this, args);
        }

        [PunRPC]
        private void PollSaveRequestReceived(SerializablePoll poll, PhotonMessageInfo messageInfo)
        {
            int index = 0;
            if (poll.Flags.HasFlag(PollOptions.SaveResults))
            {
                index = savedPolls.Count + 1;
                savedPolls.Add(poll);
            }
            else
            {
                index = -1 - tempSavedPolls.Count;
                tempSavedPolls.Add(poll);
            }
            photonView.RPC("PollUpdateReceived", RpcTarget.All, index, poll);
        }

        [PunRPC]
        private void PollUpdateRequestReceived(SerializablePoll poll, int i, PhotonMessageInfo messageInfo)
        {
            if (i == 0)
            {
                Debug.LogWarning("Poll Update Index == 0! This should not happen, but is not an error.");
                PollSaveRequestReceived(poll, messageInfo);
            }
            else if (i > 0)
            {
                int index = i - 1;
                if (index >= savedPolls.Count)
                    return;
                savedPolls[index] = poll;
            }
            else
            {
                int index = -i - 1;
                if (index >= tempSavedPolls.Count)
                    return;
                tempSavedPolls[index] = poll;
            }
            photonView.RPC("PollUpdateReceived", RpcTarget.All, i, poll);
        }

        [PunRPC]
        private void PollUpdateReceived(int index, SerializablePoll poll, PhotonMessageInfo messageInfo)
        {
            if (index < 0)
            {
                int tIndex = -index - 1;
                if (tempSavedPolls.Count <= tIndex)
                {
                    if (tIndex >= tempSavedPolls.Capacity)
                        tempSavedPolls.Capacity = tIndex + 1;
                    tempSavedPolls.AddRange(Enumerable.Repeat<SerializablePoll>(null, tIndex - tempSavedPolls.Count + 1));
                }
                tempSavedPolls[tIndex] = poll;

            }
            else
            {
                int tIndex = index - 1;
                if (savedPolls.Count <= tIndex)
                { // Make sure we can access index
                    if (tIndex >= savedPolls.Capacity)
                        savedPolls.Capacity = tIndex + 1;
                    savedPolls.AddRange(Enumerable.Repeat<SerializablePoll>(null, tIndex - savedPolls.Count + 1));
                }
                savedPolls[tIndex] = poll;
            }
            if (currentPoll != null && currentPoll.Question == poll.Question) // should be enough
            {
                if (currentPoll.Flags.HasFlag(PollOptions.RealtimeViz))
                {
                    if (realtimeViz == null)
                        realtimeViz = GenerateSynchronizedPollDisplay(index);
                    else
                        realtimeViz.ForceUpdatePoll(index);
                    realtimeVizIndex = index;
                }
                else
                {
                    GenerateSynchronizedPollDisplay(index);
                }
                if (currentPoll.IsFinalized)
                {
                    currentPoll = null;
                    realtimeViz = null;
                }
            }

            PollToDisplayRecieved?.Invoke(this, index);
        }

        /// <summary>
        /// Handles PUN events
        /// </summary>
        /// <param name="photonEvent"></param>
        public void OnEvent(EventData photonEvent)
        {
            bool? finished = false;
            switch (photonEvent.Code)
            {
                case PollRespondEventCode:
                    finished = currentPoll?.OnResponse(PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender), (bool[])photonEvent.CustomData);
                    if (finished == false && currentPoll.Flags.HasFlag(PollOptions.RealtimeViz))
                        photonView.RPC("PollUpdateRequestReceived", RpcTarget.MasterClient, SerializablePoll.FromPoll(currentPoll), realtimeVizIndex);
                    break;
                case PollAcknowledgedEventCode:
                    finished = currentPoll?.OnStatus(PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender), (bool)photonEvent.CustomData);
                    break;
                case PollSavedPollSyncEventCode:
                    object[] data = (object[])photonEvent.CustomData;
                    savedPolls = ((SerializablePoll[])data[0]).ToList();
                    tempSavedPolls = ((SerializablePoll[])data[1]).ToList();
                    for (int i = 0; i < savedPolls.Count; i++)
                        PollToDisplayRecieved?.Invoke(this, i + 1);
                    for (int i = 0; i < tempSavedPolls.Count; i++)
                        PollToDisplayRecieved?.Invoke(this, -i - 1);
                    break;
                case PollJoinSyncEventCode:
                    data = (object[])photonEvent.CustomData;
                    string question = (string)data[0];
                    string[] answers = (string[])data[1];
                    int syncedEndTime = (int)data[2];
                    PollOptions flags = (PollOptions)data[3];
                    int milliseconds = syncedEndTime < PhotonNetwork.ServerTimestamp ? (int.MaxValue - PhotonNetwork.ServerTimestamp + syncedEndTime) : (syncedEndTime - PhotonNetwork.ServerTimestamp);
                    PollStartEventArgs args = new PollStartEventArgs(question, answers, DateTime.Now.AddMilliseconds(milliseconds), PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender), flags);
                    PollStarted?.Invoke(this, args);
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
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { newPlayer.ActorNumber } };
                PhotonNetwork.RaiseEvent(PollSavedPollSyncEventCode, new object[] { savedPolls.ToArray(), tempSavedPolls.ToArray() }, raiseEventOptions, SendOptions.SendReliable);
            }
            if (currentPoll != null && !currentPoll.IsEnded)
            {
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { newPlayer.ActorNumber } };
                PhotonNetwork.RaiseEvent(PollJoinSyncEventCode, new object[] { currentPoll.Question, currentPoll.Answers, currentPoll.SyncedEndTime, currentPoll.Flags}, raiseEventOptions, SendOptions.SendReliable);                
            }
        }
        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            currentPoll?.OnStatus(otherPlayer, false);
            PollDiscardByPlayer?.Invoke(this, otherPlayer);
        }
        public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) { }
        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) { }
        public void OnMasterClientSwitched(Player newMasterClient) { }
        public void OnFriendListUpdate(List<FriendInfo> friendList) { }
        public void OnCreatedRoom()
        {
            ResourceManager.Instance.SceneNetworkInstantiate(pollSerializerPrefab, Vector3.zero, Quaternion.identity, obj => {});
        }
        public void OnCreateRoomFailed(short returnCode, string message) { }
        public void OnJoinedRoom()
        {
            savedPolls = new List<SerializablePoll>();
        }
        public void OnJoinRoomFailed(short returnCode, string message) { }
        public void OnJoinRandomFailed(short returnCode, string message) { }
        public void OnLeftRoom()
        {
            if (currentPoll != null)
                PollDiscardByPlayer?.Invoke(this, PhotonNetwork.LocalPlayer);
        }
    }
}