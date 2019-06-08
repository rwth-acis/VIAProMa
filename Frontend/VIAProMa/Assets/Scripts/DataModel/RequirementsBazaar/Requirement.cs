using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Requirement : IListViewItemData
{
    public int id;
    public string name;
    public string description;
    public int projectId;
    public User creator;
    public Category[] categories;
    public string creationDate;
    public string lastUpdatedDate;
}
