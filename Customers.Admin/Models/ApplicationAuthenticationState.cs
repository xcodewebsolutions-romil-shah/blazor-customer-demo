
using Customers.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Customers.Admin.Models
{
    public class ApplicationClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public partial class ApplicationAuthenticationState
    {
        public bool IsAuthenticated { get; set; }
        public string Name { get; set; }
        public IEnumerable<ApplicationClaim> Claims { get; set; }
    }

    public partial class UserClaims
    {
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public string CurrentRole { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerLogo { get; set; }
    }


    public class Details
    {
        public string OTP { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}