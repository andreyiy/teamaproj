using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamA.Attributes;

namespace TeamA.Controllers
{

    [CookieFilter]
    public class HomeController : Controller
    {
        

        public ActionResult Index()
        {
            return View();
        }

    }
}
