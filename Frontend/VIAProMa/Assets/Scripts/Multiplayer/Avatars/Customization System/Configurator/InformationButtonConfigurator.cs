using i5.ViaProMa.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InformationButtonConfigurator : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject informationButton;

    [Header("References")]
    [SerializeField] private GameObject InformationBoxPrefab;

    private GameObject informationbox;

    private void Start()
    {
        if (PhotonNetwork.InLobby)
        {
            Debug.Log("IFB:inlobby");
            informationButton.SetActive(false);
        }
        else
        {
            Debug.Log("IFB:inroom");
            informationButton.SetActive(true);
        }
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

    public void Onclicked()
    {
        Vector3 targetPosition = transform.position - 0.5f * transform.right;
        targetPosition.y -= 0.2f;
        InstantiateControl(InformationBoxPrefab, ref informationbox, targetPosition);
        informationbox.transform.parent = transform;
    }

}