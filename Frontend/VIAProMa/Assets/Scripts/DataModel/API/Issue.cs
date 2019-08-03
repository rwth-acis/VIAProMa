using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Issue : IListViewItemData
{
    [SerializeField] private DataSource source;
    [SerializeField] private int id;
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private int projectId;
    [SerializeField] private User creator;
    [SerializeField] private IssueStatus status;
    [SerializeField] private string creationDate;
    [SerializeField] private string closedDate;
    [SerializeField] private User[] developers;
    [SerializeField] private User[] commenters;

    /// <summary>
    /// The data source of the issue
    /// </summary>
    public DataSource Source { get => source; }
    /// <summary>
    /// The id of the issue
    /// </summary>
    public int Id { get => id; }
    /// <summary>
    /// The name/ title of the issue
    /// </summary>
    public string Name { get => name; }
    /// <summary>
    /// A longer description of the issue's content
    /// </summary>
    public string Description { get => description; }
    /// <summary>
    /// The id of the project to which this issue belongs
    /// </summary>
    public int ProjectId { get => projectId; }
    /// <summary>
    /// The user which created the issue
    /// </summary>
    public User Creator { get => creator; }
    /// <summary>
    /// The status of the issue (open, in progress or closed)
    /// </summary>
    public IssueStatus Status { get => status; }
    /// <summary>
    /// The string representation of the point in time when the issue was created
    /// </summary>
    public string CreationDateString { get => creationDate; }
    /// <summary>
    /// The string representation of the point in time when the issue was closed
    /// </summary>
    public string ClosedDateString { get => closedDate; }
    /// <summary>
    /// The list of developers who are assigned to the issue and are working on it
    /// </summary>
    public User[] Developers { get => developers; }

    public User[] Commenters { get => commenters; }

    /// <summary>
    /// Deep-comparion between this issue and obj based on the issue's source and id
    /// </summary>
    /// <param name="obj">The object to compare to</param>
    /// <returns>True if obj is an issue and the source and ids match</returns>
    public override bool Equals(object obj)
    {
        Issue issue = obj as Issue;
        if (issue == null)
        {
            return false;
        }

        return (source == issue.source && id == issue.id);
    }

    /// <summary>
    /// Deep-comparion between this issue and the other issue obj based on the issue's source and id
    /// </summary>
    /// <param name="issue">The issue to compare to</param>
    /// <returns>True if the source and id of this issue and issue match</returns>
    public bool Equals(Issue issue)
    {
        if (issue == null)
        {
            return false;
        }
        return (source == issue.source && id == issue.id);
    }

    /// <summary>
    /// Gets a hash code of the issue object
    /// </summary>
    /// <returns>A has code</returns>
    public override int GetHashCode()
    {
        return (int)source ^ id;
    }
}
