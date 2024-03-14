using System.Collections.ObjectModel;
using Common.DTOs;
using DataAccess.Services;

namespace QuizApp.ViewModels;

public class QuizViewModel : BaseViewModel
{
    private QuizRepository _quizRepository;
    private ObservableCollection<QuizRecord> _quizzes;
    private ObservableCollection<QuestionRecord> _currentQuizQuestions;
    private QuizRecord _selectedQuiz;
    private QuestionRecord _selectedQuestion;


    public ObservableCollection<QuizRecord> Quizzes
    {
        get => _quizzes;
        set
        {
            _quizzes = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<QuestionRecord> CurrentQuizQuestions
    {
        get => _currentQuizQuestions;
        set
        {
            _currentQuizQuestions = value;
            OnPropertyChanged();
        }
    }

    public QuizRecord SelectedQuiz
    {
        get => _selectedQuiz;
        set
        {
            if (Equals(value, _selectedQuiz)) return;
            _selectedQuiz = value;
            OnPropertyChanged();
            LoadQuestionsForQuiz();
        }
    }

    public QuestionRecord SelectedQuestion
    {
        get => _selectedQuestion;
        set
        {
            if (Equals(value, _selectedQuestion)) return;
            _selectedQuestion = value;
            OnPropertyChanged();
        }
    }


    public QuizViewModel()
    {
        _quizRepository = new QuizRepository();
        LoadQuizzes();
    }

    private void LoadQuizzes()
    {
        Quizzes = new ObservableCollection<QuizRecord>(_quizRepository.GetAllQuizzes());
    }

    private void LoadQuestionsForQuiz()
    {
        var questions = _quizRepository.GetQuestionsForQuiz(SelectedQuiz.Id);
        CurrentQuizQuestions = new ObservableCollection<QuestionRecord>(questions);
    }
}