using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.UI.MessageBadge
{
    /// <summary>
    /// Class which controls the behavior of a MessageBadge
    /// The MessageBadge is used to give feedback to the user about the success or error type of operations
    /// </summary>
    public class MessageBadge : MonoBehaviour
    {
        [SerializeField] private ProcessingEffect processingEffect;
        [SerializeField] private SpriteRenderer iconRenderer;
        [SerializeField] private TextMeshPro messageText;
        [SerializeField] private MessagePack messages;
        [SerializeField] private Interactable tryAgainButton;

        public event OnShowMessageDelegate OnShowMessage;
        public delegate void OnShowMessageDelegate(long messageID);

        private MessageContent content;
        private int processing = 0;

        public Action TryAgainAction { get; set; }

        /// <summary>
        /// Initialization
        /// Checks if the component was set up correctly
        /// </summary>
        private void Awake()
        {
            if (processingEffect == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(processingEffect));
            }
            if (iconRenderer == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(processingEffect));
            }
            if (messageText == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(processingEffect));
            }
            if (messages == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(messages));
            }
        }

        /// <summary>
        /// Shows the processing animation on the badge
        /// </summary>
        public void ShowProcessing()
        {
            ShowMessage(-2);
            processing++;
            processingEffect.IsProcessing = true;
            tryAgainButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Decrements the number of running operations
        /// If no operations are active anymore, the processing animation is deactivated
        /// </summary>
        public void DoneProcessing()
        {
            processing--;
            if (processing <= 0)
            {
                processing = 0;
                processingEffect.IsProcessing = false;
                tryAgainButton.gameObject.SetActive(true);
                Hide();
            }
        }

        /// <summary>
        /// Updates the content of the message badge based on the newContent
        /// </summary>
        /// <param name="newContent">The new message content which should be displayed on the badge</param>
        private void SetContent(MessageContent newContent)
        {
            gameObject.SetActive(true);
            content = newContent;
            iconRenderer.sprite = content.Icon;
            messageText.text = content.Text;
        }

        /// <summary>
        /// Display a message which is based on some message code
        /// The message codes should be set up in a way that they correspond to HTTP status codes + some extras
        /// </summary>
        /// <param name="messageCode">The code of the message to display</param>
        public void ShowMessage(long messageCode)
        {
            SetContent(messages.GetMessage(messageCode));
            OnShowMessage?.Invoke(messageCode);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnTryAgainClicked()
        {
            TryAgainAction?.Invoke();
        }
    }
}