using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class File : MonoBehaviour
{
    [SerializeField] TextMeshPro titleOnBack;
    [SerializeField] TextMeshPro titleOnTop;

    private string projectTitle;

    public string ProjectTitle
    {
        get
        {
            return projectTitle;
        }
        set
        {
            projectTitle = value;
            titleOnBack.text = projectTitle;
            titleOnTop.text = projectTitle;
        }
    }
}
