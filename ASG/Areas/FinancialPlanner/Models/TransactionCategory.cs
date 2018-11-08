using ASG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASG.Areas.FinancialPlanner.Models
{
    public class TransactionCategory  //Examples: Paycheck, Investment, Food, Groceries
    {

        public TransactionCategory()
        {
            Transactions = new HashSet<Transaction>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public string CreatedByUserId { get; set; }
        public virtual ApplicationUser User { get; set; }


        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}