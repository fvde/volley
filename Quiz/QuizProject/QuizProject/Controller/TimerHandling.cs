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
using System.Windows.Threading;
using QuizProject.Logic;

namespace QuizProject.Controller
    
{
    

    public class TimerHandling{

        private static DispatcherTimer timer;


        /// <summary>
        /// Initializes the timer with a specific tickrate
        /// </summary>
        /// <param name="tickrate">The tickrate. determines in which interval the assigned method is called</param>
        public static void initialize(TimeSpan tickrate){

                 timer = new DispatcherTimer();
                timer.Interval = tickrate;
	            timer.Tick += new EventHandler (timer_Tick);
	            timer.Start();
        }


        private static void timer_Tick(object sender, EventArgs e)
        {
            GamesLogic.timeElapsed(timer.Interval.Milliseconds);
            ViewHandler.refreshTimer();
        }


        public static void restart(TimeSpan tickrate)
        {
            if (timer == null)
            {
                initialize(tickrate);
            }
            else
            {
                timer.Stop();
                timer.Interval = tickrate;
                timer.Start();
            }
        }

    }
}
