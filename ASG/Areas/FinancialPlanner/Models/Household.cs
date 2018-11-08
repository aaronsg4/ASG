using ASG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASG.Areas.FinancialPlanner.Models
{
    public class Household
    {
        public Household()
        {
            Accounts = new HashSet<FinancialAccount>();
            Budgets = new HashSet<Budget>();
            Users = new HashSet<ApplicationUser>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DissolvedDate { get; set; }
        public string InvitedEmail { get; set; }
        public int? TransactionCategoryId { get; set; }
        public string UserId { get; set; }


        public virtual ICollection<Invitations> Invitation { get; set; }
        public virtual TransactionCategory TransactionCategeory { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<FinancialAccount> Accounts { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

    }
}