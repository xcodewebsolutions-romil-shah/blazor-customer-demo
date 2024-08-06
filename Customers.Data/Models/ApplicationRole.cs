using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Customers.Data.Models
{
    public partial class ApplicationRole : IdentityRole<int>
    {
        [JsonIgnore]
        public ICollection<ApplicationUser> Users { get; set; }

    }
}