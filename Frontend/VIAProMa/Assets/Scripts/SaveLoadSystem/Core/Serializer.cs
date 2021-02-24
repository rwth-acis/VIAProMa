using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Administers the serialization scrips on the GameObject
/// It distributes the serialization and deserialization calls to the scripts
/// </summary>
public class Serializer : MonoBehaviour
{

    /// <summary>
    /// The serialization scripts which are placed on the GameObject
    /// </summary>
    private ISerializable[] iserializers;
    /// <summary>
    /// The prefab name of this object
    /// Assumes that the name is not altered by another script
    /// </summary>
    private string prefabName;

    /// <summary>
    /// The instance ID of the object
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    /// Generates a new Id for the object and initializes the prefabName field
    /// </summary>
    private void Awake()
    {
        Id = Guid.NewGuid().ToString();
        prefabName = gameObject.name.Replace("(Clone)", "");
    }

    /// <summary>
    /// Registers this serializer script at the save load manager
    /// </summary>
    private void OnEnable()
    {
        SaveLoadManager.Instance.RegisterSerializer(this);
    }

    /// <summary>
    /// Called if the GameObject is destroyed
    /// Un-registers this serializer instance from the save load manager
    /// </summary>
    private void OnDisable()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.UnRegisterSerializer(this);
        }
    }

    /// <summary>
    /// Deserializes the serializedObject and applies its data to the GameObject
    /// This is done by distributing the serializedObject to the actual serialization scripts
    /// </summary>
    /// <param name="serializedObject">The serialized object which contains the data</param>
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

    /// <summary>
    /// Serializes the object
    /// This is done by collecting and merging the serialization data from the actual serialization scripts.
    /// </summary>
    /// <returns>The serialized object which contains the relevant save data of this GameObject</returns>
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
