﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace ASG.Areas.FinancialPlanner.Models
{
    public static class Extensions
    {
        public static string GetHouseholdId(this IIdentity user)
        {
            var claimsIdentity = (ClaimsIdentity)user;
            var HouseholdClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "HouseholdId");

            if (HouseholdClaim != null)
                return HouseholdClaim.Value;
            else
                return null;
        }

        public static bool IsInHousehold(this IIdentity user)
        {
            var cUser = (ClaimsIdentity)user;
            var hId = cUser.Claims.FirstOrDefault(c => c.Type == "HouseholdId");
            return (hId != null && !string.IsNullOrWhiteSpace(hId.Value));
        }
    }
}