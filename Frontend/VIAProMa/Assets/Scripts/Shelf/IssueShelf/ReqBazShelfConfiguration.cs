using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReqBazShelfConfiguration : IShelfConfiguration
{
    public bool IsValidConfiguration
    {
        get { return SelectedProject != null; } // the configuration is valid if a project was selected (stays valid if no category was selected)
    }

    public ReqBazShelfConfiguration()
    {
        SelectedSource = DataSource.REQUIREMENTS_BAZAAR;
    }

    public ReqBazShelfConfiguration(Project selectedProject) : this()
    {
        SelectedProject = selectedProject;
    }

    public ReqBazShelfConfiguration(Project selectedProject, Category selectedCategory) : this(selectedProject)
    {
        SelectedCategory = selectedCategory;
    }

    public DataSource SelectedSource { get; private set; }

    public Project SelectedProject { get; set; }

    public Category SelectedCategory { get; set; }
}
