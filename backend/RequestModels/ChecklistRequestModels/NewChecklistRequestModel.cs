using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.ChecklistRequestModels
{
    public class NewChecklistRequestModel
    {
        [Required]
        public string Name { get; set; }

        public bool IsCompleted { get; set; } = false;

        [Required]
        public int TaskId { get; set; }
    }
}
