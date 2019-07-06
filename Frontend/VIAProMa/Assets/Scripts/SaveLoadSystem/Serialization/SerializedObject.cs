using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedObject
{
    [SerializeField] private string id;
    [SerializeField] private string prefabName;

    [SerializeField] private SerializableDictionaryInt integers;
    [SerializeField] private SerializableDictionaryString strings;
    [SerializeField] private SerializableDictionaryFloat floats;
    [SerializeField] private SerializableDictionaryBool bools;
    [SerializeField] private SerializableDictionaryVector3 vector3s;
    [SerializeField] private SerializableDictionaryQuaternion quaternions;

    public string Id { get => id; set { id = value; } }
    public string PrefabName { get => prefabName; set { prefabName = value; } }

    public Dictionary<string, int> Integers { get; private set; }
    public Dictionary<string, string> Strings { get; private set; }
    public Dictionary<string, float> Floats { get; private set; }
    public Dictionary<string, bool> Bools { get; private set; }
    public Dictionary<string, Vector3> Vector3s { get; private set; }
    public Dictionary<string, Quaternion> Quaternions { get; private set; }

    public SerializedObject()
    {
        Integers = new Dictionary<string, int>();
        Strings = new Dictionary<string, string>();
        Floats = new Dictionary<string, float>();
        Bools = new Dictionary<string, bool>();
        Vector3s = new Dictionary<string, Vector3>();
        Quaternions = new Dictionary<string, Quaternion>();
    }

    public void PackData()
    {
        integers = new SerializableDictionaryInt(Integers);
        strings = new SerializableDictionaryString(Strings);
        floats = new SerializableDictionaryFloat(Floats);
        bools = new SerializableDictionaryBool(Bools);
        vector3s = new SerializableDictionaryVector3(Vector3s);
        quaternions = new SerializableDictionaryQuaternion(Quaternions);
    }

    public void UnPackData()
    {
        Integers = integers.ToDictionary();
        Strings = strings.ToDictionary();
        Floats = floats.ToDictionary();
        Bools = bools.ToDictionary();
        Vector3s = vector3s.ToDictionary();
        Quaternions = quaternions.ToDictionary();
    }

    public static SerializedObject Merge(SerializedObject data1, SerializedObject data2)
    {
        data1.Integers = MergeDictionary(data1.Integers, data2.Integers);
        data1.Strings = MergeDictionary(data1.Strings, data2.Strings);
        data1.Floats = MergeDictionary(data1.Floats, data2.Floats);
        data1.Bools = MergeDictionary(data1.Bools, data2.Bools);
        data1.Vector3s = MergeDictionary(data1.Vector3s, data2.Vector3s);
        data1.Quaternions = MergeDictionary(data1.Quaternions, data2.Quaternions);
        return data1;
    }

    private static Dictionary<string, T> MergeDictionary<T>(Dictionary<string, T> dictionary1, Dictionary<string, T> dictionary2)
    {
        foreach(KeyValuePair<string, T> entryDictionary2 in dictionary2)
        {
            if (dictionary1.ContainsKey(entryDictionary2.Key))
            {
                Debug.LogError("Error merging save data: duplicate key " + entryDictionary2.Key);
            }
            else
            {
                dictionary1.Add(entryDictionary2.Key, entryDictionary2.Value);
            }
        }
        return dictionary1;
    }
}
