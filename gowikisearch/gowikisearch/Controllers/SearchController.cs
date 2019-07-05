using System.Web.Mvc;
using gowikisearch.ViewModels;
using PagedList;
using gowikisearch.Models;
using System.Collections.Generic;
using System.Linq;
using Korzh.EasyQuery.Linq;
using System;
using System.Web;
using gowikisearch.HelperClass;
using System.Collections;

namespace gowikisearch.Controllers
{
    public class SearchController : Controller
    {
        private readonly DatabaseContext _context;
        public SearchController()
        {
            _context = new DatabaseContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Search
        [HttpGet]
        [OutputCache(Duration = 30)]
        [Route("Search/")]
        [Route("Search/{query}")]
        [Route("Search/{query}/{pageNumber:regex(^[1-9]{0,3}$)}")]
        public ActionResult Index(string query, int? pageNumber)
        {
            int pageSize = 10; // items per pages
            if (!pageNumber.HasValue)
            {
                pageNumber = 1;
            }
            ViewBag.Query = query;
            WikipediaPages wikiPages = null;
            if (!string.IsNullOrEmpty(query))
            {
                wikiPages = new WikipediaPages(query, 60);
            }
            if (wikiPages == null)
            {
                return View();
            }
            return View(wikiPages.RetrievePages().ToPagedList((int)pageNumber, pageSize));
        }

        [HttpGet]
        [Route("Search/UpdatePagePopularity/{title}")]
        [Route("Search/Update/Popularity/{title}")]
        public ActionResult UpdatePagePopularity(string title)
        {
            try
            {
                WikipediaPageTitle pageTitleInstance = _context.WikipediaPageTitles.SingleOrDefault(p => p.Title == title);
                pageTitleInstance.Popularity += 1;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }

            return new EmptyResult();
        }

        private string FormatQueryForFullTextSearch(string q)
        {
            if (q.Length == 0)
            {
                return "";
            }
            List<string> stopWords = (List<string>)HttpRuntime.Cache["StopWords"];
            char[] qChars = q.Where(c => (char.IsLetter(c) || char.IsWhiteSpace(c))).ToArray();
            q = new string(qChars);
            string[] qArray = q.Split(' ');

            if (qArray.Length == 1)
            {
                return string.Format("\"{0}*\"", qArray.First());
            }

            q = "";
            foreach (string s in qArray)
            {
                if (!stopWords.Contains(s))
                {
                    if (s.Equals(qArray.Last()))
                    {
                        q += string.Format("\"{0}*\"", s);
                        break;
                    }
                    q += string.Format("{0} AND ", s);
                }
            }
            return q;
        }

        // GET: Search Autocomplete
        //[OutputCache(Duration = 30, VaryByParam = "query")]
        [HttpGet]
        [Route("Search/Autocomplete")]
        [Route("Search/Autocomplete/{query}")]
        public ActionResult Autocomplete(string query)
        {
            if (query == null)
            {
                return new EmptyResult();
            }
            ViewBag.Query = query;
            query = query.ToLower();
            string formattedQuery = FormatQueryForFullTextSearch(query);
            short maxSuggestions = 15;
            short minSuggestions = 5;
            short minimumResultFromDatabase = 2500;
            // Retrieve trie structure from runtime cache, key: 'Trie';
            TrieDataStructure trie = (TrieDataStructure)HttpRuntime.Cache["Trie"];
            // initialize container for WikipediaPageTitle objects 
            IEnumerable<WikipediaPageTitle> querySuggestions;
            // get suggestions
            List<string> suggestionArray = trie.Suggestions(query);
            // check if trie has suggestions
            if (suggestionArray.Count < minSuggestions)
            {
                // if no result, query the database using full-text search for faster response
                string sqlQuery = string.Format("SELECT TOP({0}) * FROM [dbo].[WikipediaPageTitle] " +
                    "WHERE CONTAINS(Title, '{1}')" +
                    "ORDER BY Popularity DESC, Title DESC;", minimumResultFromDatabase, formattedQuery);
                querySuggestions = _context.WikipediaPageTitles.SqlQuery(sqlQuery).AsEnumerable();
                // add result to the trie
                foreach (var suggestion in querySuggestions)
                {
                    trie.Add(suggestion.Title.ToLower());
                }
                // save to runtime cache
                HttpRuntime.Cache["Trie"] = trie;
            }
            suggestionArray = trie.Suggestions(query);
            querySuggestions = _context.WikipediaPageTitles
                .Where(s => suggestionArray.Contains(s.Title))
                .OrderByDescending(s => s.Popularity)
                .OrderBy(s => s.Title)
                .Take(maxSuggestions)
                .AsEnumerable();
            return View(querySuggestions);
        }
    }
}