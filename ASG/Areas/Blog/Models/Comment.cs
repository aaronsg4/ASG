using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASG.Areas.Blog.Models
{   
        public class Comment
        {
            public Comment()
            {
                Replies = new HashSet<Reply>();
            }



            public int Id { get; set; }
            public int PostId { get; set; }
            public string CommenterId { get; set; }
            public string Body { get; set; }
            [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
            public DateTimeOffset createdDate { get; set; }
            [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
            public DateTimeOffset? updatedDate { get; set; }
            public string updatedReason { get; set; }
            public int Count { get; set; }

            public virtual ICollection<Reply> Replies { get; set; }
            public virtual ApplicationUser Commenter { get; set; }
            public virtual BlogPost Post { get; set; }


        }

    }


