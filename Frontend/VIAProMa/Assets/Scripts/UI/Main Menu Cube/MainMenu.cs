using i5.VIAProMa.Multiplayer;
using i5.VIAProMa.ResourceManagagement;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.Anchoring;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;


namespace i5.VIAProMa.UI.MainMenuCube
{
    [RequireComponent(typeof(FoldController))]
    public class MainMenu : MonoBehaviourPunCallbacks
    {
        [Header("UI Elements")]
        [SerializeField] private Interactable avatarConfigurationButton;
        [SerializeField] private Interactable serverConnectionButton;
        [SerializeField] private Interactable saveButton;
        [SerializeField] private Interactable loadButton;
        [SerializeField] private Interactable anchoringButton;
        [SerializeField] private Interactable issueShelfButton;
        [SerializeField] private Interactable visualizationShelfButton;
        [SerializeField] private Interactable loginButton;
        [SerializeField] private Interactable roomButton;
        [SerializeField] private TextMeshPro roomButtonText;
        [SerializeField] private Interactable chatButton;
        [SerializeField] private Interactable microphoneButton;
        [SerializeField] private Interactable undoRedoButton;

        [Header("References")]
        [SerializeField] private GameObject issueShelfPrefab;
        [SerializeField] private GameObject visualizationShelfPrefab;
        [SerializeField] private GameObject loadShelfPrefab;
        [SerializeField] private GameObject avatarConfiguratorPrefab;
        [SerializeField] private GameObject undoRedoMenuPrefab;


        private FoldController foldController;

        // instances:
        private GameObject issueShelfInstance;
        private GameObject visualizationShelfInstance;
        private GameObject loadShelfInstance;
        private GameObject avatarConfiguratorInstance;

        private void Awake()
        {
            if (avatarConfigurationButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(avatarConfigurationButton));
            }
            if (serverConnectionButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(serverConnectionButton));
            }
            if (saveButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(saveButton));
            }
            if (loadButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(loadButton));
            }
            if (issueShelfButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(issueShelfButton));
            }
            if (visualizationShelfButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(visualizationShelfButton));
            }
            if (loginButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(loginButton));
            }
            if (roomButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(roomButton));
            }
            if (roomButtonText == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(roomButtonText));
            }
            if (chatButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(chatButton));
            }
            if (microphoneButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(microphoneButton));
            }
            if (undoRedoButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(undoRedoButton));
            }

            if (issueShelfPrefab == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(issueShelfPrefab));
            }
            if (visualizationShelfPrefab == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(visualizationShelfPrefab));
            }
            if (loadShelfPrefab == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(loadShelfPrefab));
            }
            if (avatarConfiguratorPrefab == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(avatarConfiguratorPrefab));
            }
            if (undoRedoMenuPrefab == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(undoRedoMenuPrefab));
            }

            foldController = gameObject.GetComponent<FoldController>();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            LobbyManager.Instance.LobbyJoinStatusChanged += OnLobbyStatusChanged;
            Launcher.Instance.ConnectionStatusChanged += OnConnectionStatusChanged;
        }

        public override void OnDisable()
        {
            LobbyManager.Instance.LobbyJoinStatusChanged -= OnLobbyStatusChanged;
            Launcher.Instance.ConnectionStatusChanged -= OnConnectionStatusChanged;
            base.OnDisable();
        }

        private void Start()
        {
            CheckButtonStates();
        }

        private void OnConnectionStatusChanged(object sender, EventArgs e)
        {
            CheckButtonStates();
        }

        private void CheckButtonStates()
        {
            roomButton.IsEnabled = PhotonNetwork.IsConnected;
            chatButton.IsEnabled = PhotonNetwork.InRoom;
            microphoneButton.IsEnabled = PhotonNetwork.InRoom;
            saveButton.IsEnabled = PhotonNetwork.InRoom;
            loadButton.IsEnabled = PhotonNetwork.InRoom;
            issueShelfButton.IsEnabled = PhotonNetwork.InRoom;
            visualizationShelfButton.IsEnabled = PhotonNetwork.InRoom;
            anchoringButton.IsEnabled = PhotonNetwork.InRoom;
            // TODO: Photon Integration undoRedoButton = PhotonNetwork...
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient && newPlayer.UserId != PhotonNetwork.LocalPlayer.UserId)
            {
                int issueShelfId = 0;
                int visualizationShelfId = 0;
                if (issueShelfInstance != null)
                {
                    issueShelfId = issueShelfInstance.GetComponent<PhotonView>().ViewID;
                }
                if (visualizationShelfInstance != null)
                {
                    visualizationShelfId = visualizationShelfInstance.GetComponent<PhotonView>().ViewID;
                }

                Debug.Log("Sending main menu initialization data.\nIssueShelfId: " + issueShelfId + "; visualizationShelfId" + visualizationShelfId);
                photonView.RPC("Initialize", RpcTarget.Others, issueShelfId, visualizationShelfId);
            }
        }

        private void OnLobbyStatusChanged(object sender, EventArgs e)
        {
            if (PhotonNetwork.InLobby)
            {
                roomButtonText.text = "Rooms";
            }
            else
            {
                roomButtonText.text = "Leave Room";
            }
            CheckButtonStates();
        }

        public void ShowSaveMenu()
        {
            WindowManager.Instance.SaveProjectWindow.Open(saveButton.transform.position + 0.4f * transform.right - AnchorManager.Instance.AnchorParent.transform.position, transform.localEulerAngles);
            foldController.InitalizeNewCloseTimer();
        }

        public void ShowAnchorMenu()
        {
            WindowManager.Instance.AnchorMenu.Open(saveButton.transform.position + 0.4f * transform.right - AnchorManager.Instance.AnchorParent.transform.position, transform.localEulerAngles);
            foldController.InitalizeNewCloseTimer();
        }

        public void ShowIssueShelf()
        {
            Vector3 targetPosition = transform.position - 2f * transform.right;
            targetPosition.y = 0f + AnchorManager.Instance.AnchorParent.transform.position.y;
            SceneNetworkInstantiateControl(issueShelfPrefab, ref issueShelfInstance, targetPosition, IssueShelfCreated);
            foldController.InitalizeNewCloseTimer();
        }

        private void IssueShelfCreated(GameObject obj)
        {
            issueShelfInstance = obj;
            PhotonView view = obj.GetComponent<PhotonView>();
            photonView.RPC("SetIssueShelfInstance", RpcTarget.Others, view.ViewID);
        }

        public void ShowVisualizationShelf()
        {
            Vector3 targetPosition = transform.position - 1f * transform.right;
            targetPosition.y = 0f + AnchorManager.Instance.AnchorParent.transform.position.y;
            NetworkInstantiateControl(visualizationShelfPrefab, ref visualizationShelfInstance, targetPosition, "SetVisualizationShelfInstance");
            foldController.InitalizeNewCloseTimer();
        }

        public void ShowLoadShelf()
        {
            Vector3 targetPosition = transform.position + 1f * transform.right;
            targetPosition.y = 0f + AnchorManager.Instance.AnchorParent.transform.position.y;
            InstantiateControl(loadShelfPrefab, ref loadShelfInstance, targetPosition);
            foldController.InitalizeNewCloseTimer();
        }

        public void ShowLoginMenu()
        {
            WindowManager.Instance.LoginMenu.Open(loginButton.transform.position - AnchorManager.Instance.AnchorParent.transform.position, loginButton.transform.eulerAngles);
            foldController.InitalizeNewCloseTimer();
        }

        public void ShowAvatarConfiguration()
        {
            InstantiateControl(
                avatarConfiguratorPrefab,
                ref avatarConfiguratorInstance,
                transform.position - 1f * transform.right);
            foldController.InitalizeNewCloseTimer();
        }

        public void ShowServerStatusMenu()
        {
            WindowManager.Instance.ServerStatusMenu.Open(serverConnectionButton.transform.position - 0.5f * transform.right - AnchorManager.Instance.AnchorParent.transform.position, transform.localEulerAngles);
            foldController.InitalizeNewCloseTimer();
        }

        public void ShowUndoRedoMenu()
        {
            WindowManager.Instance.UndoRedoMenu.Open(undoRedoButton.transform.position - 0.5f * transform.right - AnchorManager.Instance.AnchorParent.transform.position, transform.localEulerAngles);
            foldController.InitalizeNewCloseTimer();
        }

        public void RoomButtonClicked()
        {
            // if in lobby: show room menu
            // otherwise: leave the current room
            if (PhotonNetwork.InLobby)
            {
                WindowManager.Instance.RoomMenu.Open(roomButton.transform.position - 0.6f * transform.right - AnchorManager.Instance.AnchorParent.transform.position, transform.localEulerAngles);
            }
            else
            {
                PhotonNetwork.LeaveRoom();
                AnchorManager.Instance.DisableAnchoring();

            }
            foldController.InitalizeNewCloseTimer();
        }

        public void ChatButtonClicked()
        {
            WindowManager.Instance.ChatMenu.Open(chatButton.transform.position - 0.6f * transform.right, transform.localEulerAngles);
            foldController.InitalizeNewCloseTimer();
        }

        private void InstantiateControl(GameObject prefab, ref GameObject instance, Vector3 targetPosition)
        {
            Quaternion targetRotation = transform.rotation;

            if (instance != null)
            {
                instance.SetActive(true);
                instance.transform.position = targetPosition;
                instance.transform.rotation = targetRotation;
            }
            else
            {
                instance = GameObject.Instantiate(prefab, targetPosition, targetRotation);
                HoloToolkit.Unity.Singleton<AnchorManager>.Instance.AttachToAnchor(instance.gameObject);
            }
        }

        private void NetworkInstantiateControl(GameObject prefab, ref GameObject instance, Vector3 targetPosition, string instantiationRPC)
        {
            Quaternion targetRotation = transform.rotation;

            if (instance != null)
            {
                instance.SetActive(true);
                instance.transform.position = targetPosition;
                instance.transform.rotation = targetRotation;
            }
            else
            {
                instance = ResourceManager.Instance.NetworkInstantiate(prefab, targetPosition, targetRotation);
                HoloToolkit.Unity.Singleton<AnchorManager>.Instance.AttachToAnchor(instance.gameObject);
                PhotonView view = instance.GetComponent<PhotonView>();
                photonView.RPC(instantiationRPC, RpcTarget.Others, view.ViewID);
            }
        }

        private void SceneNetworkInstantiateControl(GameObject prefab, ref GameObject instance, Vector3 targetPosition, Action<GameObject> OnCreated)
        {
            Quaternion targetRotation = transform.rotation;

            if (instance != null)
            {
                instance.SetActive(true);
                instance.transform.position = targetPosition;
                instance.transform.rotation = targetRotation;
            }
            else
            {
                ResourceManager.Instance.SceneNetworkInstantiate(prefab, targetPosition, targetRotation, OnCreated);
                HoloToolkit.Unity.Singleton<AnchorManager>.Instance.AttachToAnchor(instance.gameObject);
            }
        }

        [PunRPC]
        private void Initialize(int issueShelfId, int visualizationShelfId)
        {
            Debug.Log("RPC: Initialize with issueShelfId " + issueShelfId + " and visualizationShelfId " + visualizationShelfId);
            if (issueShelfId != 0)
            {
                SetIssueShelfInstance(issueShelfId);
            }
            if (visualizationShelfId != 0)
            {
                SetVisualizationShelfInstance(visualizationShelfId);
            }
        }

        [PunRPC]
        private void SetIssueShelfInstance(int photonId)
        {
            PhotonView view = PhotonView.Find(photonId);
            issueShelfInstance = view.gameObject;
            Debug.Log("RPC: setting issue shelf instance to " + issueShelfInstance.name + " (id " + photonId + ")");
        }

        [PunRPC]
        private void SetVisualizationShelfInstance(int photonId)
        {
            PhotonView view = PhotonView.Find(photonId);
            visualizationShelfInstance = view.gameObject;
            Debug.Log("RPC: setting visualization shelf instance to " + issueShelfInstance.name + " (id " + photonId + ")");
        }
    }
}