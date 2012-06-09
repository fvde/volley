using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;
using System;
using System.Collections.Generic;

namespace FarseerPhysics.HelloWorld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _batch;
        private KeyboardState _oldKeyState;
        private GamePadState _oldPadState;
        private SpriteFont _font;

        private World _world;

        private Body _circleBody;
        private Body _groundBody;
        private Body _leftBorder;
        private Body _rightBorder;
        private Body _topBorder;
        private Body _bottomBorder;

        // Blob
        // Blob Controller
        private bool _blobControlActive = false;
        private Body _blobMainBody;
        private List<Body> _blobBodies = new List<Body>();
        // The outer joins are the joins between the outer elements of the blob
        private List<DistanceJoint> _blobOuterJoints = new List<DistanceJoint>();
        // The inner joins are the joins between the outer elements of the blob and the blob
        private List<DistanceJoint> _blobInnerJoints = new List<DistanceJoint>();
        private int _blobBodyNumber = 25;
        private float _blobRadius = 1.0f;

        // Bridge
        private List<Body> _bridgeBodies;
        private int NumberOfBridgeElements = 12;
        private float BridgeElementSize = 1.0f;

        // Join test objects
        private Body _testBody1;


        private bool jointActive = false;
        private DistanceJoint testJoint;

        private Texture2D _circleSprite;
        private Texture2D _groundSprite;
        private Texture2D _bridgeSprite;

        // Simple camera controls
        private Matrix _view;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;


#if !XBOX360
        const string Text = "Press C to to switch to blob control (and back)\n" +
                            "Press J to activate/deactivate the distance join\n" +
                            "Press I to increase the distance of the join";
#else
                const string Text = "Use left stick to move\n" +
                                    "Use right stick to move camera\n" +
                                    "Press A to jump\n";
#endif
        // Farseer expects objects to be scaled to MKS (meters, kilos, seconds)
        // 1 meters equals 64 pixels here
        // (Objects should be scaled to be between 0.1 and 10 meters in size)
        private const float MeterInPixels = 64f;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 480;

            Content.RootDirectory = "Content";

            _world = new World(new Vector2(0, 5));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Initialize camera controls
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;

            _screenCenter = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2f,
                                                _graphics.GraphicsDevice.Viewport.Height / 2f);

            _batch = new SpriteBatch(_graphics.GraphicsDevice);
            _font = Content.Load<SpriteFont>("font");

            // Load sprites
            _circleSprite = Content.Load<Texture2D>("circleSprite"); //  96px x 96px => 1.5m x 1.5m
            _groundSprite = Content.Load<Texture2D>("groundSprite"); // 512px x 64px =>   8m x 1m
            _bridgeSprite = Content.Load<Texture2D>("bridgeSprite");

            /* Circle */
            // Convert screen center from pixels to meters
            Vector2 circlePosition = (_screenCenter / MeterInPixels) + new Vector2(0, -1.5f);

            // Create the circle fixture
            _circleBody = BodyFactory.CreateCircle(_world, 96f / (2f * MeterInPixels), 1f, circlePosition);
            _circleBody.BodyType = BodyType.Dynamic;

            // Give it some bounce and friction
            _circleBody.Restitution = 0.3f;
            _circleBody.Friction = 0.5f;

            

            /* Ground */
            Vector2 groundPosition = (_screenCenter / MeterInPixels) + new Vector2(0, 1.25f);
            Vector2 leftBorderPosition = new Vector2(this.GraphicsDevice.Viewport.Bounds.Left/MeterInPixels,this.GraphicsDevice.Viewport.Height /2 /MeterInPixels);
            Vector2 rightBorderPosition = new Vector2(this.GraphicsDevice.Viewport.Bounds.Right / MeterInPixels, this.GraphicsDevice.Viewport.Height / 2 / MeterInPixels);
            Vector2 bottomBorderPosition = new Vector2(this.GraphicsDevice.Viewport.Width / 2 / MeterInPixels,
                this.GraphicsDevice.Viewport.Bounds.Bottom  / MeterInPixels);
            Vector2 topBorderPosition = new Vector2(this.GraphicsDevice.Viewport.Width / 2 / MeterInPixels,
                this.GraphicsDevice.Viewport.Bounds.Top  / MeterInPixels);


            // Create the ground fixture
           
            _groundBody = BodyFactory.CreateRectangle(_world, 512f / MeterInPixels, 64f / MeterInPixels, 1f, groundPosition);
            _groundBody.IsStatic = true;
            _groundBody.Restitution = 0.3f;
            _groundBody.Friction = 0.5f;
            _groundBody.Enabled = false;
            
             



            // Boder


            _leftBorder = BodyFactory.CreateRectangle(_world, 1.0f / MeterInPixels, this.GraphicsDevice.Viewport.Height / MeterInPixels, 1f, leftBorderPosition);
            _leftBorder.IsStatic = true;
            _leftBorder.Restitution = 1.0f;
            _leftBorder.Friction = 0.0f;


            _rightBorder = BodyFactory.CreateRectangle(_world, 1.0f / MeterInPixels, this.GraphicsDevice.Viewport.Height / MeterInPixels, 1f, rightBorderPosition);
            _rightBorder.IsStatic = true;
            _rightBorder.Restitution = 1.0f;
            _rightBorder.Friction = 0.0f;

            _topBorder = BodyFactory.CreateRectangle(_world, this.GraphicsDevice.Viewport.Width / MeterInPixels, 1.0f / MeterInPixels, 1f, topBorderPosition);
            _topBorder.IsStatic = true;
            _topBorder.Restitution = 1.0f;
            _topBorder.Friction = 0.0f;

            _bottomBorder = BodyFactory.CreateRectangle(_world, this.GraphicsDevice.Viewport.Width / MeterInPixels, 1.0f / MeterInPixels, 1f, bottomBorderPosition);
            _bottomBorder.IsStatic = true;
            _bottomBorder.Restitution = 1.0f;
            _bottomBorder.Friction = 0.0f;
             

            Console.WriteLine(this.GraphicsDevice.Viewport.Width);
            Console.WriteLine(this.GraphicsDevice.Viewport.Height);

            

            //Blob Path

            Path rectanglePath = new Path();
            rectanglePath.Add(new Vector2(this.GraphicsDevice.Viewport.Bounds.Center.X - 6, this.GraphicsDevice.Viewport.Bounds.Center.X - 11) / MeterInPixels);
            rectanglePath.Add(new Vector2(this.GraphicsDevice.Viewport.Bounds.Center.X - 6, this.GraphicsDevice.Viewport.Bounds.Center.X + 1) / MeterInPixels);
            rectanglePath.Add(new Vector2(this.GraphicsDevice.Viewport.Bounds.Center.X + 6, this.GraphicsDevice.Viewport.Bounds.Center.X + 1) / MeterInPixels);
            rectanglePath.Add(new Vector2(this.GraphicsDevice.Viewport.Bounds.Center.X + 6, this.GraphicsDevice.Viewport.Bounds.Center.X - 11) / MeterInPixels);
            rectanglePath.Closed = true;
            /*
            //Creating two shapes. A circle to form the circle and a rectangle to stabilize the soft body.
            List<Shape> shapes = new List<Shape>(2);
            shapes.Add(new PolygonShape(PolygonTools.CreateRectangle(0.5f / MeterInPixels, 0.5f / MeterInPixels, new Vector2(-0.1f , 0f), 0f), 1f));
            shapes.Add(new CircleShape(0.5f / MeterInPixels, 1f));

            //We distribute the shapes in the rectangular path.
            _softBodies = PathManager.EvenlyDistributeShapesAlongPath(_world, rectanglePath, shapes,
                                                                      BodyType.Dynamic, 5);
            //_softBodyBox = new Sprite(ScreenManager.Assets.TextureFromShape(shapes[0], MaterialType.Blank, Color.Silver * 0.8f, 1f));
            //_softBodyBox.Origin += new Vector2(ConvertUnits.ToDisplayUnits(0.1f), 0f);
            //_softBodyCircle = new Sprite(ScreenManager.Assets.TextureFromShape(shapes[1], MaterialType.Waves, Color.Silver, 1f));

            //Attach the bodies together with revolute joints. The rectangular form will converge to a circular form.
            PathManager.AttachBodiesWithRevoluteJoint(_world, _softBodies, new Vector2(0f, -0.5f), new Vector2(0f, 0.5f),
                                                      true, true);
            **/
            /*
            //Bridge
            Path bridgePath = new Path();
            bridgePath.Add(leftBorderPosition);
            bridgePath.Add(rightBorderPosition);
            bridgePath.Closed = false;

            Vertices box = PolygonTools.CreateRectangle(0.125f * BridgeElementSize, 0.5f * BridgeElementSize);
            PolygonShape shape = new PolygonShape(box, 5);

            _bridgeBodies = PathManager.EvenlyDistributeShapesAlongPath(_world, bridgePath, shape,
                                                                        BodyType.Dynamic, NumberOfBridgeElements);


            for (int i = 0; i < _bridgeBodies.Count; ++i)
            {
                //_bridgeBodies[i].Restitution = 1.0f;
                //_bridgeBodies[i].Friction = 0.0f;
            }

            //Attach the first and last fixtures to the world
            JointFactory.CreateFixedRevoluteJoint(_world, _bridgeBodies[0], new Vector2(0f, -0.5f), leftBorderPosition);
            JointFactory.CreateFixedRevoluteJoint(_world, _bridgeBodies[_bridgeBodies.Count - 1], new Vector2(0, 0.5f), rightBorderPosition);

            //PathManager.AttachBodiesWithRevoluteJoint(_world, _bridgeBodies, new Vector2(0f, -0.5f), new Vector2(0f, 0.5f), false, true);
            PathManager.AttachBodiesWithSliderJoint(_world, _bridgeBodies, new Vector2(0f, -0.5f), new Vector2(0f, 0.5f), false, false, 5 / MeterInPixels, 10 / MeterInPixels);
            **/

            //Test objects to individually test Joints
            _testBody1 = BodyFactory.CreateCircle(_world, 96f / (2f * MeterInPixels), 1f, circlePosition);
            _testBody1.BodyType = BodyType.Dynamic;

            // Blob
            _blobMainBody = BodyFactory.CreateCircle(_world, 24f / (2f * MeterInPixels), 1f, circlePosition);
            _blobMainBody.BodyType = BodyType.Dynamic;

            // Initialize Blob surrounding bodies
            while (_blobBodyNumber > 0)
            {
                _blobBodyNumber--;
                _blobBodies.Add(BodyFactory.CreateCircle(_world, 24f / (2f * MeterInPixels), 1f, circlePosition));
            }

            // Adjust blob elements
            _blobBodies[0].BodyType = BodyType.Dynamic;
            for (int i = 1; i < _blobBodies.Count; ++i)
            {
                _blobBodies[i].BodyType = BodyType.Dynamic;
                _blobOuterJoints.Add(JointFactory.CreateDistanceJoint(_world, _blobBodies[i-1], _blobBodies[i], new Vector2(0, 0), new Vector2(0, 0)));
                _blobInnerJoints.Add(JointFactory.CreateDistanceJoint(_world, _blobMainBody, _blobBodies[i], new Vector2(0, 0), new Vector2(0, 0))); 
            }
            _blobInnerJoints.Add(JointFactory.CreateDistanceJoint(_world, _blobMainBody, _blobBodies[0], new Vector2(0, 0), new Vector2(0, 0))); 
            _blobOuterJoints.Add(JointFactory.CreateDistanceJoint(_world, _blobBodies[_blobBodies.Count-1], _blobBodies[0], new Vector2(0, 0), new Vector2(0, 0))); 

            //Adjust Joins

            for (int i = 0; i < _blobOuterJoints.Count; ++i)
            {
                _blobOuterJoints[i].Length = (float) (_blobRadius*2*Math.PI)/_blobBodies.Count;
                _blobInnerJoints[i].Length = _blobRadius;
                // Frequency determs how fast the bodies move back to default distance
                _blobOuterJoints[i].Frequency = 1.0f;
                _blobInnerJoints[i].Frequency = 15.0f;
                // Damping Ratio determs how "soft" the bodies will react to collisions: Higher Damping ratio means that the bodies will become more solid.
                _blobOuterJoints[i].DampingRatio = 0.5f;
                _blobInnerJoints[i].DampingRatio = 0.1f;
            }

            
           


        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            HandleGamePad();
            HandleKeyboard();

            //We update the world
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            base.Update(gameTime);
        }

        private void HandleGamePad()
        {
            GamePadState padState = GamePad.GetState(0);

            if (padState.IsConnected)
            {
                if (padState.Buttons.Back == ButtonState.Pressed)
                    Exit();

                if (padState.Buttons.A == ButtonState.Pressed && _oldPadState.Buttons.A == ButtonState.Released)
                    _circleBody.ApplyLinearImpulse(new Vector2(0, -10));

                _circleBody.ApplyForce(padState.ThumbSticks.Left);
                _cameraPosition.X -= padState.ThumbSticks.Right.X;
                _cameraPosition.Y += padState.ThumbSticks.Right.Y;

                _view = Matrix.CreateTranslation(new Vector3(_cameraPosition - _screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));

                _oldPadState = padState;
            }
        }

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

            // Switch between circle body and camera control
            if (state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift))
            {
                // Move camera
                if (state.IsKeyDown(Keys.A))
                    _cameraPosition.X += 4.5f;

                if (state.IsKeyDown(Keys.D))
                    _cameraPosition.X -= 4.5f;

                if (state.IsKeyDown(Keys.W))
                    _cameraPosition.Y += 4.5f;

                if (state.IsKeyDown(Keys.S))
                    _cameraPosition.Y -= 4.5f;

                



                _view = Matrix.CreateTranslation(new Vector3(_cameraPosition - _screenCenter, 0f)) *
                        Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));
            }
            else
            {
                // Stop it

                if (state.IsKeyDown(Keys.B) && _oldKeyState.IsKeyUp(Keys.B))
                {
                    _world.Enabled = !_world.Enabled;
                        
                }

                // We make it possible to rotate the circle body
                if (state.IsKeyDown(Keys.A))
                {
                    if (_blobControlActive)
                        _blobMainBody.ApplyForce(new Vector2(-10, 0));
                    else
                        _circleBody.ApplyForce(new Vector2(-5, 0));
                }

                if (state.IsKeyDown(Keys.D))
                {
                    if (_blobControlActive)
                        _blobMainBody.ApplyForce(new Vector2(10, 0));
                    else
                        _circleBody.ApplyForce(new Vector2(5, 0));
                }

                if (state.IsKeyDown(Keys.W))
                {
                    if (_blobControlActive)
                        _blobMainBody.ApplyForce(new Vector2(0, -10));
                    else
                        _circleBody.ApplyForce(new Vector2(0, -5));
                }
                    

                if (state.IsKeyDown(Keys.S))
                {
                    if (_blobControlActive)
                        _blobMainBody.ApplyForce(new Vector2(0, 10));
                    else
                        _circleBody.ApplyForce(new Vector2(0, 5));
                }
                    

                


                if (state.IsKeyDown(Keys.Space) && _oldKeyState.IsKeyUp(Keys.Space))
                    if (_blobControlActive)
                        _blobMainBody.ApplyLinearImpulse(new Vector2(0, -10));
                    else
                        _circleBody.ApplyLinearImpulse(new Vector2(0, -30));

                // [C]hange control to blob
                if (state.IsKeyDown(Keys.C) && _oldKeyState.IsKeyUp(Keys.C))
                    _blobControlActive = !_blobControlActive;

                if (state.IsKeyDown(Keys.R) && _oldKeyState.IsKeyUp(Keys.R))
                {
                    _circleBody.Position = new Vector2(this.GraphicsDevice.Viewport.Bounds.Center.X /MeterInPixels, this.GraphicsDevice.Viewport.Bounds.Center.Y /MeterInPixels);
                    Console.WriteLine(this.GraphicsDevice.Viewport.Bounds.Center.X);
                    Console.WriteLine(this.GraphicsDevice.Viewport.Bounds.Center.Y);
                    _circleBody.ResetDynamics();
                }

                // Press I to increase the distance of the joint.
                if (state.IsKeyDown(Keys.I) && _oldKeyState.IsKeyUp(Keys.I))
                {
                    testJoint.Length = testJoint.Length + 1;
                }


                if (state.IsKeyDown(Keys.J) && _oldKeyState.IsKeyUp(Keys.J))
                {
                    if (!jointActive)
                    {
                        // Distance joint, with the default distance of the position of the circle body minus the position of the test body 1.
                        testJoint = JointFactory.CreateDistanceJoint(_world, _circleBody, _testBody1, new Vector2(0, 0), new Vector2(0, 0));
                        // Frequency determs how fast the bodies move back to default distance
                        testJoint.Frequency = 1.0f;
                        // Damping Ratio determs how "soft" the bodies will react to collisions: Higher Damping ratio means that the bodies will become more solid.
                        testJoint.DampingRatio = 0.1f;

                    }
                    else
                    {
                        testJoint.Enabled = false;
                    }

                    jointActive = !jointActive;

                }
            }

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            _oldKeyState = state;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            /* Circle position and rotation */
            // Convert physics position (meters) to screen coordinates (pixels)
            Vector2 circlePos = _circleBody.Position * MeterInPixels;
            float circleRotation = _circleBody.Rotation;

            /* Ground position and origin */
            Vector2 groundPos = _groundBody.Position * MeterInPixels;
            Vector2 groundOrigin = new Vector2(_groundSprite.Width / 2f, _groundSprite.Height / 2f);

            // Align sprite center to body position
            Vector2 circleOrigin = new Vector2(_circleSprite.Width / 2f, _circleSprite.Height / 2f);

            _batch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);

            //Draw circle
            _batch.Draw(_circleSprite, circlePos, null, Color.White, circleRotation, circleOrigin, 1f, SpriteEffects.None, 0f);

            //Draw ground
            //_batch.Draw(_groundSprite, groundPos, null, Color.White, 0f, groundOrigin, 1f, SpriteEffects.None, 0f);

            /*
            // Draw Blob

            for (int i = 0; i < _softBodies.Count; ++i)
            {
                _batch.Draw(_circleSprite, _softBodies[i].Position, null,
                                               Color.White, _softBodies[i].Rotation, _softBodies[i].Position, 1f,
                                               SpriteEffects.None, 0f);

            }
            
             */
            /*
            // Draw Bridge
            Vector2 bridgeOrigin = new Vector2(_bridgeSprite.Width / 2f, _bridgeSprite.Height / 2f);
            for (int i = 0; i < _bridgeBodies.Count; ++i)
            {
                _batch.Draw(_bridgeSprite, _bridgeBodies[i].Position * MeterInPixels, null,
                                               Color.White, _bridgeBodies[i].Rotation, bridgeOrigin, 1f,
                                               SpriteEffects.None, 0f);
            }
            **/
            
            // Display instructions
            _batch.DrawString(_font, Text, new Vector2(14f, 14f), Color.Black);
            _batch.DrawString(_font, Text, new Vector2(12f, 12f), Color.White);
            
            //Draw testing objects
            Vector2 testObject1Pos = _testBody1.Position * MeterInPixels;
            _batch.Draw(_circleSprite, testObject1Pos, null, Color.White, _testBody1.Rotation, circleOrigin, 1f, SpriteEffects.None, 0f);

            //Draw blob
            Vector2 blobPos = _blobMainBody.Position * MeterInPixels;
            Vector2 blobOrigin = new Vector2(_bridgeSprite.Width / 2f, _bridgeSprite.Height / 2f);
            _batch.Draw(_bridgeSprite, blobPos, null, Color.Black, _blobMainBody.Rotation, blobOrigin, 1f, SpriteEffects.None, 0f);

            for (int i = 0; i < _blobBodies.Count; ++i)
            {
                _batch.Draw(_bridgeSprite, _blobBodies[i].Position * MeterInPixels, null,
                                               Color.White, _blobBodies[i].Rotation, blobOrigin, 1f,
                                               SpriteEffects.None, 0f);
            }


            _batch.End();

            base.Draw(gameTime);
        }
    }
}