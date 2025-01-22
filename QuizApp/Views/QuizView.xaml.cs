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

        public QuizView()
        {
            InitializeComponent();
            _quizRepository = new QuizRepository();
        }

        private void ClearSelectionBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var mainViewModel = (MainViewModel)DataContext;
            mainViewModel.SelectedQuiz = null;
            mainViewModel.NewQuiz = new QuizRecord("", "", "", []);
            QuizSelector.SelectedItem = null;
        }

        private void RemoveQuizBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var mainViewModel = (MainViewModel)DataContext;
            if (mainViewModel.SelectedQuiz is null)
            {
                return;
            }

            string quizId = mainViewModel.SelectedQuiz.Id;
            mainViewModel.SelectedQuiz = null;
            _quizRepository.DeleteQuiz(quizId);
            mainViewModel.ReloadQuizzes();
        }

        private void RemoveQuestionFromQuizBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var mainViewModel = (MainViewModel)DataContext;
            if (mainViewModel.SelectedQuiz is null || mainViewModel.SelectedQuestionOfQuiz is null)
            {
                return;
            }
            _quizRepository.RemoveQuestionFromQuiz(mainViewModel.SelectedQuiz.Id, mainViewModel.SelectedQuestionOfQuiz.Id);
            mainViewModel.ReloadQuestionsForQuiz();
        }
    }
}
