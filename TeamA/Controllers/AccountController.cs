using BusinessLayer;
using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamA.Attributes;
using TeamA.Authorize;
using TeamA.Models;

namespace TeamA.Controllers
{   [AllowAnonymous]
    [CookieFilter]
   
    public class AccountController : Controller
    {



        UserService userService = new UserService();
        
        public ActionResult Index()
        {
            
          return View();
        }

        public ActionResult Login()
        {
            if (Session["SessionUser"] == null)
                return View();
            else
            {
                LogOut();
                return View();
            }
        }

        [HttpPost]
        public ActionResult Login(AccountVM vm,string ReturnUrl)
        {
            try
            {
                userService.Login(vm.UserName, vm.Password);
                
                Session["SessionUser"] = vm.UserName;
                Session["SessionUserId"] = userService.GetUser(vm.UserName).Item1;


                

                //if (vm.Remember){
                var cookie = new HttpCookie("Cookie");
                cookie.Expires = DateTime.Now.AddDays(30);
                cookie["username"] = vm.UserName;
                cookie["password"] = vm.Password;


                Response.AppendCookie(cookie);
            //}
                string role = userService.GetRole(vm.UserName);

                if ((ReturnUrl == "") || (ReturnUrl == null))
                    return RedirectToAction("Index", role);
                else
                    return RedirectToAction(ReturnUrl);
            }
            catch (Exception e) 
            {
                return View("Error", (object)e.Message);
            }
        }

        public ActionResult Register()
        {
            TeacherListVM vm = new TeacherListVM()
            {
                TeacherNameList = userService.GetAllTeachers().Select(x => x.Username).ToList()
            };
            vm.TeacherNameList.Insert(0, null);

            return View(vm);
        }


        [HttpPost]
        public ActionResult Register(AccountVM vm,TeacherListVM tvm)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    userService.CreateStudentUser(vm.UserName, vm.Password, vm.Email, vm.TeacherName);
                    return View("MessageView", (object)"A confirmation message has benn sent. Please confirm!");
                }
                catch (Exception)
                {
                    return View("MessageView", (object)"An error has ocurred.");
                }
            }
            else
                return View("MessageView", (object)"An error has ocurred.");
        }

        public ActionResult ConfirmRegistration(string GUID)
        {
            if (userService.CheckGuid(GUID) == 1)
                return View("MessageView", (object)"Registration confirmed! Enjoy!");
            else return View("Error", "Invalid confirmation link!");
        }

        public AccountVM CheckCookie()
        {   AccountVM account=null;
               HttpCookie  cookie=null;
            string username = string.Empty; string password = string.Empty;
            if (HttpContext.Request.Cookies["Cookie"] != null) 
             cookie = Request.Cookies["Cookie"];
               username = cookie["username"];
               password = cookie["password"];
            

            if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                account = new AccountVM
                {
                    UserName = username,
                    Password = password
                };
            return account;
        }

        [HttpPost]
        public ActionResult LogOut()
        {
            Session.Clear();
            if (Request.Cookies["Cookie"] != null)
            {
                HttpCookie myCookie = new HttpCookie("Cookie");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);
            }
            return View("~/Views/Home/Index.cshtml");

        }


}

}
