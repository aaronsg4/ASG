using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASG.Models
{
    public class BlogPost


    {

        public BlogPost()
        {
            Comments = new HashSet<Comment>();
            Topics = new HashSet<Topic>();
        }
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
        public DateTimeOffset createdDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
        public DateTimeOffset? updatedDate { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public string MediaURL { get; set; }
        public bool Published { get; set; }

  
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Topic>Topics { get; set; }
    }
}