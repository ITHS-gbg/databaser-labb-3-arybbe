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
    /// Interaction logic for QuizView.xaml
    /// </summary>
    public partial class QuizView : UserControl
    {
        private readonly QuizRepository _quizRepository;
        private readonly QuizViewModel _quizViewModel;

        public QuizView()
        {
            InitializeComponent();
            _quizRepository = new QuizRepository();
            _quizViewModel = new QuizViewModel();
            DataContext = _quizViewModel;

            _quizRepository.UpdateQuizList += ReloadQuizzes;
            _quizRepository.UpdateQuestionListForQuiz += ReloadQuestionsForQuiz;
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

        private void RemoveQuizBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _quizRepository.DeleteQuiz(_quizViewModel.SelectedQuiz.Id);
            ReloadQuizzes();
        }

        private void RemoveQuestionFromQuizBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _quizRepository.RemoveQuestionFromQuiz(_quizViewModel.SelectedQuiz.Id, _quizViewModel.SelectedQuestion.Id);
            ReloadQuestionsForQuiz();
        }
    }
}
