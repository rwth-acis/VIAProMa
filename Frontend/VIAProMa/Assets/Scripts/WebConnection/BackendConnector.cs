﻿using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VIAProMa.WebConnection
{
    public static class BackendConnector
    {
        /// <summary>
        /// Sends the save data to the backend
        /// </summary>
        /// <param name="saveName">The name of the save file</param>
        /// <param name="saveJson">The content to save</param>
        /// <returns>Asynchronous operation</returns>
        public static async Task<bool> Save(string saveName, string saveJson)
        {
            Response resp = await Rest.PostAsync(ConnectionManager.Instance.BackendAPIBaseURL + "saveData/" + saveName, saveJson);
            ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
            if (resp.Successful)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Requests the save data from the backend
        /// </summary>
        /// <param name="saveName">The name of the save file</param>
        /// <returns>The save data</returns>
        public static async Task<ApiResult<string>> Load(string saveName)
        {
            Response resp =
                await Rest.GetAsync(
                    ConnectionManager.Instance.BackendAPIBaseURL + "saveData/" + saveName,
                    null,
                    -1,
                    null,
                    true);
            ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
            if (resp.Successful)
            {
                return new ApiResult<string>(resp.ResponseBody);
            }
            else
            {
                return new ApiResult<string>(resp.ResponseCode, resp.ResponseBody);
            }
        }

        /// <summary>
        /// Method which checks whether the backend server is reachable
        /// </summary>
        /// <returns>returns true if the server is reachable, false otherwise</returns>
        public static async Task<bool> Ping()
        {
            Response resp = await Rest.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "ping");
            ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
            if (resp.ResponseCode == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static async void SendLogs(string log)
        {
            Response resp = await Rest.PostAsync(ConnectionManager.Instance.BackendAPIBaseURL + "consoleLog", new UTF8Encoding().GetBytes(log));
            ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        }

        /// <summary>
        /// Gets the saved projects and scene configurations
        /// </summary>
        /// <returns></returns>
        public static async Task<ApiResult<string[]>> GetProjects()
        {
            Response resp = await Rest.GetAsync(
                ConnectionManager.Instance.BackendAPIBaseURL + "saveData",
                null,
                -1,
                null,
                true);
            ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
            if (!resp.Successful)
            {
                Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
                return new ApiResult<string[]>(resp.ResponseCode, resp.ResponseBody);
            }
            else
            {
                string[] projects = JsonArrayUtility.FromJson<string>(resp.ResponseBody);
                return new ApiResult<string[]>(projects);
            }
        }
    }
}