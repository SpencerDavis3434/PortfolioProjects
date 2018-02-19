using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio_Projects.Models
{
    public class LinkShortenerModel
    {
        public int Id { get; set; }

        public string LongLink { get; set; }

        public string ShortLinkExtension { get; set; }

        public DateTime ExpirationDate { get; set; }

    }
}