namespace FinalYearProject.Models.ViewModels
{
    public class CreateEmployeeVM
    {
        public string? username { get; set; }
        public string? password { get; set; }
        public string? repassword { get; set; }
        public EmployeeDetails? employeeDetails { get; set; }
    }
}
