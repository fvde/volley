using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Torrero.Controller;
using Torrero.Model;
using Microsoft.Xna.Framework.Input.Touch;
using System.Diagnostics;

namespace Torrero
{
    public partial class GamePage : PhoneApplicationPage
    {
        GameTimer timer;
        ContentManager contentManager;

        private GameLogic gameLogic;
        private GridGenerator gridGenerator;
        private GameInstance gameInstance;
        private Graphics graphics;
        private StorageManager storageManager;

        // For rendering the XAML onto a texture
        UIElementRenderer elementRenderer;


        public GamePage()
        {
            InitializeComponent();

            TouchPanel.EnabledGestures = GestureType.Flick;

            contentManager = (Application.Current as App).Content;

            // Timer für diese Seite erstellen
            timer = new GameTimer();
            timer.UpdateInterval = TimeSpan.FromTicks(133333);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;

            // Use the LayoutUpdate event to know when the page layout 
            // has completed so that we can create the UIElementRenderer.
            LayoutUpdated += new EventHandler(GamePage_LayoutUpdated);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Freigabemodus des Grafikgeräts so einstellen, das es sich beim XNA-Rendering einschaltet
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Setup Controllers
            storageManager = new StorageManager(contentManager);
            storageManager.loadTexturesForDictionary();
                        
            gridGenerator = new GridGenerator();
            graphics = new Graphics(contentManager);
            gameInstance = new GameInstance(gridGenerator.getNewGrid(5, TorreroConstants.ColumnNumber));

            // Update once to create new blocks if required
            gridGenerator.update(gameInstance);

            gameLogic = new GameLogic(gameInstance);

            // Timer starten
            timer.Start();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Timer stoppen
            timer.Stop();

            // Freigabemodus des Grafikgeräts so einstellen, das es sich beim XNA-Rendering ausschaltet
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Ermöglicht der Seite die Ausführung der Logik, wie zum Beispiel Aktualisierung der Welt,
        /// Überprüfung auf Kollisionen, Erfassung von Eingaben und Abspielen von Ton.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            // TODO: Fügen Sie Ihre Aktualisierungslogik hier hinzu
            gridGenerator.update(gameInstance);
            gameLogic.update(e.ElapsedTime);            
        }

        /// <summary>
        /// Ermöglicht der Seite, selbst zu zeichnen.6
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            // Render the Silverlight controls using the UIElementRenderer.
            elementRenderer.Render();

            graphics.drawGame(gameInstance, e.ElapsedTime, elementRenderer);
        }

        private void StackPanel_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            //Check if touch Gesture is available
            if (TouchPanel.IsGestureAvailable)
            {
                // Read the gesture so that you can handle the gesture type
                GestureSample gesture = TouchPanel.ReadGesture();
                switch (gesture.GestureType)
                {
                    case GestureType.Flick:
                        if (Math.Abs(gesture.Delta.Y - gesture.Delta2.Y) < 250)
                        {
                            if (gesture.Delta.X - gesture.Delta2.X < -100)
                            {
                                gameInstance.Player.DirectionAtCrossing = MovingDirection.Left;
                                Debug.WriteLine("LEFT!");
                                break;
                            }
                            if (gesture.Delta.X - gesture.Delta2.X > 100)
                            {
                                gameInstance.Player.DirectionAtCrossing = MovingDirection.Right;
                                Debug.WriteLine("RIGHT!");
                                break;
                            }
                        }
                        else if (Math.Abs(gesture.Delta.X - gesture.Delta2.X) < 300)
                        {
                            if (gesture.Delta.Y - gesture.Delta2.Y < -250)
                            {
                                gameInstance.Player.DirectionAtCrossing = MovingDirection.Top;
                                Debug.WriteLine("TOP!");
                                break;
                            }
                            
                        }

                        Debug.WriteLine("FLICK GESTURE NOT KNOWN!");

                        break;

                    default:

                        Debug.WriteLine("OTHER GESTURE!");
                        break;
                }
            }
        }

        // Switch to the red rectangle.
        private void leftButton_Click(object sender, RoutedEventArgs e)
        {
            gameInstance.Player.DirectionAtCrossing = MovingDirection.Left;
        }

        // Switch to the green rectangle.
        private void middleButton_Click(object sender, RoutedEventArgs e)
        {
            gameInstance.Player.DirectionAtCrossing = MovingDirection.Top;
        }

        // Switch to the blue rectangle.
        private void rightButton_Click(object sender, RoutedEventArgs e)
        {
            gameInstance.Player.DirectionAtCrossing = MovingDirection.Right;
        }

        void GamePage_LayoutUpdated(object sender, EventArgs e)
        {
            // Create the UIElementRenderer to draw the XAML page to a texture.

            // Check for 0 because when we navigate away the LayoutUpdate event
            // is raised but ActualWidth and ActualHeight will be 0 in that case.
            if ((ActualWidth > 0) && (ActualHeight > 0))
            {
                SharedGraphicsDeviceManager.Current.PreferredBackBufferWidth = (int)ActualWidth;
                SharedGraphicsDeviceManager.Current.PreferredBackBufferHeight = (int)ActualHeight;
            }

            if (null == elementRenderer)
            {
                elementRenderer = new UIElementRenderer(this, (int)ActualWidth, (int)ActualHeight);
            }
        }
    }
}
