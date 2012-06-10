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
using Torrero.Model;
using System.IO.IsolatedStorage;

namespace Torrero.Subpages
{
    public partial class HighscorePage : PhoneApplicationPage
    {
        /// <summary>
        /// Settings with highscores.
        /// </summary>
        private IsolatedStorageSettings appSettings;

        public HighscorePage()
        {
            InitializeComponent();
            appSettings = IsolatedStorageSettings.ApplicationSettings;
            if (!appSettings.Contains("highscoresLocal"))
            {
                appSettings["highscoresLocal"] = new LinkedList<HighscoreItem>();
            }
            Loaded += new RoutedEventHandler(HighscorePage_Loaded);
        }

        private void HighscorePage_Loaded(object sender, RoutedEventArgs e){

            //List<HighscoreItem> highscoresList = new List<HighscoreItem>();
            //for (int i = 0; i < 10; i++)
            //{
            //    HighscoreItem highscore = (HighscoreItem)appSettings["highscoresLocal"];
            //    if (highscore != null)
            //    {
            //        highscoresList.Add(highscore);
            //    }
            //}
            LocalHighscoreList.ItemsSource = (LinkedList<HighscoreItem>)appSettings["highscoresLocal"];
        }
    }
}