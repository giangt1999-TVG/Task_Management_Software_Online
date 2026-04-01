using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.TaskRequestModels
{
    public class UpdateDuedateRequestModel
    {
        [Required]
        public int TaskId { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
    }

    public class UpdateStartdateRequestModel
    {
        [Required]
        public int TaskId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
    }

}
