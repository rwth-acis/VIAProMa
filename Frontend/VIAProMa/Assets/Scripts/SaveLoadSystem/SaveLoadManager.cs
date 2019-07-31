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
    /// The serializers are placed on GameObjects which should be saved
    /// When they are initialized, they register in this list
    /// </summary>
    public List<Serializer> Serializers { get; private set; }

    /// <summary>
    /// The list of instance Ids which are tracked
    /// This list is used in order to determine if a new instance of the GameObject must be created or if the settings can be applied to an existing instance
    /// </summary>
    private List<string> trackedIds;

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

    private void UpdateTrackedIds()
    {
        trackedIds.Clear();
        for (int i=0;i<Serializers.Count;i++)
        {
            trackedIds.Add(Serializers[i].Id);
        }
    }

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

    public void RegisterSerializer(Serializer serializer)
    {
        Serializers.Add(serializer);
    }

    public void UnRegisterSerializer(Serializer serializer)
    {
        Serializers.Remove(serializer);
    }
}
