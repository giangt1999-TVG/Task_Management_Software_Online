using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.Models
{
    public class UserRole : IdentityUserRole<string>
    {
        public int? ProjectId { get; set; }
    }
}
