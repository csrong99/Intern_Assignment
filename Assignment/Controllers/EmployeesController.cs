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
				Employee employee = EmployeeViewModel.CreateViewToEmployee(emp_view);
				if (db.Employees.Find(employee.Employee_ID) != null) {
					ModelState.AddModelError(string.Empty, "Employee ID existed in Database");
				}

				if (db.Employees.Where(x => x.Username.Equals(employee.Username)).Count() != 0) {
					ModelState.AddModelError(string.Empty, "Username existed");

				}
				else {
					db.Employees.Add(employee);
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
					employee.Password = new_employee.Password;
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

		// POST: Employees/Delete/5
		[Authorize]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id) {
			Employee employee = db.Employees.Find(id);
			employee.Status = 2;
			db.SaveChanges();
			return RedirectToAction("Index");
		}
		// GET : GetDeletePartial/5
		[Authorize]
		[HttpGet]
		public PartialViewResult GetDeletePartial(int id) {
			var deleteItem = db.Employees.Find(id);
			return PartialView("Delete", deleteItem);
		}

		// GET: Employees/IsUserAvailable/abc123
		[HttpGet]
		public JsonResult IsUsernameAvailable(string username) {
			bool username_available = false;

			username_available = db.Employees.Where(x => x.Username.Equals(username)).Count() == 0 ? true : false;

			return Json(username_available.ToString(), JsonRequestBehavior.AllowGet);

		}

		[HttpGet]
        public JsonResult IsIDExisted(string id) {

            bool emp_id_existed = false;
            int emp_id;
            if (!string.IsNullOrEmpty(id)) {
                try {
                    emp_id = int.Parse(id);
                    emp_id_existed = db.Employees.Find(emp_id) != null ? true : false;

                }
                catch (Exception) {

                }
            }

            return Json(emp_id_existed.ToString(), JsonRequestBehavior.AllowGet);

        }


        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
			base.OnActionExecuting(filterContext);
			System.Diagnostics.Debug.WriteLine("Executing");
			System.Diagnostics.Debug.WriteLine("sess id: " + Session.SessionID);
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
