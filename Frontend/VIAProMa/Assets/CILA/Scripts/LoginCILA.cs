
using UnityEngine;
using System;
using System.Threading.Tasks;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using i5.VIAProMa.Login;



public class LoginCILA : MonoBehaviour
{
    private OpenIDConnectService oidcService;
    private bool subscribedToOidc = false;
    private IUserInfo cachedUserInfo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoginButtonPressed()
    {
        if (oidcService == null && ServiceManager.ServiceExists<LearningLayersOidcService>())
        {
            oidcService = ServiceManager.GetService<LearningLayersOidcService>();
        }
        print(ServiceManager.ServiceExists<LearningLayersOidcService>());
        if (oidcService == null)
        {
            return;
        }
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
        if (oidcService == null)
        {
            return;
        }

        // un-subscribe from the event and also remember that we are not subscribed anymore
        oidcService.LoginCompleted -= OnLogin;
        subscribedToOidc = false;
        string userName = await GetUserNameAsync();
    }

    // Fetches the user name which should be displayed
    private async Task<string> GetUserNameAsync()
    {
        // if we are logged in, we take the user data which are provided by the login service
        //if (oidcService != null && oidcService.IsLoggedIn)
        //{
            if (cachedUserInfo == null)
            {
                cachedUserInfo = await oidcService.GetUserDataAsync();
            }
            return cachedUserInfo.FullName;
        //}
        // if we are not logged in, we take the nick name given by the PhotonNetwork
        //else
        //{
           // return PhotonNetwork.NickName;
       // }
    }
}
