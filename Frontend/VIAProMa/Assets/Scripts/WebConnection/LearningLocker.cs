using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class LearningLocker: MonoBehaviour
{   
    public TextAsset jsonFile;
    public async Task<IInformation> GetInformation(string name)
    {
        Debug.Log("get information from learning locker");
        //string address = "https://lrs.tech4comp.dbis.rwth-aachen.de/api/statements/aggregate?pipeline=[{\"$match\": {\"statement.verb.id\": \"http://activitystrea.ms/schema/1.0/complete\",\"statement.actor.name\": \"" + name + "\"}},{\"$project\": {\"userName\": \"$statement.actor.name\",\"email\": \"$statement.actor.account.name\",\"duration\": \"$metadata.https://learninglocker%2646;net/result-duration.seconds\",\"grade\": \"$statement.result.score.scaled\"}},{\"$group\": {\"_id\": {\"name\": \"$userName\",\"email\": \"$email\"},\"average_score\": {\"$avg\": \"$grade\"},\"assignments\": {\"$push\": \"$grade\"},\"durations\": {\"$push\": \"$duration\"}}}]";
        //string address = "https://lrs.tech4comp.dbis.rwth-aachen.de/api/statements/aggregate?pipeline=[{\"$match\": {\"statement.verb.id\": \"http://activitystrea.ms/schema/1.0/complete\",\"statement.actor.name\": \"Sylvia Schulze-Achatz\"}},{\"$project\": {\"userName\": \"$statement.actor.name\",\"email\": \"$statement.actor.account.name\",\"duration\": \"$metadata.https://learninglocker%2646;net/result-duration.seconds\",\"grade\": \"$statement.result.score.scaled\"}},{\"$group\": {\"_id\": {\"name\": \"$userName\",\"email\": \"$email\"},\"average_score\": {\"$avg\": \"$grade\"},\"assignments\": {\"$push\": \"$grade\"},\"durations\": {\"$push\": \"$duration\"}}}]";
        var myHeader = new Dictionary<string, string>(){{"Authorization", "Basic MzRiMGFkNDQzZTM1ZjY0ZTZmOWU3YjA2ZGE5NmQzYTVhNGE3MTYyZTpmMjQxNTZmMTZlMTU2YThiY2UyOWJjMzQ1NzBiOWY1ODBjODNiMDlk"}};
        //Debug.Log(address.Length);

        Response resp = await Rest.GetAsync("https://lrs.tech4comp.dbis.rwth-aachen.de/api/statements/aggregate?" + GetPipeline(name), myHeader);

        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            return null;
        }
        else
        {
            //Debug.Log(resp.ResponseBody.GetType());
            Debug.Log(resp.ResponseBody);
            string res = JsonArrayUtility.EncapsulateInWrapper(resp.ResponseBody);

            IInformation[] data = JsonArrayUtility.FromJson<IInformation>(res);
            
            return data[0];
        }
    }

    private string GetPipeline(string name)
    {
        //string pipeline = $"pipeline=%5B%7B%0A%09%09%22$match%22:%20%7B%0A%09%09%09%22statement.verb.id%22:%20%22http://activitystrea.ms/schema/1.0/complete%22,%0A%09%09%09%22statement.actor.name%22:%20%22{System.Uri.EscapeDataString(name)}%22%0A%09%09%7D%0A%09%7D,%0A%09%7B%0A%09%09%22$project%22:%20%7B%0A%09%09%09%22userName%22:%20%22$statement.actor.name%22,%0A%09%09%09%22email%22:%20%22$statement.actor.account.name%22,%0A%09%09%09%22grade%22:%20%22$statement.result.score.scaled%22%0A%09%09%7D%0A%09%7D,%0A%09%7B%0A%09%09%22$group%22:%20%7B%0A%09%09%09%22_id%22:%20%7B%0A%09%09%09%09%22name%22:%20%22$userName%22,%0A%09%09%09%09%22email%22:%20%22$email%22%0A%09%09%09%7D,%0A%09%09%09%22average_score%22:%20%7B%0A%09%09%09%09%22$avg%22:%20%22$grade%22%0A%09%09%09%7D,%0A%09%09%09%22assignments%22:%20%7B%0A%09%09%09%09%22$push%22:%20%7B%0A%09%09%09%09%09%22score%22:%20%22$grade%22%0A%09%09%09%09%7D%0A%09%09%09%7D%0A%0A%09%09%7D%0A%09%7D%0A%5D";
        string pipeline = $"pipeline=%5B%7B%0A%09%09%22$match%22:%20%7B%0A%09%09%09%22statement.verb.id%22:%20%22http://activitystrea.ms/schema/1.0/complete%22,%0A%09%09%09%22statement.actor.name%22:%20%22{System.Uri.EscapeDataString(name)}%22%0A%09%09%7D%0A%09%7D,%0A%09%7B%0A%09%09%22$project%22:%20%7B%0A%09%09%09%22userName%22:%20%22$statement.actor.name%22,%0A%09%09%09%22email%22:%20%22$statement.actor.account.name%22,%0A%09%09%09%22grade%22:%20%22$statement.result.score.scaled%22%0A%09%09%7D%0A%09%7D,%0A%09%7B%0A%09%09%22$group%22:%20%7B%0A%09%09%09%22_id%22:%20%7B%0A%09%09%09%09%22name%22:%20%22$userName%22,%0A%09%09%09%09%22email%22:%20%22$email%22%0A%09%09%09%7D,%0A%09%09%09%22average_score%22:%20%7B%0A%09%09%09%09%22$avg%22:%20%22$grade%22%0A%09%09%09%7D,%0A%09%09%09%22assignments%22:%20%7B%0A%09%09%09%09%22$push%22:%20%22$grade%22%0A%09%09%09%7D%0A%0A%09%09%7D%0A%09%7D%0A%5D";

        return pipeline;
    }

}