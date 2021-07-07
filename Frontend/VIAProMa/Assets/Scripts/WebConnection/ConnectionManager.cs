using HoloToolkit.Unity;
using System;
using UnityEngine;

namespace i5.VIAProMa.WebConnection
{
    /// <summary>
    /// provides the necessary data for a connection to the backend
    /// </summary>
    public class ConnectionManager : Singleton<ConnectionManager>
    {
        [Tooltip("The address of the backend, i.e. its url")]
        [SerializeField] private string backendAddress = "http://10.108.126.2";

        [Tooltip("The base path of the backend's API.")]
        [SerializeField] private string basePath = "resources";

        [Tooltip("The port of the backend")]
        [SerializeField] private int port = 8080;

        private const string backendAddressPrefKey = "backendAddress";

        /// <summary>
        /// The address of the backend, i.e. its url and port
        /// </summary>
        public string BackendAddress
        {
            get
            {
                return backendAddress;
            }
            set
            {
                backendAddress = value;
                PlayerPrefs.SetString(backendAddressPrefKey, backendAddress);
                TestConnection();
            }
        }

        public string FullBackendAddress
        {
            get { return backendAddress + ":" + port; }
        }

        /// <summary>
        /// Combination of hte backend address and the base path of the backend's API
        /// </summary>
        public string BackendAPIBaseURL
        {
            get
            {
                return FullBackendAddress + "/" + basePath + "/";
            }
        }

        /// <summary>
        /// True if the backend server is reachable
        /// This value is updated automatically with every request which is made to the server
        /// </summary>
        /// <value></value>
        public bool BackendReachable
        {
            get; private set;
        }

        private void Start()
        {
            BackendAddress = PlayerPrefs.GetString(backendAddressPrefKey, "http://10.108.126.2");
        }

        /// <summary>
        /// Checks the status code in order to determine if the backend is reachable
        /// </summary>
        /// <param name="code"></param>
        public void CheckStatusCode(long code)
        {
            if (code == 0) // device unavailable
            {
                SetBackendReachableStatus(false);
            }
            else
            {
                SetBackendReachableStatus(true);
            }
        }

        /// <summary>
        /// Sets the backend reachable status to the given value
        /// If the value is different from the previous state, the BackendOnlineStatusChanged event is invoked
        /// </summary>
        /// <param name="reachable"></param>
        private void SetBackendReachableStatus(bool reachable)
        {
            bool previousStatus = BackendReachable;
            BackendReachable = reachable;
            if (previousStatus != BackendReachable)
            {
                BackendOnlineStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Posts a request to the backend in order to find out if the connection to the backend is valid
        /// </summary>
        private async void TestConnection()
        {
            await BackendConnector.Ping();
            // no need to check the result here; it is already checked in the Ping method
        }

        /// <summary>
        /// Raised if the reachability status of the backend is changed
        /// </summary>
        public event EventHandler BackendOnlineStatusChanged;
    }
}