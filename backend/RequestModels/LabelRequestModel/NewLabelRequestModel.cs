using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.LabelRequestModel
{
    public class NewLabelRequestModel
    {
       
        [Required]
        public string Name { get; set; }
       
        public string Color { get; set; }
        [Required]
        public int ProjectId { get; set; }
    }
}
