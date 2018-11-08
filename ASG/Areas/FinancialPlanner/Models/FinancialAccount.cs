using ASG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASG.Areas.FinancialPlanner.Models
{
    public class FinancialAccount   //Bank account, etc
    {
        public FinancialAccount()
        {
            Transactions = new HashSet<Transaction>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal ActualBalance { get; set; }
        public decimal ReconciledBalance { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public bool Include { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public string AccountHolderUserId { get; set; }
        public virtual ApplicationUser User { get; set; }


        public int? HouseholdId { get; set; }
        public virtual Household Household { get; set; }


        public int AccountTypeId { get; set; }
        public virtual FinancialAccountType AccountType { get; set; }
    }
}