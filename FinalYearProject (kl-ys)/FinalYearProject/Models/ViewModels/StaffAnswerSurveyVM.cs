namespace FinalYearProject.Models.ViewModels
{
    public class StaffAnswerSurveyVM
    {
        public string survey_id { get; set; }

        public string surveyTaken_id { get; set; }
        public Question question { get; set; }
        public List<Question> questionSet { get; set; }

        public List<Answer> answersSet { get; set; }

        public decimal total_score { get; set; }

        public List<Question> questionSetType { get; set; }

        public string survey_name { get; set; }


    }
}
