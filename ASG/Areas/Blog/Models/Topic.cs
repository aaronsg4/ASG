using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASG.Areas.Blog.Models
{
  
        public class Topic
        {
            public int Id { get; set; }
            public string TopicName { get; set; }

            public virtual ICollection<BlogPost> Posts { get; set; }
        }
    }
  