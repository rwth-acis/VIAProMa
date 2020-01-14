using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionLinesMenu : MonoBehaviour
{
    public GameObject connectionMenu;

    private bool isConnectionMenuOpen;

    void Start()
    {
        isConnectionMenuOpen = false;
        connectionMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// The Connection Menu appears when the Menu is open and are deactivated when it is closed.
    /// </summary>
    void Update()
    {
        if (isConnectionMenuOpen)
        {
            connectionMenu.SetActive(transform.parent.parent.parent.parent.parent.parent.parent.GetComponent<FoldController>().MenuOpen);
        }
        if (!transform.parent.parent.parent.parent.parent.parent.parent.GetComponent<FoldController>().MenuOpen)
        {
            isConnectionMenuOpen = false;
            connectionMenu.gameObject.SetActive(false);
        }
    }

    public void SwitchMenuMode()
    {
        connectionMenu.gameObject.SetActive(!isConnectionMenuOpen);
        isConnectionMenuOpen = !isConnectionMenuOpen;
    }
}
