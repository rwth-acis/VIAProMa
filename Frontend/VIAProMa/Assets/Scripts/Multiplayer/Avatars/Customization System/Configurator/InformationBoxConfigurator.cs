﻿using Microsoft.MixedReality.Toolkit.UI;
using i5.ViaProMa.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InformationBoxConfigurator : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject informationBox;
    [SerializeField] private Interactable BarchartButton;
    [SerializeField] private Interactable ScatterplotButton;
    [SerializeField] private Interactable ProgressButton;


    [Header("References")]
    [SerializeField] private GameObject BarchartPrefab;
    [SerializeField] private GameObject ScatterplotPrefab;
    [SerializeField] private GameObject ProgressBarPrefab;

    // instances
    private GameObject table;
    private GameObject barchartInstance;
    private GameObject scatterplotInstance;
    private GameObject progressbarInstance;
    private PhotonView photonView;
    private string name;
    
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        BarchartButton.Enabled = true;
        ScatterplotButton.Enabled = true;
        ProgressButton.Enabled = true;
        informationBox.SetActive(false);
        if (photonView != null)
        {
            name = photonView.Owner.NickName;
        }
        else
        {
            name = "Wendy";
            Debug.Log("photonview error");
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        table = GameObject.Find("Table(Clone)");
        informationBox.SetActive(false);

        if (UserManager.Instance.UserRole != UserRoles.TUTOR)
        {
            Debug.Log("button false");
            BarchartButton.Enabled = false;
            ScatterplotButton.Enabled = false;
        }
    }

    public void Open()
    {
        informationBox.SetActive(true);
    }

    public void Close()
    {
        informationBox.SetActive(false);
    }

    private void InstantiateControl(GameObject prefab, ref GameObject instance, Vector3 targetPosition)
    {
        Quaternion targetRotation = Quaternion.identity;

        if (instance != null)
        {
            instance.SetActive(true);
            instance.transform.position = targetPosition;
            instance.transform.rotation = targetRotation;
        }
        else
        {
            instance = ResourceManager.Instance.NetworkInstantiate(prefab, targetPosition, targetRotation);
            instance.transform.parent = table.transform;
        }
    }

    public void BarchartButtonClicked()
    {
        Debug.Log("Barchart Button Clicked");
        Vector3 targetPosition = table.transform.position;
        targetPosition.y += 1.2f;
        //targetPosition.x -= 1f;
        targetPosition.z += 0.5f;
        InstantiateControl(BarchartPrefab, ref barchartInstance, targetPosition);
        BarchartSynchronizer synch = (BarchartSynchronizer) barchartInstance.GetComponent(typeof(BarchartSynchronizer));
        synch.Initial(name);
        Close();
    }
    
    public void ScatterplotButtonClicked()
    {
        Debug.Log("Scatterplot Button Clicked");
        Vector3 targetPosition = table.transform.position;
        targetPosition.y += 1.2f;
        targetPosition.x -= 1f;
        targetPosition.z += 0.5f;
        InstantiateControl(ScatterplotPrefab, ref scatterplotInstance, targetPosition);
        ScatterSynchronizer synch = (ScatterSynchronizer) scatterplotInstance.GetComponent(typeof(ScatterSynchronizer));
        synch.Initial(name);
        Close();
    }

    public void ProgressBarButtonClicked()
    {
        Debug.Log("ProgressBar Button Clicked");
        Vector3 targetPosition = table.transform.position;
        targetPosition.y += 1.22f;
        targetPosition.x -= 2f;
        targetPosition.z += 0.5f;
        InstantiateControl(ProgressBarPrefab, ref progressbarInstance, targetPosition);
        ProgressSynchronizer synch = (ProgressSynchronizer) progressbarInstance.GetComponent(typeof(ProgressSynchronizer));
        synch.Initial(name);
        Close();
    }

}