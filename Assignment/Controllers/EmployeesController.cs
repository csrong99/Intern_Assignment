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

namespace Assignment.Views {
	public class EmployeesController : Controller {
		private MyCompanyEntities db = new MyCompanyEntities();

		// GET: Employees
		[Authorize]
		public ActionResult Index() {
			var employees = db.Employees.Include(e => e.Position1).Include(e => e.Status1).Include(e => e.Team1);
			return View(employees.ToList());
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
		public ActionResult Create([Bind(Include = "Username,Employee_ID,Email,Full_Name,Password,Confirm_Password,Join_Date,Position,Team,Security_Phrase,Status")] EmployeeCreateViewModel emp_view) {
			if (ModelState.IsValid) {
				Employee emp = EmployeeViewModel.CreateViewToEmployee(emp_view);
				if (db.Employees.Find(emp.Username) != null) {
					ModelState.AddModelError(string.Empty, "Username existed");
				}

				if (db.Employees.Where(x => x.Employee_ID == emp.Employee_ID).Count() != 0) {
					ModelState.AddModelError(string.Empty, "Employee ID existed in Database");

				}
				else {
					db.Employees.Add(emp);
					db.SaveChanges();
					return RedirectToAction("Index");
				}

			}

			ViewBag.Position = new SelectList(db.Positions, "Position_ID", "Name", emp_view.Position);
			ViewBag.Status = new SelectList(db.Status, "Status_ID", "Name", emp_view.Status);
			ViewBag.Team = new SelectList(db.Teams, "Team_ID", "Name", emp_view.Team);
			return View(emp_view);
		}

		// GET: Employees/Edit/5
		[Authorize]
		public ActionResult Edit(string id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Employee employee = db.Employees.Find(id);

			if (employee == null) {
				return HttpNotFound();
			}
			EmployeeEditViewModel emp_edit_view = EmployeeViewModel.EmployeeToEditView(employee);

			ViewBag.Position = new SelectList(db.Positions, "Position_ID", "Name", employee.Position);
			ViewBag.Status = new SelectList(db.Status, "Status_ID", "Name", employee.Status);
			ViewBag.Team = new SelectList(db.Teams, "Team_ID", "Name", employee.Team);
			return View(emp_edit_view);
		}

		// POST: Employees/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Username,Employee_ID,Email,Full_Name,Password,Confirm_Password,Join_Date,Position,Team,Security_Phrase,Status")] EmployeeEditViewModel emp_view) {
			if (ModelState.IsValid) {
				Employee new_emp = EmployeeViewModel.EditViewToEmployee(emp_view);
				Employee emp = db.Employees.Find(new_emp.Username);
				emp = db.Employees.Attach(emp);
				emp.Email = new_emp.Email;
				emp.Full_Name = new_emp.Full_Name;
				emp.Join_Date = new_emp.Join_Date;
				emp.Status = new_emp.Status;
				emp.Team = new_emp.Team;
				emp.Position = new_emp.Position;
				emp.Security_Phrase = new_emp.Security_Phrase;
				if (!string.IsNullOrEmpty(new_emp.Password)) {
					emp.Password = new_emp.Password;
				}
				try {
					db.SaveChanges();
				}
				catch (DbEntityValidationException e) {
					foreach (var eve in e.EntityValidationErrors) {
						System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
							eve.Entry.Entity.GetType().Name, eve.Entry.State);
						foreach (var ve in eve.ValidationErrors) {
							System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
								ve.PropertyName, ve.ErrorMessage);
						}
					}
					throw;
				}

				return RedirectToAction("Index");
			}
			ViewBag.Position = new SelectList(db.Positions, "Position_ID", "Name", emp_view.Position);
			ViewBag.Status = new SelectList(db.Status, "Status_ID", "Name", emp_view.Status);
			ViewBag.Team = new SelectList(db.Teams, "Team_ID", "Name", emp_view.Team);
			return View(emp_view);
		}

		// GET: Employees/Delete/5
		[Authorize]
		public ActionResult Delete(string id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Employee employee = db.Employees.Find(id);
			if (employee == null) {
				return HttpNotFound();
			}
			return View(employee);
		}

		// POST: Employees/Delete/5
		[Authorize]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(string id) {
			Employee employee = db.Employees.Find(id);
			db.Employees.Remove(employee);
			db.SaveChanges();
			return RedirectToAction("Index");
		}
		// GET : GetDeletePartial/5
		[Authorize]
		[HttpGet]
		public PartialViewResult GetDeletePartial(string id) {
			var deleteItem = db.Employees.Find(id);
			return PartialView("Delete", deleteItem);
		}

		// GET: Employees/IsUserAvailable/abc123
		[HttpGet]
		public JsonResult IsUserAvailable(string id) {
			bool userAvailable = false;

			if (!string.IsNullOrWhiteSpace(id)) {
				userAvailable = db.Employees.Find(id) == null ? true : false;
			}


			return Json(userAvailable.ToString(), JsonRequestBehavior.AllowGet);

		}

		[HttpGet]
		public JsonResult IsIDExisted(string id) {
			bool emp_id_existed = false;
			int emp_id;
			try {
				emp_id = int.Parse(id);
				emp_id_existed = db.Employees.Where(x => x.Employee_ID == (emp_id)).Count() != 0 ? true : false;

			}
			catch (Exception ex) {

			}
			return Json(emp_id_existed.ToString(), JsonRequestBehavior.AllowGet);

		}


		protected override void OnActionExecuting(ActionExecutingContext filterContext) {
			base.OnActionExecuting(filterContext);
			System.Diagnostics.Debug.WriteLine("Executing");
			System.Diagnostics.Debug.WriteLine("sess id: " + Session.SessionID);
			if (Session["logon"] != null) {
				string username = ((Log)Session["logon"]).Username;
				if (!Session.SessionID.Equals(HttpContext.Application[username])) {
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
