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

            _quizRepository.AddQuestion(new QuestionRecord("", "h", new []{"dsa", "wa", "ada"}, "0"));
        }

        private void AddQuizBtn_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
