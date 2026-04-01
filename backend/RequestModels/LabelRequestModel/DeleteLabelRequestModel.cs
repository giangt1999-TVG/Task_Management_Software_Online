using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.LabelRequestModel
{
    public class DeleteLabelRequestModel
    {
        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int LabelId { get; set; }
    }
}
