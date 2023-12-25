namespace FinalYearProject.Models.ViewModels
{
    public class EmployeeRatingVM
    {
        public EmployeeDetails employee { get; set; }

        public IEnumerable<Rating> rating { get; set; }

        public float averageRate { get; set; }

        public int count { get; set; }
    }
}
