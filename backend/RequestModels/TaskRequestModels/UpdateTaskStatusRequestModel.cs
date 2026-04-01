using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.TaskRequestModels
{
    public class UpdateTaskStatusRequestModel
    {
        [Required]
        public int TaskId { get; set; }

        [Required]
        public int StatusId { get; set; }
    }
}
