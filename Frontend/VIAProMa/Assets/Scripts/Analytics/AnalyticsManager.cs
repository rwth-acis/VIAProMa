using System;
using HoloToolkit.Unity;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using i5.VIAProMa.Login;
using VIAProMa.Assets.Scripts.ProjectSettings;
using Photon.Pun;
using Photon.Realtime;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using System.Threading.Tasks;

namespace VIAProMa.Assets.Scripts.Analytics
{
    public class AnalyticsManager : Singleton<AnalyticsManager>
    {
        Guid _projectID = Guid.Empty;

        /// <summary>
        /// This ID identifies a VIAProMA project. It is initialized with entering the VIAProMa application for the first time, preserved across saving and loading and used to reference the <c>ProjectSettings</c> as well as any <c>Logpoint</c>s that are recorded in the project.
        /// It is important to differentiate a VIAProMa project (contains (serialized) <c>GameObject</c>s, savable, persistant ID) from Photon Rooms (manage Players, ephemeral).
        /// </summary>
        public Guid ProjectID
        {
            get { return _projectID; }
            set
            {
                if (!(_projectID == value))
                {
                    _projectID = value;
                    Debug.Log("Loaded VIAProMa project with ID: " + value);

                    // Fetch settings for new project.
                    SettingsManager.Instance.GetSettingsFromBackendAsync().ConfigureAwait(false);
                }
                else
                {
                    Debug.LogWarning("Attempted to load project ID " + value + ", but it was already loaded. Project ID if kept.");
                }
            }
        }



        public IUserInfo UserInfo { get; set; }

        public void Start()
        {
            // Initialize data about the current user as anonymous (user is neither logged into GitHub nor the RequirementsBazaar).
            SetUserAnonymous();

            // Generate a new GUID for the VIAProMa project that the analytics refer to. If the project ID is already set (for example because the project has been saved and loaded), do nothing and use the old ID.
            if (ProjectID.Equals(Guid.Empty))
                ProjectID = Guid.NewGuid();

            if (SettingsManager.Instance.IsAnalyticsEnabled && !CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingEnabled)
            {
                Debug.LogError("Eye tracking is disabled! Instuctions: " +
                    "https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk2/features/input/eye-tracking/eye-tracking-basic-setup?view=mrtkunity-2022-05#testing-your-unity-app-on-a-hololens-2");
            }


            // When the user starts VIAProMa, display whether analytics are disabled.
            StartCoroutine(SettingsManager.Instance.ShowAnalyticsPopup());
        }

        /// <summary>
        /// Sets the project ID for the current player/client to the parameter. This method is called via RPC by the Photon Master client (the one loading the VIAProMa project) when a new project is loaded from the saves shelf.
        /// </summary>
        /// <param name="projectID">The project ID to be set.</param>
        [PunRPC]
        private void SetProjectID(string projectID)
        {
            ProjectID = Guid.Parse(projectID);
        }

        /// <summary>
        /// Sends the project id that has just been loaded to the specified player. This method is executed when a new player joins the room.
        /// </summary>
        /// <param name="newPlayer">The player to send the project ID to.</param>
        public void SendProjectIDToNewPlayer(Player newPlayer)
        {
            PhotonView.Get(this).RPC("SetProjectID", newPlayer, ProjectID.ToString());
        }

        /// <summary>
        /// Sets the project ID for all other players/clients to the parameter. This method is executed by the Photon Master client (the one loading the VIAProMa project) when a new project is loaded from the saves shelf.
        /// </summary>
        /// <param name="projectID">The project ID to be set.</param>
        public void SetProjectIDAllOtherPlayers(string projectID)
        {
            PhotonView.Get(this).RPC("SetProjectID", RpcTarget.Others, ProjectID.ToString());
        }

        public async Task FetchLearningLayersUserDataFromServiceManager()
        {
            IUserInfo userInfo = await ServiceManager.GetService<LearningLayersOidcService>().GetUserDataAsync();
            if (userInfo != null)
                this.UserInfo = userInfo;
        }

        public void SetUserAnonymous()
        {
            this.UserInfo = new LearningLayersUserInfo("AnonymousUser", "anonymoususer@viaproma.com", "Anonymous User");
        }
    }
}
