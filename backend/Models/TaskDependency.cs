using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.Models
{
    public partial class TaskDependency
    {
        public int TaskDependencyId { get; set; }
        public int TaskDependId { get; set; }
        public int DependencyId { get; set; }
        public int TaskId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Tasks Task { get; set; }
        public virtual Dependency Dependency { get; set; }
    }
}
