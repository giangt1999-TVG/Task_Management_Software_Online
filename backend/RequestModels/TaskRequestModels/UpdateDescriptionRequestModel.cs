using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.TaskRequestModels
{
    public class UpdateDescriptionRequestModel
    {
        [Required]
        public int TaskId { get; set; }

        public string Description { get; set; }
    }
}
