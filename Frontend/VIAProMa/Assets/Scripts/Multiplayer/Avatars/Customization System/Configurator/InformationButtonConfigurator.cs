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

    //[Header("References")]
    //[SerializeField] private GameObject InformationBoxPrefab;

    //public GameObject informationbox;
    //private GameObject informationbox;
    private UserRoles userRole;

    private void Start()
    {
        userRole = UserManager.Instance.UserRole;

        if (PhotonNetwork.InLobby)
        {
            Debug.Log("IFB:inlobby");
            informationButton.SetActive(false);
        }
        /*
        else if (userRole==UserRoles.TUTOR)
        {
            Debug.Log("tutor");
            informationButton.SetActive(false);
        }
        */
        else
        {
            Debug.Log("IFB:inroom");
            informationButton.SetActive(true);
        }
    }

/*
    private void Update()
    {
        userRole = UserManager.Instance.UserRole;
        if (userRole==UserRoles.TUTOR)
        {
            informationButton.SetActive(false);
        }
        else
        {
            informationButton.SetActive(true);
        }
    }
*/

/*
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
*/

/*
    public void Onclicked()
    {
        Vector3 targetPosition = transform.position - 0.5f * transform.right;
        targetPosition.y -= 0.2f;
        InstantiateControl(InformationBoxPrefab, ref informationbox, targetPosition);
        informationbox.transform.parent = transform;
    }
*/


}