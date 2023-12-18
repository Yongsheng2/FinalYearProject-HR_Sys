using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class SurveyQuestion
    {
        [Key, Column(Order = 0)] // Specify the order for the composite key
        public string? survey_id { get; set; }

        [Key, Column(Order = 1)] // Specify the order for the composite key
        public string? question_id { get; set; }

        [ForeignKey("survey_id")]
        public virtual Survey survey { get; set; }

        [ForeignKey("question_id")]
        public virtual Question Question { get; set; }
    }
}
