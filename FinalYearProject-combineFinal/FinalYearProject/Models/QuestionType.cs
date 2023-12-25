using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class QuestionType
    {
      [Key]
      public string? questionType_id { get; set; }

      public string? questionType_name { get; set; }
    }
}
