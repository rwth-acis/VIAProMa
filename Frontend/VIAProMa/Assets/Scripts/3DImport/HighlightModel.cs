using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static SessionBrowserRefresher;

public class HighlightModel : MonoBehaviour
{
    public GameObject model;
    private bool highlighted;

    private void Awake()
    {
        highlighted = false;
    }

    public void HighlightObject()
    {
        if (highlighted)
        {
            Renderer[] rr = model.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in rr)
            {
                r.material.SetColor("_Color", Color.white);
            }
            gameObject.transform.parent.GetChild(0).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.white);
            GetComponentInChildren<TextMeshPro>().color = Color.white;
            GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.white);
            model.tag = "Untagged";
            highlighted = false;
        }
        else
        {
            Renderer[] rr = model.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in rr)
            {
                r.material.SetColor("_Color", Color.yellow);
            }
            gameObject.transform.parent.GetChild(0).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.yellow);
            GetComponentInChildren<TextMeshPro>().color = Color.yellow;
            GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.yellow);
            model.tag = "Highlighted";
            highlighted = true;
        }
    }
}
