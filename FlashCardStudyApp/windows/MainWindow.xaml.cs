using FlashCardStudyApp.windows;
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

namespace FlashCardStudyApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();

         
        }

        private void CreateFlashcardsSetButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            createFlashCardWindow buildSetWindow = new createFlashCardWindow(this);
            buildSetWindow.Show();
        }

       
        private void StudyFlashCardDecksButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

             studyFlashCardsWindow studyCardWindow = new studyFlashCardsWindow(this);
            studyCardWindow.Show();
        }

        private void QuitApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
   
}
