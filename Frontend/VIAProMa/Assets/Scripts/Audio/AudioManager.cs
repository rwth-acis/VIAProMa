using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Tooltip("minDistance and maxDistance of AudioSource")]
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;

    // AudioManager is a singleton
    private void Start()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Plays the clip of the sound at the given position, using a temporary empty GameObject.
    /// </summary>
    /// <param name="sound"></param>
    /// <param name="position"></param>
    public void PlaySoundOnceAt(Sound sound, Vector3 position)
    {
        GameObject soundObject = new GameObject();
        soundObject.transform.position = position;

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.clip = sound.clip;
        source.volume = sound.volume;
        source.pitch = sound.pitch;

        source.playOnAwake = false;
        source.spatialBlend = 1;

        source.minDistance = minDistance;
        source.maxDistance = maxDistance;

        source.Play();

        //Destroy the empty GameObject after the clip has been played.
        Destroy(soundObject, sound.clip.length + 1f);
    }


}

[Serializable]
public class Sound
{
    public AudioClip clip;

    [Range(0,1)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    public Sound(AudioClip clip, float volume, float pitch)
    {
        this.clip = clip;
        this.volume = volume;
        this.pitch = pitch;
    }

}