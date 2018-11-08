using System.Web.Mvc;

namespace ASG.Areas.FinancialPlanner
{
    public class FinancialPlannerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "FinancialPlanner";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "FinancialPlanner_default",
                "FinancialPlanner/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}