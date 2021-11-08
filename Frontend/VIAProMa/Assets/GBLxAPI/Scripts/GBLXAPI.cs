// -------------------------------------------------------------------------------------------------
// GBLXAPI.cs
// Project: GBLXAPI
// Created: 2017/05/16
// Last Updated: 2020/10/23
// Version: 1.05
// Copyright 2018 Dig-It! Games, LLC. All rights reserved.
// This code is licensed under the MIT License (See LICENSE.txt for details)
// -------------------------------------------------------------------------------------------------

using System;
using DIG.GBLXAPI.Builders;
using DIG.GBLXAPI.Internal;
using Newtonsoft.Json.Linq;
using TinCan;
using UnityEngine;

namespace DIG.GBLXAPI
{
    public static class GBLXAPI
	{
		public static bool debugMode = false;

		public const string LrsURL = GBLConfig.LrsURL;

		public static bool IsInit { get; private set; }
		public static GBLConfig Configuration { get; private set; }

		public static DurationSlotTracker Timers { get; private set; }

		public static JObject StandardsJson { get; private set; }

		// Object Builders
		public static AgentBuilder.IInverseIdentifier Agent { get { return AgentBuilder.Start(); } }
		public static VerbBuilder.IAction Verb { get { return VerbBuilder.Start(StandardsJson); } }
		public static ActivityBuilder.IUri Activity { get { return ActivityBuilder.Start(StandardsJson); } }
		public static ActivityDefinitionBuilder.IType ActivityDefinition { get { return ActivityDefinitionBuilder.Start(StandardsJson); } }
		public static ContextBuilder Context { get { return ContextBuilder.Start(); } }
		public static ResultBuilder Result { get { return ResultBuilder.Start(); } }
		public static ExtensionsBuilder Extensions { get { return ExtensionsBuilder.Start(StandardsJson); } }
		public static ChoiceInteractionBuilder.IID ChoiceInteraction { get { return ChoiceInteractionBuilder.Start(); } }

		public static StatementBuilder.IAgent Statement { get { return StatementBuilder.Start(); } }

		private static LrsRemoteQueue _lrsQueue;

		public static void Init(GBLConfig config, int queueDepth = 1000)
        {
			if (IsInit) { return; }

			Configuration = config;

			_lrsQueue = LrsRemoteQueue.Instance;
			_lrsQueue.Init(config, queueDepth);

			Timers = new DurationSlotTracker();

			// Load the educational standard defaults
			StandardsJson = null;
			try
            {
				string defaultJsonText = Resources.Load<TextAsset>(GBLConfig.StandardsDefaultPath).text;
				StandardsJson = JObject.Parse(defaultJsonText);
            }
			catch
            {
				Debug.LogError($"Missing GBLxAPI default vocabulary! Learning standards cannot be tracked without this file. Run Vocabulary/GBL_Json_Parser.py and ensure that the resulting GBLxAPI_Vocab_Default.json file is moved into the Resources/Data folder.");
			}

			// Load the user configured standards
			JObject userJson = null;
			try
			{
				string userJsonText = Resources.Load<TextAsset>(GBLConfig.StandardsUserPath).text;
				userJson = JObject.Parse(userJsonText);
			}
			catch
			{
				Debug.LogWarning("Missing GBLxAPI vocabulary user overrides. Default vocabulary will be used for learning tracking. To implement user overrides, run Vocabulary/GBL_Json_Parser.py and ensure that the resulting GBLxAPI_Vocab_User.json file is moved to the Assets/Resources/Data folder.");
			}

			// Merge the two json files, letting the user config overwrite the defaults
			if (StandardsJson != null && userJson != null)
			{
				StandardsJson.Merge(userJson);
			}

			IsInit = true;
		}

		public static void EnqueueStatement(Statement statement, Action<bool, string> sendCallback = null)
		{
			//Debug.Log(statement.ToJSON(true));
			_lrsQueue.EnqueueStatement(statement, sendCallback);
		}
	}
}
