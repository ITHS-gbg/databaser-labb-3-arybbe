using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common.DTOs;
using DataAccess.Services;
using QuizApp.ViewModels;

namespace QuizApp.Views
{
    /// <summary>
    /// Interaction logic for QuestionView.xaml
    /// </summary>
    public partial class QuestionView : UserControl
    {
        private readonly QuizRepository _quizRepository;
        private readonly QuestionViewModel _questionViewModel;
        private readonly QuizViewModel _quizViewModel;


        public QuestionView()
        {
            InitializeComponent();
            _quizRepository = new QuizRepository();
            _questionViewModel = new QuestionViewModel();
            _quizViewModel = new QuizViewModel();
            DataContext = _questionViewModel;
        }

        private void ReloadQuestionsForQuiz()
        {
            _quizViewModel.CurrentQuizQuestions.Clear();
            var questions = _quizRepository.GetQuestionsForQuiz(_quizViewModel.SelectedQuiz.Id);
            foreach (var question in questions)
            {
                _quizViewModel.CurrentQuizQuestions.Add(question);
            }
        }

        private void ReloadQuizzes()
        {
            _quizViewModel.Quizzes.Clear();
            var quizzes = _quizRepository.GetAllQuizzes();
            foreach (var quiz in quizzes)
            {
                _quizViewModel.Quizzes.Add(quiz);
            }
        }

        private void ReloadQuestions()
        {
            _questionViewModel.Questions.Clear();
            var questions = _quizRepository.GetAllQuestions();
            foreach (var question in questions)
            {
                _questionViewModel.Questions.Add(question);
            }
        }

        private void AddQuizBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (_questionViewModel.NewQuiz is null)
            {
                MessageBox.Show("Please enter a quiz name");
                return;
            }

            if (_quizRepository.GetQuizByName(_questionViewModel.NewQuiz.Name) is not null)
            {
                MessageBox.Show("Quiz with that Name already exists");
                return;
            }
            _quizRepository.AddQuiz(_questionViewModel.NewQuiz);
            _quizViewModel.ReloadQuizzes();
            _questionViewModel.NewQuiz = new QuizRecord("", "", "", []);
        }

        private void RemoveQuestionBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _quizRepository.DeleteQuestion(_questionViewModel.SelectedQuestion.Id);
            _questionViewModel.ReloadQuestions();
        }

        private void AddQuestionToQuizBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _quizRepository.AddQuestionToQuiz(_quizViewModel.SelectedQuiz.Id, _questionViewModel.SelectedQuestion.Id);
            ReloadQuestionsForQuiz();
        }

        private void AddQuestionBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (_questionViewModel.NewQuestion is null)
            {
                MessageBox.Show("Please enter a question");
                return;
            }

            if (_quizRepository.GetQuestionByText(_questionViewModel.NewQuestion.Text) is not null)
            {
                MessageBox.Show("Question with that Text already exists");
                return;
            }

            var updatedQuestion = _questionViewModel.NewQuestion with
            {
                CorrectOptionIndex = _questionViewModel.CorrectOptionIndex.ToString()
            };
            _quizRepository.AddQuestion(updatedQuestion);

            _questionViewModel.ReloadQuestions();
            _questionViewModel.NewQuestion = new QuestionRecord("", "", [], "");
        }
    }
}
