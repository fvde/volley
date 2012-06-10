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

namespace QuizProject.View
{
    public partial class MainPage2 : PhoneApplicationPage
    {
        // Constructor
        public MainPage2()
        {
            InitializeComponent();
           
            ViewHandler.answerButtons =new  Button[] {button1, button2, button3, button4};
            ViewHandler.currentQuestion = textBlock1;
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //Colorchange.Begin();
            //Storyboard sb = XamlReader.Load(xamlTemplates.Changecolor) as Storyboard;
            
           // Storyboard.SetTarget(sb.Children[0],button1 as DependencyObject);
            //sb.Begin();
            
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button1_Click_1(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));
        }
    }
}