using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonArrayUtility
{
    public static T[] FromJson<T>(string json)
    {
        JsonArrayWrapper<T> wrapper = JsonUtility.FromJson<JsonArrayWrapper<T>>(json);
        return wrapper.array;
    }

    public static string ToJson<T>(T[] array)
    {
        JsonArrayWrapper<T> wrapper = new JsonArrayWrapper<T>();
        wrapper.array = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        JsonArrayWrapper<T> wrapper = new JsonArrayWrapper<T>();
        wrapper.array = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    public static string EncapsulateInWrapper(string json)
    {
        string res = "{ \"array\": " + json + "}";
        return res;
    }

    [Serializable]
    private class JsonArrayWrapper<T>
    {
        public T[] array;
    }
}