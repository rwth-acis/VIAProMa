using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UWPConnection
{
    /// <summary>
    /// This is an example implementation of the IUWPReceiver interface and the controller for the demo scenes. It uses the UWPConnectionManager
    /// prefab to tell the WSA app to use some WinRT APIs, and listen to the message sent back by it (the result of a WinRT speech recognition interaction).
    /// </summary>
    public class ExampleController : MonoBehaviour, IUWPReceiver
    {
        private UWPConnectionManager Connection;
        public Text UWPCallbackText;
        public InputField NarrationInput;

        private static string MainSceneName = "Main - Connector Example";
        private static string AboutSceneName = "Use WinRT in WSA/Demo Scenes/About - Connector Example";
        private bool InMainScene = false;

        void Start()
        {
            //Find the UWPConnectionManager prefab and save its connection component to send messages through.
            var connectionObject = GameObject.Find("UWPConnectionManager");
            if (connectionObject != null)
            {
                Connection = connectionObject.GetComponent<UWPConnectionManager>();
                if (Connection == null)
                    Debug.Log("Error: UWPConnectionManager object don't contains a component of type \"UWPConnectionManager\"");
            }
            else
                Debug.Log("Error: Can't find UWPConnectionManager object");

            //Determine if this is the main scene for the back button behavior
            if (SceneManager.GetActiveScene().name.Equals(MainSceneName))
            {
                InMainScene = true;
            }
        }
        
        #region WinRT API calls and call back
        public void NarrateButtonClicked()
        {
            string[] message = new string[2];

            message[0] = "Narrate";
            message[1] = (NarrationInput != null && NarrationInput.text.Length > 0) ? NarrationInput.text : "The narration input field is empty";
            Connection.SendToUWP(message);
        }

        public void SpeechRecognitionButtonClicked()
        {
            string[] message = new string[1];

            message[0] = "Speech";
            Connection.SendToUWP(message);
        }

        public void DialogButtonClicked()
        {
            string[] message = new string[1];

            message[0] = "Dialog";
            Connection.SendToUWP(message);
        }

        //IUWPReceiver implementation
        public void ReceiveFromUWP(object arg)
        {
            if (arg is string && UWPCallbackText != null)
                UWPCallbackText.text = arg as string;
        }
        #endregion

        #region BackButton behavior and navigation
        void Update()
        {
            if (InMainScene)
            {
                if (Input.GetKey(KeyCode.Escape))
                    Application.Quit();
            }
            else
            {
                if (Input.GetKey(KeyCode.Escape))
                    SceneManager.LoadScene(MainSceneName);
            }
        }

        public void LoadAboutScene()
        {
            var aboutSceneIndex = SceneUtility.GetBuildIndexByScenePath(AboutSceneName);
            
            if (aboutSceneIndex >= 0)
                SceneManager.LoadScene(AboutSceneName);
            else
                Debug.Log("Error loading the About scene. Have you added it to the build in \"Build Settings\"");
        }

        #endregion

        #region Other functions and misc
        public void LoadWebsite(string webSiteUrl)
        {
            Application.OpenURL(webSiteUrl);
        }
        #endregion
    }

}