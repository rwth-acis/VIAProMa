using System;
using System.IO;
using System.Threading.Tasks;
using i5.VIAProMa.WebConnection;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace VIAProMa.Assets.Scripts.Analytics.FileExport
{
    public class ExportAnalytics : MonoBehaviour
    {
        public async void ExportToJsonAsync()
        {
            // Generate path for JSON file export.
            string now = DateTime.Now.ToString("ddMMyyHHmmss");
            string json = await FetchAnalyticsDataJSONAsync();
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string exportPath = userProfile + "\\VIAProMa_Exports\\Analytics\\";
            Directory.CreateDirectory(exportPath);

            StreamWriter writer = new StreamWriter(exportPath + now + ".json");
            await writer.WriteAsync(json);
            writer.Flush();
            await writer.WriteLineAsync(json);
        }

        public async Task<string> FetchAnalyticsDataJSONAsync()
        {
            string projectID = AnalyticsManager.Instance.ProjectID.ToString();

            Response resp =
                    await Rest.GetAsync(
                        ConnectionManager.Instance.BackendAPIBaseURL + "projects/analytics/json-export/" + projectID,
                        null,
                        -1,
                        null,
                        true);
            ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
            return await resp.GetResponseBody();
        }
    }
}