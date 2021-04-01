
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;

public class CardSideBarHandler : MonoBehaviour, IMixedRealityFocusHandler
{
    [SerializeField] private GameObject editButton;

    private bool isAuthenticated;


    public void Start()
    {
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LoginCompleted += LoginCompleted;
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LogoutCompleted += LogoutCompleted;
        isAuthenticated = false;
    }

    public void LoginCompleted(object sender, System.EventArgs e)
    {
        isAuthenticated = true;
    }

    public void LogoutCompleted(object sender, System.EventArgs e)
    {
        isAuthenticated = false;
    }

    void IMixedRealityFocusHandler.OnFocusEnter(FocusEventData eventData)
    {
        if (isAuthenticated)
        {
            editButton.SetActive(true);
        }
    }

    void IMixedRealityFocusHandler.OnFocusExit(FocusEventData eventData)
    {
        StartCoroutine(Wait());

    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(5f);
        editButton.SetActive(false);
    }
}