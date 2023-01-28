using System;
using System.IO;
using System.Threading.Tasks;
using HoloToolkit.Unity;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.WebConnection;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace VIAProMa.Assets.Scripts.Analytics.FileExport
{
    public class ExportAnalytics : Singleton<ExportAnalytics> 
    {
        public async void ExportAsync(ExportSelection format)
        {
            // Generate path for JSON file export.
            string now = DateTime.Now.ToString("ddMMyyHHmmss");
            byte[] data = await FetchAnalyticsDataAsync(format);
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string exportPath = userProfile + "\\VIAProMa_Exports\\Analytics\\";
            Directory.CreateDirectory(exportPath);

            string fileExtension = "";

            switch (format)
            {
                case ExportSelection.JSON:
                    fileExtension = ".json";
                    break;

                case ExportSelection.SQLITE:
                    fileExtension = ".sqlite";
                    break;

                case ExportSelection.CSV:
                    fileExtension = ".zip";
                    break;

                default:
                    fileExtension = ".txt";
                    break;
            }
            Debug.Log("Data is:" + data);
            await File.WriteAllBytesAsync(exportPath + now + fileExtension, data);
           
        }

        public async Task<byte[]> FetchAnalyticsDataAsync(ExportSelection format)
        {
            string projectID = AnalyticsManager.Instance.ProjectID.ToString();

            Response resp =
                    await Rest.GetAsync(
                        ConnectionManager.Instance.BackendAPIBaseURL + "analytics/" + format.ToString().ToLower() + "-export/" + projectID,
                        null,
                        -1,
                        null,
                        true);
            ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
            return resp.ResponseData;
        }
    }
}