using System;
using System.IO;
using System.Threading.Tasks;
using i5.VIAProMa.WebConnection;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace VIAProMa.Assets.Scripts.Analytics.FileExport
{
    public class ExportAnalytics
    {
        public async void ExportToJsonAsync()
        {
            string now = DateTime.Now.ToString();
            string json = await FetchAnalyticsDataJSONAsync();
            string userProfile = Environment.GetEnvironmentVariable("USERPROFILE");
            string exportPath = userProfile + "\\VIAProMa_Exports\\Analytics\\";
            StreamWriter writer = new StreamWriter(exportPath + "AnalyticsExport_" + now + ".json");
            Debug.Log(exportPath);
            await writer.WriteLineAsync(json);
        }

        public async Task<string> FetchAnalyticsDataJSONAsync()
        {
            string projectID = AnalyticsManager.Instance.ProjectID.ToString();

            Response resp =
                    await Rest.GetAsync(
                        ConnectionManager.Instance.BackendAPIBaseURL + "analytics/json-export/" + projectID,
                        null,
                        -1,
                        null,
                        true);
            ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
            return await resp.GetResponseBody();
        }
    }
}