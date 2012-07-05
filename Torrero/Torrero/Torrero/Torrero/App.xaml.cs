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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Torrero
{
    public partial class App : Application
    {
        /// <summary>
        /// Bietet problemlosen Zugriff auf den Root-Frame der Telefonanwendung.
        /// </summary>
        /// <returns>Der Root-Frame der Telefonanwendung</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Bietet Zugriff auf einen ContentManager für die Anwendung.
        /// </summary>
        public ContentManager Content { get; private set; }

        /// <summary>
        /// Bietet Zugriff auf einen GameTimer, der für das Verteilen des FrameworkDispatcher eingerichtet ist.
        /// </summary>
        public GameTimer FrameworkDispatcherTimer { get; private set; }

        /// <summary>
        /// Bietet Zugriff auf den AppServiceProvider für die Anwendung.
        /// </summary>
        public AppServiceProvider Services { get; private set; }

        /// <summary>
        /// Konstruktor für das Anwendungsobjekt.
        /// </summary>
        public App()
        {
            // Globaler Handler für unerwarteten Ausnahmefehler. 
            UnhandledException += Application_UnhandledException;

            // Standard-Silverlight-Initialisierung
            InitializeComponent();

            // Telefonspezifische Initialisierung
            InitializePhoneApplication();

            // XNA-Initialisierung
            InitializeXnaApplication();

            // Grafikprofilierungsinformationen beim Debugging anzeigen.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Aktuelle Frame-Rate-Zähler anzeigen.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Bereiche der Anwendung anzeigen, die in jedem Frame erneut gezeichnet werden.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Nichtproduktion-Analysevirtualisierungsmodus aktivieren, 
                // der Bereiche der Seite zeigt, die der GPU mit einem farbigen Overlay übergeben werden.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Erkennung des Anwendungsruhezustands durch Einstellung der Eigenschaft UserIdleDetectionMode des
                // PhoneApplicationService-Objekts der Anwendung auf "Deaktiviert" deaktivieren.
                // Achtung:- Dies nur im Debug-Modus verwenden, Anwendung, die die Benutzerruhezustandserkennung deaktiviert, wird weiterhin ausgeführt
                // und verbraucht Batteriestrom, wenn der Benutzer das Telefon nicht verwendet.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        // Code, der beim Starten (z.B. von Start aus) der Anwendung ausgeführt wird
        // Dieser Code wird nicht ausgeführt, wenn die Anwendung erneut aktiviert wird
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code, der beim Aktivieren (in den Vordergrund bringen) der Anwendung ausgeführt wird
        // Dieser Code wird nicht ausgeführt, wenn die Anwendung erstmals gestartet wird
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code, der beim Deaktivieren (in den Hintergrund schicken) der Anwendung ausgeführt wird
        // Dieser Code wird nicht ausgeführt, wenn die Anwendung geschlossen wird
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code, der beim Schließen (z.B. Benutzer drückt "Zurück") der Anwendung ausgeführt wird
        // Dieser Code wird nicht ausgeführt, wenn die Anwendung deaktiviert wird
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code, der ausgeführt wird, wenn die Navigation fehlschlägt
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Eine Navigation ist fehlgeschlagen; Debugger unterbrechen
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code, der bei unbehandelten Ausnahmen ausgeführt wird
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Eine unbehandelte Ausnahme ist aufgetreten; Debugger unterbrechen
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Doppelte Initialisierung vermeiden
        private bool phoneApplicationInitialized = false;

        // Keinen zusätzlichen Code zu dieser Methode hinzufügen
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Frame erstellen, aber noch nicht als RootVisual einstellen; dadurch bleibt der
            // Splash-Bildschirm aktiv, bis die Anwendung für das Rendering bereit ist.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Navigationsfehler behandeln
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Sicherstellen, dass keine erneute Initialisierung durchgeführt wird
            phoneApplicationInitialized = true;
        }

        // Keinen zusätzlichen Code zu dieser Methode hinzufügen
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Root-Visual so einstellen, dass die Anwendung das Rendering durchführen kann
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Diesen Handler entfernen, da er nicht mehr benötigt wird
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion

        #region XNA application initialization

        // Führt die Initialisierung der XNA-Typen durch, die für die Anwendung erforderlich sind.
        private void InitializeXnaApplication()
        {
            // Dienstanbieter erstellen
            Services = new AppServiceProvider();

            // SharedGraphicsDeviceManager zu den Diensten als IGraphicsDeviceService für die App hinzufügen
            foreach (object obj in ApplicationLifetimeObjects)
            {
                if (obj is IGraphicsDeviceService)
                    Services.AddService(typeof(IGraphicsDeviceService), obj);
            }

            // ContentManager erstellen, damit die Anwendung vorkompilierte Assets laden kann
            Content = new ContentManager(Services, "Content");

            // GameTimer erstellen, um den XNA FrameworkDispatcher zu verteilen
            FrameworkDispatcherTimer = new GameTimer();
            FrameworkDispatcherTimer.FrameAction += FrameworkDispatcherFrameAction;
            FrameworkDispatcherTimer.Start();
        }

        // Ein Event-Handler, der den FrameworkDispatcher zu jedem Frame verteilt.
        // FrameworkDispatcher ist für viele XNA-Ereignisse und
        // für bestimmte Funktionen, zum Beispiel die SoundEffect-Wiedergabe, erforderlich.
        private void FrameworkDispatcherFrameAction(object sender, EventArgs e)
        {
            FrameworkDispatcher.Update();
        }

        #endregion
    }
}
