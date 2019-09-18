using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data structure for serialized save data
/// </summary>
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

    /// <summary>
    /// The id of the stored object
    /// This is used in order to address the object again in the load procedure
    /// </summary>
    public string Id { get => id; set { id = value; } }

    /// <summary>
    /// The prefab name of the object
    /// This is used to instantiate the object again
    /// </summary>
    public string PrefabName { get => prefabName; set { prefabName = value; } }

    /// <summary>
    /// Collection of stored integer values
    /// </summary>
    public Dictionary<string, int> Integers { get; private set; }
    /// <summary>
    /// Collection of stored string values
    /// </summary>
    public Dictionary<string, string> Strings { get; private set; }
    /// <summary>
    /// Collection of stored float values
    /// </summary>
    public Dictionary<string, float> Floats { get; private set; }
    /// <summary>
    /// Collection of stored Boolean values
    /// </summary>
    public Dictionary<string, bool> Bools { get; private set; }
    /// <summary>
    /// Collection of stored vectors
    /// </summary>
    public Dictionary<string, Vector3> Vector3s { get; private set; }
    /// <summary>
    /// Collection of stored rotations
    /// </summary>
    public Dictionary<string, Quaternion> Quaternions { get; private set; }

    /// <summary>
    /// Creates the serialized object and intializes its collections
    /// </summary>
    public SerializedObject()
    {
        Integers = new Dictionary<string, int>();
        Strings = new Dictionary<string, string>();
        Floats = new Dictionary<string, float>();
        Bools = new Dictionary<string, bool>();
        Vector3s = new Dictionary<string, Vector3>();
        Quaternions = new Dictionary<string, Quaternion>();
    }

    /// <summary>
    /// Writes the data into the serialized fields
    /// Use this method before serializing this object to a string or binary format
    /// </summary>
    public void PackData()
    {
        integers = new SerializableDictionaryInt(Integers);
        strings = new SerializableDictionaryString(Strings);
        floats = new SerializableDictionaryFloat(Floats);
        bools = new SerializableDictionaryBool(Bools);
        vector3s = new SerializableDictionaryVector3(Vector3s);
        quaternions = new SerializableDictionaryQuaternion(Quaternions);
    }

    /// <summary>
    /// Writes the data from the serialized fields into the properties
    /// Use this method after deserializing a string or binary format in the SerializedObject
    /// </summary>
    public void UnPackData()
    {
        Integers = integers.ToDictionary();
        Strings = strings.ToDictionary();
        Floats = floats.ToDictionary();
        Bools = bools.ToDictionary();
        Vector3s = vector3s.ToDictionary();
        Quaternions = quaternions.ToDictionary();
    }

    /// <summary>
    /// Adds a list with the key to a given dictionary
    /// </summary>
    /// <typeparam name="T">The type which is stored in the list</typeparam>
    /// <param name="key">The key which should be used to store the list</param>
    /// <param name="list">The list which should be stored</param>
    /// <param name="target">The target dictionary to which the list should be written</param>
    public static void AddList<T>(string key, List<T> list, Dictionary<string, T> target)
    {
        for (int i=0;i<list.Count;i++)
        {
            target.Add(ConstructKey(key, i), list[i]);
        }
    }

    /// <summary>
    /// Gets a list from the given dictionary
    /// </summary>
    /// <typeparam name="T">The type which is stored in the list</typeparam>
    /// <param name="key">The key under which the list is stored</param>
    /// <param name="target">The dictionary which contains the list</param>
    public static List<T> GetList<T>(string key, Dictionary<string, T> target)
    {
        List<T> res = new List<T>();

        for (int i=0;i<int.MaxValue;i++) // breaks if no more elements were found
        {
            string indexedKey = ConstructKey(key, i);
            if (target.ContainsKey(indexedKey))
            {
                res.Add(target[indexedKey]);
            }
            else
            {
                // list finished (gaps are not allowed)
                break;
            }
        }
        return res;
    }

    private static string ConstructKey(string key, int index)
    {
        return key + "#" + index;
    }

    /// <summary>
    /// Merges two SerializedObjects into one
    /// The SerializedObjects should not have the same key in the same collection
    /// </summary>
    /// <param name="data1">The first SerializedObject</param>
    /// <param name="data2">The second SerializedObject</param>
    /// <returns>The merged SerializedObject which contains all keys from both data objects</returns>
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

    /// <summary>
    /// Merges two dictionary collections of the Serialized Objects
    /// The two dictionaries should have distinct keys
    /// If the same key is contained in both dictionaries, the value of dictionary1 will be chosen
    /// </summary>
    /// <typeparam name="T">The type of the dictionary</typeparam>
    /// <param name="dictionary1">The first dictionary collection</param>
    /// <param name="dictionary2">The second dictionary collection</param>
    /// <returns>The merged dictionary with the key-value pairs of both dictionaries.</returns>
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
