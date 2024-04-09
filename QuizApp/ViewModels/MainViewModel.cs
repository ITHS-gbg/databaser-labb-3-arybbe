namespace QuizApp.ViewModels;

public class MainViewModel(QuizViewModel quizViewModel, QuestionViewModel questionViewModel)
{
    public QuizViewModel QuizViewModel { get; } = quizViewModel;
    public QuestionViewModel QuestionViewModel { get; } = questionViewModel;
}