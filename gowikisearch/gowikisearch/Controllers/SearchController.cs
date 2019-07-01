using System.Web.Mvc;
using gowikisearch.ViewModels;
using PagedList;
using gowikisearch.Models;
using System.Collections.Generic;
using System.Linq;
using Korzh.EasyQuery.Linq;

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
        public ViewResult Index(string query, int? pageNumber)
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

        // GET: Search Autocomplete
        [HttpGet]
        [Route("Search/Autocomplete")]
        [Route("Search/Autocomplete/{query}")]
        public ViewResult Autocomplete(string query)
        {
            query = "Kim";
            query = query.ToLower();
            int maxSuggestions = 10;
            string sqlQuery = string.Format("SELECT TOP({0}) * " +
                "FROM [dbo].[WikipediaPageTitle] " +
                "WHERE CONTAINS(Title, '\"{1}*\"')", maxSuggestions, query
                );
            ViewBag.DatabaseName = _context.Database.Connection;
            IEnumerable<WikipediaPageTitle> suggestions = _context.WikipediaPageTitles.SqlQuery(sqlQuery).AsEnumerable();
            return View(suggestions);
        }
    }
}