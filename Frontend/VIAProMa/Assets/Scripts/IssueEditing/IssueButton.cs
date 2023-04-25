using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using i5.VIAProMa.DataDisplays;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.Login;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Button to alter a card over an external service like GitHub. The button is only displayed when the user is logged in.
/// </summary>
public abstract class IssueButton : MonoBehaviour
{
    [SerializeField] protected SourceDisplay source;
    protected int resourceID;
    private Dictionary<EventHandler, DataSource> subscribedEventHandlers = new Dictionary<EventHandler, DataSource>();

    /// <summary>
    /// Sets initial values, subscribes to the login/out events corresponding to the provided sources and performs the inital source status check.
    /// </summary>
    /// <param name="sources"></param>
    protected void Setup(List<DataSource> sources)
    {
        resourceID = source.Content.Id;
        SubscribeToServices(sources);
        InitialLoginStatusCheck(sources);
    }

    // Subscribes to the login/out events corresponding to sources
    private void SubscribeToServices(List<DataSource> sources)
    {
        foreach (var dataSource in sources)
        {
            EventHandler handler = RecieveLoginEvent(dataSource);
            DataSourceToService(dataSource).LoginCompleted += handler;
            subscribedEventHandlers.Add(handler, dataSource);

            handler = RecieveLogoutEvent(dataSource);
            DataSourceToService(dataSource).LogoutCompleted += handler;
            subscribedEventHandlers.Add(handler, dataSource);
        }
    }

    // Sets this GameObject active when the provided datasource equals the datasource of this GameObject. For handling login events.
    private EventHandler RecieveLoginEvent(DataSource dataSource)
    {
        return (x, y) =>
        {
            if (dataSource == source.Content.Source)
                gameObject.SetActive(true);
        };
    }

    //Sets this GameObject inactive when the provided datasource equals the datasource of this GameObject. For handling logout events.
    private EventHandler RecieveLogoutEvent(DataSource dataSource)
    {
        return (x, y) =>
        {
            if (dataSource == source.Content.Source)
                gameObject.SetActive(false);
        };
    }

    // Sets the button active status corresponding to the current source status
    private void InitialLoginStatusCheck(List<DataSource> sources)
    {
        if (sources.Contains(source.Content.Source))
        {
            gameObject.SetActive(DataSourceToService(source.Content.Source).IsLoggedIn);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Converts the source enum to the corresponding service
    private OpenIDConnectService DataSourceToService(DataSource dataSource)
    {
        switch (dataSource)
        {
            case DataSource.GITHUB:
                if (ServiceManager.ServiceExists<GitHubOidcService>())
                {
                    return ServiceManager.GetService<GitHubOidcService>();
                }
                break;
            case DataSource.REQUIREMENTS_BAZAAR:
                if (ServiceManager.ServiceExists<LearningLayersOidcService>())
                {
                    return ServiceManager.GetService<LearningLayersOidcService>();
                }
                break;
        }
        return null;
    }

    // Unsubscribes from all login/out events that where subscribed to in the setup.
    private void UnsubscribeFromAllServices()
    {
        foreach (var handlerPair in subscribedEventHandlers)
        {
            OpenIDConnectService service = DataSourceToService(handlerPair.Value);
            if (service != null)
            {
                service.LoginCompleted -= handlerPair.Key;
                service.LogoutCompleted -= handlerPair.Key;
            }
        }
    }

    public void OnDestroy()
    {
        UnsubscribeFromAllServices();
    }

}
