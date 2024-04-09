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
        private readonly QuestionViewModel _questionViewModel;

        public QuizView()
        {
            InitializeComponent();
            _quizRepository = new QuizRepository();
            _quizViewModel = new QuizViewModel();
            _questionViewModel = new QuestionViewModel();
            DataContext = _quizViewModel;
        }

        


        private void RemoveQuizBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (_quizViewModel.SelectedQuiz is null)
            {
                return;
            }
            _quizRepository.DeleteQuiz(_quizViewModel.SelectedQuiz.Id);
            _quizViewModel.SelectedQuiz = null;
            _quizViewModel.ReloadQuizzes();
        }

        private void RemoveQuestionFromQuizBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _quizRepository.RemoveQuestionFromQuiz(_quizViewModel.SelectedQuiz.Id, _quizViewModel.SelectedQuestion.Id);
            _quizViewModel.ReloadQuestionsForQuiz();
        }
    }
}
