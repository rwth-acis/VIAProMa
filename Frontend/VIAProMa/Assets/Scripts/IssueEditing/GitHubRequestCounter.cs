using i5.Toolkit.Core.ServiceCore;
using i5.VIAProMa.Login;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Org.Git_Hub.API;
using System;
using i5.VIAProMa.WebConnection;
using i5.VIAProMa.Multiplayer.Chat;

public class GitHubRequestCounter : MonoBehaviour
{
    [SerializeField] NotificationSystem notificationWidget;
    [SerializeField] bool outputRequestNumberIntoLog;
    private int requestLimit = 60;
    private int requestCount = 0;
    
    DateTime firstRequestAt;

    public void Start()
    {
        ServiceManager.GetService<GitHubOidcService>().LoginCompleted += LoginCompleted;
        ServiceManager.GetService<GitHubOidcService>().LogoutCompleted += LogoutCompleted;
        GitHubManager.RequestSent += RequestSent;
        GitHub.RequestSent += RequestSent;
        requestLimit = 60;
        ResetCount();
    }

    // Increases a counter for each web request send to GitHub, prints the counter of every 5 equests
    public void RequestSent(object sender, System.EventArgs e)
    {
        DateTime now = DateTime.Now;
        if(now.Subtract(firstRequestAt).TotalHours >= 1)
        {
            ResetCount();
        }
        requestCount += 1;
        if (requestCount == requestLimit && outputRequestNumberIntoLog)
        {
            Debug.Log("The GitHub request limit has been reached during the running time of the application.");
        }
        else if(outputRequestNumberIntoLog)
        {
            Debug.Log(requestCount + " GitHub requests have been send within the last hour of running the application.");
        }
        if(requestCount % 5 == 1)
        {
            StartCoroutine(ShowMessage());
        }
    }

    // Reminds the user to log into GitHub to increase the number of allowed requests per hour
    IEnumerator ShowMessage()
    {
        notificationWidget.ShowMessage("It is recommended to log into GitHub.");
        yield return new WaitForSeconds(10f);
        notificationWidget.HideMessage();
    }

    // Resets the counter
    public void ResetCount()
    {
        requestCount = 0;
        firstRequestAt = DateTime.Now;
    }

    public void LoginCompleted(object sender, System.EventArgs e)
    {
        requestLimit = 5000;
    }

    public void LogoutCompleted(object sender, System.EventArgs e)
    {
        requestLimit = 60;
    }
}
