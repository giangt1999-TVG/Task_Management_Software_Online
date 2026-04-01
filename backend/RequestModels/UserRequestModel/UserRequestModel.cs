using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.UserRequestModel
{
    public class UserRequestModel
    {
        [Required]
        public string Id { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public string Description { get; set; }
        public string RollNumber { get; set; }
        public bool IsDeleted { get; set; }
    }
}
