using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using UnityEngine;

namespace i5.VIAProMa.UI.AppBar
{
    /// <summary>
    /// Spawns an app bar
    /// </summary>
    public class AppBarSpanwer : Spawner
    {
        [Tooltip("The target bounding box to which the app bar should be attached")]
        [SerializeField] protected BoundingBox targetBoundingBox;

        protected override void Awake()
        {
            if (targetBoundingBox == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(targetBoundingBox));
            }

            base.Awake();
        }

        protected override void Setup()
        {
            base.Setup();
            AppBarPlacer placer = instance.GetComponent<AppBarPlacer>();
            if (placer == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(AppBarPlacer), instance);
            }
            placer.TargetBoundingBox = targetBoundingBox;
            AppBarActions actions = instance.GetComponent<AppBarActions>();
            if (actions == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(AppBarActions), instance);
            }
            PhotonView photonView = targetBoundingBox.Target.GetComponent<PhotonView>();
            actions.TargetNetworked = (photonView != null);
        }
    }
}