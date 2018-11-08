using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASG.Models
{
    public class BlogTopicsViewModel
    {
        public BlogPost Post { get; set; }
        public string[] Topics { get; set; }
    }
}