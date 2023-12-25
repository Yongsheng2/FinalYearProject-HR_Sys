namespace FinalYearProject.Models.ViewModels
{
    public class RatingListVM
    {
        public IEnumerable<EmployeeRatingVM> employeesRating { get; set; }

        public EmployeeRatingVM employerRating { get; set; }
    }
}
