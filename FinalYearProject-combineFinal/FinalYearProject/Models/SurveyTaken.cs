using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class SurveyTaken
    {
        [Display(Name = "SurveyTaken ID")]
        [Key]
        public string? surveyTaken_id { get; set; }


        public string? employee_id { get; set; }
        [ForeignKey("employee_id")]
        public virtual EmployeeDetails? EmployeeDetails { get; set; }


        public string? survey_id { get; set; }
        [ForeignKey("survey_id")]
        public virtual Survey? Survey { get; set; }

        public decimal? total_score { get; set; }

        public bool? Statisfy_the_Company { get; set; }

    }
}
