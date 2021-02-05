using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Assignment.Models;
using System.Web.Security;


namespace Assignment.Controllers {
	public class LoginController : Controller {
		private MyCompanyEntities db = new MyCompanyEntities();
		// GET: Login
		public ActionResult Index() {
			return View();
		}

		// POST: Login/ValidateUser
		[HttpPost]
		public ActionResult ValidateUser([Bind(Include = "username,password")] EmployeeLoginViewModel emp_login_view) {

			if (ModelState.IsValid) {
				string username = emp_login_view.Username;
				string password = emp_login_view.Password;

				Employee employee_logon = db.Employees.Where(emp => emp.Username.ToLower().Equals(username.ToLower())).FirstOrDefault();

				string ipv4 = GetIp();
				Log new_log = new Log {
					Attempt_Time = DateTime.Now,
					Ipv4 = ipv4,
				};
				
				if(db.Ipv4Blacklist.Any(r => r.Ipv4.Equals(ipv4)))
                {
					return Json(new { EnableError = true, ErrorTitle = "Error", ErrorMsg = "Blacklisted IP Address. Please contact system admin." });
				}

				// Check if valid username
				if (employee_logon != null) {

					// Check if the user is suspended
					if (employee_logon.Status == 3 || employee_logon.Status == 2) {
						new_log.successful = false;
						new_log.Employee_ID = employee_logon.Employee_ID;
						db.Logs.Add(new_log);
						db.SaveChanges();

						string error_msg = "Your account has been suspended or disabled. Please contact admin";
						return Json(new { EnableError = true, ErrorTitle = "Error", ErrorMsg = error_msg });
					}
					// Check if the password is correct 
					else if (Hashing.ValidatePassword(password, employee_logon.Password)) {
						new_log.successful = true;
						new_log.Employee_ID = employee_logon.Employee_ID;
						db.Logs.Add(new_log);
						db.SaveChanges();

						HttpApplicationStateBase app_state = HttpContext.Application;

						FormsAuthentication.SetAuthCookie(new_log.Employee_ID.ToString(), false);
						// Create a new logon session
						Session["logon"] = new_log;

						app_state.Lock();
						// If the username didnt logon in before
						if (app_state[new_log.Employee_ID.ToString()] == null) {
							
							app_state.Add(new_log.Employee_ID.ToString(), Session.SessionID);
							
						} 
						// If the username is logged in and have active session
						else {
							string sess_ID = app_state[new_log.Employee_ID.ToString()] as string;
							if (!sess_ID.Equals(Session.SessionID)) {
								app_state[new_log.Employee_ID.ToString()] = Session.SessionID;
							}
							
						}
						app_state.UnLock();

						return Json(new { EnableSuccess = true, RedirectUrl = "/Employees" });

					}
					// if user is suspended or password is incorrect
					else {
						new_log.successful = false;
						new_log.Employee_ID = employee_logon.Employee_ID;
						db.Logs.Add(new_log);
						db.SaveChanges();
					}

				}

				// if username is invalid
				else {
					new_log.successful = false;
					db.Logs.Add(new_log);
					db.SaveChanges();
				}

			}

			return Json(new { EnableError = true, ErrorTitle = "Error", ErrorMsg = "Invalid username or password" });
		}

		[HttpGet]
		public ActionResult Logout() {
			FormsAuthentication.SignOut();
			Log log = Session["logon"] as Log;

			HttpContext.Application.Remove(log.Employee_ID.ToString());
			Session.Clear();
			Session.Abandon();

			return new RedirectToRouteResult(
				new System.Web.Routing.RouteValueDictionary(
				new { controller = "Login", action = "Index" }
				)
			);

		}

		public string GetIp() {
			string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			if (string.IsNullOrEmpty(ip)) {
				ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
			}
			return ip;
		}
	}



}