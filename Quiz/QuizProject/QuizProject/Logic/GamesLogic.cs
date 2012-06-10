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
using QuizProject.Model;
using QuizProject.Tools;
using QuizProject.View;
using QuizProject.Controller;


namespace QuizProject.Logic
{

    public enum Gamestate { QuestionDisplayed, Menue };

    public class GamesLogic
    {
        private static Question currentQuestion;
        public static Random random = new Random();
        public static Gamestate gamestate = Gamestate.QuestionDisplayed;
        public static int millisecondsForQuestion;

        

        /// <summary>
        /// News the question.
        /// </summary>
        public static void newQuestion()
        {
            millisecondsForQuestion = 5000;
            TimerHandling.restart(new TimeSpan(0, 0, 0, 0,100));
            currentQuestion = createQuestion(true);

            ViewHandler.displayNewQuestion ( currentQuestion.QuestionString,createAnswerTags(currentQuestion.PossibleAnswers));   
        }

        /// <summary>
        /// tells the game logic that a specific amount of milliseconds elapsed. should be called by timer
        /// </summary>
        /// <param name="milliseconds"></param>
        public static void timeElapsed(int milliseconds)
        {
            millisecondsForQuestion -= milliseconds;
            if (millisecondsForQuestion <= 0)
            {
                newQuestion();
            }
        }

        /// <summary>
        /// handles buttonEvents passed by the ViewHandler
        /// </summary>
        /// <param name="b"></param>
        public static void handleButton(Button b)
        {
            if (gamestate == Gamestate.QuestionDisplayed)
            {
                if (b.Tag is AnswerTag)
                {

                    if (currentQuestion.isRightAnswer(((AnswerTag)b.Tag).Answer))
                    {
                        handleRightAnswer();
                    }
                    else
                    {
                        handleWrongAnswer();
                    }
                }
                else
                {
                    throw new Exception("wrong oder no tag attached to pressed button");
                }
            }
        }


        /// <summary>
        /// handles logic for a right answer
        /// </summary>
        private static void handleRightAnswer()
        {
            newQuestion();
        }

        /// <summary>
        /// handles logic for a wrong answer
        /// </summary>
        private static void handleWrongAnswer()
        {
            return;
        }

        /// <summary>
        /// creates answerTags from the possible answers which should be assigned to the buttons
        /// </summary>
        /// <param name="possibleAnswers"></param>
        /// <returns></returns>
        private static AnswerTag[] createAnswerTags(String[] possibleAnswers)
        {
            AnswerTag[] tags = new AnswerTag[possibleAnswers.Length];
            for (int i = 0; i < possibleAnswers.Length; i++)
            {
                tags[i] = new AnswerTag(possibleAnswers[i], i);
            }

            //shuffle the array so that the answers are not everytime displayed in the same order
            random.shuffleArray(tags);

            return tags;

        }

        /// <summary>
        /// creates a new Question (should be from a database)
        /// </summary>
        /// <returns></returns>
        private static Question createQuestion()
        {
            //TODO
            throw new NotImplementedException("creating new Questions should be implemented as soon a database is used");
            return null;
        }


        /// <summary>
        /// returns a new questions from the testquestions
        /// </summary>
        /// <param name="isTestWithoutDatabase"></param>
        /// <returns></returns>
        private static Question createQuestion(Boolean isTestWithoutDatabase)
        {
            if (isTestWithoutDatabase)
            {
                return TestQuestions.getQuestion();
            }
            else
                //funny, isn't it ^^
                throw new Exception("fool");

            
        }
    }
}
