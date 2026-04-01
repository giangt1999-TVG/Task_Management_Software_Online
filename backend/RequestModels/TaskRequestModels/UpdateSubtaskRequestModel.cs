using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.TaskRequestModels
{
    public class UpdateSubtaskRequestModel
    {
        public int CurrentSubtaskId { get; set; }
        public int UpdateSubtaskId { get; set; }
        public int TaskId { get; set; }
    }
}
