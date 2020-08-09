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

    /// <summary>
    /// Gets or sets enabled/disabled state of the window
    /// </summary>
    public bool WindowEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether the window is currently open
    /// </summary>
    public bool WindowOpen => gameObject.activeSelf;

    /// <summary>
    /// Event which is raised once the window is closed
    /// </summary>
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

    /// <summary>
    /// Closes the window
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
        WindowClosed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Opens the window
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);
        Initialize();
    }

    /// <summary>
    /// Opens the window at the given position and rotation
    /// </summary>
    /// <param name="position">The position of the window</param>
    /// <param name="eulerAngles">The rotation in euler angles of the window</param>
    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
    }

    // Initializes the controls of the window
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

    // Fetches the user name which should be displayed
    private async Task<string> GetUserNameAsync()
    {
        // if we are logged in, we take the user data which are provided by the login service
        if (oidcService.IsLoggedIn)
        {
            if (cachedUserInfo == null)
            {
                cachedUserInfo = await oidcService.GetUserDataAsync();
            }
            return cachedUserInfo.FullName;
        }
        // if we are not logged in, we take the nick name given by the PhotonNetwork
        else
        {
            return PhotonNetwork.NickName;
        }
    }

    // sets the button's visibility based on whether the user is logged in or logged out
    private void SetButtonStates()
    {
        logoutButton.gameObject.SetActive(oidcService.IsLoggedIn);
        learningLayersLoginButton.gameObject.SetActive(!oidcService.IsLoggedIn);
    }

    /// <summary>
    /// Called if the login button is pressed
    /// </summary>
    public void LoginButtonPressed()
    {
        // only subscribe to the LoginCompleted event if we are not already subscribed
        // this is to avoid that the raised event is handled multiple times,
        // e.g. if the user does not complete the login the first time
        if (!subscribedToOidc)
        {
            oidcService.LoginCompleted += OnLogin;
            subscribedToOidc = true;
        }
        oidcService.OpenLoginPage();
    }

    // Called by the LoginCompleted event once the login has happened successfully
    private async void OnLogin(object sender, EventArgs e)
    {
        // un-subscribe from the event and also remember that we are not subscribed anymore
        oidcService.LoginCompleted -= OnLogin;
        subscribedToOidc = false;
        string userName = await GetUserNameAsync();
        nameLabel.text = userName;
        SetPhotonName(userName);
        SetButtonStates();
    }

    /// <summary>
    /// Called if the logout button is pressed
    /// </summary>
    public void LogoutButtonPressed()
    {
        // since the Learning Layers logout always succeeds,
        // there is no need to remember the event subscription status like with the login procedure
        oidcService.LogoutCompleted += OnLogout;
        oidcService.Logout();
        nameLabel.text = UserManager.Instance.DefaultName;
        SetPhotonName(UserManager.Instance.DefaultName);
    }

    // Called by the LogoutCompleted event
    private void OnLogout(object sender, EventArgs e)
    {
        // un-subscribe from hte LogoutCompleted event
        oidcService.LogoutCompleted -= OnLogout;
        // update the button statuses
        SetButtonStates();
        // reset the photon nickname to the default guest name that was determined in the beginning
        // this way, a person will always have the same guest name in one session
        SetPhotonName(UserManager.Instance.DefaultName);
    }

    // Sets the photon name in the photon network to the given userName
    private void SetPhotonName(string userName)
    {
        if (!string.IsNullOrWhiteSpace(userName))
        {
            PhotonNetwork.NickName = userName;
            RaiseNameChangedEvent();
        }
    }

    // broadcasts to all other participants that the name of this user was changed
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

    /// <summary>
    /// Called by the radio buttons if the user role is changed
    /// Updates the user role in the system
    /// </summary>
    public void OnRoleChanged()
    {
        UserManager.Instance.UserRole = (UserRoles)roleToggles.CurrentIndex;
    }
}
