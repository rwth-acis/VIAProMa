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

    public DataSource Source { get => source; }
    public int Id { get => id; }
    public string Name { get => name; }
    public string Description { get => description; }
    public int ProjectId { get => projectId; }
    public User Creator { get => creator; }
    public IssueStatus Status { get => status; }
    public string CreationDateString { get => creationDate; }
    public string ClosedDateString { get => closedDate; }
    public User[] Developers { get => developers; }

    public override bool Equals(object obj)
    {
        Issue issue = obj as Issue;
        if (issue == null)
        {
            return false;
        }

        return (source == issue.source && id == issue.id);
    }

    public bool Equals(Issue issue)
    {
        if (issue == null)
        {
            return false;
        }
        return (source == issue.source && id == issue.id);
    }

    public override int GetHashCode()
    {
        return (int)source ^ id;
    }
}
