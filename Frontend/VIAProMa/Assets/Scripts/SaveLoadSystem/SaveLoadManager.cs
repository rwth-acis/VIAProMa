using HoloToolkit.Unity;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    public List<Serializer> Serializers { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Serializers = new List<Serializer>();
    }

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
        SerializedObject[] serializedObjects = JsonArrayUtility.FromJson<SerializedObject>(json);
        for (int i = 0; i < serializedObjects.Length; i++)
        {
            serializedObjects[i].UnPackData();
            GameObject instantiated = ResourceManager.Instance.NetworkInstantiate(serializedObjects[i].PrefabName, Vector3.zero, Quaternion.identity);
            Serializer serializer = instantiated.GetComponent<Serializer>();
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

    public void RegisterSerializer(Serializer serializer)
    {
        Serializers.Add(serializer);
    }

    public void UnRegisterSerializer(Serializer serializer)
    {
        Serializers.Remove(serializer);
    }
}
