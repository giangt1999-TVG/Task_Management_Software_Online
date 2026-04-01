using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tms_api.RequestModels.DependencyRequestModel
{
    public class NewDependencyRequestModel
    {
      
        [Required]
        public int TaskId { get; set; }
        [Required]
        public int TaskDependId { get; set; }
        [Required]
        public int DependencyId { get; set; }
    }
}
