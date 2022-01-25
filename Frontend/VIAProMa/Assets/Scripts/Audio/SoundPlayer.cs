using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
namespace i5.VIAProMa.Audio
{
    public class SoundPlayer : MonoBehaviour
    {

        [SerializeField] private Sound sound = new Sound(null, 1, 1);

        private void Awake()
        {
            if (GetComponent<Interactable>() == null)
            {
                Debug.LogWarning("SoundPlayer component could not find Interactable component!");
                return;
            }
            GetComponent<Interactable>().OnClick.AddListener(PlaySoundHere);
        }

        private void OnDestroy()
        {
            if (GetComponent<Interactable>() == null)
            {
                Debug.LogWarning("SoundPlayer component could not find Interactable component!");
                return;
            }
            GetComponent<Interactable>().OnClick.RemoveListener(PlaySoundHere);
        }

        public void PlaySoundHere()
        {
            AudioManager.instance?.PlaySoundOnceAt(sound, gameObject.transform.position);
        }

    }
}