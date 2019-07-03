﻿using System.Web.Mvc;
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
        [OutputCache(Duration = 60)]
        [Route("Search/")]
        [Route("Search/{query}")]
        [Route("Search/{query}/{pageNumber:regex(^[1-9]{0,2}$)}")]
        public ActionResult Index(string query, int? pageNumber)
        {
            int pageSize = 6; // items per pages
            if (!pageNumber.HasValue)
            {
                pageNumber = 1;
            }
            ViewBag.Query = query;
            WikipediaPages wikiPages = null;
            if (!string.IsNullOrEmpty(query))
            {
                wikiPages = new WikipediaPages(query);
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

        // GET: Search Autocomplete
        [OutputCache(Duration = 10, VaryByParam = "query")]
        [HttpGet]
        [Route("Search/Autocomplete")]
        [Route("Search/Autocomplete/{query}")]
        public ActionResult Autocomplete(string query)
        {
            if (query == null)
            {
                return new EmptyResult();
            }
            query = query.ToLower();
            short maxSuggestions = 20;
            short minimumSuggestions = 5;
            short minimumResultFromDatabase = 2500;
            // Retrieve trie structure from runtime cache, key: 'Trie';
            TrieDataStructure trie = (TrieDataStructure)HttpRuntime.Cache["Trie"];
            // initialize container for WikipediaPageTitle objects 
            IEnumerable<WikipediaPageTitle> querySuggestions = Enumerable.Empty<WikipediaPageTitle>();
            // get suggestions
            ArrayList suggestionArray = trie.Suggestions(query);
            // check if trie has suggestions
            if (suggestionArray.Count >= minimumSuggestions)
            {
                querySuggestions = _context.WikipediaPageTitles
                    .Where(s => suggestionArray.Contains(s.Title))
                    .OrderByDescending(s => s.Popularity)
                    .OrderBy(s => s.Title)
                    .Take(maxSuggestions)
                    .AsEnumerable();
                return View(querySuggestions);
            }
            // if no result, query the database using full-text search for faster response
            string sqlQuery = string.Format("SELECT TOP({0}) * FROM [dbo].[WikipediaPageTitle] " + 
                "WHERE CONTAINS(Title, '\"{1}*\"')" + 
                "ORDER BY Popularity DESC, Title;", minimumResultFromDatabase, query);
            querySuggestions = _context.WikipediaPageTitles.SqlQuery(sqlQuery).AsEnumerable();
            // add result to the trie
            foreach (var suggestion in querySuggestions)
            {
                trie.Add(suggestion.ToString());
            }
            // save to runtime cache
            HttpRuntime.Cache["Trie"] = trie;
            return View(querySuggestions.Take(maxSuggestions).AsEnumerable());
        }
    }
}