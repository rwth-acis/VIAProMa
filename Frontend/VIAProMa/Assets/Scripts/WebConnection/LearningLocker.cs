using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Net;
using i5.VIAProMa.WebConnection;
using i5.Toolkit.Core.Utilities;

[Serializable]
public class LearningLocker: MonoBehaviour
{

    private TextAsset textFile;
    private string pp;

    private void Awake()
    {
        textFile = (TextAsset)Resources.Load("learningRequest");
        pp = textFile.text;
    }

    //public TextAsset jsonFile;
    public async Task<IInformation> GetInformation(string name)
    {
        Debug.Log("get information from learning locker");
        var myHeader = new Dictionary<string, string>(){{"Authorization", "Basic MzRiMGFkNDQzZTM1ZjY0ZTZmOWU3YjA2ZGE5NmQzYTVhNGE3MTYyZTpmMjQxNTZmMTZlMTU2YThiY2UyOWJjMzQ1NzBiOWY1ODBjODNiMDlk"}};
        Response resp = await Rest.GetAsync("https://lrs.tech4comp.dbis.rwth-aachen.de/api/statements/aggregate?" + GetPipeline(name), myHeader);

        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            return null;
        }
        else
        {
            Debug.Log(resp.ResponseBody);
            string res = JsonArrayUtility.EncapsulateInWrapper(resp.ResponseBody);

            IInformation[] data = JsonArrayUtility.FromJson<IInformation>(res);
            
            return data[0];
        }
    }

    private string GetPipeline(string name)
    {
        Debug.Log(pp);
        pp = pp.Replace("___", name);

        string pipeline = "pipeline=" + WebUtility.UrlEncode(pp);

        return pipeline;
    }

}