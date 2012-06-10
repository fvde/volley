using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using ManhattanMorning.Misc;
using ManhattanMorning.Model;
using ManhattanMorning.Model.GameObject;
using Microsoft.Xna.Framework.Graphics;
using ManhattanMorning.Misc.Tasks;
using ManhattanMorning.Model.HUD;
using ManhattanMorning.View;



namespace ManhattanMorning.Controller
{
    /// <summary>
    /// PhysicEngine Wrapper-Class
    /// Includes Farseer Engine and provides properties
    /// and methods to access physics world.
    /// uses singleton pattern.
    /// </summary>
    class Physics : IController, IObserver
    {

        #region Properties

        /// <summary>
        /// Necessary for Singleton Pattern
        /// </summary>
        public static Physics Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Physics();
                }
                return instance;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// instance of the physic (singleton pattern)
        /// </summary>
        private static Physics instance = null;

        /// <summary>
        /// instance of the logger (singleton)
        /// </summary>
        private Logger logger;

        /// <summary>
        /// Maximal Y Velocity when bouncing off the ground. The point of this value is to prevent players from adding up jump forces.
        /// </summary>
        float maximalYForce;

        /// <summary>
        /// instance of the
        /// </summary>
        private SettingsManager settingsManager;

        /// <summary>
        /// Internal removal list
        /// </summary>
        private List<Body> internalRemoveList;

        /// <summary>
        /// Physical world of the level (Farseer)
        /// </summary>
        private World world;

        /// <summary>
        /// Size of the level
        /// </summary>
        private Vector2 levelSize;

        /// <summary>
        /// Middle border of the level
        /// </summary>
        private Border middleBorder;

        /// <summary>
        /// Middle border of the level
        /// </summary>
        private Border leftHandBlockBorder;

        /// <summary>
        /// Middle border of the level
        /// </summary>
        private Border rightHandBlockBorder;

        /// <summary>
        /// Middle border of the level
        /// </summary>
        private Border leftSideHandDistanceBorder;

        /// <summary>
        /// Middle border of the level
        /// </summary>
        private Border rightSideHandDistanceBorder;

        /// <summary>
        /// The current SuperBomb.
        /// </summary>
        private List<Bomb> superBombList;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random random;

        #endregion


        #region Initialization

        /// <summary>
        /// Hidden initialization which is used by public method Instance()
        /// </summary>
        private Physics()
        {
            // Initialize logger
            logger = Logger.Instance;

            // Initialize Settings
            settingsManager = SettingsManager.Instance;
            // Register as observer
            settingsManager.registerObserver(this);

            notify(settingsManager);

            internalRemoveList = new List<Body>();
            superBombList = new List<Bomb>();
            random = new Random();

            maximalYForce = (float)settingsManager.get("maximumYBounceVelocity");
        }


        #endregion

        #region Methods



        #region Update


        /// <summary>
        /// Updates physics engine every cycle
        /// </summary>
        /// <param name="gameTime">latest timespan of game</param>
        /// <param name="activeObjectsList">all objecst which are influenced physically</param>
        /// NOTE: We should divide this into subfunctions!
        public void update(GameTime gameTime, LayerList<ActiveObject> activeObjectList)
        {
            // Apply forces on active objects
            foreach (ActiveObject activeObject in activeObjectList)
            {

                // Filter for Player
                if (activeObject is Player)
                {
                    // Cast ActiveObject to Player
                    Player player = (Player)activeObject;

                    if (!player.Flags.Contains(PlayerFlag.Stunned))
                    {
                        // Handle jump
                        if (player.SimpleJump)
                        {
                            float jumpHeightModifier = 1.0f;
                            if (player.Flags.Contains(PlayerFlag.DoubleJump))
                            {
                                TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.playerJump, (int)IngameSound.Jump));
                                jumpHeightModifier = (float)settingsManager.get("increasedJumpHeight");
                            }

                            // Just apply jump if there isn't already a big enough velocity in jump direction
                            if (player.Body.LinearVelocity.Y > -5f)
                            {
                                player.Body.ApplyLinearImpulse(jumpHeightModifier * (Vector2)settingsManager.get("jumpForce"));
                            }

                            player.SimpleJump = false;
                        }

                        // Apply Movementspeed to player, in the current Version, the Player stops abrupt without smothly slowing down
                        //player.Body.ApplyForce(player.MovingImpulse * (float)settingsManager.get("movementSpeedScalar"));

                        float downMovement = 0.0f;
                        if (player.MovingImpulse.Y > 0)
                        {
                            downMovement = player.MovingImpulse.Y * (float)settingsManager.get("downMovementScalar");
                        }
                        player.Body.LinearVelocity = new Vector2(player.MovingImpulse.X * (float)settingsManager.get("movementSpeedScalar"), player.Body.LinearVelocity.Y + downMovement);

                        if (!player.HandAmplitude.Equals(Vector2.Zero) && !player.Flags.Contains(PlayerFlag.HandStunned))
                        {

                            Vector2 handImpulse = (float)settingsManager.get("handImpulseScalar") * player.HandAmplitude;
                            player.HandBody.ApplyLinearImpulse(handImpulse);
                            // Contrary force
                            player.Body.ApplyLinearImpulse(-handImpulse);
                        }

                    }
                }

            }

            // Process tasks from other controllers:
            foreach (PhysicsTask task in TaskManager.Instance.PhysicsTasks)
            {
                if (task.Type.Contains(PhysicsTask.PhysicTaskType.RemoveStun))
                {
                    task.AffectedPlayer.Flags.Remove(PlayerFlag.Stunned);
                }
                else if (task.Type.Contains(PhysicsTask.PhysicTaskType.CreateNewBall))
                {
                    createBall(task.Position);
                }
                else if (task.Type.Contains(PhysicsTask.PhysicTaskType.CreateDoubleBall))
                {
                    setDoubleBallPowerUp();
                }
                else if (task.Type.Contains(PhysicsTask.PhysicTaskType.RemoveHandStun))
                {
                    task.AffectedPlayer.Flags.Remove(PlayerFlag.HandStunned);
                }
                else if (task.Type.Contains(PhysicsTask.PhysicTaskType.CreateLava))
                {
                    createLava(getRandomPositionAtTheTop());
                }
                else if (task.Type.Contains(PhysicsTask.PhysicTaskType.CreateSuperBomb))
                {
                    createSuperBomb(getRandomPositionAtTheTop());
                }
                else if (task.Type.Contains(PhysicsTask.PhysicTaskType.DetonateSuperBomb))
                {
                    explodeSuperBomb();
                }
            }
            // MAKE SURE TO CLEAR:
            TaskManager.Instance.PhysicsTasks.Clear();

            // Remove bodies that have been requested to be removed.
            for (int x = 0; x < internalRemoveList.Count; x++)
            {
                world.RemoveBody(internalRemoveList[x]);
            }
            internalRemoveList.Clear();

            // Update physic
            world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
        }

        #endregion

        #region Object Initialization
        /// <summary>
        /// Adds an active object to the physics simulator. Depending on the object that calls the method different attributes are set. For example static for the net or dynamic for the ball.
        /// </summary>
        /// <param name="activeObject"> Object that should be added</param>
        /// <param name="Size"> Size of the object.</param>
        /// <param name="Position">Position of the object.</param>
        /// <returns></returns>
        public Body addObjectToPhysicsSimulator(ActiveObject activeObject, Vector2 Size, Vector2 Position)
        {
            if (activeObject is Player)
            {
                // create a new player
                return createNewPlayerObject(Size, Position, (Player)activeObject);
            }
            else if (activeObject is Net)
            {
                // create a new net
                return createNewNetObject(Size, Position);
            }
            else if (activeObject is Ball)
            {
                // create a new ball
                return createNewBallObject(Size, Position);
            }
            else if (activeObject is PowerUp)
            {
                // create a new powerUp

                return createNewPowerupObject(Size, Position);
            }
            else if (activeObject is Border)
            {
                // create a new border
                return createNewBorderObject(Size, Position);
            }
            else if (activeObject is Bomb)
            {
                // create a new border
                return createNewBombObject(Size, Position);
            }
            else if (activeObject is Lava)
            {
                // create a new border
                return createNewLavaObject(Size, Position);
            }
            else
            {
                // a kind of object that isn't supported by physics engine
                logger.log(Sender.Physics, "Object could not be added to physics simulator! Invalid type.", PriorityLevel.Priority_5);
                return null;
            }
        }

        /// <summary>
        /// Initializes a good Collisions listener for the current object. This is called in the constructor of all active objects.
        /// </summary>
        /// <param name="body">The body of the active object.</param>
        public void initializeCollisionListenerForbody(Body body)
        {
            if (body.LinkedActiveObject is Player)
            {
                body.OnCollision += Player_OnCollision;
                ((Player)body.LinkedActiveObject).HandBody.OnCollision += Player_OnCollision;
            }
            else if (body.LinkedActiveObject is Ball)
            {
                body.OnCollision += Ball_OnCollision;
            }
            else if (body.LinkedActiveObject is PowerUp)
            {
                body.OnCollision += PowerUp_OnCollision;
            }
            else if (body.LinkedActiveObject is Bomb)
            {
                body.OnCollision += Bomb_OnCollision;
            }
        }

        /// <summary>
        /// Creates a new player object
        /// </summary>
        /// <param name="Size">Size of the new object. Size is in meter, which is why the factor metersToPixel is required.</param>
        /// <param name="Position">Position of the new object. Position points at the MIDDLE of the object.</param>
        /// <returns></returns>
        private Body createNewPlayerObject(Vector2 Size, Vector2 Position, Player player)
        {
            Body playerBody = new Body();
            logger.log(Sender.Physics, "Player added to physic simulator. Position: (" + Position.X + "/" + Position.Y + ") Radius: (" + Size.X + ")", PriorityLevel.Priority_2);
            playerBody = BodyFactory.CreateCircle(world, Size.X / 2, 1, Position);
            playerBody.BodyType = BodyType.Dynamic;
            playerBody.Restitution = (float)settingsManager.get("playerRestitution");
            playerBody.Friction = (float)settingsManager.get("playerFriction");
            playerBody.FixedRotation = true;
            playerBody.Mass = (float)settingsManager.get("playerMass");



            // Player Hand
            Body playerHandBody = createNewPlayerHandObject(Size, Position);
            // Setting the linked active object to player, so that Collisions work. (Example picking up PowerUps)
            playerHandBody.LinkedActiveObject = player;



            // Connection between MainBody and HandBody
            // Process of thoughts: We will need at least 4 joints to make a stable connection that behaves equally in all directions. This is the case
            // because if we use just one (which I tried) the hand will be extremely unstable. With two joints the connection becomes a lot more stable,
            // but it doesnt behave the same way in all directions.
            float jointFrequency = 3.0f;
            float jointDampingRatio = 0.5f;
            float PlayerAnchorOffset = Size.X * 0.5f;
            float maxDistanceBetweenPlayerAndHand = 0.6f;

            DistanceJoint[] bodyHandDistanceJoints = new DistanceJoint[4];

            bodyHandDistanceJoints[0] = JointFactory.CreateDistanceJoint(world, playerBody, playerHandBody, new Vector2(PlayerAnchorOffset, 0), new Vector2(0, 0));
            bodyHandDistanceJoints[1] = JointFactory.CreateDistanceJoint(world, playerBody, playerHandBody, new Vector2(-PlayerAnchorOffset, 0), new Vector2(0, 0));
            bodyHandDistanceJoints[2] = JointFactory.CreateDistanceJoint(world, playerBody, playerHandBody, new Vector2(0, PlayerAnchorOffset), new Vector2(0, 0));
            bodyHandDistanceJoints[3] = JointFactory.CreateDistanceJoint(world, playerBody, playerHandBody, new Vector2(0, -PlayerAnchorOffset), new Vector2(0, 0));

            for (int x = 0; x < bodyHandDistanceJoints.Length; x++)
            {
                bodyHandDistanceJoints[x].DampingRatio = jointDampingRatio;
                bodyHandDistanceJoints[x].Frequency = jointFrequency;
            }

            //Adding a rope joint between the player and the hand. the rope limits the maximum distance between them.
            RopeJoint ropejoint = new RopeJoint(playerBody, playerHandBody, new Vector2(0, 0), new Vector2(0, 0));
            ropejoint.MaxLength = maxDistanceBetweenPlayerAndHand;
            world.AddJoint(ropejoint);



            disableCollisionBetweenBodies(playerBody, playerHandBody);

            // add hand-body to player
            player.HandBody = playerHandBody;

            return playerBody;
        }

        /// <summary>
        /// Create a new hand for a player object
        /// </summary>
        /// <param name="Size"></param>
        /// <param name="Position"></param>
        /// <returns></returns>
        private Body createNewPlayerHandObject(Vector2 Size, Vector2 Position)
        {
            Body HandBody = new Body();
            logger.log(Sender.Physics, "Player hand added to player.", PriorityLevel.Priority_2);
            // position adjusted a very small bit. This is necessary because otherwise playerposition == handposition and Farseer crashes if i.e. a rope joint between them is added.
            HandBody = BodyFactory.CreateCircle(world, Size.X / 2.0f * (float)SettingsManager.Instance.get("playerHandRatio"), 1, Position - new Vector2(0.000001f, 0.000001f));
            HandBody.BodyType = BodyType.Dynamic;
            HandBody.Restitution = (float)settingsManager.get("handRestitution");
            HandBody.Friction = (float)settingsManager.get("handFriction");
            HandBody.Mass = (float)settingsManager.get("playerMass") / 10;

            return HandBody;
        }


        /// <summary>
        /// Creates a new net object
        /// </summary>
        /// <param name="Size">Size of the new object. Size is in meter, which is why the factor metersToPixel is required.</param>
        /// <param name="Position">Position of the new object. Position points at the MIDDLE of the object.</param>
        /// <returns></returns>
        private Body createNewNetObject(Vector2 Size, Vector2 Position)
        {
            Body netBody = new Body();
            logger.log(Sender.Physics, "Net added to physic simulator. Position: (" + Position.X + "/" + Position.Y + ") Size: (" + Size.X + "/" + Size.Y + ")", PriorityLevel.Priority_2);
            netBody = BodyFactory.CreateRectangle(world, Size.X, Size.Y, 1, Position);
            netBody.BodyType = BodyType.Static;
            netBody.Restitution = (float)settingsManager.get("netRestitution");
            netBody.Friction = (float)settingsManager.get("netFriction");
            return netBody;
        }

        /// <summary>
        /// Creates a new ball object
        /// </summary>
        /// <param name="Size">Size of the new object. Size is in meter, which is why the factor metersToPixel is required.</param>
        /// <param name="Position">Position of the new object. Position points at the MIDDLE of the object.</param>
        /// <returns></returns>
        private Body createNewBallObject(Vector2 Size, Vector2 Position)
        {
            Body ballBody = new Body();
            logger.log(Sender.Physics, "Ball added to physic simulator. Position: (" + Position.X + "/" + Position.Y + ") Size: (" + Size.X + "/" + Size.Y + ")", PriorityLevel.Priority_2);
            ballBody = BodyFactory.CreateCircle(world, (Size.X / 2), 1.0f, Position);
            ballBody.BodyType = BodyType.Dynamic;
            ballBody.Restitution = (float)settingsManager.get("ballRestitution");
            ballBody.Friction = (float)settingsManager.get("ballFriction");
            ballBody.Mass = (float)settingsManager.get("ballMass");
            ballBody.SleepingAllowed = false;

            return ballBody;
        }

        /// <summary>
        /// Creates a new border object
        /// </summary>
        /// <param name="Size">Size of the new object. Size is in meter, which is why the factor metersToPixel is required.</param>
        /// <param name="Position">Position of the new object. Position points at the MIDDLE of the object.</param>
        /// <returns></returns>
        private Body createNewBorderObject(Vector2 Size, Vector2 Position)
        {
            Body borderBody = new Body();
            logger.log(Sender.Physics, "Border added to physic simulator. Position: (" + Position.X + "/" + Position.Y + ") Size: (" + Size.X + "/" + Size.Y + ")", PriorityLevel.Priority_2);
            borderBody = BodyFactory.CreateRectangle(world, Size.X, Size.Y, 1, Position);
            borderBody.BodyType = BodyType.Static;
            borderBody.Restitution = (float)settingsManager.get("borderRestitution");
            borderBody.Friction = (float)settingsManager.get("borderFriction");

            return borderBody;
        }

        /// <summary>
        /// Creates a new powerup object.
        /// </summary>
        /// <param name="Size">Size of the new object. Size is in meter, which is why the factor metersToPixel is required.</param>
        /// <param name="Position">Position of the new object. Position points at the MIDDLE of the object.</param>
        /// <returns></returns>
        private Body createNewPowerupObject(Vector2 Size, Vector2 Position)
        {
            Body powerupBody = new Body();
            logger.log(Sender.Physics, "Powerup/Misc object added to physic simulator. Position: (" + Position.X + "/" + Position.Y + ") Size: (" + Size.X + "/" + Size.Y + ")", PriorityLevel.Priority_2);
            powerupBody = BodyFactory.CreateRectangle(world, Size.X, Size.Y, 1.0f, Position);
            powerupBody.BodyType = BodyType.Dynamic;
            powerupBody.Restitution = 0.1f;
            powerupBody.Friction = 0.1f;
            powerupBody.Mass = 0.1f;
            powerupBody.SleepingAllowed = false;

            disableCollisionBetweenBodies(powerupBody, middleBorder.Body);
            disableCollisionBetweenBodies(powerupBody, rightHandBlockBorder.Body);
            disableCollisionBetweenBodies(powerupBody, leftHandBlockBorder.Body);
            disableCollisionBetweenBodies(powerupBody, leftSideHandDistanceBorder.Body);
            disableCollisionBetweenBodies(powerupBody, rightSideHandDistanceBorder.Body);


            return powerupBody;
        }

        /// <summary>
        /// Creates a new lava object
        /// </summary>
        /// <param name="Size">Size of the new object. Size is in meter, which is why the factor metersToPixel is required.</param>
        /// <param name="Position">Position of the new object. Position points at the MIDDLE of the object.</param>
        /// <returns></returns>
        private Body createNewLavaObject(Vector2 Size, Vector2 Position)
        {
            Body lavaBody = new Body();
            logger.log(Sender.Physics, "Lava added to physic simulator. Position: (" + Position.X + "/" + Position.Y + ") Size: (" + Size.X + "/" + Size.Y + ")", PriorityLevel.Priority_2);

            if (random.Next(2) == 0)
            {
                lavaBody = BodyFactory.CreateRectangle(world, Size.X, Size.Y, 1.0f);
            }
            else
            {
                lavaBody = BodyFactory.CreateCircle(world, (Size.X / 2), 1.0f, Position);
            }

            lavaBody.BodyType = BodyType.Dynamic;
            lavaBody.Restitution = 0.5f;
            lavaBody.Friction = 0.5f;

            return lavaBody;
        }

        /// <summary>
        /// Creates a new lava object
        /// </summary>
        /// <param name="Size">Size of the new object. Size is in meter, which is why the factor metersToPixel is required.</param>
        /// <param name="Position">Position of the new object. Position points at the MIDDLE of the object.</param>
        /// <returns></returns>
        private Body createNewBombObject(Vector2 Size, Vector2 Position)
        {
            Body bombBody = new Body();
            logger.log(Sender.Physics, "Bomb added to physic simulator. Position: (" + Position.X + "/" + Position.Y + ") Size: (" + Size.X + "/" + Size.Y + ")", PriorityLevel.Priority_2);
            bombBody = BodyFactory.CreateCircle(world, (Size.X / 2), 1.0f, Position);
            bombBody.BodyType = BodyType.Dynamic;
            bombBody.Restitution = 0.5f;
            bombBody.Friction = 0.5f;

            return bombBody;
        }

        /// <summary>
        /// Creates a static Rectangle, used for example for the stones in the maya level.
        /// </summary>
        /// <param name="Size">Size of the new object. Size is in meter, which is why the factor metersToPixel is required.</param>
        /// <param name="Position">Position of the new object. Position points at the MIDDLE of the object.</param>
        /// <param name="rotation">The rotation in radians.</param>
        /// <returns>The Body created with the given settings.</returns>
        public Body createStaticRectangleObject(Vector2 Size, Vector2 Position, float rotation)
        {
            Body rectangle = new Body();
            rectangle = BodyFactory.CreateRectangle(world, Size.X, Size.Y, 1.0f);
            rectangle.Position = Position;
            rectangle.BodyType = BodyType.Static;
            rectangle.Restitution = 0.5f;
            rectangle.Friction = 0.5f;
            rectangle.Rotation = rotation;

            return rectangle;
        }





        #endregion

        #region ColisionListener

        /// <summary>
        /// Collision Handling for the Ball
        /// </summary>
        /// <param name="fixtureA">Ball.</param>
        /// <param name="fixtureB">Any body.</param>
        /// <param name="contact">The contact.</param>
        /// <returns></returns>
        bool Ball_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            logger.log(Sender.Physics, "Ball collided", PriorityLevel.Priority_1);
            GameLogic.Instance.addCollision(fixtureA.Body.LinkedActiveObject, fixtureB.Body.LinkedActiveObject);
            return true;
        }

        /// <summary>
        /// Collision Handling for the Player
        /// </summary>
        /// <param name="fixtureA">Player.</param>
        /// <param name="fixtureB">Any body.</param>
        /// <param name="contact">The contact.</param>
        /// <returns></returns>
        bool Player_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            // Report collisions
            if (!(fixtureB.Body.LinkedActiveObject is Ball) && !(fixtureB.Body.LinkedActiveObject is Net) && !(fixtureB.Body.LinkedActiveObject is Player))
            {
                GameLogic.Instance.addCollision(fixtureA.Body.LinkedActiveObject, fixtureB.Body.LinkedActiveObject);
                logger.log(Sender.Physics, "Player collided", PriorityLevel.Priority_1);

                // Cap jump force, to prevent jump additions
                if (fixtureB.Body.LinkedActiveObject is Border && String.Equals(fixtureB.Body.LinkedActiveObject.Name, "bottomBorder"))
                {
                    fixtureA.Body.LinearVelocity = new Vector2(fixtureA.Body.LinearVelocity.X, Math.Min(fixtureA.Body.LinearVelocity.Y, maximalYForce));
                }
            }

            // Stun the player hand if its touching the net
            if (fixtureA.Body == ((Player)fixtureA.Body.LinkedActiveObject).HandBody && (  fixtureB.Body.LinkedActiveObject is Net 
                                                                                                || fixtureB.Body.LinkedActiveObject == leftHandBlockBorder
                                                                                                || fixtureB.Body.LinkedActiveObject == rightHandBlockBorder))
            {
                Player p = fixtureA.Body.LinkedActiveObject as Player;
                p.Flags.Add(PlayerFlag.HandStunned);
                TaskManager.Instance.addTask(new PhysicsTask(350, p, PhysicsTask.PhysicTaskType.RemoveHandStun));
            }
            return true;
        }

        /// <summary>
        /// Collision Handling for the powerups
        /// </summary>
        /// <param name="fixtureA">Powerup.</param>
        /// <param name="fixtureB">Any body.</param>
        /// <param name="contact">The contact.</param>
        /// <returns></returns>
        bool PowerUp_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (fixtureB.Body.LinkedActiveObject is Player)
            {
                logger.log(Sender.Physics, "Powerup collided with player", PriorityLevel.Priority_1);
                PowerUp powerUp = (PowerUp)fixtureA.Body.LinkedActiveObject;
                powerUp.Owner = (Player)fixtureB.Body.LinkedActiveObject;
            }


            return true;
        }

        /// <summary>
        /// Collision Handling for the powerups
        /// </summary>
        /// <param name="fixtureA">Powerup.</param>
        /// <param name="fixtureB">Any body.</param>
        /// <param name="contact">The contact.</param>
        /// <returns></returns>
        bool Bomb_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            Bomb b = (Bomb)fixtureA.Body.LinkedActiveObject;

            if (b.Exploded || b.IsSuperBomb)
            {
                return true;
            }
            else if (fixtureB.Body.LinkedActiveObject != null)
            {
                if (String.Equals(fixtureB.Body.LinkedActiveObject.Name, "leftBorder")
                || String.Equals(fixtureB.Body.LinkedActiveObject.Name, "rightBorder"))
                {
                    return true;
                }
            }
            
            b.Exploded = true;
            createExplosionAtPoint(b.Body.Position, (float)settingsManager.get("lavaRange"), (float)settingsManager.get("lavaImpact"), false);
            SuperController.Instance.removeGameObjectFromGameInstance(b);
            ParticleSystemsManager.Instance.stopBombFalling(b);

            return true;
        }

        #endregion

        #region Object Management


        #region collisionSettings
        /// <summary>
        /// Disables collision between two bodies.
        /// </summary>
        /// <param name="firstBody">1.Body that shouldn't collide</param>
        /// <param name="secondBody">2.Body that shouldn't collide</param>
        public void disableCollisionBetweenBodies(Body firstBody, Body secondBody)
        {
            firstBody.IgnoreCollisionWith(secondBody);
        }

        /// <summary>
        /// Disables collision between active Objects.
        /// </summary>
        /// <param name="firstBody">1.activeObject that shouldn't collide</param>
        /// <param name="secondBody">2.activeObject that shouldn't collide</param>
        public void disableCollisionBetweenActiveObjects(ActiveObject firstActiveObject, ActiveObject secondActiveObject)
        {
            disableCollisionBetweenBodies(firstActiveObject.Body, secondActiveObject.Body);
        }

        /// <summary>
        /// Sets the collision category of a Body. used to determine with what a body should collide
        /// </summary>
        /// <param name="body">The body.</param>
        public void setCollisionCategory(Body body, Category cat)
        {
            body.CollisionCategories = cat;
        }

        /// <summary>
        /// Sets the collision category of a Body. used to determine with what a body should collide
        /// Important: if the object is a player only the category of the inner main body ist set
        /// if you want to change the category of the Player hull please use setPlayerHullCollisionCategory
        /// </summary>
        /// <param name="body">The body.</param>
        public void setCollisionCategory(ActiveObject activeObject, Category cat)
        {
            setCollisionCategory(activeObject.Body, cat);
        }


        /// <summary>
        /// Sets the collision-category of the playerHull (Blobelements).
        /// does not set the collision-category of the mainbody
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="cat">The cat.</param>
        public void setPlayerHandCollisionCategory(Player player, Category cat)
        {

            player.HandBody.CollisionCategories = cat;


        }



        /// <summary>
        /// Disables the collision between an activeObject and a collision-category.
        /// </summary>
        /// <param name="activeObject">The activeObject.</param>
        /// <param name="cat">The category.</param>
        public void disableCollisionBetweenActiveObjectAndCollisionCategory(ActiveObject activeObject, Category cat)
        {
            disableCollisionBetweenBodyAndCollisionCategory(activeObject.Body, cat);
        }


        /// <summary>
        /// Disables the collision between a body and a collision-category.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="cat">The category.</param>
        private void disableCollisionBetweenBodyAndCollisionCategory(Body body, Category cat)
        {

            foreach (Fixture f in body.FixtureList)
            {
                if (f.CollisionCategories == cat)
                {
                    // throw new ArgumentException("cannot disable collision with own collision group");
                }
                f.CollidesWith = f.CollidesWith & ~cat;
            }
        }




        #endregion

        /// <summary>
        /// Use this method to enable or disable the physic simulation. Note that forces that are added to the body, while the simulation is on hold are added when the simulation continues!
        /// </summary>
        /// <param name="newState">True to enable the physics simulation, false to disable the physics simulation.</param>
        public void pause(bool on)
        {
            if (!on)
                logger.log(Sender.Physics, "Physic enabled.", PriorityLevel.Priority_3);
            else
                logger.log(Sender.Physics, "Physic disabled.", PriorityLevel.Priority_3);
            //world.Enabled = !on;
        }

        /// <summary>
        /// Returns true if the hand of a player is out of range of his main body. This happens mostly when the player circles the hand around the main body.
        /// </summary>
        /// <param name="p">Player that is being checked.</param>
        /// <returns></returns>
        public bool playerHandIsOutOfRange(Player p)
        {
            Vector2 playerPosition = p.Body.Position;
            Vector2 handPosition = p.HandBody.Position;
            //return false;
            return (1.25 < LineTools.DistanceBetweenPointAndPoint(ref playerPosition, ref handPosition));
        }

        /// <summary>
        /// Remove an object from the simulator. This should be called in the destructor of all active objects.
        /// </summary>
        /// <param name="activeObject">The active object. Its Body will be removed from the world.</param>
        public void removeObjectFromPhysicSimulation(ActiveObject activeObject)
        {
            removeBodyFromPhysicSimulation(activeObject.Body);
        }

        /// <summary>
        /// Remove a body from the simulator. This should only be called for bodies created by the createStaticRectangle Method.
        /// All other objects should be removed with removeObjectFromPhysicSimulation.
        /// </summary>
        /// <param name="body"></param>
        public void removeBodyFromPhysicSimulation(Body body)
        {
            internalRemoveList.Add(body);
        }

        /// <summary>
        /// Resets the physic simulator. All current Bodies will be removed from the world.
        /// </summary>
        public void clear()
        {
            middleBorder = null;
            leftHandBlockBorder = null;
            rightHandBlockBorder = null;
            levelSize = Vector2.Zero;

            world = null;
            internalRemoveList = new List<Body>();
            superBombList = new List<Bomb>();
            random = new Random();
        }

        /// <summary>
        /// Necessary for interface, but mustn't be called
        /// </summary>
        public void initialize()
        {
            throw new Exception("Wrong Physics initialization");
        }

        /// <summary>
        /// Initializes the Controller to handle a new GameInstance
        /// </summary>
        /// <param name="levelSize">The size of the level</param>
        public void initialize(Vector2 levelSize)
        {
            // store parameters
            this.levelSize = levelSize;
            // create world
            world = new World((Vector2)settingsManager.get("gravity"));
        }

        public void setBodyState(ActiveObject activeObject, bool active)
        {
            activeObject.Body.Enabled = active;
        }

        /// <summary>
        /// Resets the current dynamics applied to a body and applies the new impulse
        /// </summary>
        /// <param name="activeObject">Active object.</param>
        /// <param name="impulse">The applied impulse.</param>
        public void setImpulse(ActiveObject activeObject, Vector2 impulse)
        {
            activeObject.Body.ResetDynamics();
            activeObject.Body.ApplyLinearImpulse(impulse);
        }

        /// <summary>
        /// Implemented, because this class is an observer
        /// Everytime an observed object has changed, this method is called
        /// </summary>
        /// <param name=“observableObject”>The object which changed</param>
        public void notify(ObservableObject observableObject)
        {
            logger.log(Sender.Physics, this.ToString() + " received a notification", PriorityLevel.Priority_2);
        }

        /// <summary>
        /// Creates an explosion at the specified point.
        /// </summary>
        /// <param name="targetPoint"></param>
        /// <param name="ExplosionRange"></param>
        /// <param name="ExplosionMagnitude"></param>
        public void createExplosionAtPoint(Vector2 targetPoint, float explosionRange, float ExplosionMagnitude, bool isSuperBomb)
        {
            Vector2 forceDirection = new Vector2(0, 0);
            float distance = 0.0f;

            // Notify Graphics:
            ParticleSystemsManager.Instance.playBombExplosion(targetPoint, new Vector2(explosionRange*2), isSuperBomb);
            if (isSuperBomb)
            {
                Vector2 size = new Vector2(explosionRange*2);
                PassiveObject passive = new PassiveObject("exRad", true, StorageManager.Instance.getTextureByName("bomb_explosion"), null, null, size, targetPoint - size / 2, 56, MeasurementUnit.Meter);
                passive.BlendColor = Color.Yellow;
                FadingAnimation fading = new FadingAnimation(false, false, 250, true, 450);
                fading.Inverted = true;
                ScalingAnimation scaling = new ScalingAnimation(false, false, 0, true, 250);
                scaling.ScalingRange = new Vector2(0.05f, 1);
                passive.FadingAnimation = fading;
                passive.ScalingAnimation = scaling;

                SuperController.Instance.addGameObjectToGameInstance(passive);
                TaskManager.Instance.addTask(new GameLogicTask(400, passive));
            }

            foreach (Body body in getBodiesInCircle(targetPoint, explosionRange))
            {
                if (body.LinkedActiveObject is Bomb || body.BodyType == BodyType.Static)
                {
                    continue;
                }
                if (body.Position != targetPoint)
                {
                    forceDirection = Vector2.Normalize(body.Position - targetPoint);
                }
                else
                {
                    forceDirection = new Vector2(2*(float)random.NextDouble()-1, 2*(float)random.NextDouble()-1);
                }

                // Magic number for safety
                distance = Math.Max(0.2f, Vector2.Distance(body.Position, targetPoint));
                body.ApplyLinearImpulse((forceDirection / distance) * ExplosionMagnitude);

                // Stun affected players:
                if (body.LinkedActiveObject is Player)
                {
                    Player p = body.LinkedActiveObject as Player;
                    p.Flags.Add(PlayerFlag.Stunned);

                    int duration;
                    if (isSuperBomb)
                    {
                        duration = (int)SettingsManager.Instance.get("superBombStunDuration");
                        TaskManager.Instance.addTask(new PhysicsTask(duration, p, PhysicsTask.PhysicTaskType.RemoveStun));
                        TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.bombBig, (int)IngameSound.ExplosionBig));
                    }
                    else
                    {
                        duration = (int)SettingsManager.Instance.get("lavaStunDuration");
                        TaskManager.Instance.addTask(new PhysicsTask(duration, p, PhysicsTask.PhysicTaskType.RemoveStun));
                        TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.bombSmall, (int)IngameSound.ExplosionSmall));
                    }
                    ParticleSystemsManager.Instance.playStun(p, duration);
                }
            }


        }

        /// <summary>
        /// Returns all Bodies in a circle.
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public List<Body> getBodiesInCircle(Vector2 middle, float radius)
        {
            List<Body> result = new List<Body>();
            foreach (Body body in world.BodyList)
            {
                if (Vector2.Distance(middle, body.Position) < radius)
                {
                    result.Add(body);
                }
            }
            return result;
        }

        /// <summary>
        /// Removes the stun effect from a player.
        /// </summary>
        /// <param name="player"></param>
        private void removeStunFromPlayer(Player player)
        {
            player.Flags.Remove(PlayerFlag.Stunned);
        }

        /// <summary>
        /// Returns a random location at the top
        /// </summary>
        /// <returns></returns>
        public Vector2 getRandomPositionAtTheTop()
        {
            return getRandomPositionAtTheTop(-1);
        }

        /// <summary>
        /// Returns a random location at the top
        /// </summary>
        /// <returns></returns>
        public Vector2 getRandomPositionAtTheTop(int team)
        {
            float leftFactor = 0.1f;
            float rightFactor = 0.9f;

            switch (team)
            {
                case 1:
                    {
                        // Set the minimum to the right side of the field
                        leftFactor = 0.6f;
                        break;
                    }
                case 2:
                    {
                        // Set the maximum to the left side of the field
                        rightFactor = 0.4f;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            float xMin = levelSize.X * leftFactor;
            float xMax = levelSize.X * rightFactor;
            float xPos = Math.Min(Math.Max(levelSize.X * (float)random.NextDouble(), xMin), xMax);

            return new Vector2(xPos, 0);
        }

        /// <summary>
        /// Sets various borders that have to be manipulated after loading the game objects.
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="leftHandBlock"></param>
        /// <param name="rightHandBlock"></param>
        public void SetBorders(Border middle, Border leftHandBlock, Border rightHandBlock, Border rightSideHandDistance, Border leftSideHandDistance)
        {
            this.middleBorder = middle;
            this.leftHandBlockBorder = leftHandBlock;
            this.rightHandBlockBorder = rightHandBlock;
            this.rightSideHandDistanceBorder = rightSideHandDistance;
            this.leftSideHandDistanceBorder = leftSideHandDistance;
        }


        #endregion

        #region PowerUps

        /// <summary>
        /// Modifies the gravity, to simulate wind.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="on"></param>
        public void setWindPowerUpForTeam(int team, bool on)
        {
            if (on)
            {
                TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.windPowerup, (int)IngameSound.WindPowerUp, PowerUpManager.Instance.getDurationFromType(PowerUpType.Wind)));
                // Team identification has to be added.
                if (team == 1)
                {
                    world.Gravity = (Vector2)settingsManager.get("gravity") + (Vector2)settingsManager.get("windStrength");
                }
                else
                {
                    world.Gravity = (Vector2)settingsManager.get("gravity") - (Vector2)settingsManager.get("windStrength");
                }
            }
            else
            {
                world.Gravity = (Vector2)settingsManager.get("gravity");
            }
        }

        /// <summary>
        /// Modifies the jumpheight for a player.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="on"></param>
        public void setJumpheightPowerUpForTeam(int team, bool on)
        {
            if (on)
            {
                foreach (Player p in SuperController.Instance.getAllPlayers())
                {
                    if (p.Team == team)
                    {
                        p.Flags.Add(PlayerFlag.DoubleJump);
                    }
                }
            }
            else
            {
                foreach (Player p in SuperController.Instance.getAllPlayers())
                {
                    if (p.Team == team)
                    {
                        p.Flags.Remove(PlayerFlag.DoubleJump);
                    }
                }
            }
        }

        /// <summary>
        /// Modifies the gravity.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="on"></param>
        public void setNoGravityPowerUp(bool on)
        {
            if (on)
            {
                world.Gravity = new Vector2(0.0f, 0.0f);
            }
            else
            {
                // Default
                world.Gravity = (Vector2)settingsManager.get("gravity");
            }
        }

        /// <summary>
        /// Adds another ball.
        /// </summary>
        /// <param name="on"></param>
        /// <param name="activeObjectList">List of all active objects</param>
        public void setDoubleBallPowerUp()
        {
            foreach (Ball currentBall in SuperController.Instance.getAllBalls())
            {
                createBall(currentBall.Position);
                return;
            }

            // If no ball was found spawn the new ball at a later point
            TaskManager.Instance.addTask(new PhysicsTask(200, PhysicsTask.PhysicTaskType.CreateDoubleBall));
        }

        /// <summary>
        /// Creates a Ball.
        /// </summary>
        /// <param name="position">The position of the ball</param>
        private void createBall(Vector2 position)
        {
            // Get ball size
            Vector2 ballSize = (Vector2)SettingsManager.Instance.get("ballSize");

            // Create ballIndicator
            HUD ballIndicator = new HUD("BallIndicator", false, StorageManager.Instance.getTextureByName("BallIndicator"), null, new Vector2(Graphics.Instance.convertUnits(ballSize,
                MeasurementUnit.Meter, MeasurementUnit.PercentOfScreen).X, 0.02f), new Vector2(0.5f, 0.009f), 84, MeasurementUnit.PercentOfScreen);
            SuperController.Instance.addGameObjectToGameInstance(ballIndicator);

            // Add ball
            Ball newBall = new Ball("OriginalBall", true, StorageManager.Instance.getTextureByName("Ball"), null, null, ballSize, position, 52,
                Model.MeasurementUnit.Meter, ballIndicator);
            SuperController.Instance.addGameObjectToGameInstance(newBall);
            disableCollisionBetweenActiveObjects(newBall, middleBorder);
            disableCollisionBetweenActiveObjects(newBall, rightHandBlockBorder);
            disableCollisionBetweenActiveObjects(newBall, leftHandBlockBorder);
            disableCollisionBetweenActiveObjects(newBall, leftSideHandDistanceBorder);
            disableCollisionBetweenActiveObjects(newBall, rightSideHandDistanceBorder);

            // Add highlight
            PassiveObject ballHighlight = new PassiveObject("BallHighlight", true, StorageManager.Instance.getTextureByName("highlight_ball"), null, null, ballSize, position, 53,
                MeasurementUnit.Meter, null);
            SuperController.Instance.addGameObjectToGameInstance(ballHighlight);

            newBall.attachObject(ballHighlight);

            // Add Light if necessary
            Graphics.Instance.addLightToObject(newBall);
        }

        /// <summary>
        /// Creates an additional Ball.
        /// </summary>
        /// <param name="position"></param>
        /*
        private void createAdditionalBall(Vector2 position)
        {
            // Get ball size
            Vector2 ballSize = (Vector2)SettingsManager.Instance.get("ballSize");

            // Create ballIndicator
            HUD ballIndicator = new HUD("BallIndicator", false, StorageManager.Instance.getTextureByName("BallIndicator"), null, new Vector2(Graphics.Instance.convertUnits(ballSize,
                MeasurementUnit.Meter, MeasurementUnit.PercentOfScreen).X, 0.02f), new Vector2(0.5f, 0.009f), 84, MeasurementUnit.PercentOfScreen);
            SuperController.Instance.addGameObjectToGameInstance(ballIndicator);

            // Add ball
            Ball newBall = new Ball("PowerupBall", true, StorageManager.Instance.getTextureByName("Ball"), null, null, ballSize, position, 52,
                Model.MeasurementUnit.Meter, ballIndicator);
            SuperController.Instance.addGameObjectToGameInstance(newBall);
            disableCollisionBetweenActiveObjects(newBall, middleBorder);
            disableCollisionBetweenActiveObjects(newBall, rightHandBlockBorder);
            disableCollisionBetweenActiveObjects(newBall, leftHandBlockBorder);
            disableCollisionBetweenActiveObjects(newBall, leftSideHandDistanceBorder);
            disableCollisionBetweenActiveObjects(newBall, rightSideHandDistanceBorder);

            // Add Light if necessary
            Graphics.Instance.addLightToObject(newBall);
        }*/

        /// <summary>
        /// Creates lava.
        /// </summary>
        /// <param name="position"></param>
        private void createLava(Vector2 position)
        {
            // Create Bomb
            Bomb lava = new Bomb(StorageManager.Instance.getTextureByName("PowerUp_Lava"), 
                null, 
                null, 
                (Vector2)SettingsManager.Instance.get("lavaSize"), 
                new Vector2(position.X, position.Y - 1), 
                58,
                MeasurementUnit.Meter,
                false);

            lava.Body.ApplyLinearImpulse(new Vector2((float)(2 * random.NextDouble()) - 1, (float)(2 * random.NextDouble()) - 1));
            lava.Body.ApplyTorque(10 * (float)random.NextDouble());

            SuperController.Instance.addGameObjectToGameInstance(lava);
            ParticleSystemsManager.Instance.playBombFalling(lava);
            // Collision
            disableCollisionBetweenActiveObjects(lava, middleBorder);
            disableCollisionBetweenActiveObjects(lava, rightHandBlockBorder);
            disableCollisionBetweenActiveObjects(lava, leftHandBlockBorder);
            disableCollisionBetweenActiveObjects(lava, leftSideHandDistanceBorder);
            disableCollisionBetweenActiveObjects(lava, rightSideHandDistanceBorder);
        }

        /// <summary>
        /// Creates a SuperBomb.
        /// </summary>
        /// <param name="position"></param>
        private void createSuperBomb(Vector2 position)
        {
            // Create Bomb
            Bomb bomb = new Bomb(StorageManager.Instance.getTextureByName("PowerUp_Bomb"),
                null,
                null,
                (Vector2)SettingsManager.Instance.get("superBombSize"),
                position,
                58,
                MeasurementUnit.Meter,
                true);

            // Add a fading animation so that the black bomb fades out
            bomb.FadingAnimation = new FadingAnimation(false, false, 0, true, (int)SettingsManager.Instance.get("superBombDuration"));
            bomb.FadingAnimation.Inverted = true;

            // Create the bomb top
            Vector2 size = new Vector2((265f / 527f) * bomb.Size.X, (308f / 527f) * bomb.Size.Y);
            PassiveObject bomb_top = new PassiveObject("Bomb_top", true, StorageManager.Instance.getTextureByName("PowerUp_Bomb_top"), null, null,
                size, position, 59, MeasurementUnit.Meter);

            Vector2 offset = new Vector2(0.03f, -0.7f * bomb.Size.Y);
            bomb_top.Offset = offset;
            bomb_top.RotateWithOffset = true;

            bomb.attachObject(bomb_top);
            SuperController.Instance.addGameObjectToGameInstance(bomb_top);

            // Create the red background
            PassiveObject bomb_red = new PassiveObject("Bomb_red", true, StorageManager.Instance.getTextureByName("PowerUp_Bomb_red"), null, null,
                (Vector2)SettingsManager.Instance.get("superBombSize"), position, 57, MeasurementUnit.Meter);

            // Attach it to the bomb
            bomb.attachObject(bomb_red);
            SuperController.Instance.addGameObjectToGameInstance(bomb_red);

            // Save the created bomb.
            TaskManager.Instance.addTask(new PhysicsTask((int)SettingsManager.Instance.get("superBombDuration"), PhysicsTask.PhysicTaskType.DetonateSuperBomb));

            bomb.Body.Mass = (float)SettingsManager.Instance.get("superBombMass");

            bomb.Body.ApplyLinearImpulse(new Vector2((float)(2 * random.NextDouble()) - 1, (float)(2 * random.NextDouble()) - 1));
            bomb.Body.ApplyTorque(10 * (float)random.NextDouble());

            SuperController.Instance.addGameObjectToGameInstance(bomb);
            superBombList.Add(bomb);
            ParticleSystemsManager.Instance.playSparclingBomb(bomb, offset + new Vector2(0.05f, -0.18f), false);

            // Collision
            disableCollisionBetweenActiveObjects(bomb, middleBorder);
            disableCollisionBetweenActiveObjects(bomb, rightHandBlockBorder);
            disableCollisionBetweenActiveObjects(bomb, leftHandBlockBorder);
            disableCollisionBetweenActiveObjects(bomb, leftSideHandDistanceBorder);
            disableCollisionBetweenActiveObjects(bomb, rightSideHandDistanceBorder);
        }

        /// <summary>
        /// Explodes the superBomb.
        /// </summary>
        private void explodeSuperBomb()
        {
            if (superBombList.Count > 0)
            {
                createExplosionAtPoint(superBombList.First().Body.Position, (float)settingsManager.get("superBombRange"), (float)settingsManager.get("superBombImpact"), true);
                SuperController.Instance.removeGameObjectFromGameInstance(superBombList.First());

                while (superBombList.First().AttachedObjects.Count > 0)
                {
                    SuperController.Instance.removeGameObjectFromGameInstance(superBombList.First().AttachedObjects.First());
                }

                ParticleSystemsManager.Instance.stopSparcle(superBombList.First());
                superBombList.Remove(superBombList.First());
            }
        }

        #endregion

        #endregion


    }
}
