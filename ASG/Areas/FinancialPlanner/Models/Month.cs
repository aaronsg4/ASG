using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASG.Areas.FinancialPlanner.Models
{
    public class Month
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MonthNumber { get; set; }
        public DateTime Year { get; set; }

        public int HouseholdId { get; set; }
        public virtual Household Household { get; set; }
    }
}