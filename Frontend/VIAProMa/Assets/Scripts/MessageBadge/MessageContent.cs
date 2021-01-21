using UnityEngine;

namespace i5.VIAProMa.UI.MessageBadge
{
    /// <summary>
    /// Content configuration for the message badge
    /// </summary>
    [CreateAssetMenu(fileName = "New message", menuName = "MessageBadge Content", order = 51)]
    public class MessageContent : ScriptableObject
    {
        /// <summary>
        /// Message code; this will be used to determine which message to show
        /// </summary>
        [SerializeField] private long code;
        /// <summary>
        /// Icon texture to show on the message badge; should be circular
        /// </summary>
        [SerializeField] private Sprite icon;
        /// <summary>
        /// Text message on the icon
        /// </summary>
        [SerializeField] private string text;

        /// <summary>
        /// The message code
        /// </summary>
        public long Code { get => code; set => code = value; }
        /// <summary>
        /// The icon to display on the message badge
        /// </summary>
        public Sprite Icon { get => icon; }
        /// <summary>
        /// The text to show on hte message badge
        /// </summary>
        public string Text { get => text; }
    }
}