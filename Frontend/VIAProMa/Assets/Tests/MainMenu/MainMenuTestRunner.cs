using i5.VIAProMa.UI.MainMenuCube;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuTestRunner : MonoBehaviour
{
    public FoldController cubeFoldController;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (cubeFoldController.MenuOpen)
            {
                cubeFoldController.FoldCube();
            }
            else
            {
                cubeFoldController.UnFoldCube();
            }
        }
    }
}
