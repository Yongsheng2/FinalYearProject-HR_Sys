using NuGet.Versioning;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class PayBack
    {
        [Key]
        public string? payback_id { get; set; }
        
        public string? advance_id { get; set; }

        [ForeignKey("advance_id")]
        public virtual SalaryAdvance? SalaryAdvance { get; set; }

        public float? payback_amount { get; set; }

        public string? status { get; set; }


    }
}
