using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A dictionary data structure with string keys which can be serialized by Unity
/// Internally, the dictionary is serialized into two lists of the same length: one key list with the string keys and one value list
/// </summary>
/// <typeparam name="T">The type of the values</typeparam>
[Serializable]
public class SerializableDictionary<T>
{
    [SerializeField] private List<string> keys;
    [SerializeField] private List<T> values;

    /// <summary>
    /// Creates a serializable dictionary object by copying an existing dictionary
    /// </summary>
    /// <param name="originalDictionary">The original dictionary which should be copied</param>
    public SerializableDictionary(Dictionary<string, T> originalDictionary)
    {
        keys = new List<string>();
        values = new List<T>();
        foreach(KeyValuePair<string, T> pair in originalDictionary)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    /// <summary>
    /// Converts a SerializableDictionary to a native string-to-T dictionary 
    /// </summary>
    /// <returns>The native dictionary with the same values contained in the SerializableDictionary</returns>
    public Dictionary<string, T> ToDictionary()
    {
        Dictionary<string, T> res = new Dictionary<string, T>();
        // check if the dictionary is valid
        if (keys.Count != values.Count)
        {
            Debug.LogWarning("Converting SerializableDictionary to Dictionary may miss some values because key and value lists have different lengths. This should not happen.");
        }

        // add its values to a native dictionary
        for (int i=0;i<Math.Min(keys.Count, values.Count); i++)
        {
            res.Add(keys[i], values[i]);
        }
        return res;
    }

    /// <summary>
    /// Creates a SerializableDictionary from a native dictionary
    /// </summary>
    /// <param name="dictionary">The string-to-T dictionary which should be converted to a SerializableDictionary</param>
    /// <returns>The SerializableDictionary with the same key-value paris as the input dictionary</returns>
    public static SerializableDictionary<T> FromDictionary(Dictionary<string, T> dictionary)
    {
        SerializableDictionary<T> res = new SerializableDictionary<T>(dictionary);
        return res;
    }
}

/// <summary>
/// Integer-instance of the SerializableDictionary
/// </summary>
[Serializable]
public class SerializableDictionaryInt : SerializableDictionary<int>
{
    public SerializableDictionaryInt(Dictionary<string, int> originalDictionary) : base(originalDictionary)
    { }
}

/// <summary>
/// String-instace of the SerializableDictionary
/// </summary>
[Serializable]
public class SerializableDictionaryString : SerializableDictionary<string>
{
    public SerializableDictionaryString(Dictionary<string, string> originalDictionary) : base(originalDictionary)
    { }
}

/// <summary>
/// float-instace of the SerializableDictionary
/// </summary>
[Serializable]
public class SerializableDictionaryFloat : SerializableDictionary<float>
{
    public SerializableDictionaryFloat(Dictionary<string, float> originalDictionary) : base(originalDictionary)
    { }
}

/// <summary>
/// Bool-instace of the SerializableDictionary
/// </summary>
[Serializable]
public class SerializableDictionaryBool : SerializableDictionary<bool>
{
    public SerializableDictionaryBool(Dictionary<string, bool> originalDictionary) : base(originalDictionary)
    { }
}

/// <summary>
/// Vector3-instace of the SerializableDictionary
/// </summary>
[Serializable]
public class SerializableDictionaryVector3 : SerializableDictionary<Vector3>
{
    public SerializableDictionaryVector3(Dictionary<string, Vector3> originalDictionary) : base(originalDictionary)
    { }
}

/// <summary>
/// Quaternion-instace of the SerializableDictionary
/// </summary>
[Serializable]
public class SerializableDictionaryQuaternion : SerializableDictionary<Quaternion>
{
    public SerializableDictionaryQuaternion(Dictionary<string, Quaternion> originalDictionary) : base(originalDictionary)
    { }
}