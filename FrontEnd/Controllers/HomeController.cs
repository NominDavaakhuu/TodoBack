using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FrontEnd.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["ApiLoggedIn"] != null && (bool)Session["ApiLoggedIn"] == true)
            {
                return RedirectToAction("Index", "TodoItem");
            }
            return View();
        }
    }
}