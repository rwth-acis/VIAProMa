using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MenuPlacement {
    /// <summary>
    /// The base of menu scripts. Its functions will be called in the MenuHandler.
    /// There should be just one script inheriting MenuBase on a GameObject.
    /// </summary>
    public abstract class MenuBase : MonoBehaviour {
        /// <summary>
        /// Initialize() will be called in the Open() function of MenuHandler. 
        /// Initialize your properties here.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// OnClose() will be called in the Close() function of MenuHandler. 
        /// The menu should be recovered to the status before Initialize() because it will be returned to the ObjectPool.
        /// </summary>
        public abstract void OnClose();

    }

}
