using System;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Web.Mvc;
using OnlineExam.Models;
using OnlineExam.Utils;

namespace OnlineExam.Helpers
{
    public static class RemoveForExtensions
    {
        public static void RemoveFor<TModel>(this ModelStateDictionary modelState,
            Expression<Func<TModel, object>> expression)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);

            foreach (var ms in modelState.ToArray())
            {
                if (ms.Key.StartsWith(expressionText + "."))
                {
                    modelState.Remove(ms);
                }
            }
        }
    }

    public static class StringExtensions
    {
        public static string ToString(this DateTime inputDate, bool showOnlyDate)
        {
            if (showOnlyDate)
            {
                string returnString;

                returnString = inputDate == DateTime.MinValue ? "N/A" : inputDate.ToString("dd/MM/yyyy");
                return returnString;
            }
            return inputDate.ToString().Replace("-", "/");
        }
    }

    public class CustomClaimsPrincipal : ClaimsPrincipal
    {
        public CustomClaimsPrincipal(ClaimsPrincipal principal)
            : base(principal)
        { }

        public static new CustomClaimsPrincipal Current
        {
            get
            {
                return new CustomClaimsPrincipal(ClaimsPrincipal.Current);
            }
        }
        public bool IsAusVisaAdmin
        {
            get
            {
                return HasClaim(ClaimTypes.Role, AppConstants.Roles.StaffAdmin);
            }
        }
        public bool IsAusVisaManager
        {
            get
            {
                return HasClaim(ClaimTypes.Role, AppConstants.Roles.StaffManager);
            }
        }
        public bool IsAusVisaOperator
        {
            get
            {
                return HasClaim(ClaimTypes.Role, AppConstants.Roles.StaffOperator);
            }
        }
        public bool IsAusVisaUser
        {
            get
            {
                return Claims.Where(c => c.Type == ClaimTypes.Email).Any(c => c.Value.Contains(AppConstants.DomainName));
            }
        }
        public bool IsACDAStoreUser
        {
            get
            {
                return CurrentUserEmail.Contains(AppConstants.DomainName);
            }
        }
        public string UserFullName
        {
            get
            {
                var claim = Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
                if (claim != null)
                    return claim.Value;
                return string.Empty;
            }
        }
        public string UserType
        {
            get
            {
                var claim = Claims.FirstOrDefault(c => c.Type == "UserType");
                if (claim != null)
                    return claim.Value;
                return string.Empty;
            }
        }
        public string ClassTypes
        {
            get
            {
                var claim = Claims.FirstOrDefault(c => c.Type == "ClassTypes");
                if (claim != null)
                    return claim.Value;
                return string.Empty;
            }
        }
        public string MembershipPlan
        {
            get
            {
                var claim = Claims.FirstOrDefault(c => c.Type == "MembershipPlan");
                if (claim != null)
                    return claim.Value;
                return string.Empty;
            }
        }
        public string MembershipPlanCode
        {
            get
            {
                var claim = Claims.FirstOrDefault(c => c.Type == "MembershipPlanCode");
                if (claim != null)
                    return claim.Value;
                return string.Empty;
            }
        }
        public string Avatar
        {
            get
            {
                var claim = Claims.FirstOrDefault(c => c.Type == "Avatar");
                if (claim != null)
                    return claim.Value;
                return string.Empty;
            }
        }
        public string CurrentRole
        {
            get
            {
                var claim = Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                if (claim != null)
                    return claim.Value;
                return string.Empty;
            }
        }
        public string CurrentUserEmail
        {
            get
            {
                var claim = Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (claim != null)
                    return claim.Value;
                return string.Empty;
            }
        }

        public string UserId
        {
            get
            {
                return Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                           .Select(c => c.Value)
                           .FirstOrDefault();
            }
        }

        public string GetTouristVisaAppliedBy(string refNumber)
        {
            string name = "";
            using (var db = new ApplicationDbContext())
            {
                var userObj = db.Users.Include("UserProfile").Include("Company").FirstOrDefault(a => a.Id == refNumber || a.Company.CompanyCode == refNumber);
                if (userObj != null)
                {
                    name = userObj.Company != null ? userObj.Company.Name : userObj.UserProfile.FirstName + " " + userObj.UserProfile.LastName;
                }
            }
            return name;
        }
        public string GetRefNumberForLoggedInUser()
        {
            string refNumber = "";

            if (this.IsAusVisaUser)
                return refNumber;

            if (!string.IsNullOrWhiteSpace(this.UserId))
            {
                var db = new ApplicationDbContext();
                var userObj = db.Users.Include("UserProfile").Include("Company").FirstOrDefault(a => a.Id == CustomClaimsPrincipal.Current.UserId);
                if (userObj != null)
                {
                    refNumber = userObj.Company != null ? userObj.Company.CompanyCode : Convert.ToString(CustomClaimsPrincipal.Current.UserId);
                }
            }
            return refNumber;
        }
    }
}