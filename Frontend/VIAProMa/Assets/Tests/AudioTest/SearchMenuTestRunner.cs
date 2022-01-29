using i5.VIAProMa.UI.SearchMenu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchMenuTestRunner : MonoBehaviour
{

    public SearchMenu menu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            menu.gameObject.SetActive(true);
        }

    }
}
