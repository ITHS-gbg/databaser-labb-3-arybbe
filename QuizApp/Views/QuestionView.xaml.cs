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

        public QuestionView()
        {
            InitializeComponent();
            _quizRepository = new QuizRepository();
        }

        private void AddQuizBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var mainViewModel = (MainViewModel)DataContext;
            if (mainViewModel.NewQuiz is null || string.IsNullOrWhiteSpace(mainViewModel.NewQuiz.Name))
            {
                MessageBox.Show("Please enter a quiz name");
                return;
            }

            try
            {
                if (mainViewModel.SelectedQuiz is not null)
                {
                    if (_quizRepository.QuizNameExists(mainViewModel.NewQuiz.Name, mainViewModel.SelectedQuiz.Id))
                    {
                        MessageBox.Show("A quiz with this name already exists. Please choose a different name.");
                        return;
                    }

                    var updatedQuiz = new QuizRecord(
                        mainViewModel.SelectedQuiz.Id,
                        mainViewModel.NewQuiz.Name,
                        mainViewModel.NewQuiz.Description,
                        mainViewModel.NewQuiz.Questions
                    );

                    _quizRepository.UpdateQuiz(updatedQuiz);
                }
                else
                {
                    if (_quizRepository.QuizNameExists(mainViewModel.NewQuiz.Name))
                    {
                        MessageBox.Show("A quiz with this name already exists. Please choose a different name.");
                        return;
                    }

                    _quizRepository.AddQuiz(mainViewModel.NewQuiz);
                }

                mainViewModel.ReloadQuizzes();
                mainViewModel.NewQuiz = new QuizRecord("", "", "", []);
                mainViewModel.SelectedQuiz = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void RemoveQuestionBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var mainViewModel = (MainViewModel)DataContext;
            if (mainViewModel.SelectedQuestion == null)
            {
                return;
            }

            _quizRepository.DeleteQuestion(mainViewModel.SelectedQuestion.Id);
            mainViewModel.ReloadQuestions();

            if (mainViewModel.SelectedQuiz != null)
            {
                mainViewModel.ReloadQuestionsForQuiz();
            }
        }

        private void AddQuestionToQuizBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var mainViewModel = (MainViewModel)DataContext;
            if (mainViewModel.SelectedQuiz == null)
            {
                MessageBox.Show("Please select a quiz before adding a question.");
                return;
            }
            if (mainViewModel.SelectedQuestion == null)
            {
                MessageBox.Show("Please select a question to add.");
                return;
            }
            _quizRepository.AddQuestionToQuiz(mainViewModel.SelectedQuiz.Id, mainViewModel.SelectedQuestion.Id);
            mainViewModel.ReloadQuestionsForQuiz();
        }

        private void AddQuestionBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var mainViewModel = (MainViewModel)DataContext;
            if (mainViewModel.NewQuestion is null)
            {
                MessageBox.Show("Please enter a question text");
                return;
            }

            if (string.IsNullOrWhiteSpace(mainViewModel.NewQuestion.Text))
            {
                MessageBox.Show("Please enter a question text");
                return;
            }

            try
            {
                if (mainViewModel.SelectedQuestion is not null)
                {
                    if (_quizRepository.QuestionTextExists(mainViewModel.NewQuestion.Text, mainViewModel.SelectedQuestion.Id))
                    {
                        MessageBox.Show("A question with this text already exists. Please use different text.");
                        return;
                    }

                    var updatedQuestion = new QuestionRecord(
                        mainViewModel.SelectedQuestion.Id,
                        mainViewModel.NewQuestion.Text,
                        mainViewModel.NewQuestion.Options,
                        mainViewModel.CorrectOptionIndex.ToString()
                    );

                    _quizRepository.UpdateQuestion(updatedQuestion);
                    mainViewModel.ReloadQuestions();
                }
                else
                {
                    if (_quizRepository.QuestionTextExists(mainViewModel.NewQuestion.Text))
                    {
                        MessageBox.Show("A question with this text already exists. Please use different text.");
                        return;
                    }

                    mainViewModel.NewQuestion.CorrectOption = mainViewModel.CorrectOptionIndex.ToString();
                    _quizRepository.AddQuestion(mainViewModel.NewQuestion);
                    mainViewModel.ReloadQuestions();
                }

                mainViewModel.NewQuestion = new QuestionRecord("", "", [], "0");
                mainViewModel.SelectedQuestion = null;
                mainViewModel.CorrectOptionIndex = -1; 

                mainViewModel.OnPropertyChanged(nameof(mainViewModel.OptionOne));
                mainViewModel.OnPropertyChanged(nameof(mainViewModel.OptionTwo));
                mainViewModel.OnPropertyChanged(nameof(mainViewModel.OptionThree));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void NewQuestionBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var mainViewModel = (MainViewModel)DataContext;

            mainViewModel.NewQuestion = new QuestionRecord("", "", [], "0");
            mainViewModel.SelectedQuestion = null;
            mainViewModel.CorrectOptionIndex = -1; 

            mainViewModel.OnPropertyChanged(nameof(mainViewModel.OptionOne));
            mainViewModel.OnPropertyChanged(nameof(mainViewModel.OptionTwo));
            mainViewModel.OnPropertyChanged(nameof(mainViewModel.OptionThree));
        }
    }
}
