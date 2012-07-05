using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManhattanMorning.Misc
{
    /// <summary>
    /// Objects inheriting from this class can be observed and can notify their registered Observers.
    /// </summary>
    public class ObservableObject
    {

        #region Properties
        //all Properties right here
        #endregion

        #region Members

        //List of all the registered Observers
        private List<IObserver> observerList = new List<IObserver>();

        //The current Instance of the Logger
        Logger logger = Logger.Instance;

        #endregion

        /// <summary>
        /// Register a new Observer to the Observable Object
        /// </summary>
        /// <param name="observer">The Observer that want to listen</param>
        public void registerObserver(IObserver observer)
        {
            observerList.Add(observer);
            logger.log(Sender.ObservableObject, observer.ToString() + " added as Observer to "+this.ToString(), PriorityLevel.Priority_1);
        }

        /// <summary>
        /// Remove an existing Observer from an Observable Object
        /// </summary>
        /// <param name="observer">The Observer to be removed</param>
        public void removeObserver(IObserver observer)
        {
            observerList.Remove(observer);
            logger.log(Sender.ObservableObject, observer.ToString() + " was removed as Observer from "+this.ToString(),PriorityLevel.Priority_1);
        }

        /// <summary>
        /// Notify all registered Observers
        /// </summary>
        public void notifyObservers()
        {
            foreach (IObserver observer in observerList)
            {
                logger.log(Sender.ObservableObject, "Notifying " + observer.ToString(),PriorityLevel.Priority_2);
                observer.notify(this);
            }
        }

    }
}
