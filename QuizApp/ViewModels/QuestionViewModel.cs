﻿using System.Collections.ObjectModel;
using Common.DTOs;
using DataAccess.Services;

namespace QuizApp.ViewModels;

public class QuestionViewModel : BaseViewModel
{
    private readonly QuizRepository _quizRepository;
    private ObservableCollection<QuestionRecord> _questions;
    private QuestionRecord _selectedQuestion;
    private int _correctOptionIndex;
    private QuizRecord _newQuiz;
    private QuestionRecord _newQuestion;

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

    public QuestionViewModel()
    {
        _quizRepository = new QuizRepository();
        NewQuestion = new QuestionRecord("", "", [], "");
        NewQuiz = new QuizRecord("", "", "", []);
        LoadQuestions();
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