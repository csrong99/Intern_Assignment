using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Assignment.Models;

namespace Assignment.Controllers
{
    public class LoginController : Controller
    {
        private MyCompanyEntities db = new MyCompanyEntities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }


    }
}