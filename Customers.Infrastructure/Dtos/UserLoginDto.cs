using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Infrastructure.Dtos
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "This field is required")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string? Password { get; set; }
    }

    public class OTPDto
    {
        public string? OTP { get; set; }
    }

    public class ForgotPasswordModel
    {
        public string UserName { get; set; }
    }
}
