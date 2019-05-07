using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New message pack", menuName = "MessageBadge Pack", order = 51)]
public class MessagePack : ScriptableObject
{
    [SerializeField] private MessageContent defaultMessage;
    [SerializeField] private List<MessageContent> availableMessages;

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
