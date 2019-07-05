using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serializer : MonoBehaviour
{
    private const string objectTypeKey = "objectType";
    private const string idKey = "id";

    public ObjectType objectType;

    private ISerializer[] iserializers;

    public string Id { get; private set; }

    private void Awake()
    {
        Id = Guid.NewGuid().ToString();
    }

    private void Start()
    {
        SaveLoadManager.Instance.RegisterSerializer(this);
    }

    private void OnDestroy()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.UnRegisterSerializer(this);
        }
    }

    public virtual void Deserialize(SerializedData serializedData)
    {
        objectType = (ObjectType)serializedData.Integers[objectTypeKey];
        Id = serializedData.Strings[idKey];

        // get the serializers on the object
        iserializers = GetComponentsInChildren<ISerializer>(true);

        for (int i=0; i<iserializers.Length; i++)
        {
            iserializers[i].Deserialize(serializedData);
        }
    }

    public virtual SerializedData Serialize()
    {
        // get the serializers on the object
        iserializers = GetComponentsInChildren<ISerializer>(true);

        // create a dictionary which stores the values which should be serialized
        SerializedData serializedData = new SerializedData();
        serializedData.Integers.Add(objectTypeKey, (int)objectType);
        serializedData.Strings.Add(idKey, Id);
        // serialize each serializer and add the result to the dictionary of serialized values
        for (int i = 0; i < iserializers.Length; i++)
        {
            serializedData = SerializedData.Merge(serializedData, iserializers[i].Serialize());
        }
        return serializedData;
    }
}

[Serializable]
public enum ObjectType
{

}
