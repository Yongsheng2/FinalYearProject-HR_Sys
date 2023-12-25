using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class EmployeeDetails
    {
        [Display(Name = "Employee ID")]
        [Key]
        public string? employee_id { get; set; }

        [Display(Name = "Employee Company ID")]
        public string? employee_id_by_company { get; set; }

        [Display(Name = "Name")]
        [Required]
        public string? employee_name { get; set; }

        [Display(Name = "Username")]
        [Required]
        public string? user_id { get; set; }

        [Display(Name = "Company")]
        [Required]
        public string? parent_company { get; set; }

        [ForeignKey("parent_company")]
        public virtual Company? Company { get; set; }

        [Display(Name = "Role")]
        public string? staff_role { get; set; }

        [ForeignKey("staff_role")]
        public virtual Role? Role { get; set; }

        [Display(Name = "Password")]
        public string? acc_pass { get; set; }

        [Display(Name = "Employer")]
        public string? employer_id { get; set; }

        [ForeignKey("employer_id")]
        public virtual EmployeeDetails? Employer { get; set; }

        [Display(Name = "Employment Start Date")]
        public DateTime? employment_start_date { get; set; }

        public string? types_of_wages { get; set; }

        [Display(Name = "Wages Rate")]
        public float wages_rate { get; set; }

        public bool employement_letter { get; set; }

        public float monthly_deduction { get; set; }

        //Not in UML
        [Display(Name = "I.C. No.")]
        public string? ic_no { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime? dob { get; set; }

        [Display(Name = "Gender")]
        public string? gender { get; set; }

        [Display(Name = "Nationality")]
        public string? nationality { get; set; }

        [Display(Name = "Phone Number")]
        public string? phone_no { get; set; }

        [Display(Name = "Email Address")]
        public string? email { get; set; }

        [Display(Name = "EPF No.")]
        public string? epf_no { get; set; }

        [Display(Name = "Sosco No.")]
        public string? sosco_no { get; set; }

        [Display(Name = "I-Tax No.")]
        public string? itax_no { get; set; }

        [Display(Name = "Bank")]
        public string? bank_name { get; set; }

        [Display(Name = "Bank Acc. No.")]
        public string? bank_no { get; set; }

        [Display(Name = "ASP User ID")]
        public string? asp_id { get; set; }

        //Media
        [Display(Name = "Profile Picture")]
        public string? profileImg_path { get; set; }

        //Database Control
        [Display(Name = "Status")]
        public bool? is_active { get; set; }

        [Display(Name = "Religion")]
        public string? religion { get; set; }

        [Display(Name = "Sick Leave Left")]
        [Required]
        public int? sickLeaveHourLeft { get; set; }

        [Display(Name = "Paid Leave Left")]
        [Required]
        public int? paidLeaveHourLeft { get; set; }

        [Display(Name = "Sick Leave Applied")]
        [Required]
        public int? sickLeaveOnBargain { get; set; }

        [Display(Name = "Paid Leave Applied")]
        [Required]
        public int? paidLeaveOnBargain { get; set; }

        [Display(Name = "Device Binded")]
        public string? uuid { get; set; }

        [Display(Name = "Leave Last Updated")]
        [Required]
        public DateTime leaveUpdate { get; set; }
    }
}
