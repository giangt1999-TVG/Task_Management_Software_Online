using System.ComponentModel.DataAnnotations;

namespace tms_api.RequestModels
{
    public class ImportUserRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FullName { get; set; }

        public string RollNumber { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
