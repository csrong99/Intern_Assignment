using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCrypt.Net;

namespace Assignment.Models {
    public class Hashing {

        static private string GenerateSalt() {
            return BCrypt.Net.BCrypt.GenerateSalt(12);
        }
        static public string HashPassword(string password) {
            // adding some salt to the password
            return BCrypt.Net.BCrypt.HashPassword(password, GenerateSalt());
        }

        static public bool ValidatePassword(string password, string hashed_password) {
            return BCrypt.Net.BCrypt.Verify(password, hashed_password);
        }
    }
}