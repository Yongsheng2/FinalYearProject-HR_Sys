using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Models
{
    public class Answer
    {
        
        [Key]
        public string? answer_id { get; set; }
      
        public string? surveyTaken_id { get; set; }
        public string? question_id { get; set; }
        public int? answer { get; set; }

    }
}
