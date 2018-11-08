using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASG.Areas.FinancialPlanner.Models
{
    public class FinancialAccountType  //Checking Saving etc
    {
        public FinancialAccountType()
        {
            Accounts = new HashSet<FinancialAccount>();
        }


        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }




        public virtual ICollection<FinancialAccount> Accounts { get; set; }

    }
}