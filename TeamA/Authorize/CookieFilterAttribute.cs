using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Providers.Entities;


namespace TeamA.Attributes
{
    public class CookieFilterAttribute : ActionFilterAttribute

    {
        private BusinessLayer.UserService _userService = new BusinessLayer.UserService();

        public override void OnActionExecuting(ActionExecutingContext filterContext )
        {

            string username = string.Empty;string password = string.Empty;
            var cookie = filterContext.HttpContext.Request.Cookies.Get("Cookie");
            if (cookie != null)
            {
                username = cookie["username"];
                password = cookie["password"];
                if (_userService.Login(username, password))
                {
                    HttpContext.Current.Session["SessionUser"] = username;
                    HttpContext.Current.Session["SessionUserId"] = _userService.GetUser(username).Item1;
                }

            }
            else
            {               
            }
            base.OnActionExecuting(filterContext);
        }      
    }
}