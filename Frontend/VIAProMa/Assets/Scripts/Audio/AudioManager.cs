using System;
using i5.VIAProMa.UI.MainMenuCube;
using i5.VIAProMa.UI.Chat;
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
    [SerializeField] private Sound progressBarSound = new Sound(null);
    [SerializeField] private Sound buildingProgressSound = new Sound(null);
    [SerializeField] private Sound pingSound = new Sound(null);
    [SerializeField] private Sound errorSound = new Sound(null);
    [SerializeField] private Sound micOn = new Sound(null);
    [SerializeField] private Sound micOff = new Sound(null);

    private Transform mainCam;


    private void Start()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        mainCam = Camera.main.transform;

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

    internal void PlayErrorSound(Vector3 at)
    {
        PlaySoundOnceAt(errorSound, at);
    }

    public void PlayLogoffSound(Vector3 at)
    {
        PlaySoundOnceAt(logoffSound, at);
    }

    internal void PlayBuildingProgressSound(Vector3 at)
    {
        PlaySoundOnceAt(buildingProgressSound, at);
    }

    public void PlayProgressBarSound(Vector3 at)
    {
        PlaySoundOnceAt(progressBarSound, at);
    }

    public void PlayerPingSound(Vector3 at)
    {
        pingSound.maxDistance = Vector3.Distance(mainCam.position, at) + 1;
        PlaySoundOnceAt(pingSound, at);
    }

    /// <summary>
    /// Plays the Message sound at either the open Chat Menu or the Main Menu Cube
    /// </summary>
    public void PlayMessageSound()
    {
        Vector3 soundPlayPosition = new Vector3();
        if (FindObjectOfType<MainMenu>()) soundPlayPosition = FindObjectOfType<MainMenu>().transform.position;
        if (FindObjectOfType<ChatMenu>().enabled) soundPlayPosition = FindObjectOfType<ChatMenu>().transform.position;

        messageSound.maxDistance = Vector3.Distance(mainCam.position, soundPlayPosition) + 1; // +1 for a bit of buffer
        PlaySoundOnceAt(messageSound, soundPlayPosition);
    }

    public void PlayMicOnSound(Vector3 at)
    {
        PlaySoundOnceAt(micOn, at);
    }
    public void PlayMicOffSound(Vector3 at)
    {
        PlaySoundOnceAt(micOff, at);
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