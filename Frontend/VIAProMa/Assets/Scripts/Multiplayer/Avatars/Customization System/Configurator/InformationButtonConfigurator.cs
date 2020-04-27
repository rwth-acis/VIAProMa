using i5.ViaProMa.UI;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InformationButtonConfigurator : MonoBehaviour
{
    [SerializeField] private GameObject informationButton;
    // Start is called before the first frame update
    private void Start()
    {
        informationButton.SetActive(true);
    }

    public void Open()
    {
        informationButton.SetActive(true);
    }

    public void Close()
    {
        informationButton.SetActive(false);
    }

}