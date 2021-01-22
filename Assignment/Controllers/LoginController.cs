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
			System.Diagnostics.Debug.WriteLine("passed in here");

			if (ModelState.IsValid) {
				string username = emp_login_view.Username;
				string password = emp_login_view.Password;

				Employee emp_logon = db.Employees.Where(emp => emp.Username.ToLower().Equals(username.ToLower())).FirstOrDefault();

				string ipv4 = GetIp();
				Log new_log = new Log {
					Attempt_Time = DateTime.Now,
					Ipv4 = ipv4,
				};

				// Check if valid username
				if (emp_logon != null) {

					// Check if the user is suspended
					if (emp_logon.Status == 3) {
						new_log.successful = false;
						new_log.Username = emp_logon.Username;
						db.Logs.Add(new_log);
						db.SaveChanges();

						string error_msg = "Your account has been suspended. Please contact admin";
						return Json(new { EnableError = true, ErrorTitle = "Error", ErrorMsg = error_msg });
					}
					// Check if the password is correct 
					else if (emp_logon.Password.Equals(password)) {
						new_log.successful = true;
						new_log.Username = emp_logon.Username;
						db.Logs.Add(new_log);
						db.SaveChanges();


						System.Diagnostics.Debug.WriteLine("Successful Login");
						HttpApplicationStateBase app_state = HttpContext.Application;

						FormsAuthentication.SetAuthCookie(new_log.Username, false);
						// Create a new logon session
						Session["logon"] = new_log;

						// If the username didnt logon in before
						if (app_state[new_log.Username] == null) {
							System.Diagnostics.Debug.WriteLine("Didnt Login Before");
							app_state.Add(new_log.Username, Session.SessionID);
						} 
						// If the username is logged in and have active session
						else {
							System.Diagnostics.Debug.WriteLine("Login Before");
							string sess_ID = app_state[new_log.Username] as string;
							if (!sess_ID.Equals(Session.SessionID)) {
								app_state[new_log.Username] = Session.SessionID;
							}
							
						}


						return RedirectToAction("Index", "Employees");

					}
					// if user is suspended or password is incorrect
					else {
						new_log.successful = false;
						new_log.Username = emp_logon.Username;
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
			System.Diagnostics.Debug.WriteLine("Failed");
			ViewBag.error = "Invalid username or password";

			return View("Index");
		}

		[HttpGet]
		public ActionResult Logout() {
			FormsAuthentication.SignOut();
			Log log = Session["logon"] as Log;

			HttpContext.Application.Remove(log.Username);
			Session.Clear();
			Session.Abandon();

			return new RedirectToRouteResult(
				new System.Web.Routing.RouteValueDictionary(
				new { controller = "Login", action = "Index", errorCode = "2" }
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