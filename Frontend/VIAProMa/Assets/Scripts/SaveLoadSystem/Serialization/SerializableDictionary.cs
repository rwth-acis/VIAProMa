using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<T>
{
    [SerializeField] private List<string> keys;
    [SerializeField] private List<T> values;

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

    public Dictionary<string, T> ToDictionary()
    {
        Dictionary<string, T> res = new Dictionary<string, T>();
        if (keys.Count != values.Count)
        {
            Debug.LogWarning("Converting SerializableDictionary to Dictionary may miss some values because key and value lists have different lengths. This should not happen.");
        }

        for (int i=0;i<Math.Min(keys.Count, values.Count); i++)
        {
            res.Add(keys[i], values[i]);
        }
        return res;
    }

    public static SerializableDictionary<T> FromDictionary(Dictionary<string, T> dictionary)
    {
        SerializableDictionary<T> res = new SerializableDictionary<T>(dictionary);
        return res;
    }
}

[Serializable]
public class SerializableDictionaryInt : SerializableDictionary<int>
{
    public SerializableDictionaryInt(Dictionary<string, int> originalDictionary) : base(originalDictionary)
    { }
}

[Serializable]
public class SerializableDictionaryString : SerializableDictionary<string>
{
    public SerializableDictionaryString(Dictionary<string, string> originalDictionary) : base(originalDictionary)
    { }
}

[Serializable]
public class SerializableDictionaryFloat : SerializableDictionary<float>
{
    public SerializableDictionaryFloat(Dictionary<string, float> originalDictionary) : base(originalDictionary)
    { }
}

[Serializable]
public class SerializableDictionaryBool : SerializableDictionary<bool>
{
    public SerializableDictionaryBool(Dictionary<string, bool> originalDictionary) : base(originalDictionary)
    { }
}

[Serializable]
public class SerializableDictionaryVector3 : SerializableDictionary<Vector3>
{
    public SerializableDictionaryVector3(Dictionary<string, Vector3> originalDictionary) : base(originalDictionary)
    { }
}

[Serializable]
public class SerializableDictionaryQuaternion : SerializableDictionary<Quaternion>
{
    public SerializableDictionaryQuaternion(Dictionary<string, Quaternion> originalDictionary) : base(originalDictionary)
    { }
}