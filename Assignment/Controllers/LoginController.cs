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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult ValidateUser(string username, string password, bool rememberme) {

            if (ModelState.IsValid) {
                string username = emp_login_view.Username;
                string password = emp_login_view.Password;

                Employee emp = db.Employees.Find(username);
                if(emp != null) {
                    if(emp.Password.Equals(password)) {
                        return RedirectToRoute("EmployeeIndex");
					}
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
				}
            }

            return View();
        }

    }
}