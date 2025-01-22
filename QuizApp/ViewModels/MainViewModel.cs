using Common.DTOs;
using DataAccess.Services;
using System.Collections.ObjectModel;

namespace QuizApp.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly QuizRepository _quizRepository;
    private ObservableCollection<QuizRecord> _quizzes = [];
    private ObservableCollection<QuestionRecord> _currentQuizQuestions = [];
    private QuizRecord _selectedQuiz;
    private QuestionRecord _selectedQuestion;
    private ObservableCollection<QuestionRecord> _questions;
    private int _correctOptionIndex;
    private QuizRecord _newQuiz;
    private QuestionRecord _newQuestion;


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
            if (_selectedQuiz is not null)
            {
                NewQuiz = new QuizRecord(_selectedQuiz.Id, _selectedQuiz.Name, _selectedQuiz.Description, _selectedQuiz.Questions);
            }
            else
            {
                NewQuiz = new QuizRecord("", "", "", []);
            }
        }
    }

    public QuestionRecord SelectedQuestionOfQuiz
    {
        get => _selectedQuestion;
        set
        {
            if (Equals(value, _selectedQuestion)) return;
            _selectedQuestion = value;
            OnPropertyChanged();
        }
    }

    public int CorrectOptionIndex
    {
        get => _correctOptionIndex;
        set
        {
            if (_correctOptionIndex != value)
            {
                _correctOptionIndex = value;
                if (NewQuestion != null)
                {
                    NewQuestion.CorrectOption = value.ToString();
                }
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<QuestionRecord> Questions
    {
        get => _questions;
        set
        {
            _questions = value;
            OnPropertyChanged();
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
            if (_selectedQuestion is not null)
            {
                
                var optionsCopy = new List<string>(_selectedQuestion.Options ?? new List<string>());
                NewQuestion = new QuestionRecord(
                    _selectedQuestion.Id,
                    _selectedQuestion.Text,
                    optionsCopy,
                    _selectedQuestion.CorrectOption);
                CorrectOptionIndex = int.Parse(_selectedQuestion.CorrectOption);
                
                OnPropertyChanged(nameof(OptionOne));
                OnPropertyChanged(nameof(OptionTwo));
                OnPropertyChanged(nameof(OptionThree));
            }
            else
            {
                NewQuestion = new QuestionRecord("", "", new List<string>(), "0");
                CorrectOptionIndex = 0;
            }
        }
    }

    public QuizRecord NewQuiz
    {
        get => _newQuiz;
        set
        {
            if (Equals(value, _newQuiz)) return;
            _newQuiz = value;
            OnPropertyChanged();
        }
    }

    public QuestionRecord NewQuestion
    {
        get => _newQuestion;
        set
        {
            if (Equals(value, _newQuestion)) return;
            _newQuestion = value;
            OnPropertyChanged();
        }
    }

    public string OptionOne
    {
        get => NewQuestion?.Options?.Count > 0 ? NewQuestion.Options[0] : "";
        set
        {
            if (NewQuestion.Options == null)
                NewQuestion.Options = new List<string>();
            if (NewQuestion.Options.Count > 0)
                NewQuestion.Options[0] = value;
            else
                NewQuestion.Options.Add(value);
            OnPropertyChanged();
        }
    }

    public string OptionTwo
    {
        get => NewQuestion?.Options?.Count > 1 ? NewQuestion.Options[1] : "";
        set
        {
            if (NewQuestion.Options == null)
                NewQuestion.Options = new List<string>();
            if (NewQuestion.Options.Count > 1)
                NewQuestion.Options[1] = value;
            else
                NewQuestion.Options.Add(value);
            OnPropertyChanged();
        }
    }

    public string OptionThree
    {
        get => NewQuestion?.Options?.Count > 2 ? NewQuestion.Options[2] : "";
        set
        {
            if (NewQuestion.Options == null)
                NewQuestion.Options = new List<string>();
            if (NewQuestion.Options.Count > 2)
                NewQuestion.Options[2] = value;
            else
                NewQuestion.Options.Add(value);
            OnPropertyChanged();
        }
    }


    public MainViewModel()
    {
        _quizRepository = new QuizRepository();
        LoadQuizzes();
        NewQuestion = new QuestionRecord("", "", [], "-1");
        CorrectOptionIndex = -1;
        NewQuiz = new QuizRecord("", "", "", []);
        LoadQuestions();
    }

    private void LoadQuizzes()
    {
        Quizzes = new ObservableCollection<QuizRecord>(_quizRepository.GetAllQuizzes());
    }

    private void LoadQuestionsForQuiz()
    {
        if (SelectedQuiz is null)
        {
            CurrentQuizQuestions.Clear();
            return;
        }
        var questions = _quizRepository.GetQuestionsForQuiz(SelectedQuiz.Id);
        if (questions is null)
        {
            return;
        }

        CurrentQuizQuestions = new ObservableCollection<QuestionRecord>(questions);
    }

    public void ReloadQuizzes()
    {
        Quizzes.Clear();
        var quizzes = _quizRepository.GetAllQuizzes();
        foreach (var quiz in quizzes)
        {
            Quizzes.Add(quiz);
        }

        SelectedQuiz = null;

        NewQuiz = new QuizRecord("", "", "", []);
    }

    public void ReloadQuestionsForQuiz()
    {
        CurrentQuizQuestions.Clear();
        var questions = _quizRepository.GetQuestionsForQuiz(SelectedQuiz.Id);
        foreach (var question in questions)
        {
            CurrentQuizQuestions.Add(question);
        }
    }

    private void LoadQuestions()
    {
        Questions = new ObservableCollection<QuestionRecord>(_quizRepository.GetAllQuestions());
    }

    public void ReloadQuestions()
    {
        Questions.Clear();
        var questions = _quizRepository.GetAllQuestions();
        foreach (var question in questions)
        {
            Questions.Add(question);
        }
    }
}