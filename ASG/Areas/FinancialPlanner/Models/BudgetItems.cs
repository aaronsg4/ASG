using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASG.Areas.FinancialPlanner.Models
{
    public class BudgetItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }


        public int BudgetId { get; set; }
        public virtual Budget Budget { get; set; }

        public int? TransactionCategoryId { get; set; }
        public TransactionCategory TransactionCategory { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

    }
}