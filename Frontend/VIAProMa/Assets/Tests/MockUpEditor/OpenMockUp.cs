using i5.VIAProMa.Multiplayer;
using i5.VIAProMa.ResourceManagagement;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenMockUp : MonoBehaviourPunCallbacks
{
    private AssetBundle myLoadedAssetBundle;

    [Header("References")]
    [SerializeField] private GameObject mockUpWindowPrefab;
    [SerializeField] private GameObject descriptionPrefab;
    [SerializeField] private TextMeshPro descriptionText;

    // instances:
    private GameObject mockUpWindowInstance;
    private GameObject descriptionInstance;

    // Start is called before the first frame update
    void Start()
    {
        //myLoadedAssetBundle = AssetBundle.LoadFromFile("Assets/Tests/MockUpEditor");
    }

    public void openNewScene()
    {
        SceneManager.LoadScene("MockUpTest", LoadSceneMode.Single);
    }

    public void showMockUpWindow()
    {
        InstantiateControl(
                    mockUpWindowPrefab,
                    ref mockUpWindowInstance,
                    transform.position - 0.15f * transform.right);
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

    public void Close()
    {
        Destroy(transform.parent.gameObject);
    }

    public void showDescription()
    {
        InstantiateControl(
                    descriptionPrefab,
                    ref descriptionInstance,
                    transform.position + 0.2565f * transform.right - 0.0886f * transform.up);
    }
}
