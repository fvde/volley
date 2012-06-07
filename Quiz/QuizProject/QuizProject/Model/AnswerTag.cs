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

namespace QuizProject.Model
{
    public class AnswerTag
    {
        private String answer;
        private int answerIndex;

        public String Answer { get { return answer; } }
        public int AnswerIndex { get { return answerIndex; } }

        public AnswerTag(String answer, int answerIndex)
        {
            this.answer = answer;
            this.answerIndex = answerIndex;
        }
    }
}
