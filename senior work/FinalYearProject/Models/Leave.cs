using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class Leave
    {
        [Key]
        [Display(Name = "Leave ID")]
        public string? leave_id { get; set; }

        [Required]
        [Display(Name = "Staff ID")]
        public string? staff_id { get; set; }

        [ForeignKey("staff_id")]
        public virtual EmployeeDetails? EmployeeDetails { get; set; }

        [Display(Name = "Status")]
        public string? approval_status { get; set; }

        [Display(Name = "Decided By")]
        public string? approved_by { get; set; }

        [ForeignKey("approved_by")]
        public virtual EmployeeDetails? ApprovedByEmployee { get; set; }

        [Required]
        [Display(Name = "Date Created")]
        public DateTime date_created { get; set; }

        [Required]
        [Display(Name = "Leave Start Time")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime leave_start { get; set; }

        [Required]
        [Display(Name = "Leave End Time")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime leave_end { get; set; }

        [Display(Name = "Reason for Leave")]
        public string? leave_reason { get; set; }

        [Display(Name = "Response")]
        public string? response_message { get; set; }

        [Display(Name = "Leave Document")]
        public string? doc_filepath { get; set; }

        [Display(Name = "Leave Type")]
        public string? leaveType { get; set; }
    }
}
