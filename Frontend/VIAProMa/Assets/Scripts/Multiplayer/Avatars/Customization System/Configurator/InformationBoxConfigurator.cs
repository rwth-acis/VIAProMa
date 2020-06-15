using i5.ViaProMa.UI;
using Photon.Pun;
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

    [Header("References")]
    [SerializeField] private GameObject BarchartPrefab;
    [SerializeField] private GameObject ScatterplotPrefab;
    [SerializeField] private GameObject ProgressBarPrefab;

    // instances
    private GameObject barchartInstance;
    private GameObject scatterplotInstance;
    private GameObject progressbarInstance;
    private Scene currentScene;
    string sceneName;

    // Start is called before the first frame update
    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        Debug.Log(sceneName);
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

    private void InstantiateControl(GameObject prefab, ref GameObject instance, Vector3 targetPosition)
    {
        Quaternion targetRotation = transform.rotation;

        if (instance != null)
        {
            instance.SetActive(true);
            instance.transform.position = targetPosition;
            instance.transform.rotation = targetRotation;
        }
        else
        {
            instance = GameObject.Instantiate(prefab, targetPosition, targetRotation);
        }
    }

    public void BarchartButtonClicked()
    {
        Debug.Log("Barchart Button Clicked");
        Vector3 targetPosition = GameObject.Find("Table").transform.position;
        targetPosition.y = 2f;
        targetPosition.x -= 1f;
        targetPosition.z += 0.5f;
        Debug.Log(targetPosition);
        InstantiateControl(BarchartPrefab, ref barchartInstance, targetPosition);
    }
    
    public void ScatterplotButtonClicked()
    {
        Debug.Log("Scatterplot Button Clicked");
        Vector3 targetPosition = GameObject.Find("Table").transform.position;
        targetPosition.y = 2f;
        targetPosition.x -= 2.4f;
        targetPosition.z += 0.5f;
        InstantiateControl(ScatterplotPrefab, ref scatterplotInstance, targetPosition);
    }

    public void ProgressBarButtonClicked()
    {
        Debug.Log("ProgressBar Button Clicked");
        Vector3 targetPosition = GameObject.Find("Table").transform.position;
        targetPosition.y = 2f;
        targetPosition.x -= 4f;
        targetPosition.z += 0.5f;
        InstantiateControl(ProgressBarPrefab, ref progressbarInstance, targetPosition);
    }

}