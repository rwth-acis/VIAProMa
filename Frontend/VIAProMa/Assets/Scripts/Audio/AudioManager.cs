using System;
using i5.VIAProMa.UI.MainMenuCube;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // AudioManager is a singleton
    public static AudioManager instance;

    [SerializeField]
    private AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0) });

    [SerializeField] private Sound loginSound = new Sound(null);
    [SerializeField] private Sound logoffSound = new Sound(null);
    [SerializeField] private Sound messageSound = new Sound(null);

    private void Start()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        foreach (Keyframe key in curve.keys)
        {
            if (key.value > 1 || key.value < 0)
            {
                Debug.LogWarning("AudioManager: Curve goes out of bounds! (0 to 1)");
                break;
            }
        }

        gameObject.AddComponent<AudioListener>();
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

    public void PlayLoginSound(Vector3 at)
    {
        PlaySoundOnceAt(loginSound, at);
    }

    public void PlayLogoffSound(Vector3 at)
    {
        PlaySoundOnceAt(logoffSound, at);
    }

    public void PlayMessageSound()
    {
        Vector3 mainMenuPosition = FindObjectOfType<MainMenu>().gameObject.transform.position;
        messageSound.maxDistance = Vector3.Distance(Camera.main.transform.position, mainMenuPosition) + 1; // +1 for a bit of buffer
        PlaySoundOnceAt(messageSound, mainMenuPosition);
    }
}

/// <summary>
/// A class that holds information for an AudioClip
/// </summary>
[Serializable]
public class Sound
{
    public AudioClip clip;

    [Range(0,1)]
    public float volume = 0.42f;
    [Range(0.1f, 3f)]
    public float pitch = 1;

    public float maxDistance = 5;

    public Sound(AudioClip clip, float volume = 1, float pitch = 1, float maxDistance = 5)
    {
        this.clip = clip;
        this.volume = volume;
        this.pitch = pitch;
        this.maxDistance = maxDistance;
    }

}