using i5.VIAProMa.Shelves.Widgets;
using i5.VIAProMa.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static SessionBrowserRefresher;
using HoloToolkit.Unity;

public class DeleteFile : MonoBehaviour
{
    public string url;

    public void DeleteObject()
    {
		ModelDownloader downloader = Singleton<ModelDownloader>.Instance;
		downloader.Yeet(url);
    }

}
