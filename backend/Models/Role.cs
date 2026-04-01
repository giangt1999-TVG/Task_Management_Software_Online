using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.Models
{
    public class Role : IdentityRole
    {
        public string ParentId { get; set; }
    }
}
