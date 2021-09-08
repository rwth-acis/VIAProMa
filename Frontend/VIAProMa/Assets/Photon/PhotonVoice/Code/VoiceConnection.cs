// ----------------------------------------------------------------------------
// <copyright file="Recorder.cs" company="Exit Games GmbH">
//   Photon Voice for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//  Component that represents a client voice connection to Photon Servers.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

//#define USE_NEW_TRANSPORT

using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace Photon.Voice.Unity
{
    /// <summary> Component that represents a client voice connection to Photon Servers. </summary>
    [AddComponentMenu("Photon Voice/Voice Connection")]
    [HelpURL("https://doc.photonengine.com/en-us/voice/v2/getting-started/voice-intro")]
    public class VoiceConnection : ConnectionHandler, ILoggable
    {
        #region Private Fields

        private VoiceLogger logger;

        [SerializeField]
        private DebugLevel logLevel = DebugLevel.INFO;

        /// <summary>Key to save the "Best Region Summary" in the Player Preferences.</summary>
        private const string PlayerPrefsKey = "VoiceCloudBestRegion";
        
        private LoadBalancingTransport client;
        [SerializeField]
        private bool enableSupportLogger = false;

        private SupportLogger supportLoggerComponent;

        /// <summary>
        /// time [ms] between consecutive SendOutgoingCommands calls
        /// </summary>
        [SerializeField]
        private int updateInterval = 50;

        private int nextSendTickCount;

        #if UNITY_EDITOR || !UNITY_ANDROID && !UNITY_IOS
        [SerializeField]
        private bool runInBackground = true;
        #endif

        /// <summary>
        /// time [ms] between statistics calculations
        /// </summary>
        [SerializeField]
        private int statsResetInterval = 1000;

        private int nextStatsTickCount;

        private float statsReferenceTime;
        private int referenceFramesLost;
        private int referenceFramesReceived;

        [SerializeField]
        private GameObject speakerPrefab;

        private bool cleanedUp;

        protected List<RemoteVoiceLink> cachedRemoteVoices = new List<RemoteVoiceLink>();

        [SerializeField]
        [FormerlySerializedAs("PrimaryRecorder")]
        private Recorder primaryRecorder;

        private bool primaryRecorderInitialized;

        [SerializeField]
        private DebugLevel globalRecordersLogLevel = DebugLevel.INFO;
        [SerializeField]
        private DebugLevel globalSpeakersLogLevel = DebugLevel.INFO;

        [SerializeField]
        private int globalPlaybackDelay = 200;

        [SerializeField]
        private PlaybackDelaySettings globalPlaybackDelaySettings = new PlaybackDelaySettings
        {
            MinDelaySoft = PlaybackDelaySettings.DEFAULT_LOW,
            MaxDelaySoft = PlaybackDelaySettings.DEFAULT_HIGH,
            MaxDelayHard = PlaybackDelaySettings.DEFAULT_MAX
        };

        private List<Speaker> linkedSpeakers = new List<Speaker>();

        #endregion

        #region Public Fields

        /// <summary> Settings to be used by this voice connection</summary>
        public AppSettings Settings;
        #if UNITY_EDITOR
        [HideInInspector]
        public bool ShowSettings = true;
        #endif

        /// <summary> Special factory to link Speaker components with incoming remote audio streams</summary>
        public Func<int, byte, object, Speaker> SpeakerFactory;
        /// <summary> Fires when a speaker has been linked to a remote audio stream</summary>
        public event Action<Speaker> SpeakerLinked;
        /// <summary> Fires when a remote voice stream is added</summary>
        public event Action<RemoteVoiceLink> RemoteVoiceAdded;

#if UNITY_PS4 || UNITY_SHARLIN
        /// <summary>PlayStation user ID of the local user</summary>
        /// <remarks>Pass the userID of the local PlayStation user who should receive any incoming audio. This value is used by Photon Voice when sending output to the headphones on the PlayStation.
        /// If you don't provide a user ID, then Photon Voice uses the user ID of the user at index 0 in the list of local users
        /// and in case that there are multiple local users, the audio output might be sent to the headphones of a different user than intended.</remarks>
        public int PlayStationUserID = 0; // set from your games code
#endif
        
        /// <summary>Configures the minimal Time.timeScale at which Voice client will dispatch incoming messages within LateUpdate.</summary>
        /// <remarks>
        /// It may make sense to dispatch incoming messages, even if the timeScale is near 0.
        /// In some cases, stopping the game time makes sense, so this option defaults to -1f, which is "off".
        /// Without dispatching messages, Voice client won't change state and does not handle updates.
        /// </remarks>
        public float MinimalTimeScaleToDispatchInFixedUpdate = -1f;
        
        /// <summary> Auto instantiate a GameObject and attach a Speaker component to link to a remote audio stream if no candidate could be found </summary>
        public bool AutoCreateSpeakerIfNotFound = true;
        
        #endregion

        #region Properties
        /// <summary> Logger used by this component</summary>
        public VoiceLogger Logger
        {
            get
            {
                if (this.logger == null)
                {
                    this.logger = new VoiceLogger(this, string.Format("{0}.{1}", this.name, this.GetType().Name), this.logLevel);
                }
                return this.logger;
            }
            protected set { this.logger = value; }
        }
        /// <summary> Log level for this component</summary>
        public DebugLevel LogLevel
        {
            get
            {
                if (this.Logger != null)
                {
                    this.logLevel = this.Logger.LogLevel;
                }
                return this.logLevel;
            }
            set
            {
                this.logLevel = value;
                if (this.Logger == null)
                {
                    return;
                }
                this.Logger.LogLevel = this.logLevel;
            }
        }

        public new LoadBalancingTransport Client
        {
            get
            {
                if (this.client == null)
                {
                    #if USE_NEW_TRANSPORT
                    this.client = new LoadBalancingTransport2();
                    #else
                    this.client = new LoadBalancingTransport();
                    #endif
                    this.client.VoiceClient.OnRemoteVoiceInfoAction += this.OnRemoteVoiceInfo;
                    this.client.StateChanged += this.OnVoiceStateChanged;
                    if (this.Settings != null && this.LogLevel < this.Settings.NetworkLogging)
                    {
                        this.client.LoadBalancingPeer.DebugOut = this.Settings.NetworkLogging;
                    }
                    else
                    {
                        this.client.LoadBalancingPeer.DebugOut = this.LogLevel;
                    }
                    base.Client = this.client;
                    this.StartFallbackSendAckThread();
                }
                return this.client;
            }
        }
        
        /// <summary>Returns underlying Photon Voice client.</summary>
        public VoiceClient VoiceClient { get { return this.Client.VoiceClient; } }

        /// <summary>Returns Photon Voice client state.</summary>
        public ClientState ClientState { get { return this.Client.State; } }

        /// <summary>Number of frames received per second.</summary>
        public float FramesReceivedPerSecond { get; private set; }
        /// <summary>Number of frames lost per second.</summary>
        public float FramesLostPerSecond { get; private set; }
        /// <summary>Percentage of lost frames.</summary>
        public float FramesLostPercent { get; private set; }

        /// <summary> Prefab that contains Speaker component to be instantiated when receiving a new remote audio source info</summary>
        public GameObject SpeakerPrefab
        {
            get { return this.speakerPrefab; }
            set
            {
                if (value != this.speakerPrefab)
                {
                    if (value != null && value.GetComponentInChildren<Speaker>() == null)
                    {
                        #if UNITY_EDITOR
                        Debug.LogError("SpeakerPrefab must have a component of type Speaker in its hierarchy.", this);
                        #else
                        if (this.Logger.IsErrorEnabled)
                        {
                            this.Logger.LogError("SpeakerPrefab must have a component of type Speaker in its hierarchy.");
                        }
                        #endif
                        return;
                    }
                    this.speakerPrefab = value;
                }
            }
        }

        
        #if UNITY_EDITOR
        public List<RemoteVoiceLink> CachedRemoteVoices
        {
            get { return this.cachedRemoteVoices; }
        }
        #endif

        /// <summary> Main Recorder to be used for transmission by default</summary>
        public Recorder PrimaryRecorder
        {
            get
            {
                if (!this.primaryRecorderInitialized)
                {
                    this.TryInitializePrimaryRecorder();
                }
                return this.primaryRecorder;
            }
            set
            {
                this.primaryRecorder = value;
                this.primaryRecorderInitialized = false;
                this.TryInitializePrimaryRecorder();
            }
        }

        public DebugLevel GlobalRecordersLogLevel
        {
            get { return this.globalRecordersLogLevel; }
            set
            {
                if (this.globalRecordersLogLevel != value)
                {
                    this.globalRecordersLogLevel = value;
                    #if UNITY_EDITOR
                    Recorder[] recorders = FindObjectsOfType<Recorder>(); // todo: get rid of this ugliness
                    for (int i = 0; i < recorders.Length; i++)
                    {
                        if (!recorders[i].IgnoreGlobalLogLevel)
                        {
                            recorders[i].LogLevel = this.globalRecordersLogLevel;
                        }
                    }
                    #endif
                }
            }
        }

        public DebugLevel GlobalSpeakersLogLevel
        {
            get { return this.globalSpeakersLogLevel; }
            set
            {
                if (this.globalSpeakersLogLevel != value)
                {
                    this.globalSpeakersLogLevel = value;
                    for (int i = 0; i < this.linkedSpeakers.Count; i++)
                    {
                        this.linkedSpeakers[i].LogLevel = this.globalSpeakersLogLevel;
                    }
                }
            }
        }

        [Obsolete("Use SetGlobalPlaybackDelayConfiguration methods instead")]
        public int GlobalPlaybackDelay 
        {
            get
            {
                return this.globalPlaybackDelaySettings.MinDelaySoft;
            }
            set
            {
                if (value >= 0 && value <= this.globalPlaybackDelaySettings.MaxDelaySoft)
                {
                    this.globalPlaybackDelaySettings.MinDelaySoft = value;
                }
            }
        }

        /// <summary>Used to store and access the "Best Region Summary" in the Player Preferences.</summary>
        public string BestRegionSummaryInPreferences
        {
            get
            {
                return PlayerPrefs.GetString(PlayerPrefsKey, null);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    PlayerPrefs.DeleteKey(PlayerPrefsKey);
                }
                else
                {
                    PlayerPrefs.SetString(PlayerPrefsKey, value);
                }
            }
        }

        /// <summary>Gets the global value in ms above which the audio player tries to keep the delay.</summary>
        public int GlobalPlaybackDelayMinSoft
        {
            get
            {
                return this.globalPlaybackDelaySettings.MinDelaySoft;
            }
        }

        /// <summary>Gets the global value in ms below which the audio player tries to keep the delay.</summary>
        public int GlobalPlaybackDelayMaxSoft
        {
            get
            {
                return this.globalPlaybackDelaySettings.MaxDelaySoft;
            }
        }

        /// <summary>Gets the global value in ms that audio play delay will not exceed.</summary>
        public int GlobalPlaybackDelayMaxHard
        {
            get
            {
                return this.globalPlaybackDelaySettings.MaxDelayHard;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Connect to Photon server using <see cref="Settings"/>
        /// </summary>
        /// <param name="overwriteSettings">Overwrites <see cref="Settings"/> before connecting</param>
        /// <returns>If true voice connection command was sent from client</returns>
        public bool ConnectUsingSettings(AppSettings overwriteSettings = null)
        {
            if (this.Client.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("ConnectUsingSettings() failed. Can only connect while in state 'Disconnected'. Current state: {0}", this.Client.LoadBalancingPeer.PeerState);
                }
                return false;
            }
            if (AppQuits)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Can't connect: Application is closing. Unity called OnApplicationQuit().");
                }
                return false;
            }
            if (overwriteSettings != null)
            {
                this.Settings = overwriteSettings;
            }
            if (this.Settings == null)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Settings are null");
                }
                return false;
            }
            if (string.IsNullOrEmpty(this.Settings.AppIdVoice) && string.IsNullOrEmpty(this.Settings.Server))
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Provide an AppId or a Server address in Settings to be able to connect");
                }
                return false;
            }
            DebugLevel originalDebugLevel = this.Settings.NetworkLogging;
            if (this.LogLevel > this.Settings.NetworkLogging)
            {
                this.Settings.NetworkLogging = this.LogLevel; // this is a trick to set PhotonVoiceApi (Photon Voice Core, [PV]) log level which shares the PhotonPeer one
            }
            if (this.Settings.IsMasterServerAddress && string.IsNullOrEmpty(this.Client.UserId))
            {
                this.Client.UserId = Guid.NewGuid().ToString(); // this is a workaround to use when connecting to self-hosted Photon Server v4, which does not return a UserId to the client if generated randomly server side
            }
            if (string.IsNullOrEmpty(this.Settings.BestRegionSummaryFromStorage))
            {
                this.Settings.BestRegionSummaryFromStorage = this.BestRegionSummaryInPreferences;
            }
            string originalVoiceAppId = this.Settings.AppIdVoice;
            string originalRealtimeAppId = this.Settings.AppIdRealtime;
            this.Settings.AppIdRealtime = this.Settings.AppIdVoice;
            bool result = this.client.ConnectUsingSettings(this.Settings);
            this.Settings.AppIdVoice = originalVoiceAppId;
            this.Settings.AppIdRealtime = originalRealtimeAppId;
            this.Settings.NetworkLogging = originalDebugLevel;
            return result;
        }

        /// <summary>
        /// Initializes the Recorder component to be able to transmit audio.
        /// </summary>
        /// <param name="rec">The Recorder to be initialized.</param>
        public void InitRecorder(Recorder rec)
        {
            if (rec == null)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("rec is null.");
                }
                return;
            }
            rec.Init(this);
        }

        /// <summary>
        /// Sets the global configuration for the playback behaviour in case of delays.
        /// </summary>
        /// <param name="gpds">Playback delay configuration struct.</param>
        public void SetPlaybackDelaySettings(PlaybackDelaySettings gpds)
        {
            this.SetGlobalPlaybackDelaySettings(gpds.MinDelaySoft, gpds.MaxDelaySoft, gpds.MaxDelayHard);
        }

        /// <summary>
        /// Sets the global configuration for the playback behaviour in case of delays.
        /// </summary>
        /// <param name="low">In milliseconds, audio player tries to keep the playback delay above this value.</param>
        /// <param name="high">In milliseconds, audio player tries to keep the playback below above this value.</param>
        /// <param name="max">In milliseconds, audio player guarantees that the playback delay never exceeds this value.</param>
        public void SetGlobalPlaybackDelaySettings(int low, int high, int max)
        {
            if (low >= 0 && low < high)
            {
                if (max < high)
                {
                    max = high;
                }
                this.globalPlaybackDelaySettings.MinDelaySoft = low;
                this.globalPlaybackDelaySettings.MaxDelaySoft = high;
                this.globalPlaybackDelaySettings.MaxDelayHard = max;
                for (int i = 0; i < this.linkedSpeakers.Count; i++)
                {
                    this.linkedSpeakers[i].SetPlaybackDelaySettings(this.globalPlaybackDelaySettings);
                }
            }
            else if (this.Logger.IsErrorEnabled)
            {
                this.Logger.LogError("Wrong playback delay config values, make sure 0 <= Low < High, low={0}, high={1}, max={2}", low, high, max);
            }
        }

        #endregion

        #region Private Methods

        protected override void Awake()
        {
            base.Awake();
            if (this.SpeakerFactory == null)
            {
                this.SpeakerFactory = this.SimpleSpeakerFactory;
            }
            if (this.enableSupportLogger)
            {
                this.supportLoggerComponent = this.gameObject.AddComponent<SupportLogger>();
                this.supportLoggerComponent.Client = this.Client;
                this.supportLoggerComponent.LogTrafficStats = true;
            }
            #if UNITY_EDITOR || !UNITY_ANDROID && !UNITY_IOS
            if (this.runInBackground)
            {
                Application.runInBackground = this.runInBackground;
            }
            #endif
            if (!this.primaryRecorderInitialized)
            {
                this.TryInitializePrimaryRecorder();
            }
        }

        protected virtual void Update()
        {
            this.VoiceClient.Service();
        }

        protected virtual void FixedUpdate()
        {
            this.Dispatch();
        }

        /// <summary>Dispatches incoming network messages for Voice client. Called in FixedUpdate or LateUpdate.</summary>
        /// <remarks>
        /// It may make sense to dispatch incoming messages, even if the timeScale is near 0.
        /// That can be configured with <see cref="MinimalTimeScaleToDispatchInFixedUpdate"/>.
        ///
        /// Without dispatching messages, Voice client won't change state and does not handle updates.
        /// </remarks>
        protected void Dispatch()
        {
            bool doDispatch = true;
            while (doDispatch)
            {
                // DispatchIncomingCommands() returns true of it found any command to dispatch (event, result or state change)
                Profiler.BeginSample("[Photon Voice]: DispatchIncomingCommands");
                doDispatch = this.Client.LoadBalancingPeer.DispatchIncomingCommands();
                Profiler.EndSample();
            }
        }

        private void LateUpdate()
        {
            // see MinimalTimeScaleToDispatchInFixedUpdate for explanation
            if (Time.timeScale <= this.MinimalTimeScaleToDispatchInFixedUpdate)
            {
                this.Dispatch();
            }

            int currentMsSinceStart = (int)(Time.realtimeSinceStartup * 1000); // avoiding Environment.TickCount, which could be negative on long-running platforms
            if (currentMsSinceStart > this.nextSendTickCount)
            {
                bool doSend = true;
                while (doSend)
                {
                    // Send all outgoing commands
                    Profiler.BeginSample("[Photon Voice]: SendOutgoingCommands");
                    doSend = this.Client.LoadBalancingPeer.SendOutgoingCommands();
                    Profiler.EndSample();
                }

                this.nextSendTickCount = currentMsSinceStart + this.updateInterval;
            }

            if (currentMsSinceStart > this.nextStatsTickCount)
            {
                if (this.statsResetInterval > 0)
                {
                    this.CalcStatistics();
                    this.nextStatsTickCount = currentMsSinceStart + this.statsResetInterval;
                }
            }
        }

        protected override void OnDisable()
        {
            if (AppQuits)
            {
                this.CleanUp();
                SupportClass.StopAllBackgroundCalls();
            }
        }

        protected virtual void OnDestroy()
        {
            this.CleanUp();
        }

        protected virtual Speaker SimpleSpeakerFactory(int playerId, byte voiceId, object userData)
        {
            Speaker speaker = null;
            if (this.SpeakerPrefab)
            {
                GameObject go = Instantiate(this.SpeakerPrefab);
                Speaker[] speakers = go.GetComponentsInChildren<Speaker>(true);
                if (speakers.Length > 0)
                {
                    speaker = speakers[0];
                    if (speakers.Length > 1 && this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Multiple Speaker components found attached to the GameObject (VoiceConnection.SpeakerPrefab) or its children. Using the first one we found.");
                    }
                }
                if (speaker == null)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("SpeakerPrefab does not have a component of type Speaker in its hierarchy.");
                    }
                    return null;
                }
            }
            else if (this.AutoCreateSpeakerIfNotFound)
            {
                speaker = new GameObject().AddComponent<Speaker>();
            }
            else
            {
                return null;
            }

            // within a room, users are identified via the Realtime.Player class. this has a nickname and enables us to use custom properties, too
            speaker.Actor = (this.Client.CurrentRoom != null) ? this.Client.CurrentRoom.GetPlayer(playerId) : null;
            speaker.name = speaker.Actor != null && !string.IsNullOrEmpty(speaker.Actor.NickName) ? speaker.Actor.NickName : String.Format("Speaker for Player {0} Voice #{1}", playerId, voiceId);
            speaker.OnRemoteVoiceRemoveAction += this.DeleteVoiceOnRemoteVoiceRemove;
            return speaker;
        }

        internal void DeleteVoiceOnRemoteVoiceRemove(Speaker speaker)
        {
            if (speaker != null)
            {
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("Remote voice removed, delete speaker");
                }
                Destroy(speaker.gameObject);
            }
        }
        
        private void OnRemoteVoiceInfo(int channelId, int playerId, byte voiceId, VoiceInfo voiceInfo, ref RemoteVoiceOptions options)
        {
            if (voiceInfo.Codec != Codec.AudioOpus)
            {
                if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.LogInfo("OnRemoteVoiceInfo skipped as coded {4} is not Opus, channel {0} player {1} voice #{2} userData {3}", channelId, playerId, voiceId, voiceInfo.UserData, voiceInfo.Codec);
                }
                return;
            }
            if (this.Logger.IsInfoEnabled)
            {
                this.Logger.LogInfo("OnRemoteVoiceInfo channel {0} player {1} voice #{2} userData {3}", channelId, playerId, voiceId, voiceInfo.UserData);
            }
            bool duplicate = false;
            for (int i = 0; i < this.cachedRemoteVoices.Count; i++)
            {
                RemoteVoiceLink remoteVoiceLink = this.cachedRemoteVoices[i];
                if (remoteVoiceLink.PlayerId == playerId && remoteVoiceLink.VoiceId == voiceId)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Duplicate remote voice info event channel {0} player {1} voice #{2} userData {3}", channelId, playerId, voiceId, voiceInfo.UserData);
                    }
                    duplicate = true;
                    this.cachedRemoteVoices.RemoveAt(i);
                    break;
                }
            }
            RemoteVoiceLink remoteVoice = new RemoteVoiceLink(voiceInfo, playerId, voiceId, channelId, ref options);
            this.cachedRemoteVoices.Add(remoteVoice);
            if (RemoteVoiceAdded != null)
            {
                RemoteVoiceAdded(remoteVoice);
            }
            remoteVoice.RemoteVoiceRemoved += delegate
            {
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("RemoteVoiceRemoved channel {0} player {1} voice #{2} userData {3}", channelId, playerId, voiceId, voiceInfo.UserData);
                }
                if (!this.cachedRemoteVoices.Remove(remoteVoice) && this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cached remote voice info not removed for channel {0} player {1} voice #{2} userData {3}", channelId, playerId, voiceId, voiceInfo.UserData);
                }
            };
            if (this.SpeakerFactory != null)
            {
                Speaker speaker = this.SpeakerFactory(playerId, voiceId, voiceInfo.UserData);
                if (speaker != null && duplicate && speaker.IsLinked)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Overriding speaker link for channel {0} player {1} voice #{2} userData {3}", channelId, playerId, voiceId, voiceInfo.UserData);
                    }
                    speaker.OnRemoteVoiceRemove();
                }
                this.LinkSpeaker(speaker, remoteVoice);
            }
        }

        protected virtual void OnVoiceStateChanged(ClientState fromState, ClientState toState)
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("OnVoiceStateChanged from {0} to {1}", fromState, toState);
            }
            if (fromState == ClientState.Joined)
            {
                this.ClearRemoteVoicesCache();
            }
            if (toState == ClientState.ConnectedToMasterServer && this.Client.RegionHandler != null)
            {
                if (this.Settings != null)
                {
                    this.Settings.BestRegionSummaryFromStorage = this.Client.RegionHandler.SummaryToCache;
                }
                this.BestRegionSummaryInPreferences = this.Client.RegionHandler.SummaryToCache;
            }
        }

        protected void CalcStatistics()
        {
            float now = Time.time;
            int recv = this.VoiceClient.FramesReceived - this.referenceFramesReceived;
            int lost = this.VoiceClient.FramesLost - this.referenceFramesLost;
            float t = now - this.statsReferenceTime;

            if (t > 0f)
            {
                if (recv + lost > 0)
                {
                    this.FramesReceivedPerSecond = recv / t;
                    this.FramesLostPerSecond = lost / t;
                    this.FramesLostPercent = 100f * lost / (recv + lost);
                }
                else
                {
                    this.FramesReceivedPerSecond = 0f;
                    this.FramesLostPerSecond = 0f;
                    this.FramesLostPercent = 0f;
                }
            }

            this.referenceFramesReceived = this.VoiceClient.FramesReceived;
            this.referenceFramesLost = this.VoiceClient.FramesLost;
            this.statsReferenceTime = now;
        }

        private void CleanUp()
        {
            bool clientStillExists = this.client != null;
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("Client exists? {0}, already cleaned up? {1}", clientStillExists, this.cleanedUp);
            }
            if (this.cleanedUp)
            {
                return;
            }
            this.StopFallbackSendAckThread();
            if (clientStillExists)
            {
                this.client.StateChanged -= this.OnVoiceStateChanged;
                this.client.Disconnect();
                if (this.client.LoadBalancingPeer != null)
                {
                    this.client.LoadBalancingPeer.Disconnect();
                    this.client.LoadBalancingPeer.StopThread();
                }
                this.client.Dispose();
            }
            this.cleanedUp = true;
        }

        protected void LinkSpeaker(Speaker speaker, RemoteVoiceLink remoteVoice)
        {
            if (speaker != null)
            {
                if (!speaker.IgnoreGlobalLogLevel)
                {
                    speaker.LogLevel = this.GlobalSpeakersLogLevel;
                }
                speaker.SetPlaybackDelaySettings(this.globalPlaybackDelaySettings);
                #if UNITY_PS4 || UNITY_SHARLIN
                speaker.PlayStationUserID = this.PlayStationUserID;
                #endif
                if (speaker.OnRemoteVoiceInfo(remoteVoice))
                {
                    if (speaker.Actor == null)
                    {
                        if (this.Client.CurrentRoom == null)
                        {
                            if (this.Logger.IsErrorEnabled)
                            {
                                this.Logger.LogError("RemoteVoiceInfo event received while CurrentRoom is null");
                            }
                        }
                        else
                        {
                            Player player = this.Client.CurrentRoom.GetPlayer(remoteVoice.PlayerId);
                            if (player == null)
                            {
                                if (this.Logger.IsErrorEnabled)
                                {
                                    this.Logger.LogError("RemoteVoiceInfo event for actor number {0} received while respective actor not found in the room", remoteVoice.PlayerId);
                                }
                            }
                            else
                            {
                                speaker.Actor = player;
                            }
                        }
                    }
                    if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("Speaker linked with remote voice {0}/{1}", remoteVoice.PlayerId, remoteVoice.VoiceId);
                    }
                    this.linkedSpeakers.Add(speaker);
                    remoteVoice.RemoteVoiceRemoved += delegate
                    {
                        this.linkedSpeakers.Remove(speaker);
                    };
                    if (SpeakerLinked != null)
                    {
                        SpeakerLinked(speaker);
                    }
                }
            }
            else if (this.Logger.IsWarningEnabled)
            {
                this.Logger.LogWarning("Speaker is null. Remote voice {0}/{1} not linked.", remoteVoice.PlayerId, remoteVoice.VoiceId);
            }
        }

        private void ClearRemoteVoicesCache()
        {
            if (this.cachedRemoteVoices.Count > 0)
            {
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("{0} cached remote voices info cleared", this.cachedRemoteVoices.Count);
                }
                this.cachedRemoteVoices.Clear();
            }
        }

        private void TryInitializePrimaryRecorder()
        {
            if (this.primaryRecorder != null)
            {
                if (!this.primaryRecorder.IsInitialized)
                {
                    this.primaryRecorder.Init(this);
                }
                this.primaryRecorderInitialized = this.primaryRecorder.IsInitialized;
            }
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (this.globalPlaybackDelay > 0)
            {
                if (this.globalPlaybackDelaySettings.MinDelaySoft != this.globalPlaybackDelay)
                {
                    this.globalPlaybackDelaySettings.MinDelaySoft = this.globalPlaybackDelay;
                    if (this.globalPlaybackDelaySettings.MaxDelaySoft <= this.globalPlaybackDelaySettings.MinDelaySoft)
                    {
                        this.globalPlaybackDelaySettings.MaxDelaySoft = 2 * this.globalPlaybackDelaySettings.MinDelaySoft;
                        if (this.globalPlaybackDelaySettings.MaxDelayHard < this.globalPlaybackDelaySettings.MaxDelaySoft)
                        {
                            this.globalPlaybackDelaySettings.MaxDelayHard = this.globalPlaybackDelaySettings.MaxDelaySoft + 1000;
                        }
                    }
                }
                this.globalPlaybackDelay = -1;
            }
        }
        #endif

        #endregion
    }
}

namespace Photon.Voice
{
    [Obsolete("Class renamed. Use LoadBalancingTransport instead.")]
    public class LoadBalancingFrontend : LoadBalancingTransport
    {
    }
}