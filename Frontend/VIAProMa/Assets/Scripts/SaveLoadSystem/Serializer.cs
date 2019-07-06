using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serializer : MonoBehaviour
{

    private ISerializable[] iserializers;
    private string prefabName;

    public string Id { get; private set; }

    private void Awake()
    {
        Id = Guid.NewGuid().ToString();
        prefabName = gameObject.name.Replace("(Clone)", "");
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

    public virtual void Deserialize(SerializedObject serializedObject)
    {
        Id = serializedObject.Id;

        // get the serializers on the object
        iserializers = GetComponentsInChildren<ISerializable>(true);

        for (int i=0; i<iserializers.Length; i++)
        {
            iserializers[i].Deserialize(serializedObject);
        }
    }

    public virtual SerializedObject Serialize()
    {
        // get the serializers on the object
        iserializers = GetComponentsInChildren<ISerializable>(true);

        // create a dictionary which stores the values which should be serialized
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Id = Id;
        serializedObject.PrefabName = prefabName;
        // serialize each serializer and add the result to the dictionary of serialized values
        for (int i = 0; i < iserializers.Length; i++)
        {
            serializedObject = SerializedObject.Merge(serializedObject, iserializers[i].Serialize());
        }
        return serializedObject;
    }
}
