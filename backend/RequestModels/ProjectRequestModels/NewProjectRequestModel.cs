using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tms_api.RequestModels.ProjectRequestModels
{
    public class NewProjectRequestModel
    {
        [Required]
        public string ProjectCode { get; set; }

        [Required]
        public string ProjectName { get; set; }

        public string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public string TeacherId { get; set; }

        [Required]
        public string TeamleadId { get; set; }

        public List<string> MemberIds { get; set; }

    }
}
