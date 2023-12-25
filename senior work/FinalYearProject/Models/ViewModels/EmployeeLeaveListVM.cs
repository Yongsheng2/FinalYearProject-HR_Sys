namespace FinalYearProject.Models.ViewModels
{
    public class EmployeeLeaveListVM
    {
        public EmployeeDetails owner { get; set; }

        public IEnumerable<Leave> leaveApplied { get; set; }

        public Leave displayLeave { get; set; } = new Leave();
    }
}
