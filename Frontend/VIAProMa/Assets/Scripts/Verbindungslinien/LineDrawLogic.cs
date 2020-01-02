using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LineDrawLogic : MonoBehaviour
{
    public GameObject caption;
    private bool isLineModeActivated;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        isLineModeActivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.SetActive(transform.parent.parent.parent.parent.parent.GetComponent<FoldController>().MenuOpen);
    }

    public void SwitchLineDrawMode()
    {
        if(isLineModeActivated)
        {
            caption.GetComponent<TextMeshPro>().SetText("Enter Line Draw");
        }
        else
        {
            caption.GetComponent<TextMeshPro>().SetText("Draw Line");
        }
        isLineModeActivated = !isLineModeActivated;
    }
}
