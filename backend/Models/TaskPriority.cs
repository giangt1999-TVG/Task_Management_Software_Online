using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.Models
{
    public partial class TaskPriority
    {
        public TaskPriority()
        {
            Tasks = new HashSet<Tasks>();
        }

        public int TaskPriorityId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Tasks> Tasks { get; set; }
    }
}
