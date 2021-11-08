// -------------------------------------------------------------------------------------------------
// RemoteLRSAsync.cs
// Project: GBLXAPI
// Created: 2017/05/30
// Copyright 2017 Dig-It! Games, LLC. All rights reserved.
// This code is licensed under the MIT License (see LICENSE.txt for details)
//
// NOTE:
// This is a slim async version of RemoteLRS.cs for WebGL that only saves statements
// There is a desktop/mobile version that uses threading and the full RemoteLRS to make async.
// -------------------------------------------------------------------------------------------------

using System;
using System.Text;
using Newtonsoft.Json.Linq;
using TinCan;
using UnityEngine.Networking;

namespace DIG.GBLXAPI.Internal
{
    public class RemoteLRSAsync
	{
		// config
		public string endpoint { get; set; }
		public TCAPIVersion version { get; set; }
		public string auth { get; set; }

		// state
		public bool complete { get; set; }
		public bool success { get; set; }
		public string response { get; set; }

		public RemoteLRSAsync(string endpoint, string username, string password)
        {
			this.endpoint = endpoint;

			// endpoint should have trailing /
			if (this.endpoint[this.endpoint.Length - 1] != '/')
			{
				this.endpoint += "/";
			}

			this.version = TCAPIVersion.latest();
			this.auth = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));

			ClearState();
		}

		// ------------------------------------------------------------------------
		// ------------------------------------------------------------------------
		public void ClearState()
		{
			complete = false;
			success = false;
			response = "";
		}

		// ------------------------------------------------------------------------
		// ------------------------------------------------------------------------
		public void PostStatement(Statement statement)
		{
			// reinit state
			ClearState();

			// https://learninglocker.dig-itgames.com/data/xAPI/statements?statementId=58098b7c-3353-4f9c-b812-1bddb08876fd
			string queryURL = endpoint + "statements";

			byte[] formBytes = Encoding.UTF8.GetBytes(statement.ToJSON(version));

			UnityWebRequest request = new UnityWebRequest(queryURL, "POST", new DownloadHandlerBuffer(), new UploadHandlerRaw(formBytes));

			request.SetRequestHeader("Content-Type", "application/json");
			request.SetRequestHeader("X-Experience-API-Version", version.ToString());
			request.SetRequestHeader("Authorization", auth);

			var requestOperation = request.SendWebRequest();
			requestOperation.completed += (operation) =>
			{
				success = !(request.isNetworkError || request.isHttpError);

				if (success)
				{
					JArray ids = JArray.Parse(request.downloadHandler.text);
					response = ids[0].ToString();
				}
				else
				{
					response = request.error;
				}

				complete = true;
			};
		}
	}
}
