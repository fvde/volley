using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Model;

namespace ManhattanMorning.Misc
{
    /// <summary>
    /// This is a new representation of a list with only DrawableObjects. This allows us to add elements sorted and there are
    /// also several new methods, such as GetPlayer or GetActiveObjects.
    /// </summary>
    /// <typeparam name="T">This list only allows DrawableObjects</typeparam>
    public class LayerList<T> : List<T> where T:LayerInterface
    {
        #region Members

        /// <summary>
        /// Internal List that represents all Players in the List.
        /// </summary>
        private LayerList<Player> getPlayerList;

        /// <summary>
        /// Internal List that represents all ActiveObjects in the List.
        /// </summary>
        private LayerList<ActiveObject> getActiveObjectList;

        /// <summary>
        /// Internal List that represents all PassiveObjects in the List.
        /// </summary>
        private LayerList<PassiveObject> getPassiveObjectList;

        /// <summary>
        /// Internal List that represents all Balls in the List.
        /// </summary>
        private LayerList<Ball> getBallList;

        /// <summary>
        /// Internal List that represents all Powerups in the List.
        /// </summary>
        private LayerList<PowerUp> getPowerupList;

        #endregion

        #region Methods

        /// <summary>
        /// Constructs a new LayerList.
        /// </summary>
        public LayerList()
            : base()
        {
            getPlayerList = new LayerList<Player>(true);
            getActiveObjectList = new LayerList<ActiveObject>(true);
            getPassiveObjectList = new LayerList<PassiveObject>(true);
            getBallList = new LayerList<Ball>(true);
            getPowerupList = new LayerList<PowerUp>(true);
        }

        /// <summary>
        /// Hidden contructor, only used to ensure that making sublists will not result in a StackOverflow.
        /// </summary>
        /// <param name="init">True or false doesn't matter. Only exists to avoid endless constructor calls.</param>
        private LayerList(bool init)
            : base()
        {
        }

        /// <summary>
        /// Removes an item in the List.
        /// </summary>
        /// <param name="item">Item you want to remove.</param>
        public void remove(T item)
        {
            // If Object is added, update the sublists
            if (item is Player)
            {
                getPlayerList.Remove(item as Player);
            }
            if (item is Ball)
            {
                getBallList.Remove(item as Ball);
            }
            if (item is PowerUp)
            {
                getPowerupList.Remove(item as PowerUp);
            }
            if (item is ActiveObject)
            {
                getActiveObjectList.Remove(item as ActiveObject);
            }
            if (item is PassiveObject)
            {
                getPassiveObjectList.Remove(item as PassiveObject);
            }

            this.Remove(item);
        }

        /// <summary>
        /// Adds an item of type DrawableObject or derived subclasses in sorted order.
        /// </summary>
        /// <param name="item">DrawableObject to add.</param>
        new public void Add(T item)
        {
            // add item if list is empty
            if (this.Count == 0)
            {
                base.Add(item);
            }
            else
            {
                //iterates over the whole list and checks the position where to insert the new object
                for(int i=0; i<this.Count; i++)
                {
                    if (item.Layer <= this.ElementAt(i).Layer)
                    {
                        base.Insert(i, item);
                        break;
                    }
                    else
                        //if item is greater then every other, add it at the end of the list
                        if (i == this.Count - 1)
                        {
                            base.Add(item);
                            break;
                        }
                }
            }

            // If Object is added, update the sublists
            if (item is Player) updatePlayerList(item);
            if (item is Ball) updateBallList(item);
            if (item is PowerUp) updatePowerupList(item);
            if (item is ActiveObject) updateActiveObjectList(item);
            if (item is PassiveObject) updatePassiveObjectList(item);
        }

        #region Get

        /// <summary>
        /// Returns the first item in the List which name equals the given String.
        /// </summary>
        /// <param name="name">Name of the Object.</param>
        /// <returns>The object with the specified name.</returns>
        public LayerInterface GetObjectByName(String name)
        {
            foreach (LayerInterface item in this)
            {
                // find item and return
                if (item.Name.Equals(name))
                    return item;
            }
            return null;
        }

        /// <summary>
        /// Returns the all Lights in the List which name equals the given String.
        /// </summary>
        /// <param name="name">Name of the Lights.</param>
        /// <returns>List of all matching items.</returns>
        public List<Light> GetLightsByName(String name)
        {
            List<Light> container = new List<Light>();
            foreach (LayerInterface item in this)
            {
                // find item and return
                if (item is Light && item.Name.Equals(name))
                    container.Add(item as Light);
            }
            return container;
        }

        /// <summary>
        /// Returns a list of all Powerups.
        /// </summary>
        /// <returns>LayerList with Powerups</returns>
        public LayerList<PowerUp> GetPowerup()
        {
            return getPowerupList;
        }

        /// <summary>
        /// Returns a list of all Players.
        /// </summary>
        /// <returns>LayerList with Players</returns>
        public LayerList<Player> GetPlayer()
        {            
            return getPlayerList;
        }

        /// <summary>
        /// Returns the specified player
        /// </summary>
        /// <param name="playerIndex">Index of the player which should be returned</param>
        /// <returns>the player with the specified index, if not in list returns a null player</returns>
        public Player GetPlayer(int playerIndex)
        {
            // go through all player and check if the one with
            // the right index is in the list
            foreach (Player temp_player in getPlayerList)
            {
                if (temp_player.PlayerIndex == playerIndex)
                {
                    return temp_player;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a list of all ActiveObjects.
        /// </summary>
        /// <returns>LayerList with ActiveObjects</returns>
        public LayerList<ActiveObject> GetActiveObjects()
        {
            return getActiveObjectList;
        }

        /// <summary>
        /// Returns a list of all Players PassiveObjects.
        /// </summary>
        /// <returns>LayerList with PassiveObjects</returns>
        public LayerList<PassiveObject> GetPassiveObjects()
        {
            return getPassiveObjectList;
        }

        /// <summary>
        /// Returns a list of all Balls.
        /// </summary>
        /// <returns>LayerList with Balls (hahahaha Eier)</returns>
        public LayerList<Ball> GetBalls()
        {
            return getBallList;

        }

        #endregion

        #region Updates

        /// <summary>
        /// Updates the representation of all Powerups in the List.
        /// </summary>
        private void updatePowerupList(LayerInterface item)
        {
            // add item if list is empty
            if (getPowerupList.Count == 0)
            {
                getPowerupList.AddObject((PowerUp)item);
            }
            else
                for (int i = 0; i < getPowerupList.Count; i++)
                {
                    if (item.Layer <= getPowerupList.ElementAt(i).Layer)
                    {
                        getPowerupList.Insert(i, (PowerUp)item);
                        break;
                    }
                    else
                        //if item is greater then every other, add it at the end of the list
                        if (i == getPowerupList.Count - 1)
                        {
                            getPowerupList.AddObject((PowerUp)item);
                            break;
                        }
                }
        }

        /// <summary>
        /// Updates the representation of all Players in the List.
        /// </summary>
        private void updatePlayerList(LayerInterface item)
        {
            // add item if list is empty
            if (getPlayerList.Count == 0)
            {
                getPlayerList.AddObject((Player)item);
            }
            else
            for (int i = 0; i < getPlayerList.Count; i++)
            {
                if (item.Layer <= getPlayerList.ElementAt(i).Layer)
                {
                    getPlayerList.Insert(i, (Player)item);
                    break;
                }
                else
                    //if item is greater then every other, add it at the end of the list
                    if (i == getPlayerList.Count - 1)
                    {
                        getPlayerList.AddObject((Player)item);
                        break;
                    }
            }
        }

        /// <summary>
        /// Updates the representation of all ActiveObjects in the List.
        /// </summary>
        private void updateActiveObjectList(LayerInterface item)
        {
            // add item if list is empty
            if (getActiveObjectList.Count == 0)
            {
                getActiveObjectList.AddObject((ActiveObject)item);
            }
            else
            for (int i = 0; i < getActiveObjectList.Count; i++)
            {
                if (item.Layer <= getActiveObjectList.ElementAt(i).Layer)
                {
                    getActiveObjectList.Insert(i, (ActiveObject)item);
                    break;
                }
                else
                    //if item is greater then every other, add it at the end of the list
                    if (i == getActiveObjectList.Count - 1)
                    {
                        getActiveObjectList.AddObject((ActiveObject)item);
                        break;
                    }
            }
        }

        /// <summary>
        /// Updates the representation of all PassiveObjects in the List.
        /// </summary>
        private void updatePassiveObjectList(LayerInterface item)
        {
            // add item if list is empty
            if (getPassiveObjectList.Count == 0)
            {
                getPassiveObjectList.AddObject((PassiveObject)item);
            }
            else
                for (int i = 0; i < getPassiveObjectList.Count; i++)
                {
                    if (item.Layer <= getPassiveObjectList.ElementAt(i).Layer)
                    {
                        getPassiveObjectList.Insert(i, (PassiveObject)item);
                        break;
                    }
                    else
                        //if item is greater then every other, add it at the end of the list
                        if (i == getPassiveObjectList.Count - 1)
                        {
                            getPassiveObjectList.AddObject((PassiveObject)item);
                            break;
                        }
                }
        }


        /// <summary>
        /// Updates the representation of all Ball Objects in the List.
        /// </summary>
        private void updateBallList(LayerInterface item)
        {
            // add item if list is empty
            if (getBallList.Count == 0)
            {
                getBallList.AddObject((Ball)item);
            }
            else
            for (int i = 0; i < getBallList.Count; i++)
            {
                if (item.Layer <= getBallList.ElementAt(i).Layer)
                {
                    getBallList.Insert(i, (Ball)item);
                    break;
                }
                else
                    //if item is greater then every other, add it at the end of the list
                    if (i == getBallList.Count - 1)
                    {
                        getBallList.AddObject((Ball)item);
                        break;
                    }
            }
        }

        #endregion

        #region Helper
        /// <summary>
        /// Used to add Objects to sublists so there is no endless calling of the Add method.
        /// </summary>
        /// <param name="item"></param>
        private void AddObject(T item)
        {
            base.Add(item);
        }

        #endregion

        #endregion
    }

}
