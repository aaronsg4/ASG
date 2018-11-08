using ASG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASG.Areas.FinancialPlanner.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public bool Void { get; set; }
        public decimal? ReconciledAmount { get; set; }
        public bool Reconciled { get; set; }

        public int? BudgetId { get; set; }
        public virtual Budget Budget { get; set; }

        public int? BudgetItemId { get; set; }
        public virtual BudgetItem BudgetItem { get; set; }

        public int FinancialAccountId { get; set; }
        public virtual FinancialAccount FinancialAccount { get; set; }


        public string SubmitterUserId { get; set; }
        public virtual ApplicationUser SubmitterUser { get; set; }

        public int TransactionTypeId { get; set; }
        public virtual TransactionType TransactionType { get; set; }

        public int? TransactionCategoryId { get; set; }
        public virtual TransactionCategory TransactionCategory { get; set; }
    }
}