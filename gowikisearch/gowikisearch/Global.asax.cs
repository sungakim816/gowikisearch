﻿using gowikisearch.HelperClass;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Collections.Generic;
using System.Text;

namespace gowikisearch
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            InitializeTrie();
            InitializeStopWordList();
        }

        private void InitializeTrie()
        {
            TrieDataStructure trie = new TrieDataStructure();
            string path = Server.MapPath("~/App_Data/words.txt");
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            trie.Populate(fileStream);
            HttpRuntime.Cache.Insert("Trie", trie, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
        }

        private void InitializeStopWordList()
        {
            List<string> stopWords = new List<string>();
            string path = Server.MapPath("~/App_Data/stopwords.csv");
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var streamReader = new StreamReader(fileStream, Encoding.UTF8);
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                stopWords.Add(line);
            }
            HttpRuntime.Cache.Insert("StopWords", stopWords, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
            fileStream.Close();
        }
    }
}
