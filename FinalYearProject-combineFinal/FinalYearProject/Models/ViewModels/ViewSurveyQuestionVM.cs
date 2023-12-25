namespace FinalYearProject.Models.ViewModels
{
    public class ViewSurveyQuestionVM
    {

        public Question question { get; set; }

        public string survey_id { get; set; }
        public List<Question> questionSet { get; set; }

        public List<Question> questionSetType { get; set; }

        public string survey_name { get; set; }
    }
}
