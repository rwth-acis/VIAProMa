using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using i5.Toolkit.Core.Utilities;

namespace i5.VIAProMa.WebConnection
{
    public static class BackendConnector
    {
        public static IRestConnector RestConnector = new UnityWebRequestRestConnector();
        public static IJsonSerializer JsonSerializer = new JsonUtilityAdapter();

        /// <summary>
        /// Sends the save data to the backend
        /// </summary>
        /// <param name="saveName">The name of the save file</param>
        /// <param name="saveJson">The content to save</param>
        /// <returns>Asynchronous operation</returns>
        public static async Task<bool> Save(string saveName, string saveJson)
        {
            WebResponse<string> resp = await RestConnector.PostAsync(ConnectionManager.Instance.BackendAPIBaseURL + "saveData/" + saveName, saveJson);
            ConnectionManager.Instance.CheckStatusCode(resp.Code);
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
            WebResponse<string> resp = await RestConnector.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "saveData/" + saveName, null);
            ConnectionManager.Instance.CheckStatusCode(resp.Code);
            if (resp.Successful)
            {
                return new ApiResult<string>(resp.Content);
            }
            else
            {
                return new ApiResult<string>(resp.Code, resp.Content);
            }
        }

        /// <summary>
        /// Method which checks whether the backend server is reachable
        /// </summary>
        /// <returns>returns true if the server is reachable, false otherwise</returns>
        public static async Task<bool> Ping()
        {
            WebResponse<string> resp = await RestConnector.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "ping");
            ConnectionManager.Instance.CheckStatusCode(resp.Code);
            if (resp.Code == 0)
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
            WebResponse<string> resp = await RestConnector.PostAsync(ConnectionManager.Instance.BackendAPIBaseURL + "consoleLog", new UTF8Encoding().GetBytes(log));
            ConnectionManager.Instance.CheckStatusCode(resp.Code);
        }

        /// <summary>
        /// Gets the saved projects and scene configurations
        /// </summary>
        /// <returns></returns>
        public static async Task<ApiResult<string[]>> GetProjects()
        {
            WebResponse<string> resp = await RestConnector.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "saveData", null);
            ConnectionManager.Instance.CheckStatusCode(resp.Code);
            if (!resp.Successful)
            {
                Debug.LogError(resp.Code + ": " + resp.Content);
                return new ApiResult<string[]>(resp.Code, resp.Content);
            }
            else
            {
                string[] projects = Utilities.JsonArrayUtility.FromJson<string>(resp.Content);
                return new ApiResult<string[]>(projects);
            }
        }
    }
}