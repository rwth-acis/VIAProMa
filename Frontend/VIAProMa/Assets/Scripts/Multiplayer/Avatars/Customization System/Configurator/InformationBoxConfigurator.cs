using i5.VIAProMa;
using Microsoft.MixedReality.Toolkit.UI;
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
    //[SerializeField] private Interactable BarchartButton;
    //[SerializeField] private Interactable ScatterplotButton;
    //[SerializeField] private Interactable ProgressButton;

    [SerializeField] private GameObject BarchartButton;
    [SerializeField] private GameObject ScatterplotButton;
    [SerializeField] private GameObject ProgressButton;

    /*
    [Header("References")]
    [SerializeField] private GameObject BarchartPrefab;
    [SerializeField] private GameObject ScatterplotPrefab;
    [SerializeField] private GameObject ProgressBarPrefab;
    */

    // organ-synchronizer
    private BOSynchronizer bosynchronizer;
    private SOSynchronizer sosynchronizer;
    private POSynchronizer posynchronizer;

    // instances
    private GameObject table;
    public GameObject barchartInstance; //{ get; set; } = null;
    public GameObject scatterplotInstance; //{ get; set; } = null;
    public GameObject progressbarInstance; //{ get; set; } = null;
    //private PhotonView photonView;
    public string name { get; set; } = "";
    
    private void Awake()
    {
        //photonView = GetComponent<PhotonView>();
        //BarchartButton.Enabled = true;
        //ScatterplotButton.Enabled = true;
        //ProgressButton.Enabled = true;
        informationBox.SetActive(false);
    }

    // Start is called before the first frame update
    private void Start()
    {
        table = GameObject.Find("Table(Clone)");

        //organizer-synchronizer
        bosynchronizer = table.GetComponent<BOSynchronizer>();
        sosynchronizer = table.GetComponent<SOSynchronizer>();
        posynchronizer = table.GetComponent<POSynchronizer>();

        informationBox.SetActive(false);

        if (UserManager.Instance.UserRole != UserRoles.TUTOR)
        {
            Debug.Log("button false");
            //BarchartButton.Enabled = false;
            //ScatterplotButton.Enabled = false;
            BarchartButton.SetActive(false);
            ScatterplotButton.SetActive(false);
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


    private void InstantiateControl(string prefab, ref GameObject instance, Vector3 targetPosition)
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
            instance = PhotonNetwork.InstantiateSceneObject(prefab, targetPosition, targetRotation);
            instance.transform.parent = table.transform;
        }
    }


    public void BarchartButtonClick()
    {
        Debug.Log("Barchart Button Clicked");
        if (barchartInstance == null)
        {
            bosynchronizer.SendClear();
            Vector3 targetPosition = table.transform.position;
            targetPosition.y += 1.5f;
            targetPosition.x += 0.78f;
            targetPosition.z -= 0.5f;
            //InstantiateControl(BarchartPrefab, ref barchartInstance, targetPosition);
            InstantiateControl("bar", ref barchartInstance, targetPosition);
            BarchartSynchronizer synch = (BarchartSynchronizer) barchartInstance.GetComponent(typeof(BarchartSynchronizer));
            synch.Initial(name);
        }
    }
    
    public void ScatterplotButtonClick()
    {
        Debug.Log("Scatterplot Button Clicked");
        if (scatterplotInstance == null)
        {
            sosynchronizer.SendClear();
            Vector3 targetPosition = table.transform.position;
            targetPosition.y += 1.5f;
            //targetPosition.x += 0.7f;
            targetPosition.z -= 0.5f;
            //InstantiateControl(ScatterplotPrefab, ref scatterplotInstance, targetPosition);
            InstantiateControl("scatter", ref scatterplotInstance, targetPosition);
            ScatterSynchronizer synch = (ScatterSynchronizer) scatterplotInstance.GetComponent(typeof(ScatterSynchronizer));
            synch.Initial(name);
        }
    }

    public void ProgressBarButtonClick()
    {
        Debug.Log("ProgressBar Button Clicked");
        if (progressbarInstance == null)
        {
            posynchronizer.SendClear();
            Vector3 targetPosition = table.transform.position;
            targetPosition.y += 1.5f;
            targetPosition.x -= 0.78f;
            targetPosition.z -= 0.5f;
            //InstantiateControl(ProgressBarPrefab, ref progressbarInstance, targetPosition);
            InstantiateControl("progress", ref progressbarInstance, targetPosition);
            ProgressSynchronizer synch = (ProgressSynchronizer) progressbarInstance.GetComponent(typeof(ProgressSynchronizer));
            synch.Initial(name);
        }
    }

}