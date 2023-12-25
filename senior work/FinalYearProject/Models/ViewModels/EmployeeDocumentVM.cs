namespace FinalYearProject.Models.ViewModels
{
    public class EmployeeDocumentVM
    {
        public string? employee_id { get; set; }
        public Document document { get; set; } = new Document();
    }
}
