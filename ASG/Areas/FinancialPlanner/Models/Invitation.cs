using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASG.Areas.FinancialPlanner.Models
{
    public class Invitations
    {
        public int Id { get; set; }
        public string ToEmail { get; set; }
        public string SenderUserId { get; set; }

        public bool Accepted { get; set; }
        public bool Expired { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public int HouseholdId { get; set; }
        public virtual Household Household { get; set; }
    }
}