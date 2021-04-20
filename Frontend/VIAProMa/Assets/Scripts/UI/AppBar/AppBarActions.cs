using i5.VIAProMa.Utilities;
using Photon.Pun;
using UnityEngine;

namespace i5.VIAProMa.UI.AppBar
{
    /// <summary>
    /// Contains the application logic and actions which are triggered if the buttons on the app bar are pressed
    /// </summary>
    [RequireComponent(typeof(AppBarPlacer))]
    public class AppBarActions : MonoBehaviour
    {
        private AppBarPlacer appBarPlacer;

        private Vector3 startPosition;
        private Quaternion startRotation;
        private Vector3 startScale;

        /// <summary>
        /// True if the target to which the app bar belongs is a networed object (with a PhotonView)
        /// </summary>
        public bool TargetNetworked { get; set; }

        /// <summary>
        /// Checks the component's setup and initializes it
        /// </summary>
        private void Awake()
        {
            appBarPlacer = GetComponent<AppBarPlacer>();
            if (appBarPlacer == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(AppBarPlacer), gameObject);
            }
        }


        /// <summary>
        /// Destroys the object (either networked or not based on the setting TargetNetworked)
        /// This also destroys the bounding box and finally the app bar
        /// </summary>
        public void RemoveObject()
        {
            if (TargetNetworked)
            {
                PhotonNetwork.Destroy(appBarPlacer.TargetBoundingBox.Target);
            }
            else
            {
                Destroy(appBarPlacer.TargetBoundingBox.Target);
            }

            // check if the bounding box still exists (in this case it was not a child of the target gameobject)
            if (appBarPlacer.TargetBoundingBox == null)
            {
                Destroy(appBarPlacer.TargetBoundingBox.gameObject);
            }

            //Remove the curves connected with the object
            ConnectionCurveManager.Instance.DeleteAllCurvesFromObject(appBarPlacer.TargetBoundingBox.Target);

            // finally also destroy the app bar
            Destroy(gameObject);
        }


        /// <summary>
        /// Puts the app bar into placement mode and stores the current position, rotation and scale for the reset option
        /// </summary>
        public void StartAdjustment()
        {
            startPosition = appBarPlacer.TargetBoundingBox.Target.transform.localPosition;
            startRotation = appBarPlacer.TargetBoundingBox.Target.transform.localRotation;
            startScale = appBarPlacer.TargetBoundingBox.Target.transform.localScale;
        }

        /// <summary>
        /// Resets the target object to its position, rotation and scale to the status when the adjustment was started
        /// </summary>
        public void ResetAdjustment()
        {
            appBarPlacer.TargetBoundingBox.Target.transform.localPosition = startPosition;
            appBarPlacer.TargetBoundingBox.Target.transform.localRotation = startRotation;
            appBarPlacer.TargetBoundingBox.Target.transform.localScale = startScale;
        }

        /// <summary>
        /// Starts the curve connection process for the object, the app bar belongs to
        /// </summary>
        public void Connect()
        {
            ConnectionCurveManager.Instance.ChangeState(ConnectionCurveManager.State.connecting, gameObject, appBarPlacer.TargetBoundingBox.Target);
        }

        /// <summary>
        /// Starts the disconnect process. It is complety independent from the app bar it was invoked on.
        /// </summary>
        public void Disconnect()
        {
            ConnectionCurveManager.Instance.ChangeState(ConnectionCurveManager.State.disconnecting, gameObject);
        }
    }
}
