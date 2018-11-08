using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASG.Areas.FinancialPlanner.Models
{
    public class BudgetHouseholdViewModel
    {
        public Budget budget { get; set; }
        public IList<Budget> budgetb { get; set; }
        public Household household { get; set; }
        public ICollection<Budget> budgets { get; set; }
        public ICollection<Household> Households { get; set; }
    }
}