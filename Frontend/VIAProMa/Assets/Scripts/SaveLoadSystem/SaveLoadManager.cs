using HoloToolkit.Unity;
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
        for (int i=0;i<Serializers.Count;i++)
        {
            SerializedObject data = Serializers[i].Serialize();
            data.ApplyDataSerialization();
            serializedObjects.Add(data);
        }

        return JsonArrayUtility.ToJson<SerializedObject>(serializedObjects.ToArray());
    }

    public void DeserializeSaveGame()
    {

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
