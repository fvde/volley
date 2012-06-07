using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ManhattanMorning.Misc
{
    /// <summary>
    /// Objects implementing this Interface can register themselves to multiple Observable Objects.
    /// </summary>
    public interface IObserver
    {
        /// <summary>
        /// Get a notification from an Observed Object.
        /// </summary>
        /// <param name="sender">The Observed Object wich sent the notification</param>
       void notify(ObservableObject sender);
    }

    /// <summary>
    /// Interface for all controllers which have to be paused while not ingame
    /// </summary>
    public interface IController
    {

        /// <summary>
        /// Does all necessary action, to bring controller back to the state after initialization
        /// </summary>
        void clear();

        /// <summary>
        /// Forces controller to pause, has to make sure that all timers etc. pause
        /// </summary>
        /// <param name="on">true: controller has to pause, false: controller has to work </param>
        void pause(bool on);

        /// <summary>
        /// Initializes the Controller to handle a new GameInstance
        /// </summary>
        void initialize();

    }


}
