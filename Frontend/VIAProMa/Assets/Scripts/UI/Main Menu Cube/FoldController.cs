using i5.VIAProMa.Multiplayer.Chat;
using i5.VIAProMa.UI.AppBar;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using UnityEngine;

namespace i5.VIAProMa.UI.MainMenuCube
{
    /// <summary>
    /// Controls the unfolding and folding procedure of the main menu cube
    /// Also takes care of the states of supplementary components so that they fit to the folded or unfolded menu
    /// </summary>
    public class FoldController : MonoBehaviour
    {
        [Tooltip("The animator which controls the folding animation")]
        [SerializeField] private Animator cubeAnimator;
        [Tooltip("Transform which poses the scaling pivot for the cube")]
        [SerializeField] private Transform cubePivot;
        [Tooltip("Reference to the button which folds and unfolds the menu")]
        [SerializeField] private Interactable onOffButton;
        [Tooltip("Parent of the labels which are shown in the compact cube form")]
        [SerializeField] private GameObject labelsParent;
        [Tooltip("The animation event handler which registers the events from the folding animation")]
        [SerializeField] private MainMenuAnimationEventHandler animationHandler;
        [Tooltip("Reference to the app bar spawner for the menu cube (should be placed on the same child which holds the bounding box")]
        [SerializeField] private AppBarSpanwer appBarSpawner;

        private const string menuOpenAnimParam = "MenuOpen";
        private const float animationLength = 1.625f;
        private const float collapsedScale = 0.25f;
        private const float expandedScale = 0.25f;

        /// <summary>
        /// True if the menu is currently open/ unfolded
        /// </summary>
        public bool MenuOpen { get; private set; }

        /// <summary>
        /// Checks the setup of the component
        /// </summary>
        private void Awake()
        {
            if (cubeAnimator == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(cubeAnimator));
            }
            if (cubePivot == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(cubePivot));
            }
            if (onOffButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(onOffButton));
            }
            if (labelsParent == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(labelsParent));
            }
            if (animationHandler == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(animationHandler));
            }
            if (appBarSpawner == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(appBarSpawner));
            }
        }

        /// <summary>
        /// Folds the menu back into the compact cube display
        /// </summary>
        public void FoldCube()
        {
            StopAllCoroutines();
            MenuOpen = false;
            animationHandler.CubeFolded += OnCubeFolded;
            cubeAnimator.SetBool(menuOpenAnimParam, false);
            StartCoroutine(Move(new Vector3(0, 3.25f * expandedScale, 0), Vector3.zero, animationLength));
        }

        /// <summary>
        /// Called if the cube has been fully folded and the animation has finished
        /// Activates the necessary controls for the cube view and returns the cube to its small size
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void OnCubeFolded(object sender, EventArgs e)
        {
            animationHandler.CubeFolded -= OnCubeFolded;
            SetControlsActive(true);
            NotificationSystem.Instance.HideMessage();
            StartCoroutine(FadeSize(collapsedScale * Vector3.one, 0.5f));
        }

        /// <summary>
        /// Unfolds the main menu from its compact cube form into its menu structure
        /// First increases the size of the cube, then deactivates controls visible in the cube form and then it starts the unfold animation
        /// </summary>
        public void UnFoldCube()
        {
            StopAllCoroutines();
            StartCoroutine(FadeSize(expandedScale * Vector3.one, 0.5f, () =>
            {
                SetControlsActive(false);
                cubeAnimator.SetBool(menuOpenAnimParam, true);
                StartCoroutine(Move(Vector3.zero, new Vector3(0, 3.25f * expandedScale, 0), animationLength));
            }));
            MenuOpen = true;
        }

        /// <summary>
        /// Fades the cube pivot between different sizes in a given time span
        /// </summary>
        /// <param name="endSize">The target size after the fade has happened</param>
        /// <param name="fadeTime">The time that fading between the current size and the endSize should take</param>
        /// <param name="OnFinished">Action which is executed once the fading has finished</param>
        /// <returns></returns>
        private IEnumerator FadeSize(Vector3 endSize, float fadeTime, Action OnFinished = null)
        {
            Vector3 startSize = cubePivot.localScale;
            float time = 0f;
            while (time < fadeTime)
            {
                cubePivot.localScale = Vector3.Lerp(startSize, endSize, time / fadeTime);
                time += Time.deltaTime;
                yield return null;
            }
            cubePivot.localScale = endSize;
            OnFinished?.Invoke();
        }

        private IEnumerator Move(Vector3 startPos, Vector3 endPos, float fadeTime, Action OnFinished = null)
        {
            float time = 0f;
            while (time < fadeTime)
            {
                cubePivot.localPosition = Vector3.Lerp(startPos, endPos, time / fadeTime);
                time += Time.deltaTime;
                yield return null;
            }
            cubePivot.localPosition = endPos;
            OnFinished?.Invoke();
        }

        /// <summary>
        /// Activates or deactivates the supplementary controls for the cube
        /// </summary>
        /// <param name="active">If true, controls will be activated</param>
        private void SetControlsActive(bool active)
        {
            onOffButton.gameObject.SetActive(active);
            appBarSpawner.gameObject.SetActive(active); // activate/deactivate bounding box-related components, e.g. collider
            appBarSpawner.SpawnedInstance.SetActive(active);
            labelsParent.SetActive(active);
        }
    }
}