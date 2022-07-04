using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Threading.Tasks;

namespace i5.VIAProMa.LiteratureSearch
{
    /// <summary>
    /// Class the includes functions to communicate with the CrossRef API.
    /// </summary>
    public class Communicator : Singleton<Communicator>
    {
        /// <summary>
        /// The URL for a standard CrossRef API call.
        /// </summary>
        private static readonly string apiURL = "https://api.crossref.org/works?query=";
        /// <summary>
        /// The URL for a CrossRef API call given a DOI.
        /// </summary>
        private static readonly string apiURLSingleRequest = "https://api.crossref.org/works/";
        /// <summary>
        /// Email as contact information for the CrossRef-API, advised in the etiquette.
        /// </summary>
        private static readonly string mailTo = "sascha.thiemann@rwth-aachen.de";

        private static Dictionary<string, Paper> paperCache = new Dictionary<string, Paper>();

        /// <summary>
        /// Executes an async API search with the query <paramref name="query"/>, <paramref name="maxResults"/> maximum results and offset <paramref name="offset"/>. 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="maxResults"></param>
        /// <param name="offset"></param>
        public static async Task<CrossRefMessage> APISearch(string query, int maxResults = 5, int offset = 0)
        {
            query = query.Replace(" ", "+");
            Response res;
            if (offset == 0)
            {
                res = await Rest.GetAsync(apiURL + query + "&rows=" + maxResults + "&mailto=" + mailTo);

            }
            else
            {
                res = await Rest.GetAsync(apiURL + query + "&rows=" + maxResults + "&offset=" + (offset * maxResults) + "&mailto=" + mailTo);

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
        /// Gets the data of a paper with a specific <paramref name="doi"/>.
        /// </summary>
        /// <param name="doi"></param>
        /// <returns>The paper with the DOI <paramref name="doi"/>.</returns>
        public static async Task<Paper> GetPaper(string doi)
        {
            if (paperCache.ContainsKey(doi))
            {
                Debug.Log("from cache + " + doi);
                return paperCache[doi];
            }
            Debug.Log("requested: " + doi);

            Response response = await Rest.GetAsync(apiURLSingleRequest + doi);
            string text = await response.GetResponseBody();

            if (!response.Successful)
            {
                Debug.Log("Paper with DOI " + doi + " not found");
                if(!paperCache.ContainsKey(doi))
                    paperCache.Add(doi, null);
                return null;
            }

            CrossRefSingleResponse resp = CrossRefSingleResponse.ExtractFromJSON(text);

            Debug.Log(resp.ToString());

            Paper paper = resp.message.ToPaper();

            // To avoid adding a paper multiple times because of race conditions
            if(!paperCache.ContainsKey(doi))
                paperCache.Add(doi, paper);

            return paper;
        }


    }
    /// <summary>
    /// Object for a CrossRef API response.
    /// </summary>
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
    /// Object for a CrossRef API response for a single paper.
    /// </summary>
    [System.Serializable]
    public class CrossRefSingleResponse
    {
        public string status;
        public string messagetype;
        public string messageversion;
        public CrossRefPaper message;

        public static CrossRefSingleResponse ExtractFromJSON(string json)
        {
            string text = json.Replace("message-type", "messagetype");
            text = text.Replace("message-version", "messageversion");
            text = text.Replace("total-results", "totalresults");
            text = text.Replace("items-per-page", "itemsperpage");
            text = text.Replace("\"abstract\":", "\"abstracttext\":");
            text = text.Replace("is-referenced-by-count", "isreferencedbycount");
            text = text.Replace("date-parts", "dateparts");
            text = text.Replace("date-time", "datetime");

            return JsonUtility.FromJson<CrossRefSingleResponse>(text);
        }
    }

    /// <summary>
    /// Class for the message body of the CrossRef API response.
    /// </summary>
    /// 
    [System.Serializable]
    public class CrossRefMessage
    {
        public int totalresults;
        public List<CrossRefPaper> items;
        public int itemsperpage;

        public override string ToString()
        {
            string output = "";
            output += "total results:" + totalresults + "\n";
            foreach (CrossRefPaper paper in items)
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
    public class CrossRefPaper
    {
        public string publisher;
        public string abstracttext;
        public string DOI;
        public string type;
        public string page;
        public int isreferencedbycount;
        public List<string> title;
        public List<Author> author;
        public List<Reference> reference;
        public string URL;
        public CrossRefDate created;

        public override string ToString()
        {
            string output = "";
            output += "DOI: " + DOI + ", ";
            output += "Type: " + type + ", ";
            output += "Title: " + (title.Count == 0 ? "no title" : title[0]) + ", ";
            output += "Authors: ";
            foreach (Author a in author)
            {
                output += a.ToString() + ", ";
            }
            output += URL;
            return output;
        }

        /// <summary>
        /// Creates a paper object from the CrossRefPaper object.
        /// </summary>
        /// <returns></returns>
        public Paper ToPaper()
        {
            System.DateTime date;
            System.DateTime.TryParse(created.datetime, out date);
            List<string> references = new List<string>();
            if(reference != null)
            {
                foreach(Reference reference in reference)
                {
                    references.Add(reference.ToString());
                }
            }
            return new Paper(publisher, abstracttext, DOI, type, page, isreferencedbycount, title, author, URL, date, references);
        }

        public static List<Paper> ToPapers(List<CrossRefPaper> papers)
        {
            List<Paper> result = new List<Paper>();
            foreach(CrossRefPaper paper in papers)
            {
                result.Add(paper.ToPaper());
            }
            return result;
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
        //public string key;
        public string DOI;
        //public string unstructured;
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
