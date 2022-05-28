using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Threading.Tasks;

namespace i5.VIAProMa.LiteratureSearch
{
    public class Communicator : Singleton<Communicator>
    {
        private readonly string apiURL = "https://api.crossref.org/works?query=";

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// Searches for <paramref name="query"/> with saved parameters.
        /// </summary>
        /// <param name="query"></param>
        //public CrossRefMessage LiteratureSearch(string query, int maxResults = 5, int searchIndex = 0)
        //{
        //    // Add more parameters later.
        //    return await ResourcesAPI(query, maxResults, 0);
        //    //StartCoroutine(APISearch(query, maxResults));
        //}

        /// <summary>
        /// Executes an async API search with the query <paramref name="query"/>, <paramref name="maxResults"/> maximum results and search index <paramref name="searchIndex"/>. 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="maxResults"></param>
        /// <param name="searchIndex"></param>
        public async Task<CrossRefMessage> APISearch(string query, int maxResults = 5, int offset = 0)
        {
            query = query.Replace(" ", "+");
            Response res;
            if (offset == 0)
            {
                res = await Rest.GetAsync(apiURL + query + "&rows=" + maxResults);

            }
            else
            {
                res = await Rest.GetAsync(apiURL + query + "&rows=" + maxResults + "&offset=" + offset);

            }
            if (!res.Successful)
            {
                Debug.LogError("Unsuccessful");
                return null;
            }
            else
            {
                string text = await res.GetResponseBody();

                CrossRefResponse resp = CrossRefResponse.ExtractFromJSON(text);

                Debug.Log(resp.ToString());

                return resp.message;
            }

        }

        /// <summary>
        /// Executes the API search 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        //private IEnumerator APISearch(string query, int maxResults)
        //{
        //    using(UnityWebRequest request = UnityWebRequest.Get(_apiURL + query + "&rows=" + maxResults))
        //    {
        //        yield return request.SendWebRequest();
        //        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.DataProcessingError)
        //        {
        //            Debug.LogError(request.error);
        //        }
        //        else
        //        {
        //            Debug.Log("Success");
        //            string text = request.downloadHandler.text;

        //            CrossRefResponse resp = CrossRefResponse.ExtractFromJSON(text);

        //            Debug.Log(resp.ToString());
                
        //            // Visualize papers
        //        }
        //    }
        //}
        /// <summary>
        /// Class for the response of the CrossRef API.
        /// </summary>
        


    }
    [System.Serializable]
    public class CrossRefResponse
    {
        public string status;
        public string messagetype;
        public string messageversion;
        public CrossRefMessage message;

        public static CrossRefResponse ExtractFromJSON(string json)
        {
            string text = json.Replace("message-type", "messagetype");
            text = text.Replace("message-version", "messageversion");
            text = text.Replace("total-results", "totalresults");
            text = text.Replace("items-per-page", "itemsperpage");
            text = text.Replace("\"abstract\":", "\"abstracttext\":");
            text = text.Replace("is-referenced-by-count", "isreferencedbycount");
            text = text.Replace("date-parts", "dateparts");
            text = text.Replace("date-time", "datetime");

            return JsonUtility.FromJson<CrossRefResponse>(text);
        }

        override public string ToString()
        {
            return "Status: " + status + "\n" + "Message-Type: " + messagetype + "\n" + "Message-Version: " + messageversion + "\n" + "Message: " + message.ToString();
        }
    }

    /// <summary>
    /// Class for the message of the CrossRef API response.
    /// </summary>
    /// 
    [System.Serializable]
    public class CrossRefMessage
    {
        public int totalresults;
        public List<Paper> items;
        public int itemsperpage;

        public override string ToString()
        {
            string output = "";
            output += "total results:" + totalresults + "\n";
            foreach (Paper paper in items)
            {
                output += paper.ToString() + "\n";
            }
            output += "items per page" + itemsperpage;

            return output;
        }
    }

    /// <summary>
    /// Class for a single Paper in the CrossRef API response.
    /// </summary>
    /// 
    [System.Serializable]
    public class Paper
    {
        public string publisher;
        public string abstracttext;
        public string DOI;
        public string type;
        public string page;
        public int isreferencedbycount;
        public List<string> title;
        public List<Author> author;
        public string URL;
        public CrossRefDate created;

        public override string ToString()
        {
            string output = "";
            output += "DOI: " + DOI + ", ";
            output += "Title: " + (title.Count == 0 ? "no title" : title[0]) + ", ";
            output += "Authors: ";
            foreach (Author a in author)
            {
                output += a.ToString() + ", ";
            }
            output += URL;
            return output;
        }
    }

    /// <summary>
    /// Class for a author of a paper.
    /// </summary>
    [System.Serializable]
    public class Author
    {
        public string given;
        public string family;
        public string sequence;

        public override string ToString()
        {
            return given + " " + family;
        }
        //Add possible affiliation later
    }

    /// <summary>
    /// Class for a reference in a paper.
    /// </summary>
    [System.Serializable]
    public class Reference
    {
        public string key;
        public string DOI;
        public string unstructured;
        public override string ToString()
        {
            return DOI;
        }
    }

    /// <summary>
    /// Class for a date given by the CrossRef API.
    /// </summary>
    [System.Serializable]
    public class CrossRefDate
    {
        public List<int[]> dateparts;
        public long timestamp;
        public string datetime;

        public override string ToString()
        {
            return dateparts[0] + "-" + dateparts[1] + "-" + dateparts[2];
        }
    }
}
