using i5.VIAProMa.Shelves.IssueShelf;
using i5.VIAProMa.Shelves.ProjectLoadShelf;
using i5.VIAProMa.Shelves.Visualizations;
using i5.VIAProMa.UI.MainMenuCube;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchMenu : MonoBehaviour
{
    public enum SearchTargets
    {
        MainMenu, IssueShelf, VisualizationShelf, ProjectShelf, IssueCard, Visualization, Other
    }

    [SerializeField] private InteractableToggleCollection radialMenu;
    [SerializeField] private GameObject searchWindow;

    public void OnSelectRadial()
    {
        int index = radialMenu.CurrentIndex;
        if (radialMenu.ToggleList[index].enabled == false) return;

        SearchTargets target = (SearchTargets)index;
        switch(target) {
            case SearchTargets.MainMenu:
            case SearchTargets.IssueShelf:
            case SearchTargets.VisualizationShelf:
            case SearchTargets.ProjectShelf:
                PingSingleton(target);
                break;
            case SearchTargets.IssueCard:
                ShowSearchMenu();
                break;
            case SearchTargets.Visualization:
                ShowSearchMenu();
                break;
            case SearchTargets.Other:
                ShowSearchMenu();
                break;
            default:
                break;
        }

    }

    public void ShowSearchMenu()
    {
        searchWindow.SetActive(true);
    }
    public void HideSearchMenu()
    {
        searchWindow.SetActive(false);
    }

    public void CloseMenu()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Pings an object that is only once in the scene (singleton).
    /// </summary>
    /// <param name="target"></param>
    private void PingSingleton(SearchTargets target)
    {
        MonoBehaviour at = null;
        if (target == SearchTargets.MainMenu) at = FindObjectOfType<MainMenu>();
        if (target == SearchTargets.IssueShelf) at = FindObjectOfType<IssuesLoader>();
        if (target == SearchTargets.VisualizationShelf) at = FindObjectOfType<VisualizationShelf>();
        if (target == SearchTargets.ProjectShelf) at = FindObjectOfType<ProjectLoader>();

        if(at == null || at.enabled == false)
        {
            AudioManager.instance.PlayErrorSound(transform.position);
            return;
        }

        AudioManager.instance.PlayerPingSound(at.gameObject.transform.position);
    }

    

}
