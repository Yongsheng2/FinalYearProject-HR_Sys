using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Models
{
    public class EmployeeTask
    {
        [Key]
        [Display(Name = "Task ID")]
        public string? emtask_id { get; set; }

        [Required]
        [Display(Name = "Task Title")]
        public string? emtask_title { get; set; } = ""; 

        [Required]
        [Display(Name = "Admin")]
        public string? current_admin { get; set; }

        [ForeignKey("current_admin")]
        public virtual Admin? Admin { get; set; }

        [Required]
        [Display(Name = "Staff ID")]
        public string? staff_id { get; set; }

        [ForeignKey("staff_id")]
        public virtual EmployeeDetails? EmployeeDetails { get; set; }

        [Display(Name = "Status")]
        public string? progress_status { get; set; } = "In Progress";

        [Required]
        [Display(Name = "Upload Date")]
        public DateTime date_upload { get; set; }

        [Display(Name = "Duration (Days)")]
        public int? emtask_duration { get; set; }

        [Display(Name = "Task Detail")]
        public string? emtaskDetail { get; set; }

        [Display(Name = "Task File")]
        public string? emtaskdoneFile { get; set; }

    }
}
