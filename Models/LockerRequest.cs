using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockerManagementSystem.Models
{
    public class LockerRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID Number is required")]
        public string IdNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Locker Number is required")]
        public string LockerNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Semester is required")]
        public string Semester { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact Number is required")]
        public string ContactNumber { get; set; } = string.Empty;

        [NotMapped]
        public IFormFile? StudyLoadDocument { get; set; }

        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public long? FileSize { get; set; }

        [Required(ErrorMessage = "You must accept the Terms of Service")]
        public bool AcceptTerms { get; set; }

        public string? Approver { get; set; }
        public string? ApproverTitle { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime SubmissionDate { get; set; } = DateTime.Now;
    }
}