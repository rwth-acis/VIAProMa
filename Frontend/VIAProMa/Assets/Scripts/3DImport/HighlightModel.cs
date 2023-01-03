using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SessionBrowserRefresher;

public class HighlightModel : MonoBehaviour
{
    public GameObject model;
    private bool highlighted;

    private void Start()
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
            highlighted = false;
        }
        else
        {
            Renderer[] rr = model.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in rr)
            {
                r.material.SetColor("_Color", Color.yellow);
            }
            highlighted = true;
        }
    }
}
