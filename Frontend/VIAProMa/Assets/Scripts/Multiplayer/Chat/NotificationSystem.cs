using HoloToolkit.Unity;
using i5.VIAProMa.UI;
using i5.VIAProMa.Utilities;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Chat
{
    public class NotificationSystem : Singleton<NotificationSystem>
    {
        [SerializeField] private GameObject notificationWidget;
        [SerializeField] private TextMeshPro notificationPreviewLabel;

        public bool CanShowMessages { get; set; } = true;

        protected override void Awake()
        {
            base.Awake();
            if (notificationWidget == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(notificationWidget));
            }
            if (notificationPreviewLabel == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(notificationPreviewLabel));
            }
        }

        private void Start()
        {
            ChatManager.Instance.MessageReceived += OnMessageReceived;
            HideMessage();
        }

        protected override void OnDestroy()
        {
            if (ChatManager.Instance != null)
            {
                ChatManager.Instance.MessageReceived -= OnMessageReceived;
            }
            base.OnDestroy();
        }

        public void ShowMessage(string text)
        {
            if (CanShowMessages)
            {
                gameObject.SetActive(true);
                notificationPreviewLabel.text = text;
            }
        }

        public void HideMessage()
        {
            notificationPreviewLabel.text = "";
            gameObject.SetActive(false);
        }

        public void OpenChatMenu()
        {
            WindowManager.Instance.ChatMenu.Open(
                transform.position - 0.6f * notificationPreviewLabel.transform.right,
                notificationPreviewLabel.transform.eulerAngles
                );
        }

        private void OnMessageReceived(object sender, ChatMessageEventArgs e)
        {
            if (e.MessageSender == null) // local message
            {
                ShowMessage(e.Message);
            }
            else
            {
                ShowMessage(e.MessageSender.NickName + ": " + e.Message);
            }
        }
    }
}
