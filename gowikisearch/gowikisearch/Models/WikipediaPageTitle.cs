using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace gowikisearch.Models
{
    public class WikipediaPageTitle
    {
        private int popularity = 0;
        public WikipediaPageTitle()
        {

        }

        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [MinLength(3)]
        [Index(IsUnique = true)]
        public string Title { get; set; }

        [Required]
        [DefaultValue(0)]
        public int Popularity { get { return popularity; } set { popularity = value; } }
    }
}