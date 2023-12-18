using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinalYearProject.Models
{
    public class Chatbox
    {
        [Key]
        [Display(Name = "Chat ID")]
        public string? chat_id { get; set; }

        [Display(Name = "Staff ID")]
        public string? staff_id { get; set; }

        [ForeignKey("staff_id")]
        public virtual EmployeeDetails? EmployeeDetails { get; set; }

        [Display(Name = "Admin ID")]
        public string? admin_id { get; set; }

        [ForeignKey("admin_id")]
        public virtual Admin? Admin { get; set; }

        [Display(Name = "Sender ID")]
        public string send_id { get; set; }

        [Display(Name = "Sender")]
        public string? send_name { get; set; }

        [Display(Name = "Receiver ID")]
        public string receive_id { get; set; }

        [Display(Name = "Receiver")]
        public string? receive_name { get; set; }

        [Required]
        [Display(Name = "Chat content")]
        public string? chat_ctn { get; set; }

        [Display(Name = "Timestamp")]
        public DateTime timestamp { get; set; }

        public string? send_userid { get; set; }
    }
}

