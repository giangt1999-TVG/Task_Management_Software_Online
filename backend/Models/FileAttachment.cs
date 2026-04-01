using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.Models
{
    public partial class FileAttachment
    {
        public int FileAttachmentId { get; set; }
        public string Name { get; set; }
        public string MediaLink { get; set; }
        public int TaskId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Tasks Task { get; set; }
    }
}
