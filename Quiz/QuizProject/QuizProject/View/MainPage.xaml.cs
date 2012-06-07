using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Markup;
using QuizProject.Controller;
using QuizProject.Logic;

namespace QuizProject.View
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            
            InitializeComponent();
            ViewHandler.answerButtons = new Button[] { button1, button2, button3, button4 };
            ViewHandler.currentQuestion = textBlock1;
            ViewHandler.timerTextBlock = textBlock2;
            GamesLogic.newQuestion();
            
        }

        

       



        private void button3_Click(object sender, RoutedEventArgs e)
        {
            ViewHandler.button_Click(sender, e);
            //NavigationService.Navigate(new Uri("/View/MainPage2.xaml", UriKind.Relative));
        }

        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            ViewHandler.button_Click(sender, e);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            ViewHandler.button_Click(sender, e);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            ViewHandler.button_Click(sender, e);
        }

       
    }
}