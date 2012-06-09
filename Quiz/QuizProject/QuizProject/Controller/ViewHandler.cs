using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using QuizProject.Tools;
using QuizProject.Model;
using QuizProject.Logic;

namespace QuizProject.Controller
{
    public class ViewHandler
    {
        public static Button[] answerButtons;
        public static TextBlock currentQuestion;
        public static TextBlock timerTextBlock;


        /// <summary>
        /// process the button clicks
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public static void button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                GamesLogic.handleButton((Button)sender);
            }
        }


        /// <summary>
        /// should refresh a displayed timer for the question
        /// </summary>
        public static void refreshTimer()
        {
            timerTextBlock.Text = GamesLogic.millisecondsForQuestion/1000+ "." + (GamesLogic.millisecondsForQuestion%1000)/100 + " s";
        }

        /// <summary>
        /// Displays a new Question at the given Elements (buttons + textbox)
        /// </summary>
        /// <param name="question">The question. (for textbox)</param>
        /// <param name="answerTags">The answer tags. (for buttons</param>
        public static void displayNewQuestion(String question, AnswerTag[] answerTags)
        {
            assignButtonsToAnswers(answerTags);
            currentQuestion.Text = question;

        }


        /// <summary>
        /// Assigns the buttons to answers or the other way round(tags).
        /// </summary>
        /// <param name="answerTags">The answer tags.</param>
        public static void assignButtonsToAnswers(AnswerTag[] answerTags)
        {
            if(answerButtons.Length != answerTags.Length){
                throw new Exception("View is not fitting to this type of Question");
            }


            for (int i = 0; i < answerTags.Length; i++)
            {
                answerButtons[i].Tag = answerTags[i];
                ((TextBlock)answerButtons[i].Content).Text = answerTags[i].Answer;

            }


        }

    }
}
