using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class CardSideBarHandler : MonoBehaviour, IMixedRealityFocusHandler
{
    [SerializeField] private GameObject deleteButton;
    [SerializeField] private GameObject editButton;

    void IMixedRealityFocusHandler.OnFocusEnter(FocusEventData eventData)
    {
        deleteButton.SetActive(true);
        editButton.SetActive(true);
    }

    void IMixedRealityFocusHandler.OnFocusExit(FocusEventData eventData)
    {
        deleteButton.SetActive(false);
        editButton.SetActive(false);
    }
}
