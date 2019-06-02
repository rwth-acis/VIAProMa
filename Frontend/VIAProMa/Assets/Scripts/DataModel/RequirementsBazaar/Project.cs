using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Project
{
    public int id;
    public string name;
    public string description;
    public bool visibility;
    public int defaultCategoryId;
    public User leader;
    public string creationDate;
    public string lastUpdatedDate;
    public int numberOfCategories;
    public int numberOfRequirements;
    public int numberOfFollowers;
    public bool isFollower;
}
