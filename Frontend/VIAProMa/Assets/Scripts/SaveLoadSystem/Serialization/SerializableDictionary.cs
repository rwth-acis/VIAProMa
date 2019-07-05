using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<T>
{
    [SerializeField] private List<DictionaryValue<T>> keyValues;

    public SerializableDictionary(Dictionary<string, T> originalDictionary)
    {
        keyValues = new List<DictionaryValue<T>>();
        foreach(KeyValuePair<string, T> pair in originalDictionary)
        {
            keyValues.Add(new DictionaryValue<T>(pair.Key, pair.Value));
        }
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