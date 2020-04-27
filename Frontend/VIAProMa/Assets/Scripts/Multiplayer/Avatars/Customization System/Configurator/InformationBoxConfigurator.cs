using i5.ViaProMa.UI;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InformationBoxConfigurator : MonoBehaviour
{
    [SerializeField] private GameObject informationBox;
    // Start is called before the first frame update
    private void Start()
    {
        informationBox.SetActive(false);
    }

    public void Open()
    {
        informationBox.SetActive(true);
    }

    public void Close()
    {
        informationBox.SetActive(false);
    }

}