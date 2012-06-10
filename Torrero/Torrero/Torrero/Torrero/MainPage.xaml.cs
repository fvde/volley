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
using Microsoft.Phone.Tasks;
using System.Threading;
using System.Windows.Threading;

namespace Torrero
{
    public partial class MainPage : PhoneApplicationPage
    {
        /**
         * If fist visit on the mainpage -> show Logos of GB and Torero
         * => Timer is needed
         */
        DispatcherTimer t1 = new DispatcherTimer();


        // Konstruktor
        public MainPage()
        {
            InitializeComponent();
            
            // usually menu -> not visible
            this.ContentPanel.Visibility = Visibility.Collapsed; 
            this.TitlePanel.Visibility = Visibility.Collapsed;

            // logos -> visible
            this.TitlePanelPreview.Visibility = Visibility.Visible;
            this.ContentPanelPreview.Visibility = Visibility.Visible;

            // set timer to 2 seconds
            t1.Interval = TimeSpan.FromMilliseconds(2000); // Intervall festlegen, hier 100 ms
            // connection to method
            t1.Tick+=new EventHandler(t1_Tick); // Eventhandler ezeugen der beim Timerablauf aufgerufen wird
            t1.Start(); // Timer starten
          
        }

        /**
         * Making usal Menu visible & logos not visible
         */
        private void t1_Tick(object sender, EventArgs e)
        {
            this.ContentPanel.Visibility = Visibility.Visible;
            this.TitlePanel.Visibility = Visibility.Visible;

            this.TitlePanelPreview.Visibility = Visibility.Collapsed;
            this.ContentPanelPreview.Visibility = Visibility.Collapsed;
        }


        // Einfache Schaltfläche Klick-Event-Handler, die uns zur zweiten Seite bringt.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
        }

        // Einfache Schaltfläche Klick-Event-Handler, die uns zur zweiten Seite bringt.
        private void Highscores_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Subpages/HighscorePage.xaml", UriKind.Relative));
        }

        // Einfache Schaltfläche Klick-Event-Handler, die uns zur zweiten Seite bringt.
        private void Credit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Subpages/HelpAndCredits.xaml", UriKind.Relative));
        }

        // Einfache Schaltfläche Klick-Event-Handler, die uns zur zweiten Seite bringt.
        private void FullVersion_Click(object sender, RoutedEventArgs e)
        {
            var marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.ContentIdentifier = "ea9a24ad-d2d1-df11-9eae-00237de2db9e";
            marketplaceDetailTask.Show();
        }
    }
}
