using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a category in the Requirements Bazaar
/// </summary>
[Serializable]
public class Category : IListViewItemData
{
    public int id;
    public string name;
    public string description;
    public int projectId;
    public ReqBazUser leader;
    public string creationDate;
    public string lastUpdatedDate;
    public int numberOfRequirements;
    public int numberOfFollowers;

    public Category()
    {
    }

    public Category(int id, string name)
    {
        this.id = id;
        this.name = name;
    }
}
