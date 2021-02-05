
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Assignment.Models {
	public static class EmployeeViewModel {
		public static EmployeeEditViewModel EmployeeToEditView(Employee emp) {
			EmployeeEditViewModel emp_edit_view = new EmployeeEditViewModel() {
				Username = emp.Username,
				Email = emp.Email,
				Employee_ID = emp.Employee_ID,
				Full_Name = emp.Full_Name,
				Join_Date = emp.Join_Date,
				Password = emp.Password,
				Position = emp.Position,
				Security_Phrase = emp.Security_Phrase,
				Status = emp.Status,
				Team = emp.Team
			};
			return emp_edit_view;
		}

		public static Employee EditViewToEmployee(EmployeeEditViewModel emp_view) {
			Employee emp = new Employee() {
				Username = emp_view.Username,
				Email = emp_view.Email,
				Employee_ID = emp_view.Employee_ID,
				Full_Name = emp_view.Full_Name,
				Join_Date = emp_view.Join_Date,
				Password = emp_view.Password,
				Position = emp_view.Position,
				Security_Phrase = emp_view.Security_Phrase,
				Status = emp_view.Status,
				Team = emp_view.Team
			};
			return emp;
		}

		public static EmployeeCreateViewModel EmployeeToCreateView(Employee emp) {
			EmployeeCreateViewModel emp_edit_view = new EmployeeCreateViewModel() {
				Username = emp.Username,
				Email = emp.Email,
				Employee_ID = emp.Employee_ID,
				Full_Name = emp.Full_Name,
				Join_Date = emp.Join_Date,
				Password = emp.Password,
				Position = emp.Position,
				Security_Phrase = emp.Security_Phrase,
				Status = emp.Status,
				Team = emp.Team
			};
			return emp_edit_view;
		}

		public static Employee CreateViewToEmployee(EmployeeCreateViewModel emp_view) {
			Employee emp = new Employee() {
				Username = emp_view.Username,
				Email = emp_view.Email,
				Employee_ID = emp_view.Employee_ID,
				Full_Name = emp_view.Full_Name,
				Join_Date = emp_view.Join_Date,
				Password = emp_view.Password,
				Position = emp_view.Position,
				Security_Phrase = emp_view.Security_Phrase,
				Status = emp_view.Status,
				Team = emp_view.Team
			};
			return emp;
		}
	}

	public class EmployeeEditViewModel {

		public string Username { get; set; }

		public int Employee_ID { get; set; }

		[DataType(DataType.EmailAddress)]
		[Required(ErrorMessage = "Please fill-in valid email address")]
		[RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "This email address is not valid")]
		public string Email { get; set; }

		[StringLength(20, ErrorMessage = "Full name must not exceed 20 characters")]
		public string Full_Name { get; set; }

		[DataType(DataType.Password)]
		[StringLength(15, ErrorMessage = "Password must be 8-15 characters, and include letters and numbers")]
		[MinLength(8, ErrorMessage = "Password must be 8-15 characters, and include letters and numbers")]
		[MaxLength(15, ErrorMessage = "Password must be 8-15 characters, and include letters and numbers")]
		[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,15}$", ErrorMessage = "Password must include letters and numbers")]
		public string Password { get; set; }

		[NotMapped]
		[DataType(DataType.Password)]
		[System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords did not match")]
		public string Confirm_Password { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[Required(ErrorMessage = "Please fill-in join date")]
		public System.DateTime Join_Date { get; set; }

		[Required(ErrorMessage = "Please select a position")]
		public int Position { get; set; }

		[Required(ErrorMessage = "Please select a team.")]
		public int Team { get; set; }

		[Required(AllowEmptyStrings = true)]
		[StringLength(40, ErrorMessage = "Security Pharase must not exceed 40 characters")]
		public string Security_Phrase { get; set; }
		public int Status { get; set; }

	}

	public class EmployeeCreateViewModel {
		[Required(ErrorMessage = "Please fill-in username")]
		[StringLength(15, ErrorMessage = "Username must be 8-15 characters, and include letters and numbers")]
		[MinLength(8)]
		[MaxLength(15)]
		[RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username must be alphanumeric")]
		[Remote("IsUsernameAvailable", "Employees", HttpMethod = "POST",ErrorMessage ="Username already exists")]
		public string Username { get; set; }

		[Range(1, 10, ErrorMessage = "Employee ID must be within 10 numbers")]
		[Required(ErrorMessage = "Please fill-in employee ID")]
		[Remote("IsIDAvailable", "Employees", HttpMethod = "POST", ErrorMessage = "Employee ID already exists")]
		public int Employee_ID { get; set; }

		[DataType(DataType.EmailAddress)]
		[Required(ErrorMessage = "Please fill-in valid email address")]
		[RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "This email address is not valid")]
		public string Email { get; set; }

		[StringLength(20, ErrorMessage = "Full name must not exceed 20 characters")]
		public string Full_Name { get; set; }

		[DataType(DataType.Password)]
		[StringLength(15, ErrorMessage = "Password must be 8-15 characters, and include letters and numbers")]
		[MinLength(8, ErrorMessage = "Password must be 8-15 characters, and include letters and numbers")]
		[MaxLength(15, ErrorMessage = "Password must be 8-15 characters, and include letters and numbers")]
		[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,15}$", ErrorMessage = "Password must include letters and numbers")]
		[Required(ErrorMessage = "Please fill-in password")]
		public string Password { get; set; }

		[NotMapped]
		[Required]
		[DataType(DataType.Password)]
		[System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords did not match")]
		public string Confirm_Password { get; set; }

		[DataType(DataType.Date)]
		[Required(ErrorMessage = "Please fill-in join date")]
		public System.DateTime Join_Date { get; set; }

		[Required(ErrorMessage = "Please select a position")]
		public int Position { get; set; }

		[Required(ErrorMessage = "Please select a team.")]
		public int Team { get; set; }

		[Required(AllowEmptyStrings = true)]
		[StringLength(40, ErrorMessage = "Security Pharase must not exceed 40 characters")]
		public string Security_Phrase { get; set; }
		public int Status { get; set; }
	}

	public class EmployeeLoginViewModel {

		[Required(ErrorMessage = "Please fill-in username")]
		public string Username { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Please fill-in password")]
		public string Password { get; set; }
	}

}