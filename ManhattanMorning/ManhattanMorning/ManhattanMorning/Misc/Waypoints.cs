using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using ManhattanMorning.Model;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Controller;

namespace ManhattanMorning.Misc
{
    /// <summary>
    /// Path is used to move Objects along a Path with several Waypoints in a List.
    /// You can also set a speed (meter/sec) and loop the waypoints.
    /// </summary>
    public class Waypoints
    {
        #region Properties

        /// <summary>
        /// Waypoints along the Path in vector2 list.
        /// </summary>
        public List<Vector2> WaypointList { get; set; }

        /// <summary>
        /// Movementspeed in meter/sec.
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// Sets the Object that has the Path so that we dont have to pass the corresponding
        /// Object all the time.
        /// </summary>
        public DrawableObject CorrespondingObject
        { get { return correspondingObject; }
          set {correspondingObject = value;
               init();
              } 
        }

        /// <summary>
        /// Used to set the waypoints active or inactive. In inactive state the object will be drawn but does not move.
        /// </summary>
        public bool Active { get { return active; } set { active = value; } }

        public bool Teleport { get { return teleport; } set { teleport = value; } }

        #endregion

        #region Members

        /// <summary>
        /// Waypoints along the Path in vector2 list.
        /// </summary>
        private List<Vector2> waypointList;

        /// <summary>
        /// Movementspeed in meter/sec.
        /// </summary>
        private float speed;

        /// <summary>
        /// Stores the distance traveled since the last Waypoint is passed.
        /// </summary>
        private float traveledDistance;

        /// <summary>
        /// How far the Object moves depending on the GameTime;
        /// </summary>
        private float stepDistance;

        /// <summary>
        /// Temp for distance between two Waypoints.
        /// </summary>
        private float distanceBetweenTwoWaypoints;

        /// <summary>
        /// Object to which the Path is related to.
        /// </summary>
        private DrawableObject correspondingObject;

        /// <summary>
        /// Temp to store a normalized vector.
        /// </summary>
        private Vector2 normalizedVector;

        /// <summary>
        /// CurrentWaypoint position.
        /// </summary>
        private Vector2 currentWaypoint;

        /// <summary>
        /// NextWaypoint position.
        /// </summary>
        private Vector2 nextWaypoint;

        /// <summary>
        /// Number of the next waypoint in the list.
        /// </summary>
        private int nextWaypointNumber;

        /// <summary>
        /// Tells us how many waypoints are in the list. Used to avoid to many calls of list.count
        /// </summary>
        private int numberOfWaypointsInList;

        /// <summary>
        /// If the object is meant to move.
        /// </summary>
        private bool active;

        /// <summary>
        /// Indicates if the object teleports to the beginning if the last waypoint is reached.
        /// </summary>
        private bool teleport;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a new Route with several waypoints in a list.
        /// </summary>
        /// <param name="waypointList">Several waypoints in a List.</param>
        /// <param name="speed">Movementspeed in meter/sec.</param>
        /// <param name="active">If it's activated or not.</param>
        /// <param name="teleport">If the Object reaches the last Waypoint it teleports to the first.</param>
        public Waypoints(List<Vector2> waypointList, float speed, bool active, bool teleport)
        {
            this.teleport = teleport;
            this.active = active;
            this.speed = speed;
            this.nextWaypointNumber = 0;

            this.waypointList = waypointList;
            this.nextWaypoint = waypointList.First();            

            numberOfWaypointsInList = this.waypointList.Count;
        }

        /// <summary>
        /// Creates a new Route with a single waypoint.
        /// </summary>
        /// <param name="waypoint">Single waypoint the Object moves to.</param>
        /// <param name="speed">Movementspeed in meter/sec.</param>
        /// <param name="active">If it's activated or not.</param>
        /// <param name="teleport">If the Object reaches the last Waypoint it teleports to the first.</param>
        public Waypoints(Vector2 waypoint, float speed, bool active, bool teleport)
        {
            this.teleport = teleport;
            this.active = active;
            this.speed = speed;
            this.nextWaypointNumber = 0;

            this.waypointList = new List<Vector2>();
            this.waypointList.Add(waypoint);
            this.nextWaypoint = new Vector2(0, 0);
            numberOfWaypointsInList = 1;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Updates the Position of the Object. Is able to skip waypoints if the speed is too high.
        /// </summary>
        /// <param name="gameTime">GameTime</param>
        public void updatePosition(GameTime gameTime)
        {
            // don't do anything if the waypoints are inactive
            if (!active || (numberOfWaypointsInList < 2) ) return;

            // how far the Object moves depending on the last update
            stepDistance = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            distanceBetweenTwoWaypoints = Vector2.Distance(currentWaypoint, nextWaypoint);

            // if distance between current Position and next Waypoint is smaller than the stepDistance, keep direction
            if ((traveledDistance + stepDistance) < distanceBetweenTwoWaypoints)
            {
                // updates traveled distance between the two waypoints
                traveledDistance += stepDistance;                
                correspondingObject.Position += (normalizedVector * stepDistance);
            }
            else
            {
                // we have to update the waypoints because the object travelled to far
                while(traveledDistance + stepDistance > distanceBetweenTwoWaypoints)
                {
                    // sets number of the next waypoint
                    if(numberOfWaypointsInList - 1 == nextWaypointNumber)
                    {
                        // if teleport is not active, go to the first waypoint
                        if (!teleport)
                            nextWaypointNumber = 0;
                        else
                        {
                            // otherwise reset everything to initial state and return
                            reset();
                            return;
                        }
                    }
                    else
                    {
                        // if we did not reach the last waypoint, increment the nextWaypointNumber
                        nextWaypointNumber++;
                    }

                    // sets next waypoints
                    currentWaypoint = nextWaypoint;
                    nextWaypoint = waypointList.ElementAt(nextWaypointNumber);
                    // subtracts the remaining distance between the last two waypoints from how far we want to go in this step
                    stepDistance -= (distanceBetweenTwoWaypoints - traveledDistance);
                    traveledDistance = 0;
                    distanceBetweenTwoWaypoints = Vector2.Distance(currentWaypoint, nextWaypoint);
                }
                // after processing over all waypoints we skipped, set travelledDistance between the latest two waypoints
                // to the remaining stepDistance
                traveledDistance = stepDistance;

                // updates traveled distance between the two waypoints
                normalizedVector = nextWaypoint - currentWaypoint;
                normalizedVector.Normalize();

                // sets the new position of the object
                correspondingObject.Position = currentWaypoint + (normalizedVector * (distanceBetweenTwoWaypoints * traveledDistance));
            }

            
        }

        /// <summary>
        /// Removes a Waypoint the given Position in the list.
        /// </summary>
        /// <param name="waypointAtPositionInList">Number of Waypoint you want to remove.</param>
        public void removeWaypoint(int waypointAtPositionInList)
        {
            // returns if the number of the waypoint is invalid
            if (numberOfWaypointsInList <= (waypointAtPositionInList)) return;

            // if the waypoint we want to remove is the next waypoint, set the nextWaypoint to the overnext
            if (waypointAtPositionInList == nextWaypointNumber)
            {
                if (numberOfWaypointsInList - 1 == nextWaypointNumber)
                {
                    nextWaypointNumber = 0;
                }
                else
                {
                    nextWaypointNumber += 1;
                }
                // sets next waypoints
                nextWaypoint = waypointList.ElementAt(nextWaypointNumber);
            }
            else
            {
                // if the waypoint we want to remove is the current waypoint, set the currentWaypoint to the last waypoint
                if (waypointAtPositionInList == nextWaypointNumber - 1)
                {
                    if (nextWaypointNumber == 1)
                    {
                        currentWaypoint = waypointList.ElementAt(numberOfWaypointsInList - 1);
                    }
                    else
                    {
                        currentWaypoint = waypointList.ElementAt(nextWaypointNumber - 1);
                    }
                }
            }

            // removes the waypoint
            waypointList.RemoveAt(waypointAtPositionInList);
            numberOfWaypointsInList--;
        }

        /// <summary>
        /// Initializes some variables, is called when the corresponding object is set.
        /// </summary>
        private void init()
        {
            //set current waypoint
            this.currentWaypoint = this.correspondingObject.Position;
            //calculate direction vector
            normalizedVector = nextWaypoint - currentWaypoint;
            normalizedVector.Normalize();
        }

        /// <summary>
        /// Resets the object to the initial values.
        /// </summary>
        private void reset()
        {
            //current waypoint is first waypoint in list
            currentWaypoint = waypointList.First();

            //next waypoint is either second waypoint in list or first waypoint in list if there is only one
            if (waypointList.Count > 1)
            {
                nextWaypoint = waypointList.ElementAt(1);

                //calculate direction vector between first an second waypoint
                normalizedVector = nextWaypoint - currentWaypoint;
                normalizedVector.Normalize();
            }
            else
                nextWaypoint = currentWaypoint;

            //reset all distances
            traveledDistance = 0;
            stepDistance = 0;
            nextWaypointNumber = 1;

            correspondingObject.Position = currentWaypoint;
        }


        #endregion
    }

        
}
