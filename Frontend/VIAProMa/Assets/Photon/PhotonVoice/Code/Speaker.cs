// ----------------------------------------------------------------------------
// <copyright file="Speaker.cs" company="Exit Games GmbH">
//   Photon Voice for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// Component representing remote audio stream in local scene.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------
//#define USE_ONAUDIOFILTERREAD

using System;
using System.Collections;
using UnityEngine;


namespace Photon.Voice.Unity
{
    /// <summary> Component representing remote audio stream in local scene. </summary>
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Photon Voice/Speaker")]
    [DisallowMultipleComponent]
    public class Speaker : VoiceComponent
    {
        #region Private Fields

        private IAudioOut<float> audioOutput;

        private RemoteVoiceLink remoteVoiceLink;

        private bool initialized; // awake called

        [SerializeField]
        private bool playbackOnlyWhenEnabled;

        private bool useSeparateCoroutine = true;

        private Coroutine playbackCoroutine;

        #if USE_ONAUDIOFILTERREAD
        private AudioSyncBuffer<float> outBuffer;
        private int outputSampleRate;
        #endif

        [SerializeField]
        private int playDelayMs = 200;

        [SerializeField] 
        private PlaybackDelaySettings playbackDelaySettings = new PlaybackDelaySettings
        {
            MinDelaySoft = PlaybackDelaySettings.DEFAULT_LOW,
            MaxDelaySoft = PlaybackDelaySettings.DEFAULT_HIGH,
            MaxDelayHard = PlaybackDelaySettings.DEFAULT_MAX
        };

        private bool playbackExplicitlyStopped;

        #endregion

        #region Public Fields

        ///<summary>Remote audio stream playback delay to compensate packets latency variations. Try 100 - 200 if sound is choppy.</summary>
        [Obsolete("Use SetPlaybackDelaySettings methods instead")]
        public int PlayDelayMs
        {
            get
            {
                return this.playbackDelaySettings.MinDelaySoft;
            }
            set
            {
                if (value >= 0 && value < this.playbackDelaySettings.MaxDelaySoft)
                {
                    this.playbackDelaySettings.MinDelaySoft = value;
                }
            }
        }

#if UNITY_PS4 || UNITY_SHARLIN
        /// <summary>Set the PlayStation User ID to determine on which users headphones to play audio.</summary> 
        /// <remarks>
        /// Note: at the moment, only the first Speaker can successfully set the User ID. 
        /// Subsequently initialized Speakers will play their audio on the headphones that have been set with the first Speaker initialized.
        public int PlayStationUserID = 0;
#endif

        #endregion

        #region Properties

        /// <summary>Is the speaker playing right now.</summary>
        public bool IsPlaying
        {
            get { return this.audioOutput != null && this.audioOutput.IsPlaying; }
        }

        /// <summary>Smoothed difference between (jittering) stream and (clock-driven) audioOutput.</summary>
        public int Lag
        {
            get { return this.audioOutput != null ? this.audioOutput.Lag : -1; }
        }

        /// <summary>
        /// Register a method to be called when remote voice removed.
        /// </summary>
        public Action<Speaker> OnRemoteVoiceRemoveAction { get; set; }

        /// <summary>Per room, the connected users/players are represented with a Realtime.Player, also known as Actor.</summary>
        /// <remarks>Photon Voice calls this Actor, to avoid a name-clash with the Player class in Voice.</remarks>
        public Realtime.Player Actor { get; protected internal set; }

        /// <summary>
        /// Whether or not this Speaker has been linked to a remote voice stream.
        /// </summary>
        public bool IsLinked
        {
            get { return this.remoteVoiceLink != null; }
        }

        #if UNITY_EDITOR
        /// <summary>
        /// USE IN EDITOR ONLY
        /// </summary>
        public RemoteVoiceLink RemoteVoiceLink
        {
            get { return this.remoteVoiceLink; }
        }
        #endif

        /// <summary> If true, component will work only when enabled and active in hierarchy.  </summary>
        public bool PlaybackOnlyWhenEnabled
        {
            get { return this.playbackOnlyWhenEnabled; }
            set
            {
                if (this.playbackOnlyWhenEnabled != value)
                {
                    this.playbackOnlyWhenEnabled = value;
                    if (this.IsLinked)
                    {
                        if (this.playbackOnlyWhenEnabled)
                        {
                            if (this.isActiveAndEnabled != this.PlaybackStarted)
                            {
                                if (this.isActiveAndEnabled)
                                {
                                    if (!this.playbackExplicitlyStopped)
                                    {
                                        this.StartPlaying();
                                    }
                                }
                                else
                                {
                                    this.StopPlaying();
                                }
                            }
                        }
                        else if (!this.PlaybackStarted && !this.playbackExplicitlyStopped)
                        {
                            this.StartPlaying();
                        }
                    }
                }
            }
        }

        /// <summary> Returns if the playback is on. </summary>
        public bool PlaybackStarted { get; private set; }

        /// <summary>Gets the value in ms above which the audio player tries to keep the delay.</summary>
        public int PlaybackDelayMinSoft
        {
            get
            {
                return this.playbackDelaySettings.MinDelaySoft;
            }
        }

        /// <summary>Gets the value in ms below which the audio player tries to keep the delay.</summary>
        public int PlaybackDelayMaxSoft
        {
            get
            {
                return this.playbackDelaySettings.MaxDelaySoft;
            }
        }

        /// <summary>Gets the value in ms that audio play delay will not exceed.</summary>
        public int PlaybackDelayMaxHard
        {
            get
            {
                return this.playbackDelaySettings.MaxDelayHard;
            }
        }

        #endregion

        #region Private Methods
        
        private void OnEnable()
        {
            if (this.IsLinked && !this.PlaybackStarted && !this.playbackExplicitlyStopped)
            {
                this.StartPlaying();
            }
        }

        private void OnDisable()
        {
            if (this.PlaybackOnlyWhenEnabled && this.PlaybackStarted)
            {
                this.StopPlaying();
            }
        }

        private void Initialize()
        {
            if (this.initialized)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Already initialized.");
                }
                return;
            }
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("Initializing.");
            }
            #if USE_ONAUDIOFILTERREAD
            this.outBuffer = new AudioSyncBuffer<float>(this.playbackDelaySettings.MinDelaySoft, this.Logger, string.Empty, this.Logger.IsInfoEnabled);
            this.outputSampleRate = AudioSettings.outputSampleRate;
            Func<IAudioOut<float>> factory = () => this.outBuffer;
            #else
            Func<IAudioOut<float>> factory = () => new UnityAudioOut(this.GetComponent<AudioSource>(), 
                new UnityAudioOut.PlayDelayConfig { Low = this.playbackDelaySettings.MinDelaySoft, High = this.playbackDelaySettings.MaxDelaySoft, Max = this.playbackDelaySettings.MaxDelayHard }, 
                this.Logger, string.Empty, this.Logger.IsInfoEnabled);
            #endif

            #if !UNITY_EDITOR && (UNITY_PS4 || UNITY_SHARLIN)
            this.audioOutput = new Photon.Voice.PlayStation.PlayStationAudioOut(this.PlayStationUserID, factory);
            #else
            this.audioOutput = factory();
            #endif
            this.initialized = true;
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("Initialized.");
            }
        }

        internal bool OnRemoteVoiceInfo(RemoteVoiceLink stream)
        {
            if (stream == null)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("RemoteVoiceLink is null, cancelled linking");
                }
                return false;
            }
            if (!this.initialized)
            {
                this.Initialize();
            }
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("OnRemoteVoiceInfo {0}/{1}", stream.PlayerId, stream.PlayerId);
            }
            if (this.IsLinked)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Speaker already linked to {0}/{1}, cancelled linking to {2}/{3}",
                        this.remoteVoiceLink.PlayerId, this.remoteVoiceLink.VoiceId, stream.PlayerId, stream.VoiceId);
                }
                return false;
            }
            if (stream.Info.Channels <= 0) // early avoid possible crash due to ArgumentException in AudioClip.Create inside UnityAudioOut.Start
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Received voice info channels is not expected: {0} <= 0, cancelled linking to {1}/{2}", stream.Info.Channels, 
                        stream.PlayerId, stream.VoiceId);
                }
                return false;
            }
            this.remoteVoiceLink = stream;
            this.remoteVoiceLink.RemoteVoiceRemoved += this.OnRemoteVoiceRemove;
            return !this.initialized || this.StartPlaying();
        }

        internal void OnRemoteVoiceRemove()
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("OnRemoteVoiceRemove {0}/{1}", this.remoteVoiceLink.PlayerId, this.remoteVoiceLink.PlayerId);
            }
            this.StopPlaying();
            if (this.OnRemoteVoiceRemoveAction != null) { this.OnRemoteVoiceRemoveAction(this); }
            this.CleanUp();
        }

        internal void OnAudioFrame(FrameOut<float> frame)
        {
            this.audioOutput.Push(frame.Buf);
            if (frame.EndOfStream)
            {
                this.audioOutput.Flush();
            }
        }
        
        private bool StartPlaying()
        {
            if (!this.IsLinked)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot start playback because speaker is not linked");
                }
                return false;
            }
            if (this.PlaybackStarted)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Playback is already started");
                }
                return false;
            }
            if (!this.initialized)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot start playback because not initialized yet");
                }
                return false;
            }
            if (this.isActiveAndEnabled && this.PlaybackOnlyWhenEnabled)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot start playback because PlaybackOnlyWhenEnabled is true and Speaker is not enabled or its GameObject is not active in the hierarchy.");
                }
                return false;
            }
            if (this.audioOutput == null)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogWarning("Cannot start playback because audioOutput is null");
                }
                return false;
            }
            VoiceInfo voiceInfo = this.remoteVoiceLink.Info;
            if (voiceInfo.Channels == 0)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Cannot start playback because remoteVoiceLink.Info.Channels == 0");
                }
                return false;
            }
            if (this.Logger.IsInfoEnabled)
            {
                this.Logger.LogInfo("Speaker about to start playback (v#{0}/p#{1}/c#{2}), i=[{3}], d={4}", 
                    this.remoteVoiceLink.VoiceId, this.remoteVoiceLink.PlayerId, this.remoteVoiceLink.ChannelId, voiceInfo, this.playbackDelaySettings);
            }
            this.audioOutput.Start(voiceInfo.SamplingRate, voiceInfo.Channels, voiceInfo.FrameDurationSamples);
            this.remoteVoiceLink.FloatFrameDecoded += this.OnAudioFrame;
            this.PlaybackStarted = true;
            this.playbackExplicitlyStopped = false;
            if (this.useSeparateCoroutine)
            {
                this.playbackCoroutine = this.StartCoroutine(this.PlaybackCoroutine());
            }
            return true;
        }

        private void OnDestroy()
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("OnDestroy");
            }
            if (this.PlaybackStarted)
            {
                this.StopPlaying();
            }
            this.CleanUp();
        }
        
        private bool StopPlaying()
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("StopPlaying");
            }
            if (!this.PlaybackStarted)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot stop playback because it's not started");
                }
                return false;
            }
            if (this.IsLinked)
            {
                this.remoteVoiceLink.FloatFrameDecoded -= this.OnAudioFrame;
            }
            else if (this.Logger.IsWarningEnabled)
            {
                this.Logger.LogWarning("Speaker not linked while stopping playback");
            }
            if (this.audioOutput != null)
            {
                this.audioOutput.Stop();
            }
            else if (this.Logger.IsWarningEnabled)
            {
                this.Logger.LogWarning("audioOutput is null while stopping playback");
            }
            if (this.useSeparateCoroutine || this.playbackCoroutine != null)
            {
                this.StopCoroutine(this.playbackCoroutine);
                this.playbackCoroutine = null;
            }
            this.PlaybackStarted = false;
            return true;
        }

        private void CleanUp()
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("CleanUp");
            }
            if (this.remoteVoiceLink != null)
            {
                this.remoteVoiceLink.RemoteVoiceRemoved -= this.OnRemoteVoiceRemove;
                this.remoteVoiceLink = null;
            }
            this.Actor = null;
        }

        private IEnumerator PlaybackCoroutine()
        {
            while (this.PlaybackStarted && this.useSeparateCoroutine)
            {
                this.audioOutput.Service();
                yield return null;
            }
        }

        private IEnumerator Start()
        {
            while (!this.useSeparateCoroutine)
            {
                this.audioOutput.Service();
                yield return null;
            }
        }

        #if USE_ONAUDIOFILTERREAD
        private void OnAudioFilterRead(float[] data, int channels)
        {
            this.outBuffer.Read(data, channels, this.outputSampleRate);
        }
        #endif

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (this.playDelayMs > 0)
            {
                if (this.playbackDelaySettings.MinDelaySoft != this.playDelayMs)
                {
                    this.playbackDelaySettings.MinDelaySoft = this.playDelayMs;
                    if (this.playbackDelaySettings.MaxDelaySoft <= this.playbackDelaySettings.MinDelaySoft)
                    {
                        this.playbackDelaySettings.MaxDelaySoft = 2 * this.playbackDelaySettings.MinDelaySoft;
                        if (this.playbackDelaySettings.MaxDelayHard < this.playbackDelaySettings.MaxDelaySoft)
                        {
                            this.playbackDelaySettings.MaxDelayHard = this.playbackDelaySettings.MaxDelaySoft + 1000;
                        }
                    }
                }
                this.playDelayMs = -1;
            }
        }
        #endif

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the audio playback of the linked incoming remote audio stream via AudioSource component.
        /// </summary>
        /// <returns>True if playback is successfully started.</returns>
        public bool StartPlayback()
        {
            return this.StartPlaying();
        }

        /// <summary>
        /// Stops the audio playback of the linked incoming remote audio stream via AudioSource component.
        /// </summary>
        /// <returns>True if playback is successfully stopped.</returns>
        public bool StopPlayback()
        {
            if (this.playbackExplicitlyStopped)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot stop playback because it was already been explicitly stopped.");
                }
                return false;
            }
            this.playbackExplicitlyStopped = this.StopPlaying();
            return this.playbackExplicitlyStopped;
        }

        /// <summary>
        /// Restarts the audio playback of the linked incoming remote audio stream via AudioSource component.
        /// </summary>
        /// <returns>True if playback is successfully restarted.</returns>
        public bool RestartPlayback()
        {
            return this.StopPlayback() && this.StartPlayback();
        }

        /// <summary>
        /// Sets the settings for the playback behaviour in case of delays.
        /// </summary>
        /// <param name="pdc">Playback delay configuration struct.</param>
        /// <returns>If a change has been made.</returns>
        public bool SetPlaybackDelaySettings(PlaybackDelaySettings pdc)
        {
            return this.SetPlaybackDelaySettings(pdc.MinDelaySoft, pdc.MaxDelaySoft, pdc.MaxDelayHard);
        }
        
        /// <summary>
        /// Sets the settings for the playback behaviour in case of delays.
        /// </summary>
        /// <param name="low">In milliseconds, audio player tries to keep the playback delay above this value.</param>
        /// <param name="high">In milliseconds, audio player tries to keep the playback below above this value.</param>
        /// <param name="max">In milliseconds, audio player guarantees that the playback delay never exceeds this value.</param>
        /// <returns>If a change has been made.</returns>
        public bool SetPlaybackDelaySettings(int low, int high, int max)
        {
            if (low >= 0 && low < high)
            {
                if (this.playbackDelaySettings.MaxDelaySoft != high ||
                    this.playbackDelaySettings.MinDelaySoft != low ||
                    this.playbackDelaySettings.MaxDelayHard != max)
                {
                    if (max < high)
                    {
                        max = high;
                    }
                    this.playbackDelaySettings.MaxDelaySoft = high;
                    this.playbackDelaySettings.MinDelaySoft = low;
                    this.playbackDelaySettings.MaxDelayHard = max;
                    bool wasPlaying = this.IsPlaying;
                    if (this.IsPlaying)
                    {
                        this.StopPlaying();
                    }
                    bool wasInitialized = this.initialized;
                    if (this.initialized)
                    {
                        this.initialized = false;
                        this.audioOutput = null;
                    }
                    if (wasInitialized)
                    {
                        this.Initialize();
                        if (wasPlaying)
                        {
                            this.StartPlaying();
                        }
                    }
                    return true;
                }
            } 
            else if (this.Logger.IsErrorEnabled)
            {
                this.Logger.LogError("Wrong playback delay config values, make sure 0 <= Low < High, low={0}, high={1}, max={2}", low, high, max);
            }
            return false;
        }

        #endregion
    }
}