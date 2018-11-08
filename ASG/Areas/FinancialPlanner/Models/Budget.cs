
using ASG.Areas.FinancialPlanner.Models;
using ASG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASG.Areas.FinancialPlanner.Models
{
    public class Budget
    {

        public Budget()
        {
            Users = new HashSet<ApplicationUser>();
            BudgetItems = new HashSet<BudgetItem>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal BudgetRemaining { get; set; }
        public string Description { get; set; }
        public decimal? SuggestedAmount { get; set; }
        public int BudgetEndDate { get; set; }
        public DateTime BudgetEnd { get; set; }
        public DateTime BudgetStartDate { get; set; }
        public bool Expired { get; set; }



        public string UserId { get; set; }

        public int? BudgetDurationPeriodId { get; set; }
        public virtual BudgetDurationPeriod BudgetDurationPeriod { get; set; }


        public int HouseholdId { get; set; }
        public virtual Household Household { get; set; }



        public virtual ICollection<ApplicationUser> Users { get; set; }


        public virtual ICollection<BudgetItem> BudgetItems { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}