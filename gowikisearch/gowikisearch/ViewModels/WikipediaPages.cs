using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using gowikisearch.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace gowikisearch.ViewModels
{
    public class WikipediaPages
    {
        private readonly List<WikipediaPage> pages;
        public string Query { get; set; }
        public String Format { get; set; }
        public String NameSpace { get; set; }
        public int Limit { get; set; }

        public WikipediaPages()
        {
            Limit = 30;
            Format = "json";
            NameSpace = "*";
            Query = "Wikipedia";
            pages = new List<WikipediaPage>();
        }
        public WikipediaPages(string query, int limit = 30, string format = "json", string nameSpace = "*")
        {
            Limit = limit;
            Format = format;
            NameSpace = nameSpace;
            Query = query;
            pages = new List<WikipediaPage>();
        }

        public List<WikipediaPage> RetrievePages()
        {
            return RetrievePages(Query);
        }

        protected List<WikipediaPage> RetrievePages(string query)
        {
            var webClient = new System.Net.WebClient();
            string encodedUrlQuery = HttpUtility.UrlEncode(query);

            string url = String.Format(
                "https://en.wikipedia.org/w/api.php?action=opensearch&search={0}&limit={1}&format={2}&formatversion=2&namespace={3}",
                encodedUrlQuery, Limit, Format, NameSpace
                );

            var wikipediaResults = webClient.DownloadString(url);

            ArrayList queryResult = JsonConvert.DeserializeObject<ArrayList>(wikipediaResults);
            JArray titles = (JArray)queryResult[1]; // titles
            JArray descriptions = (JArray)queryResult[2];// corresponding descriptions of each titles, respectively
            JArray links = (JArray)queryResult[3]; // corresponding links each titles, respectively
            for (int i = 0; i < titles.Count; i++)
            {
                pages.Add(new WikipediaPage { Title = titles[i].ToString(), Description = descriptions[i].ToString(), Link = links[i].ToString() });
            }
            return pages;
        }
    }
}