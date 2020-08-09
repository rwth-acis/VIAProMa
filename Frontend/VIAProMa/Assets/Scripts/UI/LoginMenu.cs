using ExitGames.Client.Photon;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// UI controller and actions for the login menu
/// </summary>
public class LoginMenu : MonoBehaviour, IWindow
{
    [SerializeField] private TextMeshPro nameLabel;
    [SerializeField] private Interactable learningLayersLoginButton;
    [SerializeField] private Interactable logoutButton;
    [SerializeField] private InteractableToggleCollection roleToggles;

    public bool WindowEnabled { get; set; }

    public bool WindowOpen => gameObject.activeSelf;

    public event EventHandler WindowClosed;

    private OpenIDConnectService oidcService;
    private bool subscribedToOidc = false;
    private IUserInfo cachedUserInfo;

    private void Awake()
    {
        if (nameLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(nameLabel));
        }
        if (learningLayersLoginButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(learningLayersLoginButton));
        }
        if (logoutButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(logoutButton));
        }
        if (roleToggles == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(roleToggles));
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
        WindowClosed?.Invoke(this, EventArgs.Empty);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        Initialize();
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
    }

    private async void Initialize()
    {
        if (oidcService == null)
        {
            oidcService = ServiceManager.GetService<OpenIDConnectService>();
        }
        nameLabel.text = await GetUserNameAsync();
        roleToggles.CurrentIndex = (int)UserManager.Instance.UserRole;
        for (int i = 0; i < roleToggles.ToggleList.Length; i++)
        {
            int dimension = 0;
            if (i == roleToggles.CurrentIndex)
            {
                dimension = 1;
            }
            roleToggles.ToggleList[i].SetDimensionIndex(dimension);
        }
        SetButtonStates();
    }

    private async Task<string> GetUserNameAsync()
    {
        if (oidcService.IsLoggedIn)
        {
            if (cachedUserInfo == null)
            {
                cachedUserInfo = await oidcService.GetUserDataAsync();
            }
            return cachedUserInfo.FullName;
        }
        else
        {
            return PhotonNetwork.NickName;
        }
    }

    private void SetButtonStates()
    {
        logoutButton.gameObject.SetActive(oidcService.IsLoggedIn);
        learningLayersLoginButton.gameObject.SetActive(!oidcService.IsLoggedIn);
    }

    public void LoginButtonPressed()
    {
        if (!subscribedToOidc)
        {
            oidcService.LoginCompleted += OnLogin;
            subscribedToOidc = true;
        }
        oidcService.OpenLoginPage();
    }

    private async void OnLogin(object sender, EventArgs e)
    {
        oidcService.LoginCompleted -= OnLogin;
        subscribedToOidc = false;
        string userName = await GetUserNameAsync();
        nameLabel.text = userName;
        SetPhotonName(userName);
        SetButtonStates();
    }

    public void LogoutButtonPressed()
    {
        oidcService.LogoutCompleted += OnLogout;
        oidcService.Logout();
        nameLabel.text = UserManager.Instance.DefaultName;
        SetPhotonName(UserManager.Instance.DefaultName);
    }

    private void OnLogout(object sender, EventArgs e)
    {
        oidcService.LogoutCompleted -= OnLogout;
        SetButtonStates();
        SetPhotonName(UserManager.Instance.DefaultName);
    }

    public void SetPhotonName(string userName)
    {
        if (!string.IsNullOrWhiteSpace(userName))
        {
            PhotonNetwork.NickName = userName;
            RaiseNameChangedEvent();
        }
    }

    private void RaiseNameChangedEvent()
    {
        if (PhotonNetwork.IsConnected)
        {
            byte eventCode = 1;
            byte content = 0;
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, sendOptions);
        }
    }

    public void OnRoleChanged()
    {
        UserManager.Instance.UserRole = (UserRoles)roleToggles.CurrentIndex;
    }
}
