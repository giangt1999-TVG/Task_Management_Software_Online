using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.CommentRequestModels
{
    public class NewCommentRequestModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int TaskId { get; set; }

        [Required]
        public string Content { get; set; }

        public string AttachFile { get; set; }
    }
}
