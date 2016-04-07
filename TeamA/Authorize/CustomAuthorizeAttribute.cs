using BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamA.Models;

namespace TeamA.Authorize
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private UserService _userService = new UserService();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["SessionUser"] != null)
            {              
                var loggedUser =(string) HttpContext.Current.Session["SessionUser"];

                string userRole = _userService.GetRole(loggedUser);
                foreach (string definedRole in this.Roles.Split(','))
                {                  
                    if (definedRole.Equals(userRole))
                        return true;                   
                }                   
            }           
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("http://www.google.ro");
            base.HandleUnauthorizedRequest(filterContext);
        }      
    }
}
