using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseArrowColor : MonoBehaviour
{
    public Material mat;

    public void toggleColor()
    {
        mat.color = Color.blue;
    }
}
