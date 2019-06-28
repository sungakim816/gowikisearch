using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;

namespace gowikisearch.Models
{
    public class WikipediaPage
    {
        public WikipediaPage()
        {

        }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
    }
}