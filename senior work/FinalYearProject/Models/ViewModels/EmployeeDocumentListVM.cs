namespace FinalYearProject.Models.ViewModels
{
    public class EmployeeDocumentListVM
    {
        public EmployeeDetails owner { get; set; }

        public IEnumerable<Document> documentsOwned { get; set; }

        public Document displayDocument { get; set; } = new Document();
    }
}
