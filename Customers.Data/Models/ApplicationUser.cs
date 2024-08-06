using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Permissions;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Customers.Data.Models
{    
    public partial class ApplicationUser : IdentityUser<int>
    {
        [JsonIgnore]
        public override string PasswordHash { get; set; }        
        public string FirstName { get; set; }        
        public string LastName { get; set; }                
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsOwner { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedById { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? DeletedById { get; set; }
        public DateTime? DeletedOn { get; set; }
        [NotMapped]
        public string Password { get; set; }
        [NotMapped]
        public string ConfirmPassword { get; set; }
        [JsonIgnore, NotMapped]
        public string Name
        {
            get
            {
                return UserName;
            }
            set
            {
                UserName = value;
            }
        }
        //[JsonIgnore]
        public ICollection<ApplicationRole> Roles { get; set; }
    }

    public class UpdateUserModel
    {
        public string Password { get; set; }
        public ICollection<IdentityRole> Roles { get; set; }
    }

    public class AddUserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsOwner { get; set; }
        public int CreatedById { get; set; }
        public int CustomerId { get; set; }
        public bool isSecondary { get; set; }=false;
        //public string Password { get; set; }
        //public ICollection<IdentityRole<int>> Roles { get; set; }
    }

    public class AddlogoModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string logo { get; set; }
    }

    public class ApplicationUserViewModel : ApplicationUser
    {
        public string CreatedBy { get; set; }
    }
}