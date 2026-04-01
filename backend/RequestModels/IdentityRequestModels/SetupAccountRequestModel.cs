using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.IdentityRequestModels
{
    public class SetupAccountRequestModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Role { get; set; }

        public string AvatarUrl { get; set; }

        public string Description { get; set; }
    }
}
