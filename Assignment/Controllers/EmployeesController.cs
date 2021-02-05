using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Assignment;
using Assignment.Models;
using System.Web.Security;
using PagedList;

namespace Assignment.Views {
	public class EmployeesController : Controller {
		private MyCompanyEntities db = new MyCompanyEntities();

		// GET: Employees
		[Authorize]
		public ActionResult Index(int? page) {
			var employees = db.Employees.OrderBy(e => e.Employee_ID);
			int pageSize = 5;
			int pageNumber = (page ?? 1);
			return View(employees.ToPagedList(pageNumber, pageSize));
		}

		// GET: Employees/Details/5
		[Authorize]
		public ActionResult Details(string id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Employee employee = db.Employees.Find(id);
			if (employee == null) {
				return HttpNotFound();
			}
			return View(employee);
		}

		// GET: Employees/Create
		[Authorize]
		public ActionResult Create() {
			ViewBag.Position = new SelectList(db.Positions, "Position_ID", "Name");
			ViewBag.Status = new SelectList(db.Status, "Status_ID", "Name");
			ViewBag.Team = new SelectList(db.Teams, "Team_ID", "Name");
			return View();
		}

		// POST: Employees/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Username,Employee_ID,Email,Full_Name,Password,Confirm_Password,Join_Date,Position,Team,Security_Phrase,Status")] EmployeeCreateViewModel employee_view) {
			if (ModelState.IsValid) {
				Employee employee = EmployeeViewModel.CreateViewToEmployee(employee_view);
				if (db.Employees.Find(employee.Employee_ID) != null) {
					ModelState.AddModelError(string.Empty, "Employee ID existed in Database");
				}

				if (db.Employees.Where(x => x.Username.Equals(employee.Username)).Count() != 0) {
					ModelState.AddModelError(string.Empty, "Username existed");

				}
				else {
					employee.Password = Hashing.HashPassword(employee.Password);
					db.Employees.Add(employee);
					db.SaveChanges();
					return RedirectToAction("Index");
				}

			}

			ViewBag.Position = new SelectList(db.Positions, "Position_ID", "Name", employee_view.Position);
			ViewBag.Status = new SelectList(db.Status, "Status_ID", "Name", employee_view.Status);
			ViewBag.Team = new SelectList(db.Teams, "Team_ID", "Name", employee_view.Team);
			return View(employee_view);
		}

		// GET: Employees/Edit/5
		[Authorize]
		public ActionResult Edit(string id) {
			int employee_id;

			try {
				employee_id = int.Parse(id);
            } catch (Exception) {
				return View("Error");
			}
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			
			Employee employee = db.Employees.Find(employee_id);

			if (employee == null) {
				return HttpNotFound();
			}
			EmployeeEditViewModel employee_edit_view = EmployeeViewModel.EmployeeToEditView(employee);

			ViewBag.Position = new SelectList(db.Positions, "Position_ID", "Name", employee.Position);
			ViewBag.Status = new SelectList(db.Status, "Status_ID", "Name", employee.Status);
			ViewBag.Team = new SelectList(db.Teams, "Team_ID", "Name", employee.Team);
			return View(employee_edit_view);
		}

		// POST: Employees/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EmployeeEditViewModel employee_view) {
			if (ModelState.IsValid) {
				Employee new_employee = EmployeeViewModel.EditViewToEmployee(employee_view);
				Employee employee = db.Employees.Find(new_employee.Employee_ID);
				employee = db.Employees.Attach(employee);
				employee.Email = new_employee.Email;
				employee.Full_Name = new_employee.Full_Name;
				employee.Join_Date = new_employee.Join_Date;
				employee.Status = new_employee.Status;
				employee.Team = new_employee.Team;
				employee.Position = new_employee.Position;
				employee.Security_Phrase = new_employee.Security_Phrase;
				if (!string.IsNullOrEmpty(new_employee.Password)) {
					employee.Password = Hashing.HashPassword(new_employee.Password);
				}
				try {
					db.SaveChanges();
				}
				catch (DbEntityValidationException) {
					throw;
				}

				return RedirectToAction("Index");
			}
			
			ViewBag.Position = new SelectList(db.Positions, "Position_ID", "Name", employee_view.Position);
			ViewBag.Status = new SelectList(db.Status, "Status_ID", "Name", employee_view.Status);
			ViewBag.Team = new SelectList(db.Teams, "Team_ID", "Name", employee_view.Team);
			return View(employee_view);
		}

		// GET: Employees/Delete/5
		[Authorize]
		public ActionResult Delete(int id) {
			if (id <= 0) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Employee employee = db.Employees.Find(id);
			if (employee == null) {
				return HttpNotFound();
			}
			return View(employee);
		}

		// POST: Employees/Delete/C
		[Authorize]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id) {
			Employee employee = db.Employees.Include(inc => inc.Logs).First(e => e.Employee_ID == id);
            employee = db.Employees.Remove(employee);
            db.SaveChanges();

			Log log = Session["logon"] as Log;

			if (employee.Employee_ID == log.Employee_ID)
				return RedirectToAction("Logout", "Login");

			else return RedirectToAction("Index");
		}
		// GET : GetDeletePartial/5
		[Authorize]
		[HttpGet]
		public PartialViewResult GetDeletePartial(int id) {
			var deleteItem = db.Employees.Find(id);
			return PartialView("Delete", deleteItem);
		}

		// GET: Employees/IsUserAvailable/abc123
		[HttpPost]
		public JsonResult IsUsernameAvailable(string username) {
			bool username_available = false;

			username_available = db.Employees.Where(x => x.Username.Equals(username)).Count() == 0 ? true : false;

			return Json(username_available);

		}

		[HttpPost]
        public JsonResult IsIDAvailable(int employee_id) {

            bool employee_id_available = false;
			if(employee_id > 0) {
				employee_id_available = db.Employees.Find(employee_id) == null ? true : false;
			}
            
            return Json(employee_id_available);
        }


        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
			base.OnActionExecuting(filterContext);
			if (Session["logon"] != null) {
				string employee_id = ((Log)Session["logon"]).Employee_ID.ToString();
				if (!Session.SessionID.Equals(HttpContext.Application[employee_id])) {
					// set FormAuthentication
					FormsAuthentication.SignOut();
					filterContext.Result = new RedirectToRouteResult(
						new System.Web.Routing.RouteValueDictionary(
						new { controller = "Login", action = "Index", errorCode = "2" }
						)
					);
				}
			}
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
