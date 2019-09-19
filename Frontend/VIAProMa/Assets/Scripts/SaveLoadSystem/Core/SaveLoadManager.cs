using HoloToolkit.Unity;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Save Load Manager which handles the serialization and deserialization of a scene at runtime
/// </summary>
public class SaveLoadManager : Singleton<SaveLoadManager>
{
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

        return JsonArrayUtility.ToJson<SerializedObject>(serializedObjects.ToArray());
    }

    /// <summary>
    /// Takes a json string and applies its save content to the scene
    /// </summary>
    /// <param name="json">The json string with the save data</param>
    public void DeserializeSaveGame(string json)
    {
        UpdateTrackedIds();
        SerializedObject[] serializedObjects = JsonArrayUtility.FromJson<SerializedObject>(json);
        for (int i = 0; i < serializedObjects.Length; i++)
        {
            serializedObjects[i].UnPackData();

            if (trackedIds.Contains(serializedObjects[i].Id)) // the object already exists in the scene
            {
                Serializer serializer = GetSerializer(serializedObjects[i].Id);
                if (serializer != null)
                {
                    serializer.Deserialize(serializedObjects[i]);
                }
            }
            else // the object does not yet exist in the scene => instantiate it
            {
                GameObject instantiated = ResourceManager.Instance.NetworkInstantiate(serializedObjects[i].PrefabName, Vector3.zero, Quaternion.identity);
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
    }

    /// <summary>
    /// Updates the list of tracked ids based on the registered serializers
    /// </summary>
    private void UpdateTrackedIds()
    {
        trackedIds.Clear();
        for (int i=0;i<Serializers.Count;i++)
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
        for (int i=0;i<Serializers.Count;i++)
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
}
