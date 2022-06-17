using i5.VIAProMa.LiteratureSearch;
using i5.VIAProMa.ResourceManagagement;
using i5.VIAProMa.UI;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug : MonoBehaviour
{
    [SerializeField] private GameObject displayPrefab;
    [SerializeField] private GameObject literatureWindow;

    private GameObject displayInstance;
    private ObjectManipulator handlerOnCopy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnOpenWindowClick()
    {
        WindowManager.Instance.LiteratureSearchWindow.Open(transform.position + new Vector3(0, 0, -.1f), transform.eulerAngles);

    }
}
