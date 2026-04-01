using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.UserRequestModel
{
    public class UpdateAccountRequestModel
    {
        [Required]
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Usename { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string RollNumber { get; set; }
    }
}
