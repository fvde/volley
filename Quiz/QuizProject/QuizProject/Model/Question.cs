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


namespace QuizProject.Model
{
    public class Question
    {
        private String questionString;
        private String[] possibleAnswers;
        private int rightAnswerIndex;
        private String rightAnswerString;


        public bool isRightAnswer(int answerIndex)
        {
            return rightAnswerIndex == answerIndex;
        }

        public bool isRightAnswer(String answer)
        {
            return answer.Equals(rightAnswerString);
        }

        public String QuestionString { get { return questionString; } }
        public String[] PossibleAnswers { get { return possibleAnswers; } }
        

        #region Constructors

        public Question(String question, String[] answers, int rightAnswerIndex)
        {
            this.questionString = question;
            this.possibleAnswers = answers;
            this.rightAnswerIndex = rightAnswerIndex;
            this.rightAnswerString = possibleAnswers[rightAnswerIndex];
        }

        public Question(String question, String[] answers, String rightAnswerString)
        {
            this.questionString = question;
            this.possibleAnswers = answers;
            this.rightAnswerString = rightAnswerString;
            rightAnswerIndex = Array.IndexOf(possibleAnswers, rightAnswerString);
        }

        public Question(String question, String[] answers, String rightAnswerString, int rightAnswerIndex)
        {
            this.questionString = question;
            this.possibleAnswers = answers;
            this.rightAnswerString = rightAnswerString;
            this.rightAnswerIndex = rightAnswerIndex;
        }

        #endregion

    }
}
