using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace QuizProject.Model
{
    public class TestQuestions
    {
        private static Question[] questions = 
        {
            new Question("Welches Ministerium wird umgangssprachlich als Hardthöhe bezeichnet?", new String[] {"Verteidigung", "Justiz", "Finanzen", "Gesundheit"},0),
            new Question("Wobei handelt es sich nicht um ein Reptil?", new String[] {"Wiesenotter","Kreuzotter","Puffotter","Eidotter"}, 3),
            new Question("Für Touristen aus aller Welt liegt Bad Kreuznach auf jeden Fall ....?", new String[] {"ganz nah" , "in der Nähe"  , "an der Nahe" , "in weiter Ferne so nah"}, 2),
            new Question("Wo machte man Mitte des 20. Jahrhunderts einen für das Bibelverständnis spektakulären Schriftrollen-Fund?", new String[] {"Qumran", "Chichén Itzá", "Angkor Vat" , "Herculaneum"}, 0)
            

        };


        public static Question getQuestion()
        {
            return questions[new Random().Next(questions.Length)];
        }


    }
}
