using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class Incentives
    {
        [Key]
        public string? incentives_id { get; set; }

        public string? company_id { get; set; }
        [ForeignKey("company_id")]
        public virtual Company? Company { get; set; }

        public decimal? good_rating_incentives { get; set; }
        
        public decimal? attendance_Excellence_Award { get; set;}

    }
}
