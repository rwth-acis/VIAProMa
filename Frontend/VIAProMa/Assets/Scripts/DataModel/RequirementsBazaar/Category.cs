using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Category : IListViewItemData
{
    public int id;
    public string name;
    public string description;
    public int projectId;
    public User leader;
    public string creationDate;
    public string lastUpdatedDate;
    public int numberOfRequirements;
    public int numberOfFollowers;
}
