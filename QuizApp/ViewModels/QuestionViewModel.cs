using System.Collections.ObjectModel;
using Common.DTOs;
using DataAccess.Services;

namespace QuizApp.ViewModels;

public class QuestionViewModel : BaseViewModel
{
    private readonly QuizRepository _quizRepository;
    private ObservableCollection<QuestionRecord> _questions;
    private QuestionRecord _selectedQuestion;
    private int _correctOptionIndex;

    public int CorrectOptionIndex
    {
        get => _correctOptionIndex;
        set
        {
            if (_correctOptionIndex != value)
            {
                _correctOptionIndex = value;
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
        }
    }

    public QuizRecord NewQuiz { get; set; }
    public QuestionRecord NewQuestion { get; set; }

    public QuestionViewModel()
    {
        _quizRepository = new QuizRepository();
        LoadQuestions();
    }

    private void LoadQuestions()
    {
        Questions = new ObservableCollection<QuestionRecord>(_quizRepository.GetAllQuestions());
    }

}