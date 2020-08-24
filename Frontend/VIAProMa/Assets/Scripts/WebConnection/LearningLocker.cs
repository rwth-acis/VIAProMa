using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class LearningLocker
{
    private Dictionary<string, string> myHeader = new Dictionary<string, string>(){{"Authorization", "Basic MzRiMGFkNDQzZTM1ZjY0ZTZmOWU3YjA2ZGE5NmQzYTVhNGE3MTYyZTpmMjQxNTZmMTZlMTU2YThiY2UyOWJjMzQ1NzBiOWY1ODBjODNiMDlk"}};
    public static async IInformation GetInformation(string name)
    {
        Debug.Log("get information from learning locker");
        Response resp = await Rest.GetAsync("https://lrs.tech4comp.dbis.rwth-aachen.de/api/statements/aggregate?"+Params(name), myHeader);

        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            return null;
        }
        else
        {
            IInformation data = JsonArrayUtility.FromJson<IInformation>(resp.ResponseBody);
            return data;
        }
    }

    public static string Params(string name)
    {
        return "pipeline=[{\"$match\":{\"statement.verb.id\":\"http://activitystrea.ms/schema/1.0/complete\",\"statement.actor.name\":\"" + name
        +"\"}},{\"$project\":{\"userName\":\"$statement.actor.name\",\"email\":\"$statement.actor.account.name\",\"duration\":\"$metadata.https:\/\/learninglocker&46;net\/result-duration.seconds\",\"grade\":\"$statement.result.score.scaled\"}},{\"$group\":{\"_id\":{\"name\":\"$userName\",\"email\":\"$email\"},\"average_score\":{\"$avg\":\"$grade\"},\"assignments\":{\"$push\":\"$grade\"},\"durations\":{\"$push\":\"$duration\"}}}]";
    }

}