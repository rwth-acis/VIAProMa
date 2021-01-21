using i5.VIAProMa.Utilities;
using UnityEngine;
using System.Collections.Generic;

namespace i5.VIAProMa.UI.AppBar
{
    [RequireComponent(typeof(AppBarPlacer))]
    public class AppBarStateController : MonoBehaviour
    {
        [SerializeField] private GameObject collapsedView;
        [SerializeField] private GameObject expandedView;
        [SerializeField] private GameObject adjustmentView;
        private List<AppBarStateController> appBarsInConnetMode;

        private AppBarState state = AppBarState.COLLAPSED;
        private BoundingBoxStateController boundingBoxStateController;

        public AppBarState State
        {
            get => state;
            set
            {
                state = value;
                UpdateView();
            }
        }

        private void Awake()
        {
            if (collapsedView == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(collapsedView));
            }
            if (expandedView == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(expandedView));
            }
            if (adjustmentView == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(adjustmentView));
            }
        }

        private void Start()
        {
            AppBarPlacer appBarPlacer = GetComponent<AppBarPlacer>();
            if (appBarPlacer == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(AppBarPlacer), gameObject);
            }
            boundingBoxStateController = appBarPlacer?.TargetBoundingBox?.gameObject.GetComponent<BoundingBoxStateController>();
            if (boundingBoxStateController == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BoundingBoxStateController), appBarPlacer?.TargetBoundingBox.gameObject);
            }
            appBarsInConnetMode = new List<AppBarStateController>();
        }

        private void UpdateView()
        {
            collapsedView.SetActive(state == AppBarState.COLLAPSED || state == AppBarState.CONNECTING_INVOKE || state == AppBarState.CONNECTING);
            expandedView.SetActive(state == AppBarState.EXPANDED);
            adjustmentView.SetActive(state == AppBarState.ADJUSTMENT_VIEW);

            foreach (AppBarStateController appBar in appBarsInConnetMode)
            {
                appBar.Collapse();
            }
            appBarsInConnetMode = new List<AppBarStateController>();

            if (state == AppBarState.ADJUSTMENT_VIEW)
            {
                boundingBoxStateController.BoundingBoxActive = true;
                boundingBoxStateController.manipulationHandler.enabled = true;
            }
            //This state is only active, when on ANOTHER app bar the connect button was pressed.
            //It activates the bounding box but disables the manipulation handler to make it easier to hit the corrosponding object with the ray
            else if (state == AppBarState.CONNECTING)
            {
                boundingBoxStateController.BoundingBoxActive = true;
                if (boundingBoxStateController.manipulationHandler != null)
                {
                    boundingBoxStateController.manipulationHandler.enabled = false;
                }
            }
        }

        public void Collapse()
        {
            State = AppBarState.COLLAPSED;
        }


        public void Expand()
        {
            State = AppBarState.EXPANDED;
        }

        public void AdjustmentView()
        {
            State = AppBarState.ADJUSTMENT_VIEW;
        }

        /// <summary>
        /// Is executed on the app bar, the connect button was pressed
        /// </summary>
        public void InvokeConnect()
        {
            State = AppBarState.CONNECTING_INVOKE;
            var otherappBars = FindObjectsOfType<AppBarStateController>();
            foreach (var appBar in otherappBars)
            {
                if (appBar != this)
                {
                    appBar.Connect();
                    appBarsInConnetMode.Add(appBar);
                }
            }
        }

        /// <summary>
        /// Is executed on the app bars, where the connect button was NOT pressed
        /// </summary>
        public void Connect()
        {
            State = AppBarState.CONNECTING;
        }
    }

    public enum AppBarState
    {
        COLLAPSED, EXPANDED, ADJUSTMENT_VIEW, CONNECTING_INVOKE, CONNECTING
    }
}