using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class Question
    {
        [Key]
        public string? question_id { get; set; }

        [Required]
        public string? question { get; set; }

        [Required]
        public string? questionType_id { get; set; }
        [ForeignKey("questionType_id")]
        public virtual QuestionType? questionType { get; set; }


    }
}
