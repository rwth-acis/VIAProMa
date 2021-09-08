﻿using UnityEngine;

namespace Photon.Voice.Unity.Editor
{
    using UnityEditor;
    using Unity;

    [CustomEditor(typeof(WebRtcAudioDsp))]
    public class WebRtcAudioDspEditor : Editor
    {
        private WebRtcAudioDsp processor;
        private Recorder recorder;

        private SerializedProperty aecSp;
        private SerializedProperty aecHighPassSp;
        private SerializedProperty agcSp;
        private SerializedProperty agcCompressionGainSp;
        private SerializedProperty vadSp;
        private SerializedProperty highPassSp;
        private SerializedProperty bypassSp;
        private SerializedProperty noiseSuppressionSp;
        private SerializedProperty reverseStreamDelayMsSp;

        private void OnEnable()
        {
            this.processor = this.target as WebRtcAudioDsp;
            this.recorder = this.processor.GetComponent<Recorder>();
            this.aecSp = this.serializedObject.FindProperty("aec");
            this.aecHighPassSp = this.serializedObject.FindProperty("aecHighPass");
            this.agcSp = this.serializedObject.FindProperty("agc");
            this.agcCompressionGainSp = this.serializedObject.FindProperty("agcCompressionGain");
            this.vadSp = this.serializedObject.FindProperty("vad");
            this.highPassSp = this.serializedObject.FindProperty("highPass");
            this.bypassSp = this.serializedObject.FindProperty("bypass");
            this.noiseSuppressionSp = this.serializedObject.FindProperty("noiseSuppression");
            this.reverseStreamDelayMsSp = this.serializedObject.FindProperty("reverseStreamDelayMs");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.UpdateIfRequiredOrScript();

            if (!this.processor.enabled)
            {
                EditorGUILayout.HelpBox("WebRtcAudioDsp is disabled and will not be used.", MessageType.Warning);
            }
            if (this.recorder != null && this.recorder.SourceType != Recorder.InputSourceType.Microphone)
            {
                EditorGUILayout.HelpBox("WebRtcAudioDsp is better suited to be used with Microphone as Recorder Input Source Type.", MessageType.Warning);
            }
            VoiceLogger.ExposeLogLevel(this.serializedObject, this.processor);
            bool bypassed;
            EditorGUI.BeginChangeCheck();
            if (PhotonVoiceEditorUtils.IsInTheSceneInPlayMode(this.processor.gameObject))
            {
                this.processor.Bypass = EditorGUILayout.Toggle(new GUIContent("Bypass", "Bypass WebRTC Audio DSP"), this.processor.Bypass);
                bypassed = this.processor.Bypass;
            }
            else
            {
                EditorGUILayout.PropertyField(this.bypassSp, new GUIContent("Bypass", "Bypass WebRTC Audio DSP"));
                bypassed = this.bypassSp.boolValue;
            }

            if (!bypassed)
            {
                if (PhotonVoiceEditorUtils.IsInTheSceneInPlayMode(this.processor.gameObject))
                {
                    this.processor.AEC = EditorGUILayout.Toggle(new GUIContent("AEC", "Acoustic Echo Cancellation"), this.processor.AEC);
                    if (this.processor.AEC)
                    {
                        if (this.recorder.MicrophoneType == Recorder.MicType.Photon)
                        {
                            EditorGUILayout.HelpBox("You have enabled AEC here and are using a Photon Mic as input on the Recorder, which might add its own echo cancellation. Please use only one AEC algorithm.", MessageType.Warning);
                        }
                        this.processor.ReverseStreamDelayMs = EditorGUILayout.IntField(new GUIContent("ReverseStreamDelayMs", "Reverse stream delay (hint for AEC) in Milliseconds"), this.processor.ReverseStreamDelayMs);
                        this.processor.AecHighPass = EditorGUILayout.Toggle(new GUIContent("AEC High Pass"), this.processor.AecHighPass);
                    }
                    this.processor.AGC = EditorGUILayout.Toggle(new GUIContent("AGC", "Automatic Gain Control"), this.processor.AGC);
                    if (this.processor.AGC)
                    {
                        this.processor.AgcCompressionGain = EditorGUILayout.IntField(new GUIContent("AGC Compression Gain"), this.processor.AgcCompressionGain);
                    }
                    if (this.processor.VAD && this.recorder.VoiceDetection)
                    {
                        EditorGUILayout.HelpBox("You have enabled VAD here and in the associated Recorder. Please use only one Voice Detection algorithm.", MessageType.Warning);
                    }
                    this.processor.VAD = EditorGUILayout.Toggle(new GUIContent("VAD", "Voice Activity Detection"), this.processor.VAD);
                    this.processor.HighPass = EditorGUILayout.Toggle(new GUIContent("HighPass", "High Pass Filter"), this.processor.HighPass);
                    this.processor.NoiseSuppression = EditorGUILayout.Toggle(new GUIContent("NoiseSuppression", "Noise Suppression"), this.processor.NoiseSuppression);
                }
                else
                {
                    EditorGUILayout.PropertyField(this.aecSp, new GUIContent("AEC", "Acoustic Echo Cancellation"));
                    if (this.aecSp.boolValue)
                    {
                        if (this.recorder.MicrophoneType == Recorder.MicType.Photon)
                        {
                            EditorGUILayout.HelpBox("You have enabled AEC here and are using a Photon Mic as input on the Recorder, which might add its own echo cancellation. Please use only one AEC algorithm.", MessageType.Warning);
                        }
                        EditorGUILayout.PropertyField(this.reverseStreamDelayMsSp,
                            new GUIContent("ReverseStreamDelayMs", "Reverse stream delay (hint for AEC) in Milliseconds"));
                        EditorGUILayout.PropertyField(this.aecHighPassSp, new GUIContent("AEC High Pass"));
                    }
                    EditorGUILayout.PropertyField(this.agcSp, new GUIContent("AGC", "Automatic Gain Control"));
                    if (this.agcSp.boolValue)
                    {
                        EditorGUILayout.PropertyField(this.agcCompressionGainSp, new GUIContent("AGC Compression Gain"));
                    }
                    if (this.vadSp.boolValue && this.recorder.VoiceDetection)
                    {
                        EditorGUILayout.HelpBox("You have enabled VAD here and in the associated Recorder. Please use only one Voice Detection algorithm.", MessageType.Warning);
                    }
                    EditorGUILayout.PropertyField(this.vadSp, new GUIContent("VAD", "Voice Activity Detection"));
                    EditorGUILayout.PropertyField(this.highPassSp, new GUIContent("HighPass", "High Pass Filter"));
                    EditorGUILayout.PropertyField(this.noiseSuppressionSp, new GUIContent("NoiseSuppression", "Noise Suppression"));
                }
            }
                
            if (EditorGUI.EndChangeCheck())
            {
                this.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
