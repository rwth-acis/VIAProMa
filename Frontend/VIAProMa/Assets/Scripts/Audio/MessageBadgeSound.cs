using i5.Toolkit.Core.Utilities;
using i5.VIAProMa.UI.MessageBadge;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.Audio
{
    [RequireComponent(typeof(MessageBadge))]
    public class MessageBadgeSound : MonoBehaviour
    {
        [SerializeField] private Sound errorSound = new Sound(null);
        [SerializeField] private List<long> errorIDs = new List<long>() { -1, 0, 4, 403, 500 };

        public void OnEnable()
        {
            GetComponent<MessageBadge>().OnShowMessage += OnShowMessage;
        }

        public void OnDisable()
        {
            GetComponent<MessageBadge>().OnShowMessage -= OnShowMessage;
        }

        private void OnShowMessage(long messageID)
        {
            Debug.Log(messageID);
            if (errorIDs.Contains(messageID))
            {
                Debug.Log("Playing error sound");
                AudioManager.instance.PlaySoundOnceAt(errorSound, transform.position);
            }
        }
    }
}
