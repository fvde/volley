using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using ManhattanMorning.Model;
using ManhattanMorning.Controller;
using ManhattanMorning.Model.Menu;

namespace ManhattanMorning.Misc
{

    /// <summary>
    /// Represents the ingame menu
    /// </summary>
    class IngameMenuInstance
    {

        #region Properties

        /// <summary>
        /// List of all IngameMenu objects
        /// </summary>
        public LayerList<LayerInterface> MenuObjectList
        {
            get { return this.menuObjectList; }
            set { this.menuObjectList = value; }
        }

        #endregion

        #region Members

        /// <summary>
        /// List of all menu objects
        /// </summary>
        private LayerList<LayerInterface> menuObjectList;

        /// <summary>
        /// Indicates in which state the ingame menu is:
        /// 0: Normal ingame menu
        /// 1: WinnerScreen with winner team1
        /// 2: WinnerScreen with winner team2
        /// 3: WinnerScreen with tie
        /// </summary>
        private int menuState;

        /// <summary>
        /// Saves references to all objects for the different states
        /// </summary>
        private List<LayerInterface>[,] menuStructure;

        /// <summary>
        /// Indicates the selected menu item (always start with 1!)
        /// </summary>
        private int selectedItem;

        /// <summary>
        /// The time in MS since the last button was pressed
        /// Necessary to fade in button instructions at the right time
        /// </summary>
        private int timeSinceLastInput = 1;

        /// <summary>
        /// The timespan in MS after which no input leads to an overlay
        /// </summary>
        private int timeTillOverlayAppears = 2500;

        /// <summary>
        /// The overlayObject that could be shown in the current menuState
        /// </summary>
        private MenuObject overlayObject;

        #endregion


        #region Initialization

        /// <summary>
        /// Initializes the ingame menu
        /// </summary>
        public IngameMenuInstance()
        {

            // Load the objects
            menuObjectList = StorageManager.Instance.LoadIngameMenuObjects();

            // Create the structure
            createMenuStructure();

            // set all necessary members
            selectedItem = 1;
            overlayObject = (MenuObject)menuObjectList.GetObjectByName("IngameMenu_Overlay");

        }

        #endregion

        #region Methods

        /// <summary>
        /// Displays the right screen
        /// </summary>
        /// <param name="winner">-1: normal ingame menu, 0: winner screen tie, 1: winner screen left team,
        /// 2: winner screen right team</param>
        public void activateMenu(int winner)
        {

            // First, make all objects invisible
            for (int i = 0; i < menuStructure.GetLength(0); i++)
                for (int j = 0; j < menuStructure.GetLength(1); j++)
                    if (menuStructure[i,j] != null)
                        foreach (LayerInterface menuObject in menuStructure[i, j])
                            ((DrawableObject)menuObject).Visible = false;

            overlayObject.Visible = false;
            overlayObject.FadingAnimation = null;

            // Display ingame menu
            if (winner == -1)
            {

                menuState = 0;

                // Make all necessary menu objects visible
                for (int i = 0; i < menuStructure.GetLength(1); i++)
                {
                    if (menuStructure[0, i] != null)
                    {
                        foreach (LayerInterface menuObject in menuStructure[0, i])
                        {
                            ((DrawableObject)menuObject).Visible = true;
                            // Unhighlight all buttons
                            if (i > 0)
                                ((MenuButtonObject)menuObject).Selected = false;
                        }
                    }
                }

                // Select item and highlight it
                selectedItem = 3;
                foreach (LayerInterface menuObject in menuStructure[0, 3])
                    ((MenuButtonObject)menuObject).Selected = true;

            }
            // Display winner screen with winner1
            else if (winner == 1)
            {

                menuState = 1;

                // Make all necessary menu objects visible
                for (int i = 0; i < menuStructure.GetLength(1); i++)
                {
                    if (menuStructure[1, i] != null)
                    {
                        foreach (LayerInterface menuObject in menuStructure[1, i])
                        {
                            ((DrawableObject)menuObject).Visible = true;
                        }
                    }
                }


            }
            // Display winner screen with winner2
            else if (winner == 2)
            {

                menuState = 2;

                // Make all necessary menu objects visible
                for (int i = 0; i < menuStructure.GetLength(1); i++)
                {
                    if (menuStructure[2, i] != null)
                    {
                        foreach (LayerInterface menuObject in menuStructure[2, i])
                        {
                            ((DrawableObject)menuObject).Visible = true;
                        }
                    }
                }

            }

            animateTransition(menuState);

        }

        /// <summary>
        /// Triggers all necessary actions when the down button is pressed
        /// </summary>
        public void downButtonPressed()
        {

            // when in normal ingame menu
            if (menuState == 0)
            {

                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Switch);

                // unhighlight all previous selected button items
                foreach (LayerInterface menuObject in menuStructure[0, selectedItem])
                {
                    ((MenuButtonObject)menuObject).Selected = false;
                }

                // select next item
                selectedItem++;
                if (selectedItem > 3)
                    selectedItem = 1;

                // highlight all items of the new selected button
                foreach (LayerInterface menuObject in menuStructure[0, selectedItem])
                {
                    ((MenuButtonObject)menuObject).Selected = true;
                }

                timeSinceLastInput = 0;
            }

        }

        /// <summary>
        /// Triggers all necessary actions when the up button is pressed
        /// </summary>
        public void upButtonPressed()
        {

            // when in normal ingame menu
            if (menuState == 0)
            {

                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Switch);

                // unhighlight all previous selected button items
                foreach (LayerInterface menuObject in menuStructure[0, selectedItem])
                {
                    ((MenuButtonObject)menuObject).Selected = false;
                }

                // select next item
                selectedItem--;
                if (selectedItem < 1)
                    selectedItem = 3;

                // highlight all items of the new selected button
                foreach (LayerInterface menuObject in menuStructure[0, selectedItem])
                {
                    ((MenuButtonObject)menuObject).Selected = true;
                }

                timeSinceLastInput = 0;
            }

        }

        /// <summary>
        /// Triggers all necessary actions when the enter button is pressed
        /// </summary>
        public void enterButtonPressed()
        {

            SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Select);

            // when in normal ingame menu
            if (menuState == 0)
            {
                switch (selectedItem)
                {
                    case 1: // Restart
                        SuperController.Instance.restartGame();
                        break;
                    case 2: // Quit
                        SuperController.Instance.switchFromIngameMenuToMainMenu();
                        break;
                    case 3: // Back
                        SuperController.Instance.switchFromIngameMenuToIngame();
                        break;
                }

                timeSinceLastInput = 0;
            }
            // when in winner screen
            else
            {

                // Switch to main menu
                SuperController.Instance.switchFromIngameMenuToMainMenu();

            }

        }

        /// <summary>
        /// Triggers all necessary actions when the revanche button is pressed
        /// </summary>
        public void revancheButtonPressed()
        {

            // When in winner screen
            if (menuState != 0)
            {

                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Select);

                // Restart game
                SuperController.Instance.restartGame();
            }

        }

        /// <summary>
        /// Triggers all necessary actions when the back button is pressed
        /// </summary>
        public void backButtonPressed()
        {

            // when in normal ingame menu
            if (menuState == 0)
            {
                // Close ingame menu
                SuperController.Instance.switchFromIngameMenuToIngame();
                timeSinceLastInput = 0;

            }
            // when in winner screen
            else
            {
                // Switch to main menu
                SuperController.Instance.switchFromIngameMenuToMainMenu();
            }

        }


        #endregion

        #region HelperMethods

        /// <summary>
        /// Fills the menu structure with content
        /// </summary>
        private void createMenuStructure()
        {

            // Explanation structure:
            // 1.Index: Menu state
            // 2.Index: 0: List of Menu_PassiveObjects which are always shown/never highlighted (e.g. background)
            //          all other indexes: List of Menu_Buttons which belong to a single selectable item
            menuStructure = new List<LayerInterface>[3, 4];

            // Normal ingame menu
            menuStructure[0, 0] = new List<LayerInterface>();
            menuStructure[0, 0].Add(menuObjectList.GetObjectByName("Background"));
            menuStructure[0, 0].Add(menuObjectList.GetObjectByName("Frame"));
            menuStructure[0, 1] = new List<LayerInterface>();
            menuStructure[0, 1].Add(menuObjectList.GetObjectByName("Button_Restart"));
            menuStructure[0, 2] = new List<LayerInterface>();
            menuStructure[0, 2].Add(menuObjectList.GetObjectByName("Button_Quit"));
            menuStructure[0, 3] = new List<LayerInterface>();
            menuStructure[0, 3].Add(menuObjectList.GetObjectByName("Button_Back"));

            // Winner Screen Team 1
            menuStructure[1, 0] = new List<LayerInterface>();
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("Background"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("WinnerScreen_Overlay"));
            menuStructure[1, 1] = new List<LayerInterface>();
            menuStructure[1, 1].Add(menuObjectList.GetObjectByName("WinnerScreen_Winner_Team1"));
            menuStructure[1, 2] = new List<LayerInterface>();
            menuStructure[1, 3] = new List<LayerInterface>();

            // Winner Screen Team 2
            menuStructure[2, 0] = new List<LayerInterface>();
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("Background"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("WinnerScreen_Overlay"));
            menuStructure[2, 1] = new List<LayerInterface>();
            menuStructure[2, 1].Add(menuObjectList.GetObjectByName("WinnerScreen_Winner_Team2"));
            menuStructure[2, 2] = new List<LayerInterface>();
            menuStructure[2, 3] = new List<LayerInterface>();

        }

        /// <summary>
        /// Sets the animations for the different transitions in IngameMenu
        /// </summary>
        /// <param name="menuState">The new state</param>
        private void animateTransition(int menuState)
        {

            for (int j = 0; j < 4; j++)
                foreach (DrawableObject menuObject in menuStructure[menuState, j])
                    menuObject.FadingAnimation = new FadingAnimation(false, false, 0, true, 300);

        }

        /// <summary>
        /// Updates the main menu
        /// </summary>
        /// <param name="gameTime">The gametime</param>
        public void update(GameTime gameTime)
        {

            manageOverlays(gameTime);

        }

        /// <summary>
        /// Checks if an overlay has to be shown
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        private void manageOverlays(GameTime gameTime)
        {
            if (menuState == 0)
            {

                // If there was an input, fade out the overlay
                if (timeSinceLastInput == 0)
                {
                    if (overlayObject.Visible == true)
                    {
                        overlayObject.FadingAnimation = new FadingAnimation(false, false, 0, true, 400);
                        overlayObject.FadingAnimation.Inverted = true;
                        timeSinceLastInput = 1;
                    }
                }

                // Increment time since last input
                timeSinceLastInput += gameTime.ElapsedGameTime.Milliseconds;

                // Check if an overlay should be shown
                if (timeSinceLastInput > timeTillOverlayAppears)
                {
                    if (overlayObject.Visible == false)
                        overlayObject.FadingAnimation = new FadingAnimation(false, false, 0, true, 400);

                }

            }
        }

        #endregion

    }
}
