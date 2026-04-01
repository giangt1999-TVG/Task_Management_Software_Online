using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.Models
{
    public partial class TaskStatuses
    {
        public TaskStatuses()
        {
            Tasks = new HashSet<Tasks>();
        }

        public int TaskStatusId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Tasks> Tasks { get; set; }
    }
}
