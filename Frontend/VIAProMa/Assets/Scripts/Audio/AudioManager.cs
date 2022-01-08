﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0,1), new Keyframe(1,0) });

    // AudioManager is a singleton
    private void Start()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        foreach (Keyframe key in curve.keys)
        {
            if (key.value > 1 || key.value < 0)
            {
                Debug.LogWarning("AudioManager: Curve goes out of bounds! (0 to 1)");
                break;
            }
        }
        
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

        source.rolloffMode = AudioRolloffMode.Custom;
        source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);

        source.playOnAwake = false;
        source.spatialBlend = 1;

        source.maxDistance = sound.maxDistance;

        
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

    public float maxDistance;

    public Sound(AudioClip clip, float volume = 1, float pitch = 1, float maxDistance = 5)
    {
        this.clip = clip;
        this.volume = volume;
        this.pitch = pitch;
        this.maxDistance = maxDistance;
    }

}