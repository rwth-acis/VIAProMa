using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a requirement in the Requirements Bazaar
/// </summary>
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
    public int numberOfComments;
    public int numberOfAttachments;
    public int numberOfFollowers;
    public int upVotes;
    public int downVotes;
    public string userVoted;
}
