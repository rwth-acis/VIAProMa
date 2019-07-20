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
    [SerializeField] private string owner;
    [SerializeField] private string repositoryName;
    [SerializeField] private User creator;
    [SerializeField] private IssueStatus status;
    [SerializeField] private User[] developers;

    public DataSource Source { get => source; }
    public int Id { get => id; }
    public string Name { get => name; }
    public string Description { get => description; }
    public int ProjectId { get => projectId; }
    public string Owner { get => owner;}
    public string RepositoryName { get => repositoryName; }
    public User Creator { get => creator; }
    public IssueStatus Status { get => status; }
    public User[] Developers { get => developers; }

    public Issue(DataSource source, int id, string name, string description, int projectId, User creator, IssueStatus status, User[] developers)
    {
        this.source = source;
        this.id = id;
        this.name = name;
        this.description = description;
        this.projectId = projectId;
        this.creator = creator;
        this.status = status;
        this.developers = developers;
    }

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
