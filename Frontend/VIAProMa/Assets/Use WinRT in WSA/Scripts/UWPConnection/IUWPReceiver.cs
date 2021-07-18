using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWPConnection
{
    /// <summary>
    /// You need to implement this interface to listen to messages sent from your WSA app to your Unity scripts.
    /// </summary>
    /// <remarks>
    /// The UWPConnectionManager prefab object will relay messages from your WSA app to a script that implements this interface.
    /// Please refer to the documentation for more information on how to set this up.
    /// </remarks>
    public interface IUWPReceiver
    {
        /// <summary>
        /// Receive a message sent from the WSA app.
        /// </summary>
        /// <param name="arg">
        /// This is the message sent from your WSA classes. Note that you only have this function to receive info from the app,
        /// and that you only have this parameter to encode it. However, you may possibly have many kind of “messages” and data types
        /// to receive from your Unity script. You need to encode all of that using this parameter.
        /// If you need some ideas on how to do this, please study the example implementation and consult the documentation.
        /// </param>
        void ReceiveFromUWP(object arg);
    }
}


