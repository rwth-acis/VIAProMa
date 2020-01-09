using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shareGaze2 : MonoBehaviour
{
    public Material mat;

    public void toggleColor()
    {
        if (mat.color == Color.black)
        {
            mat.color = Color.blue;
        }
        else { mat.color = Color.black; }
    }
}
