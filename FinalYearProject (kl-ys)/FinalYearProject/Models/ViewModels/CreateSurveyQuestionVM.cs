namespace FinalYearProject.Models.ViewModels
{
    public class CreateSurveyQuestionVM
    {

        public Question question { get; set; }

        public string survey_id { get; set; }
        public List<Question> questionSet { get; set; }

        public string company_id { get; set; }
        public string survey_name { get; set; }
    }
}
