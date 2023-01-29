using HoloToolkit.Unity;
using i5.VIAProMa.Anchoring;
using i5.VIAProMa.ResourceManagagement;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.WebConnection;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VIAProMa.Assets.Scripts.Analytics;

namespace i5.VIAProMa.SaveLoadSystem.Core
{
    /// <summary>
    /// Save Load Manager which handles the serialization and deserialization of a scene at runtime
    /// </summary>
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        [Tooltip("The interval in minutes for the auto save")]
        [SerializeField] private float autoSaveInterval = 5f;

        [Header("Version")]
        [SerializeField] private int saveDataVersion = 2; // Unity UI overrides the standard value => This value must be updated in Unity as well.

        private static bool savedOnQuit = false;

        /// <summary>
        /// The list of instance Ids which are tracked
        /// This list is used in order to determine if a new instance of the GameObject must be created or if the settings can be applied to an existing instance
        /// </summary>
        private List<string> trackedIds;

        /// <summary>
        /// The serializers are placed on GameObjects which should be saved
        /// When they are initialized, they register in this list
        /// </summary>
        public List<Serializer> Serializers { get; private set; }

        public string SaveName { get; set; } = "";

        public bool AutoSaveActive { get => !string.IsNullOrWhiteSpace(SaveName); }

        /// <summary>
        /// Initializes the component's lists
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Serializers = new List<Serializer>();
            trackedIds = new List<string>();
        }

        private void OnEnable()
        {
            InvokeRepeating("SaveScene", 1f, 60f * autoSaveInterval);
        }

        private async void OnDisable()
        {
            CancelInvoke("SaveScene");
            await SaveScene(); // save one last time before the component is disabled
        }

        public async Task<bool> SaveScene()
        {
            if (!AutoSaveActive)
            {
                return false;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                string json = SerializeSaveGame();
                bool successful = await BackendConnector.Save(SaveName, json);
                if (successful)
                {
                    Debug.Log("Saved scene", gameObject);
                }
                else
                {
                    Debug.Log("Failed to save scene", gameObject);
                }
                return successful;
            }
            else
            {
                Debug.LogWarning("Only the master client can save a scene.", gameObject);
                return false;
            }
        }

        public async Task<bool> LoadScene(string saveName)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                SaveName = saveName;
                ApiResult<string> res = await BackendConnector.Load(saveName);
                if (res.Successful)
                {
                    DeserializeSaveGame(res.Value);

                    // After deserialization, the master client updates all other clients with the project id that has just been retrieved from loading the project save.
                    if(PhotonNetwork.IsMasterClient) {
                        AnalyticsManager.Instance.SetProjectIDAllOtherPlayers(AnalyticsManager.Instance.ProjectID.ToString());
                    }
                    
                    return true;
                }
            }
            else
            {
                Debug.LogWarning("At the moment only the master client can load a scene", gameObject);
            }
            return false;
        }

        /// <summary>
        /// Creates a serialized json string which contains the save data of the scene
        /// It calls all serializers which return their save data
        /// </summary>
        /// <returns></returns>
        public string SerializeSaveGame()
        {
            List<SerializedObject> serializedObjects = new List<SerializedObject>();
            for (int i = 0; i < Serializers.Count; i++)
            {
                SerializedObject data = Serializers[i].Serialize();
                data.PackData();
                serializedObjects.Add(data);
            }

            // Get VIAProMa project ID.
            Guid projectID = AnalyticsManager.Instance.ProjectID;

            SaveData saveData = new SaveData(saveDataVersion, projectID);
            saveData.Data = serializedObjects;

            return JsonUtility.ToJson(saveData);
        }

        /// <summary>
        /// Takes a json string and applies its save content to the scene
        /// </summary>
        /// <param name="json">The json string with the save data</param>
        public void DeserializeSaveGame(string json)
        {
            UpdateTrackedIds();
            bool[] usedIds = new bool[trackedIds.Count];
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            if (data.AppVersion != saveDataVersion)
            {
                Debug.LogError("Cannot open this save data version (compatible version "
                    + saveDataVersion + " but save data has version " + data.AppVersion + ")");
                return;
            }

            // Set the VIAProMa project version.
            AnalyticsManager.Instance.ProjectID = Guid.Parse(data.ProjectVersion);

            // Deserialize objects.
            List<SerializedObject> serializedObjects = data.Data;
            for (int i = 0; i < serializedObjects.Count; i++)
            {
                serializedObjects[i].UnPackData();

                int indexInTrackedIds = trackedIds.IndexOf(serializedObjects[i].Id);

                if (indexInTrackedIds >= 0) // The object already exists in the scene.
                {
                    usedIds[indexInTrackedIds] = true; // Set the index to true.
                    Serializer serializer = GetSerializer(serializedObjects[i].Id);

                    if (serializer != null)
                    {
                        serializer.Deserialize(serializedObjects[i]);
                    }
                }
                else
                {
                    // The object does not yet exist in the scene => instantiate it.
                    GameObject instantiated = ResourceManager.Instance.NetworkInstantiate(serializedObjects[i].PrefabName, Vector3.zero, Quaternion.identity);
                    Singleton<AnchorManager>.Instance.AttachToAnchor(instantiated);
                    Serializer serializer = instantiated?.GetComponent<Serializer>();
                    if (serializer == null)
                    {
                        Debug.LogError("Prefab " + serializedObjects[i].PrefabName + " is loaded but does not have a serializer");
                        PhotonNetwork.Destroy(instantiated);
                        continue;
                    }
                    else
                    {
                        serializer.Deserialize(serializedObjects[i]);
                    }
                }
            }

            // Destroy other tracked serializers which were not part of the save data.
            for (int i = 0; i < usedIds.Length; i++)
            {
                if (!usedIds[i])
                {
                    Serializer serializer = GetSerializer(trackedIds[i]);
                    PhotonNetwork.Destroy(serializer.gameObject);
                }
            }
        }

        /// <summary>
        /// Updates the list of tracked ids based on the registered serializers
        /// </summary>
        private void UpdateTrackedIds()
        {
            trackedIds.Clear();
            for (int i = 0; i < Serializers.Count; i++)
            {
                trackedIds.Add(Serializers[i].Id);
            }
        }

        /// <summary>
        /// Gets the serializer with the given id
        /// </summary>
        /// <param name="id">The id of the serializer</param>
        /// <returns>The serializer with the id or null if it does not exist</returns>
        private Serializer GetSerializer(string id)
        {
            for (int i = 0; i < Serializers.Count; i++)
            {
                if (Serializers[i].Id == id)
                {
                    return Serializers[i];
                }
            }
            Debug.LogWarning("The serializer with the id " + id + " does not exist or is not tracked by the SaveLoadManager", gameObject);
            return null;
        }

        /// <summary>
        /// Registers a serializer on the manager so that it is considered in the save-load procedure
        /// </summary>
        /// <param name="serializer">The serializer which should be registered</param>
        public void RegisterSerializer(Serializer serializer)
        {
            Serializers.Add(serializer);
        }

        /// <summary>
        /// Unregisters the given serializer so that it is not considered anymore by the save-load procedure
        /// </summary>
        /// <param name="serializer">The serializer to unregister</param>
        public void UnRegisterSerializer(Serializer serializer)
        {
            Serializers.Remove(serializer);
        }

        private async void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                await SaveScene();
            }
        }

        private async void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                await SaveScene();
            }
        }

        private async void OnApplicationQuit()
        {
            if (AutoSaveActive && !savedOnQuit)
            {
                await SaveScene();
                savedOnQuit = true;
                Debug.Log("Saved, now quitting");
                Application.Quit();
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void RunOnStart()
        {
            Application.wantsToQuit += WantsToQuit;
        }

        private static bool WantsToQuit()
        {
            if (!savedOnQuit)
            {
                Debug.Log("Prevented quitting to save");
            }
            return savedOnQuit;
        }
    }
}