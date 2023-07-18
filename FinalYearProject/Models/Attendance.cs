using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class Attendance
    {
        [Key]
        public string? attendance_id { get; set; }

        [Required]
        public string? staff_id { get; set; }

        [ForeignKey("staff_id")]
        public virtual EmployeeDetails? EmployeeDetails { get; set; }

        [Required]
        public string? shift_id { get; set; }

        [ForeignKey("shift_id")]
        public virtual Shift? Shift { get; set; }

        [Required]
        public DateTime? start_time { get; set; }

        public DateTime? end_time { get; set; }

        public bool validity { get; set; }

        public bool checkInValid { get; set; }

        public bool checkOutValid { get; set; }

        public bool on_leave { get; set;}

        public string? leave_id { get; set; }

        [ForeignKey("leave_id")]
        public virtual Leave? Leave { get; set; }
    }
}
