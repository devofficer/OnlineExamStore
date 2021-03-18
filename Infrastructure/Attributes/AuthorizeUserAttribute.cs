using OnlineExam.Controllers;
using OnlineExam.Helpers;
using OnlineExam.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineExam.Infrastructure
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //TBD://
            bool isAuthorized = true;

            var rd = httpContext.Request.RequestContext.RouteData;
            string currentAction = rd.GetRequiredString("action");
            string currentController = rd.GetRequiredString("controller");
            string currentArea = rd.Values["area"] as string;

            if (!httpContext.Request.IsAuthenticated)
                isAuthorized = false;

            switch (currentController)
            {
                case "SmartQuiz":
                    if (SessionHelper.IsExisting(CustomClaimsPrincipal.Current.UserId))
                    {
                        SessionHelper.RemoveItem(CustomClaimsPrincipal.Current.UserId);
                    }
                    break;
                case "Question":
                    //if (!CustomClaimsPrincipal.Current.IsACDAStoreUser && CustomClaimsPrincipal.Current.MembershipPlan == MembershipPlanType.Trial.ToString())
                    isAuthorized = false;
                    if (CustomClaimsPrincipal.Current.IsACDAStoreUser || CustomClaimsPrincipal.Current.CurrentRole == "Teacher"
                         || CustomClaimsPrincipal.Current.CurrentRole == "Student")
                    {
                        isAuthorized = true;
                    }
                    break;
                case "QuestionPaper":
                    switch (currentAction)
                    {
                        case "Create":
                            if (!CustomClaimsPrincipal.Current.IsACDAStoreUser && CustomClaimsPrincipal.Current.MembershipPlan == MembershipPlanType.Trial.ToString())
                            {
                                isAuthorized = false;
                            }
                            break;
                        case "Edit":
                            if (!CustomClaimsPrincipal.Current.IsACDAStoreUser && CustomClaimsPrincipal.Current.MembershipPlan == MembershipPlanType.Trial.ToString())
                            {
                                isAuthorized = false;
                            }
                            break;
                        case "Delete":
                            break;
                    }
                    break;
                case "Account":
                    break;
                case "SystemSettings":
                    break;
                case "Vendor":
                    break;
                case "Voucher":
                    break;
            }

            //foreach (var role in allowedroles)
            //{
            //    var user = context.AppUser.Where(m => m.UserID == GetUser.CurrentUser/* getting user form current context */ && m.Role == role &&
            //    m.IsActive == true); // checking active users with allowed roles.  
            //    if (user.Count() > 0)
            //    {
            //        authorize = true; /* return true if Entity has current user(active) with specific role */
            //    }
            //}

            //isAuthorized = base.AuthorizeCore(httpContext);
            //if (!isAuthorized)
            //{
            //    isAuthorized= false;
            //}
            if (isAuthorized)
            {
                httpContext.Response.StatusCode = 200;
            }

            return isAuthorized;
            //string privilegeLevels = string.Join("", GetUserRights(httpContext.User.Identity.Name.ToString())); // Call another method to get rights of the user from DB

            //if (privilegeLevels.Contains(this.AccessLevel))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //User isn't logged in
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Account", action = "Login" })
                );
            }
            //User is logged in but has no access
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Account", action = "NotAuthorized" })
                );
            }
        }
    }
}