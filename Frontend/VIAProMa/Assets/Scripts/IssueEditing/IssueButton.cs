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
        SubcribeToServices(sources);
        InitialLoginStatusCheck(sources);
    }

    //Subscibes to the login/out events corresponding to sources
    private void SubcribeToServices(List<DataSource> sources)
    {
        foreach (var dataSource in sources)
        {
            EventHandler handler = ReciveLoginEvent(dataSource);
            DataSourceToService(dataSource).LoginCompleted += handler;
            subscibedEventHandlers.Add(handler, dataSource);

            handler = ReciveLogoutEvent(dataSource);
            DataSourceToService(dataSource).LogoutCompleted += handler;
            subscibedEventHandlers.Add(handler, dataSource);
        }
    }

    // Unsubscibes from all login/out events that where subscibed to in the setup.
    private void UnsubcribeFromAllServices()
    {
        foreach (var handlerPair in subscibedEventHandlers)
        {
            DataSourceToService(handlerPair.Value).LoginCompleted -= handlerPair.Key;
            DataSourceToService(handlerPair.Value).LogoutCompleted -= handlerPair.Key;
        }
    }

    //Sets the button active status corresponding to the current source status
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

    private EventHandler ReciveLoginEvent(DataSource dataSource)
    {
        return (x, y) =>
        {
            if (dataSource == source.Content.Source)
                gameObject.SetActive(true);
        };
    }

    public EventHandler ReciveLogoutEvent(DataSource dataSource)
    {
        return (x, y) =>
        {
            if (dataSource == source.Content.Source)
                gameObject.SetActive(false);
        };
    }

    //Converts the source enum to the corresponding service
    private OpenIDConnectService DataSourceToService(DataSource dataSource)
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
        UnsubcribeFromAllServices();
    }

}
