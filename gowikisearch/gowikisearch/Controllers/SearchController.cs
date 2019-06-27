using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;

namespace gowikisearch.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        [HttpGet]
        [Route("search/")]
        [Route("search/{query}")]
        [Route("search/{query}/{pageNumber:regex(^[1-9]{0,2}$)}")]
        public ActionResult Index(string query, short? pageNumber)
        {
            if (!pageNumber.HasValue)
            {
                pageNumber = 1;
            }
            ViewBag.query = query;

            if (!String.IsNullOrEmpty(query))
            {
                var webClient = new System.Net.WebClient();
                string encodedUrlQuery = HttpUtility.UrlEncode(query);

                string url = String.Format("https://en.wikipedia.org/w/api.php?action=opensearch&search={0}&limit={1}&format={2}&formatversion=2&namespace=*", encodedUrlQuery, 30, "json");
                var wikipediaResults = webClient.DownloadString(url);
                // ViewBag.wikipediaResults = wikipediaResults;
                dynamic responseObject = JsonConvert.DeserializeObject(wikipediaResults);
                ViewBag.wikipediaResults = responseObject;
            }
            return View();
        }
    }
}