//using Microsoft.AspNet.Identity.Owin;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;

//namespace ASG.Areas.FinancialPlanner.Models
//{
//    public static class RefreshAuth
//    {

//        public static async Task RefreshAuthentication(this HttpContextBase context, ApplicationUser user)
//        {
//            context.GetOwinContext().Authentication.SignOut();
//            await context.GetOwinContext().Get<FPApplicationSignInManager>().SignInAsync(user, isPersistent: false, rememberBrowser: false);
//        }
//    }
//}