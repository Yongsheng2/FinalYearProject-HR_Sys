namespace FinalYearProject.Models.ViewModels
{
    public class EmployeeClaimListVM
    {
        public EmployeeDetails owner { get; set; }

        public IEnumerable<EmployeeClaim> claimApplied { get; set; }

        public EmployeeClaim displayClaim { get; set; } = new EmployeeClaim();
    }
}
