using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.DependencyRequestModel
{
    public class UpdateDependencyInTaskRequestModel
    {
       
        public int? DependencyId { get; set; }

        public int? TaskDependId { get; set; }

        [Required]
        public int TaskDependencyId { get; set; }

        public bool IsDeleted { get; set; }
       
    }
}
