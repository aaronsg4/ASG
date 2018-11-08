using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASG.Areas.FinancialPlanner.Models
{
    public class TransactionType  //Income, Transfer, Expense
    {

        public TransactionType()
        {
            Transactions = new HashSet<Transaction>();
        }
        public int Id { get; set; }
        public string Name { get; set; }


        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}