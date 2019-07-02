using System.Web.Mvc;
using gowikisearch.ViewModels;
using PagedList;
using gowikisearch.Models;
using System.Collections.Generic;
using System.Linq;
using Korzh.EasyQuery.Linq;
using System;

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
        [Route("Search/UpdatePagePopularity/{id:regex(^[1-9]$)}")]
        [Route("Search/Update/Popularity/{id:regex(^[1-9]$)}")]
        public ActionResult UpdatePagePopularity(int id)
        {
            try
            {
                WikipediaPageTitle pageTitleInstance = _context.WikipediaPageTitles.SingleOrDefault(p => p.Id == id);
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
            int maxSuggestions = 15;
            string sqlQuery = string.Format("SELECT TOP({0}) * FROM (SELECT TOP(1000) * FROM [dbo].WikipediaPageTitle " +
                "WHERE CONTAINS(Title, '\"{1}*\"')) as result WHERE result.Title LIKE '{1}%'" +
                "ORDER BY result.Popularity DESC, result.Title;", maxSuggestions, query);
            ViewBag.DatabaseName = _context.Database.Connection;
            IEnumerable<WikipediaPageTitle> suggestions = _context.WikipediaPageTitles.SqlQuery(sqlQuery).AsEnumerable();
            return View(suggestions);
        }
    }
}