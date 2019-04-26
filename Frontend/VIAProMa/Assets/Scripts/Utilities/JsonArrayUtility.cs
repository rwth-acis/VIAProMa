using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper script for serializing and de-serializing Json Arrays
/// Unity's built in Json Serializer is not able to handle JSON data which have an unnamed array at root level
/// </summary>
public static class JsonArrayUtility
{
    /// <summary>
    /// Converts a string to an array of the provided type
    /// </summary>
    /// <typeparam name="T">The array type to convert to</typeparam>
    /// <param name="json">The json string</param>
    /// <returns>Converted array of type T</returns>
    public static T[] FromJson<T>(string json)
    {
        JsonArrayWrapper<T> wrapper = JsonUtility.FromJson<JsonArrayWrapper<T>>(json);
        return wrapper.array;
    }

    /// <summary>
    /// Converts an array of type T to a json string
    /// </summary>
    /// <typeparam name="T">The type of the array</typeparam>
    /// <param name="array">Array to convert to json data</param>
    /// <returns>JSON string</returns>
    public static string ToJson<T>(T[] array)
    {
        JsonArrayWrapper<T> wrapper = new JsonArrayWrapper<T>();
        wrapper.array = array;
        return JsonUtility.ToJson(wrapper);
    }

    /// <summary>
    /// Converts an array of type T to a json string
    /// </summary>
    /// <typeparam name="T">The type of the array</typeparam>
    /// <param name="array">Array to conver to json data</param>
    /// <param name="prettyPrint">If true, the output will be printed in a way that is more human-readable</param>
    /// <returns>Json string</returns>
    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        JsonArrayWrapper<T> wrapper = new JsonArrayWrapper<T>();
        wrapper.array = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    /// <summary>
    /// Helper function to encapsulate a json string which has an unnamed json array at root level in such a way that it can be converted using FromJson()
    /// </summary>
    /// <param name="json">The original json string</param>
    /// <returns>The encapsulated json string which is now ready for json array de-serialization</returns>
    public static string EncapsulateInWrapper(string json)
    {
        string res = "{ \"array\": " + json + "}";
        return res;
    }

    /// <summary>
    /// Help class which is used for serializing and de-serializing unnamed json arrays at root level
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    private class JsonArrayWrapper<T>
    {
        /// <summary>
        /// contains the json array data
        /// </summary>
        public T[] array;
    }
}