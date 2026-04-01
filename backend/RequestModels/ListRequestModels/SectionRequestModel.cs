using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.ListRequestModels
{
    public class UpdateListRequestModel
    {
        [Required]
        public int Id { get; set; }
        
        public int? Index { get; set; }
        
        public string Name { get; set; }
    }
}
