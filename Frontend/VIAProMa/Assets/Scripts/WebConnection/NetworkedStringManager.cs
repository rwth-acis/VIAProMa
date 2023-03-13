﻿using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using i5.Toolkit.Core.Utilities;

namespace i5.VIAProMa.WebConnection
{
    /// <summary>
    /// Manager for sharing strings
    /// The manager converts strings to numeric ids which can be sent over network
    /// It also handles the conversion back from ids to strings.
    /// </summary>
    public static class NetworkedStringManager
    {
        public static IRestConnector RestConnector = new UnityWebRequestRestConnector();
        public static IJsonSerializer JsonSerializer = new JsonUtilityAdapter();
        private const string serviceEndpoint = "networkedStrings";

        /// <summary>
        /// Converts a string to a networked id which can be sent in the sharing service
        /// </summary>
        /// <param name="text">The text to convert</param>
        /// <returns>The networked id</returns>
        public static async Task<short> StringToId(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return -1;
            }

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "text/plain"); // overwrite the content type
            headers.Add("Accept", "text/plain"); // overwrite the accept type
            Response resp = await Rest.PostAsync(
                ConnectionManager.Instance.BackendAPIBaseURL + serviceEndpoint,
                text,
                headers,
                -1,
                true);
            string responseBody = await resp.GetResponseBody();
            if (resp.Successful)
             {
                 return short.Parse(responseBody);
             }
             else
             {
                 Debug.LogError(responseBody);
                 return -1;
             }
        }

        /// <summary>
        /// Converts a text id to the corresponding string
        /// </summary>
        /// <param name="id">The id which identifies the text segment</param>
        /// <returns>The text segment of the id</returns>
        public static async Task<string> GetString(short id)
        {
            if (id == -1)
            {
                return "";
            }

            WebResponse<string> resp = await RestConnector.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + serviceEndpoint + "/" + id, null);
            if (resp.Successful)
            {
                return resp.Content;
            }
            else
            {
                Debug.LogError(resp.Content);
                return "";
            }
        }
    }
}