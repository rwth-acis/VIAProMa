using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWPConnection
{
    /// <summary>
    /// This class manage the connection between your WSA Project and your Unity Scripts. The prefab UWPConnectionManager that you can
    /// find among the asset already contain this script as a component, so you don’t need to add this script as a component to any
    /// Unity object of your own.
    /// </summary>
    /// <remarks>
    /// By design, the connection with the WSA Project is done during the application start up. This class uses DontDestroyOnLoad on Awake,
    /// so you will be able to listen to Unity messages in any scene of the game. However, it is possible (as is described in the documentation),
    /// that you create another UWPConnectionManager object in other scene should you want another IUWPReceiver object to listen to messages
    /// from the WSA app. In this case, on the scene creation, this second UWPConnectionManager object will check if there is already another
    /// UWPConnectionManager already running. If that is the case, the second object will update the UWPReceiverObject field of the first one
    /// to point to its own, thus updating it, and then destroy itself. In this way, only the original UWPConnectionManager, with the established
    /// WSA app connection, will be available at any time.
    /// </remarks>
    public class UWPConnectionManager : MonoBehaviour
    {
        public delegate void OnEvent(object arg);
        /// <summary>
        /// Because Unity cannot know what classes will be available in your WSA project when compiling your scripts, the communications between
        /// Unity and the WSA app works with this event. Object running in the latter will subscribe to this event, that will be automatically raised
        /// when you use the SendToUWP function. This is all done automatically when the asset is set up. You don’t need to use this event on your own scripts.
        /// </summary>
        public OnEvent onEvent = null;

        public GameObject UWPReceiverObject;

        //For simple singleton implementation
        private static UWPConnectionManager _instance;

        void Awake()
        {                       
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(transform.gameObject);
            }
            //Update the running instance with the receiver object of this scene
            else
            {
                _instance.UWPReceiverObject = this.UWPReceiverObject;
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// Call this function when you need to communicate with the WSA app.
        /// The Communications object in the app will receive this message and relay it to your classes.
        /// </summary>
        /// <param name="arg">
        /// Use this parameter to encode the message that you want to send to your WSA app. It will receive this object as you pass it here.
        /// Please refer to the example and documentation for a further explanation on how you can use this sole function for proper communication.</param>
        public void SendToUWP(object arg)
        {
            if (onEvent != null)
                onEvent(arg);
            else
                Debug.Log("No delegate for the event. Have you made the connection in MainPage.xaml.cs?");
        }

        /// <summary>
        /// Receive a message from the WSA app and relay it to your IUWPReceiver. This is done automatically by the asset, and you don’t need
        /// to use this function on your own scripts or classes unless to perform some testing or prototyping.
        /// </summary>
        /// <param name="arg">
        /// The message, as encoded by your classes in the WSA project. Please refer to the manual for more details.
        /// </param>
        public void ReceiveFromUWP(object arg)
        {
            if (UWPReceiverObject != null)
            {
                var receiverComponent = UWPReceiverObject.GetComponent<IUWPReceiver>();
                if (receiverComponent != null)
                    receiverComponent.ReceiveFromUWP(arg);
            }
        }
    }
}