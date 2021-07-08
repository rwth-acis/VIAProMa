using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.UI.MessageBadge
{
    /// <summary>
    /// Bundles a number of message contents togehter into one configuration
    /// </summary>
    [CreateAssetMenu(fileName = "New message pack", menuName = "MessageBadge Pack", order = 51)]
    public class MessagePack : ScriptableObject
    {
        /// <summary>
        /// The default message which should be displayed if a message code is queried for which no message content is defined
        /// </summary>
        [SerializeField] private MessageContent defaultMessage;
        /// <summary>
        /// The available messages in this bundle
        /// </summary>
        [SerializeField] private List<MessageContent> availableMessages;

        /// <summary>
        /// Returns a message with the given code from this bundle
        /// </summary>
        /// <param name="code">The message code</param>
        /// <returns>A message content with the given code; if it does not exist it returns the default message</returns>
        public MessageContent GetMessage(long code)
        {
            if (availableMessages.Exists(x => x.Code == code))
            {
                return availableMessages.Find(x => x.Code == code);
            }
            else
            {
                return defaultMessage;
            }
        }
    }
}