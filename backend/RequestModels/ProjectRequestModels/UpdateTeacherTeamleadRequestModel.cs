using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.ProjectRequestModels
{
    public class UpdateTeacherTeamleadRequestModel
    {
        [Required]
        public int ProjectId { get; set; }

        [Required]
        public string OldUserId { get; set; }

        [Required]
        public string NewUserId { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
