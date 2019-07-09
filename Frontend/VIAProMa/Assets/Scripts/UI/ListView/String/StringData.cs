using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StringData : IListViewItemData
{
    public string text;

    public StringData()
    {
    }

    public StringData(string text)
    {
        this.text = text;
    }
}
