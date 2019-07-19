using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GitHubShelfConfiguration : IShelfConfiguration
{
    public DataSource SelectedSource { get; private set; }

    public string Owner { get; set; }

    public string RepositoryName { get; set; }

    public GitHubShelfConfiguration()
    {
        SelectedSource = DataSource.GITHUB;
    }

    public GitHubShelfConfiguration(string owner, string repository) : this()
    {
        Owner = owner;
        RepositoryName = repository;
    }
}
