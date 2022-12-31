using HoloToolkit.Unity;
using i5.VIAProMa.Multiplayer;
using i5.VIAProMa.UI.Chat;
using i5.VIAProMa.Anchoring;
using UnityEngine;

namespace i5.VIAProMa.UI
{
    public class WindowManager : Singleton<WindowManager>
    {
        [SerializeField] private GameObject roomMenuPrefab;
        [SerializeField] private GameObject serverStatusMenuPrefab;
        [SerializeField] private GameObject chatMenuPrefab;
        [SerializeField] private GameObject saveProjectMenuPrefab;
        [SerializeField] private GameObject loginMenuPrefab;
        [SerializeField] private GameObject anchorMenuPrefab;
        [SerializeField] private GameObject importModelMenuPrefab;

        private void Start()
        {
            RoomMenu = (RoomMenu)InstantiateWindow(roomMenuPrefab);
            ServerStatusMenu = (ServerStatusMenu)InstantiateWindow(serverStatusMenuPrefab);
            ChatMenu = (ChatMenu)InstantiateWindow(chatMenuPrefab);
            SaveProjectWindow = (SaveProjectWindow)InstantiateWindow(saveProjectMenuPrefab);
            LoginMenu = (LoginMenu)InstantiateWindow(loginMenuPrefab);
            AnchorMenu = (AnchoringMenu)InstantiateWindow(anchorMenuPrefab);
            ImportManager = (ImportManager)InstantiateWindow(importModelMenuPrefab);
        }

        private IWindow InstantiateWindow(GameObject prefab)
        {
            GameObject windowInstance = Instantiate(prefab, transform);
            IWindow window = windowInstance.GetComponentInChildren<IWindow>();
            if (window == null)
            {
                Destroy(windowInstance);
                Debug.LogError("Window Manager tried to create a prefab without a window component.\nAdd the window component to the prefab " + prefab.name);
                return null;
            }
            window.Close();
            return window;
        }

        public RoomMenu RoomMenu { get; private set; }

        public ServerStatusMenu ServerStatusMenu { get; private set; }

        public ChatMenu ChatMenu { get; private set; }

        public SaveProjectWindow SaveProjectWindow { get; private set; }

        public LoginMenu LoginMenu { get; private set; }

        public AnchoringMenu AnchorMenu { get; private set; }

        public ImportManager ImportManager { get; private set; }
    }
}