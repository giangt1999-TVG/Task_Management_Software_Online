using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.TaskRequestModels
{
    public class NewTaskRequestModel
    {
        [Required]
        public int ListId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
