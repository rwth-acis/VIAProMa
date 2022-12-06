using i5.VIAProMa.UI.MessageBadge;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using UnityEngine;

namespace i5.VIAProMa.Shelves
{
    public class Shelf : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] protected MessageBadge messageBadge;
        [SerializeField] protected Interactable upButton;
        [SerializeField] protected Interactable downButton;

        protected int page;

        public event EventHandler PageChanged;

        public virtual int Page
        {
            get => page;
            set
            {
                page = Mathf.Max(0, value);
                PageChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected virtual void Awake()
        {
            if (messageBadge == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(messageBadge));
            }
            if (upButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(upButton));
            }
            if (downButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(downButton));
            }
        }

        protected virtual void CheckControls()
        {
            if (page <= 0)
            {
                upButton.IsEnabled = false;
            }
            else
            {
                upButton.IsEnabled = true;
            }
        }

        public void ResetPage()
        {
            Page = 0;
        }

        public virtual void ScrollUp()
        {
            Page--;
        }

        public virtual void ScrollDown()
        {
            Page++;
        }
    }
}