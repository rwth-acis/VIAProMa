using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VIAProMa.Shelves.IssueShelf;
using Org.Requirements_Bazaar.API;

public class ProjectTracker : MonoBehaviour
{
    private ShelfConfigurationMenu configurationMenu;
    private ReqBazShelfConfiguration reqBazShelfConfiguration;
    public int currentProjectID = 0;

    public void Start()
    {
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazProjectChanged += ProjectChanged;
    }


        public void ProjectChanged(object sender, System.EventArgs e)
    {
        configurationMenu = GameObject.FindObjectOfType<ShelfConfigurationMenu>();
        reqBazShelfConfiguration = (ReqBazShelfConfiguration)configurationMenu.ShelfConfiguration;
        currentProjectID = reqBazShelfConfiguration.SelectedProject.id;
    }


}
