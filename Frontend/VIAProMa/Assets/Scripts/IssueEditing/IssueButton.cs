using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VIAProMa.DataDisplays;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.Login;
using System;

/// <summary>
/// Button to alter a card over an extern service like GitHub. The button is only displayed when the user is logged in.
/// </summary>
public abstract class IssueButton : MonoBehaviour
{
    [SerializeField] protected SourceDisplay source;
    protected int resourceID;
    private Dictionary<EventHandler, DataSource> subscibedEventHandlers = new Dictionary<EventHandler, DataSource>();

    /// <summary>
    /// Sets initial values, subscibes to the login/out events corresponding to the provided sources and performs the inital source status check.
    /// </summary>
    /// <param name="sources"></param>
    protected void Setup(List<DataSource> sources)
    {
        resourceID = source.Content.Id;
        subcribeToServices(sources);
        initialLoginStatusCheck(sources);
    }

    //Subscibes to the login/out events corresponding to sources
    private void subcribeToServices(List<DataSource> sources)
    {
        foreach (var dataSource in sources)
        {
            EventHandler handler = reciveLoginEvent(dataSource);
            dataSourceToService(dataSource).LoginCompleted += handler;
            subscibedEventHandlers.Add(handler, dataSource);

            handler = reciveLogoutEvent(dataSource);
            dataSourceToService(dataSource).LogoutCompleted += handler;
            subscibedEventHandlers.Add(handler, dataSource);
        }
    }

    /// <summary>
    /// Unsubscibes from all login/out events that where subscibed to in the setup.
    /// </summary>
    protected void unsubcribeFromAllServices()
    {
        foreach (var handlerPair in subscibedEventHandlers)
        {
            dataSourceToService(handlerPair.Value).LoginCompleted -= handlerPair.Key;
            dataSourceToService(handlerPair.Value).LogoutCompleted -= handlerPair.Key;
        }
    }

    //Sets the button active status corresponding to the current source status
    private void initialLoginStatusCheck(List<DataSource> sources)
    {
        if (sources.Contains(source.Content.Source))
        {
            gameObject.SetActive(dataSourceToService(source.Content.Source).IsLoggedIn);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private EventHandler reciveLoginEvent(DataSource dataSource)
    {
        return (x, y) =>
        {
            if (dataSource == source.Content.Source)
                gameObject.SetActive(true);
        };
    }

    public EventHandler reciveLogoutEvent(DataSource dataSource)
    {
        return (x, y) =>
        {
            if (dataSource == source.Content.Source)
                gameObject.SetActive(false);
        };
    }

    //Converts the source enum to the corresponding service
    private OpenIDConnectService dataSourceToService(DataSource dataSource)
    {
        switch (dataSource)
        {
            case DataSource.GITHUB:
                return ServiceManager.GetService<GitHubOidcService>();
            case DataSource.REQUIREMENTS_BAZAAR:
                return ServiceManager.GetService<LearningLayersOidcService>();
            default:
                return null;
        }
    }

    public void OnDestroy()
    {
        unsubcribeFromAllServices();
    }

}
