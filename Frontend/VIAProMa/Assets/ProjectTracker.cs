using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VIAProMa.Shelves.IssueShelf;
using Org.Requirements_Bazaar.API;
using i5.VIAProMa.DataModel.ReqBaz;

public class ProjectTracker : MonoBehaviour
{
    private ShelfConfigurationMenu configurationMenu;
    private ReqBazShelfConfiguration reqBazShelfConfiguration;
    public int currentProjectID = 0;
    public Category currentCategory = null;

    public void Start()
    {
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazProjectChanged += ProjectChanged;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazCategoryChanged += CategoryChanged;
    }


        public void ProjectChanged(object sender, System.EventArgs e)
    {
        configurationMenu = GameObject.FindObjectOfType<ShelfConfigurationMenu>();
        reqBazShelfConfiguration = (ReqBazShelfConfiguration)configurationMenu.ShelfConfiguration;
        currentProjectID = reqBazShelfConfiguration.SelectedProject.id;
    }


    public void CategoryChanged(object sender, System.EventArgs e)
    {
        configurationMenu = GameObject.FindObjectOfType<ShelfConfigurationMenu>();
        reqBazShelfConfiguration = (ReqBazShelfConfiguration)configurationMenu.ShelfConfiguration;
        currentCategory = reqBazShelfConfiguration.SelectedCategory;
    }


}
