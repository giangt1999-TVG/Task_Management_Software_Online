using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.ProjectRequestModels
{
    public class AddMemberRequestModel
    {
        [Required]
        public int ProjectId { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
