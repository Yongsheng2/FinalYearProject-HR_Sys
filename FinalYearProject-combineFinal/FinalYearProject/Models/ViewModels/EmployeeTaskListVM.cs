namespace FinalYearProject.Models.ViewModels
{
    public class EmployeeTaskListVM
    {
        public Admin owner { get; set; }
        public EmployeeDetails owners { get; set; }
        public IEnumerable<EmployeeTask> TaskCreated { get; set; }
        public EmployeeTask displayEmployeeTask { get; set; } = new EmployeeTask();
    }
}
