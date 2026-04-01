using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.ChecklistRequestModels
{
    public class UpdateChecklistRequestModel
    {
        [Required]
        public int ChecklistId { get; set; }

        public string Name { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsDeleted { get; set; }
    }
}
