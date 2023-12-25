using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class Document
    {
        [Key]
        [Display(Name = "Document ID")]
        public string? document_id { get; set; }

        [Required]
        [Display(Name = "Owner ID")]
        public string? owner_id { get; set; }

        [ForeignKey("owner_id")]
        public virtual EmployeeDetails? EmployeeDetails { get; set; }

        [Required]
        [Display(Name = "Document Name")]
        public string? document_name { get; set; }

        [Required]
        [Display(Name = "Date Created")]
        public DateTime date_created { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime expiry_date { get; set; }

        [Display(Name = "Notify Date")]
        public DateTime notify_date { get; set; }

        [Display(Name = "Document File")]
        public string? document_path { get; set; }
    }
}