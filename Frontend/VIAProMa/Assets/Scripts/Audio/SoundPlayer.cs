using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{

    [SerializeField] private Sound sound = new Sound(null, 1, 1);

    public void PlaySoundHere()
    {
        AudioManager.instance?.PlaySoundOnceAt(sound, gameObject.transform.position);
    }

}
