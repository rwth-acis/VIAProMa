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
    string desc = "The Mockup Editor can be used to instatiate game objects in realtime. Just click on the buttons to get objects like cubes, spheres and other UI elements from MRTK.";

    // instances:
    private GameObject mockUpWindowInstance;
    private GameObject descriptionInstance;
    private GameObject descriptionObject;

    /// <summary>
    /// Opens a window where the user can select between different objects
    /// </summary>
    public void ShowMockUpWindow()
    {
        InstantiateControl(
                    mockUpWindowPrefab,
                    ref mockUpWindowInstance,
                    transform.position - 0.15f * transform.right);
    }

    /// <summary>
    /// Instantiates the given prefab
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="instance"></param>
    /// <param name="targetPosition"></param>
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

    /// <summary>
    /// Opens a window with additional information about the editor
    /// </summary>
    public void ShowDescription()
    {
        InstantiateControl(
                    descriptionPrefab,
                    ref descriptionInstance,
                    transform.position + 0.28f * transform.right - 0.0886f * transform.up);
        descriptionInstance.GetComponent<DescriptionHelper>().UpdateLabel(desc);
    }

    /// <summary>
    /// Closes the window
    /// </summary>
    public void Close()
    {
        Destroy(transform.parent.gameObject);
    }

    /// <summary>
    /// Closes the main window and all sub windows
    /// </summary>
    public void MainWindowClose()
    {
        Destroy(GameObject.FindWithTag("descriptionWindow"));
        Close();
    }
}
