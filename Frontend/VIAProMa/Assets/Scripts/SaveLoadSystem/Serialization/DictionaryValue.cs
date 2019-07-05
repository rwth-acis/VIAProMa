using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DictionaryValue<T>
{
    [SerializeField] private string key;
    [SerializeField] private T value;

    public string Key { get => key; }
    public T Value { get => value; }

    public DictionaryValue(string key, T value)
    {
        this.key = key;
        this.value = value;
    }
}
