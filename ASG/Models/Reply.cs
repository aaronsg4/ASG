using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASG.Models
{
    public class Reply
    {
        public int Id { get; set; }
        public int SpecificCommentId { get; set; }
        public string ReplierId { get; set; }
        public string Body { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
        public DateTimeOffset createdDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
        public DateTimeOffset? updatedDate { get; set; }
        public string updatedReason { get; set; }
        public int Count { get; set; }
        
        public virtual ApplicationUser Replier { get; set; }
        public virtual Comment SpecificComment { get; set; }
    }
}