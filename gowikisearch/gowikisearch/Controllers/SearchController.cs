using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;
using gowikisearch.ViewModels;
using System.Collections;
using PagedList;

namespace gowikisearch.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        [HttpGet]
        [Route("search/")]
        [Route("search/{query}")]
        [Route("search/{query}/{pageNumber:regex(^[1-9]{0,2}$)}")]
        public ActionResult Index(string query, int? pageNumber)
        {
            int pageSize = 6;
            if (!pageNumber.HasValue)
            {
                pageNumber = 1;
            }
            ViewBag.query = query;
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
    }
}