//#define SHOW_CURVE_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManhattanMorning.Misc;
using ManhattanMorning.Misc.Curves;
using ManhattanMorning.Model;
using ManhattanMorning.Misc.Tasks;
using ManhattanMorning.Model.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Model.ParticleSystem;
using ManhattanMorning.Controller;
using ManhattanMorning.Model.HUD;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Input;

namespace ManhattanMorning.View
{
    public struct Circle
    {
        public Vector2 Position;
        public Vector2 TextureCoordinate;
        public float Radius;
    }

    enum RenderSettings
    {
        Uninitialized,
        Default,
        Player,
        Particle,
        Light
    }

    /// <summary>
    /// Graphic engine.
    /// Uses XNA to draw the Objects.
    /// </summary>
    class Graphics : IObserver
    {
        
        #region Properties

        /// <summary>
        /// Only Readable Access. For now there is only one Camera.
        /// </summary>
        public Camera Camera { get { return camera; } }

        /// <summary>
        /// Implementation of Singleton Pattern
        /// </summary>
        public static Graphics Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Graphics();
                }
                return instance;
            }
        }

        /// <summary>
        /// The scene is tinted in this color. So it is possbile to make a scene darker, or brighter...
        /// </summary>
        public Color Mood { get { return mood; } set { mood = value; } }

        /// <summary>
        /// True if it is night.
        /// </summary>
        public bool IsNight
        {
            get { return isNight; }
        }

        #endregion

        #region Members

        /// <summary>
        /// Current GameTime instance.
        /// </summary>
        private GameTime gameTime;

        /// <summary>
        /// the camera (viewpoint)
        /// </summary>
        private Camera camera;

        // Implementation of the Singleton Pattern
        private static Graphics instance;

        /// <summary>
        /// Tempfiles actual object size and position in pixel.
        /// </summary>
        private Vector2 objectSize = new Vector2(); //in Pixel
        private Vector2 objectPosition = new Vector2(); // in Pixel

        //TODO add xml comment
        private GraphicsDeviceManager graphicsManager;

        /// <summary>
        /// The instance of the logger.
        /// Needed to print out/log informations or errors.
        /// </summary>
        private Logger logger;

        /// <summary>
        /// the game instance.
        /// For easy access on all game Members.
        /// </summary>
        private Game1 game;

        //TODO add xml comment
        private SpriteBatch spriteBatchAll, spriteBatchHUD, spriteBatchLight;

        /// <summary>
        /// Bounding Boxes Visibility and Grid visibility.
        /// </summary>
        private bool showBoundingBoxes, showGrid;

        /// <summary>
        /// The shader files
        /// </summary>
        private Effect motionBlur;
        private Effect softShadow;
        private Effect textureShader;
        private Effect sobelEdge;
        private Effect toonShader;

        //TODO add xml comment
        private BasicEffect basicEffect;

        /// <summary>
        /// temporary Rendertarget to apply Effects
        /// </summary>
        private RenderTarget2D rtTempBuffer, rtHUD, rtLight, rtPlayer;
        private RenderTarget2D[] rtBall = new RenderTarget2D[3];

        /// <summary>
        /// The rendertarget for balls we want to draw in. Used for motion blur.
        /// Values: 0-2
        /// </summary>
        private int ballFrame;

        /// <summary>
        /// Counts frames since a ball is on the screen.
        /// </summary>
        private int framesWithBall;

        /// <summary>
        /// The height of the current viewport (window)
        /// </summary>
        private float viewPortHeight;

        /// <summary>
        /// the width of the current viewport (window)
        /// </summary>
        private float viewPortWidth;

        /// <summary>
        /// Spritefont placeholder, used to draw different sprites
        /// </summary>
        SpriteFont font1;

        /// <summary>
        /// Point Texture
        /// </summary>
        Texture2D pointTex;

        #if DEBUG
        /// <summary>
        /// Point Texture
        /// </summary>
        Texture2D mouseTex;
        MouseState mouseState;
        Manipulator activeManipulator = null;
        #endif

        /// <summary>
        /// Point Texture
        /// </summary>
        Texture2D dotTexture;

        /// <summary>
        /// Counts frames per seconds
        /// </summary>
        private float elapsedGametime = 0;

        /// <summary>
        /// Counts frames since last FPS update.
        /// </summary>
        private float frameCounter = 0;

        /// <summary>
        /// Stores the latest FPS.
        /// </summary>
        private float framesPerSecond = 0;

        /// <summary>
        /// Instance of the settingsManager
        /// Used to apply settings concerning the graphic
        /// </summary>
        private static SettingsManager settings = SettingsManager.Instance;

        /// <summary>
        /// a multiplier needed because the physic engine internal uses an external provides all
        /// sizes an positions in meters.
        /// Can be changed in the settings.
        /// </summary>
        private float meterToPixel;

        /// <summary>
        /// Size of the hand compared to the actual blob.
        /// </summary>
        private float playerHandRatio;

        //TODO add xml comment
        private SpriteEffects flipHorizontally;

        /// <summary>
        /// Vertices List for the Draw Rectangle Method
        /// </summary>
        VertexPositionColor[] vertices = new VertexPositionColor[8];

        /// <summary>
        /// Vertex List for the Draw Circle Method. Vertex every 10 degrees + 1 center.
        /// </summary>
        VertexPositionTexture[] verticesCircle = new VertexPositionTexture[37];

        /// <summary>
        /// Indices of the vertices for the circle. 3 indices per Triangle.
        /// </summary>
        short[] indicesCircle = new short[36 * 3];

        /// <summary>
        /// Vertex List for the Draw Blob Method.
        /// </summary>
        VertexPositionTexture[] verticesConnection = new VertexPositionTexture[4];

        /// <summary>
        /// Indices of the vertices for the blob. 3 indices per Triangle.
        /// </summary>
        short[] indicesConnection = new short[6];

        /// <summary>
        /// Used for positioning depending on measurement Unit of an Object.
        /// Only transforms the camera Matrix, not the Object itself.
        /// </summary>
        private Vector2 transformedPosition;

        /// <summary>
        /// Used for resizing depending on measurement Unit of an Object.
        /// Only transforms the camera Matrix, not the Object itself.
        /// </summary>
        private Vector2 transformedSize;

        /// <summary>
        /// Creates a Matrix that is used in the textureShader.
        /// </summary>
        private Matrix worldViewProjMatrix, viewMatrix, projectionMatrix;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random randomizer = new Random();

        /// <summary>
        /// The scene is tinted in this color. So it is possbile to make a scene darker, or brighter...
        /// </summary>
        private Color mood;

        /// <summary>
        /// Stores if sunset, sunrise or other color fadings are active.
        /// 0 = no fading, 1 = sunset, 2 = sunrise
        /// </summary>
        private int fadingState;

        /// <summary>
        /// Pauses the Graphic.
        /// </summary>
        private bool paused;

        /// <summary>
        /// Reference to the gameObjects so not every methods needs them as parameter.
        /// </summary>
        private LayerList<LayerInterface> gameObjects;

        /// <summary>
        /// Time of night between sunset and sunrise.
        /// </summary>
        private int nightDurationTime;

        /// <summary>
        /// True if it is night.
        /// </summary>
        private bool isNight = false;

        /// <summary>
        /// Stores the current render settings.
        /// </summary>
        private RenderSettings currentRenderSettings;

        /// <summary>
        /// Stores the current Blendmode used for drawing.
        /// </summary>
        private BlendState lastBlendMode;

        /// <summary>
        /// Stencil that writes ones to the RendertargetStencil when sometinh is drawn to that pixel.
        /// </summary>
        DepthStencilState stampStencil;

        /// <summary>
        /// Stencil that only writes to pixels where the RendertargetStencil contains ones.
        /// </summary>
        DepthStencilState cutterStencil;

        /// <summary>
        /// Alphatest that only draws to pixels which don't have alpha value of zero.
        /// </summary>
        AlphaTestEffect alphaTestEffect;

        #endregion


        #region Initialization

        /// <summary>
        /// Creates new Graphics-System
        /// </summary>
        private Graphics()
        {
            game = Game1.Instance;
            graphicsManager = game.GraphicsDeviceManager;
            logger = Logger.Instance;

            // Register as an observer for SettingsManager
            settings.registerObserver(this);

            //Get Viewport Parameters to set up camera
            viewPortHeight = graphicsManager.GraphicsDevice.Viewport.Height;
            viewPortWidth = graphicsManager.GraphicsDevice.Viewport.Width;

            //Create Camera
            camera = new Camera(viewPortWidth, viewPortHeight);            

            //create Player
            createTexturedCircle();

            //Load Content for test purposes
            spriteBatchAll = new SpriteBatch(game.GraphicsDevice);
            spriteBatchHUD = new SpriteBatch(game.GraphicsDevice);
            spriteBatchLight = new SpriteBatch(game.GraphicsDevice);
            motionBlur = game.Content.Load<Effect>("BlurShader");
            softShadow = game.Content.Load<Effect>("SoftShadow");
            textureShader = game.Content.Load<Effect>("TextureShader");
            sobelEdge = game.Content.Load<Effect>("SobelEdgeFilter");
            toonShader = game.Content.Load<Effect>("ToonOutline");

            basicEffect = new BasicEffect(game.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            // projection uses CreateOrthographicOffCenter to create 2d projection
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, viewPortWidth, viewPortHeight, 0, 0, 1);

            //create stencils and alphatest for HUD
            stampStencil = new DepthStencilState();
            stampStencil.StencilEnable = true;
            stampStencil.StencilFunction = CompareFunction.Always;
            stampStencil.ReferenceStencil = 0;
            stampStencil.StencilPass = StencilOperation.Replace;
            stampStencil.DepthBufferEnable = true;

            cutterStencil = new DepthStencilState();
            cutterStencil.StencilEnable = true;
            cutterStencil.StencilFunction = CompareFunction.Equal;
            cutterStencil.ReferenceStencil = 1;
            cutterStencil.StencilPass = StencilOperation.Replace;
            cutterStencil.DepthBufferEnable = true;

            alphaTestEffect = new AlphaTestEffect(game.GraphicsDevice);
            alphaTestEffect.VertexColorEnabled = true;
            alphaTestEffect.DiffuseColor = Color.White.ToVector3();
            alphaTestEffect.AlphaFunction = CompareFunction.NotEqual;
            alphaTestEffect.ReferenceAlpha = 0;
            alphaTestEffect.World = Matrix.Identity;
            alphaTestEffect.View = viewMatrix;
            alphaTestEffect.Projection = projectionMatrix;

            //Create Rendertarget with same size as Backbuffer
            rtTempBuffer = new RenderTarget2D(game.GraphicsDevice, graphicsManager.PreferredBackBufferWidth, graphicsManager.PreferredBackBufferHeight);
            rtHUD = new RenderTarget2D(game.GraphicsDevice, graphicsManager.PreferredBackBufferWidth, graphicsManager.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            rtLight = new RenderTarget2D(game.GraphicsDevice, graphicsManager.PreferredBackBufferWidth, graphicsManager.PreferredBackBufferHeight);
            rtPlayer = new RenderTarget2D(game.GraphicsDevice, graphicsManager.PreferredBackBufferWidth, graphicsManager.PreferredBackBufferHeight);

            rtBall[0] = new RenderTarget2D(game.GraphicsDevice, graphicsManager.PreferredBackBufferWidth, graphicsManager.PreferredBackBufferHeight);
            rtBall[1] = new RenderTarget2D(game.GraphicsDevice, graphicsManager.PreferredBackBufferWidth, graphicsManager.PreferredBackBufferHeight);
            rtBall[2] = new RenderTarget2D(game.GraphicsDevice, graphicsManager.PreferredBackBufferWidth, graphicsManager.PreferredBackBufferHeight);

            font1 = game.Content.Load<SpriteFont>("SpriteFont1");

            pointTex = new Texture2D(graphicsManager.GraphicsDevice, 1, 1);
            pointTex.SetData<Color>(new Color[1] { Color.White });

        #if DEBUG
            mouseState = Mouse.GetState();
            dotTexture = game.Content.Load<Texture2D>("Textures/HUD/dot");
            mouseTex = game.Content.Load<Texture2D>("Textures/HUD/mouse");
        #endif

            Init();
        }

        /// <summary>
        /// Called by game1
        /// Creates the instance at the beginning of the program
        /// </summary>
        public void Init()
        {
            Logger.Instance.log(Sender.Graphics, "initialized", PriorityLevel.Priority_2);

            meterToPixel = (float)settings.get("meterToPixel");
            playerHandRatio = (float)settings.get("playerHandRatio");
            showBoundingBoxes = (bool)settings.get("showBoundingBoxes");
            showGrid = (bool)settings.get("showGrid");
            nightDurationTime = (int)SettingsManager.Instance.get("PowerupSunsetDuration");

            mood = Color.White;
            lerpC = Color.White;
            fadeTime = 0;
        }

        #endregion

        #region Methods

        #region Update

        /// <summary>
        /// is called by game1, all the functionality is implemented
        /// in this method or a submethod
        /// </summary>
        public void update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            updateFade(gameTime);

            getTasks();

            if (paused) return;
            switch (fadingState)
            {
                case 1:
                    sunset(gameTime);
                    break;
                case 2:
                    sunrise(gameTime);
                    break;
            }
        }

        private void updateFade(GameTime gameTime)
        {
            if (fadeTime != 0)
            {
                float elapsed = (float)(gameTime.TotalGameTime.TotalMilliseconds - fadeTimeStart);
                lerpC = Color.Lerp(startColor, endColor, elapsed / fadeTime);
                if (elapsed > fadeTime) fadeTime = 0;
            }
        }

        #endregion


        #region Draw Methods
        /// <summary>
        /// Is called by game1, all the drawing is done in this method
        /// or in a submethod
        /// </summary>
        public void drawGame(GameTime gameTime, LayerList<LayerInterface> gameObjectList)
        {
            gameObjects = gameObjectList;
            //update shader variables
            viewMatrix = camera.getCameraMatrix();
            worldViewProjMatrix = viewMatrix * Matrix.CreateOrthographicOffCenter(0, viewPortWidth, viewPortHeight, 0, 0, 1);            
            projectionMatrix = Matrix.CreateOrthographicOffCenter(0, viewPortWidth, viewPortHeight, 0, 0, 1);

            textureShader.Parameters["worldViewProj"].SetValue(worldViewProjMatrix);
            textureShader.Parameters["tintingColor"].SetValue(mood.ToVector4());

            currentRenderSettings = RenderSettings.Uninitialized;
            #region NEW Draw Layers
            //prepare balls
            if (gameObjects.GetBalls().Count > 0)
            {
                game.GraphicsDevice.SetRenderTarget(rtBall[ballFrame]);
                game.GraphicsDevice.Clear(Color.Transparent);
                spriteBatchAll.Begin(0, null, null, null, null, null, viewMatrix);

                foreach (Ball b in gameObjects.GetBalls())
                {
                    drawObjectWithTextureOrAnimation(b as DrawableObject);
                }

                spriteBatchAll.End();
                
            }
            else
            {
                framesWithBall = 0;
            }
            if (gameObjects.GetPlayer().Count > 0)
            {
                game.GraphicsDevice.SetRenderTarget(rtPlayer);
                game.GraphicsDevice.Clear(Color.Transparent);
                textureShader.CurrentTechnique = textureShader.Techniques["MapTexture"];
                spriteBatchAll.Begin(SpriteSortMode.FrontToBack, null, null, null, null, textureShader, viewMatrix);

                foreach (Player p in gameObjects.GetPlayer())
                {
                    drawPlayer(p);
                }
                spriteBatchAll.End();
            }

            //draw all objects
            game.GraphicsDevice.SetRenderTarget(rtTempBuffer);
            game.GraphicsDevice.Clear(Color.Transparent);

            DrawableObject drawableObject = null;
            ParticleSystem particleSystem = null;

            //begin batches for HUD stencil and lights
            spriteBatchHUD.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null);
            spriteBatchLight.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            foreach (LayerInterface l in gameObjects)
            {
                //check if object is DrawableObject or ParticleSystem
                if (l is DrawableObject)
                {
                    drawableObject = l as DrawableObject;
                    //if (drawableObject.Layer >= 70) break;
                    if (!drawableObject.Visible || drawableObject == null) continue;

                    if (drawableObject.FlipHorizontally == true) flipHorizontally = SpriteEffects.FlipHorizontally;
                    else flipHorizontally = SpriteEffects.None;
                    
                    //draw Player
                    if (drawableObject is Player)
                    {
                        if (currentRenderSettings != RenderSettings.Player)
                            changeRenderSettings(RenderSettings.Player, null);
                        //drawPlayer(drawableObject as Player);

                        sobelEdge.Parameters["yTexture"].SetValue(rtPlayer);
                        spriteBatchAll.Draw(rtPlayer, new Rectangle(0, 0, game.GraphicsDeviceManager.PreferredBackBufferWidth, game.GraphicsDeviceManager.PreferredBackBufferHeight), Color.White);
                    }
                    else
                        //draw Ball
                        if (drawableObject is Ball)
                        {
                            if (currentRenderSettings != RenderSettings.Default)
                                changeRenderSettings(RenderSettings.Default, null);
                            
                            if (framesWithBall > 1) spriteBatchAll.Draw(rtBall[(ballFrame - 2 + 3) % 3], new Rectangle(0, 0, game.GraphicsDeviceManager.PreferredBackBufferWidth, game.GraphicsDeviceManager.PreferredBackBufferHeight),
                                drawableObject.BlendColor * 0.2f);
                            if (framesWithBall > 0) spriteBatchAll.Draw(rtBall[(ballFrame - 1 + 3) % 3], new Rectangle(0, 0, game.GraphicsDeviceManager.PreferredBackBufferWidth, game.GraphicsDeviceManager.PreferredBackBufferHeight),
                                drawableObject.BlendColor * 0.5f);
                            spriteBatchAll.Draw(rtBall[ballFrame], new Rectangle(0, 0, game.GraphicsDeviceManager.PreferredBackBufferWidth, game.GraphicsDeviceManager.PreferredBackBufferHeight), drawableObject.BlendColor);                            
                        }
                        //draw every other DrawableObject
                        else
                        {
                            if (currentRenderSettings != RenderSettings.Default)
                                changeRenderSettings(RenderSettings.Default, null);
                            drawObjectWithTextureOrAnimation(drawableObject);
                        }                    
                }
                else
                {
                    particleSystem = l as ParticleSystem;
                    if (currentRenderSettings != RenderSettings.Particle)
                        changeRenderSettings(RenderSettings.Particle, particleSystem.SystemBlendState);
                    drawParticleSystem(particleSystem);
                }
            }
            spriteBatchAll.End();
            if (gameObjects.GetBalls().Count > 0)
            {
                ballFrame = (ballFrame + 1) % 3; //change rendertarget of ball for motion blur
                framesWithBall++;
            }
            #endregion

            #region Lights

            //draw all lights into special rendertarget
            game.GraphicsDevice.SetRenderTarget(rtLight);
            game.GraphicsDevice.Clear(Color.Black);
            spriteBatchLight.End();

            //draw everything into rtHUD which has a stencilbuffer so everything except object >= layer 70 are cut out. alphatest makes sure that nothing with alpha value 0 is drawn
            game.GraphicsDevice.SetRenderTarget(rtHUD);
            game.GraphicsDevice.DepthStencilState = stampStencil;
            game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, Color.Transparent, 0, 0);
            spriteBatchHUD.End();
            
            spriteBatchHUD.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, cutterStencil, null, alphaTestEffect);
            spriteBatchHUD.Draw(rtTempBuffer, rtTempBuffer.Bounds, Color.White);
            spriteBatchHUD.End();
            
            //draw to backbuffer
            textureShader.CurrentTechnique = textureShader.Techniques["Light"];
            textureShader.Parameters["yTexture"].SetValue(rtTempBuffer);
            textureShader.CurrentTechnique.Passes[0].Apply();

            game.GraphicsDevice.SetRenderTarget(null);
            game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            game.GraphicsDevice.Clear(Color.Transparent);
            
            //draw everything with mood and lights
            spriteBatchAll.Begin(SpriteSortMode.Deferred, null, null, null, null, textureShader);
            spriteBatchAll.Draw(rtLight, rtLight.Bounds, lerpC);
            spriteBatchAll.End();

            //draw hud overlay without mood!
            spriteBatchAll.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null);
            spriteBatchAll.Draw(rtHUD, rtHUD.Bounds, lerpC);
            spriteBatchAll.End();
            
            #endregion

            #region Bounding Boxes
            if (showBoundingBoxes)
                drawBoundingBoxes(gameObjects);
            #endregion

            #region Grid
            if (showGrid)
                drawGrid();
            #endregion

            #region FPS & GameTime
            drawFPS(gameTime);
            #endregion

            #region AnimationPath Visualization
            #if DEBUG            
            #if SHOW_CURVE_EDITOR
             this.mouseState = Mouse.GetState();
             spriteBatchAll.Begin();
                foreach (LayerInterface l in gameObjects)
                {
                    if (l is DrawableObject) drawableObject = l as DrawableObject; else continue;
                    if (drawableObject.PathAnimation != null)
                    {
                        if (activeManipulator == null || drawableObject.PathAnimation.Path.Manipulator.IsActive)
                        {
                            activeManipulator = drawableObject.PathAnimation.Path.Manipulator;
                            drawableObject.PathAnimation.Path.Manipulator.handleInteraction(this.mouseState);
                            if (activeManipulator.IsActive == false) activeManipulator = null;
                        }

                        if (drawableObject.PathAnimation.Path.GetType() == typeof(Curve2D))
                        {

                          

                            for (float i = 0; i < 1f; i += 0.0001f)
                            {
                                spriteBatchAll.Draw(pointTex, convertUnits(drawableObject.PathAnimation.Path.evaluate(i), MeasurementUnit.Meter, MeasurementUnit.Pixel), Color.Green);
                            }
                            //Draw Handles
                            foreach (Vector2 point in ((Curve2D)drawableObject.PathAnimation.Path).getPoints())
                            {
                                spriteBatchAll.Draw(dotTexture, convertUnits(point, MeasurementUnit.Meter, MeasurementUnit.Pixel), null, Color.White, 0f, new Vector2(dotTexture.Width * 0.5f, dotTexture.Height * 0.5f), 0.1f, SpriteEffects.None, 0f);
                            }


                        }
                        if (drawableObject.PathAnimation.Path.GetType() == typeof(BezierBatch))
                        {


                            foreach (Bezier curve in ((BezierBatch)drawableObject.PathAnimation.Path).getCurves())
                            {
                                for (float i = 0; i < 1f; i += 0.0001f)
                                {
                                    spriteBatchAll.Draw(pointTex, convertUnits(drawableObject.PathAnimation.Path.evaluate(i), MeasurementUnit.Meter, MeasurementUnit.Pixel), Color.Red);
                                }
                            }

                            //Draw Lines
                            foreach (Bezier curve in ((BezierBatch)drawableObject.PathAnimation.Path).getCurves())
                            {

                                DrawLine(m2pix(curve.P0), m2pix(curve.P1));
                                DrawLine(m2pix(curve.P2), m2pix(curve.P3));
                            }
                            //Draw Handles
                            //Color col = Color.White;
                            foreach (Vector2 point in ((BezierBatch)drawableObject.PathAnimation.Path).getPoints())
                            {                                
                                Vector2 pos = convertUnits(point, MeasurementUnit.Meter, MeasurementUnit.Pixel);
                                spriteBatchAll.Draw(dotTexture, pos, null, Color.White, 0f, new Vector2(dotTexture.Width * 0.5f, dotTexture.Height * 0.5f), 0.1f, SpriteEffects.None, 0f);
                                //col.R -= 13;
                                //col.G -= 7;
                                //spriteBatchAll.DrawString(font1, pos.X /100 + "/" + pos.Y/100, pos, col);
                            }
                        }
                    }
                }


                //Draw Mouse
                mouseState = Mouse.GetState();
                spriteBatchAll.Draw(mouseTex, new Vector2(mouseState.X, mouseState.Y), null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                spriteBatchAll.End();
                
            #endif            
            #endif
            #endregion
        }

        /// <summary>
        /// Is called by game1, all the drawing is done in this method
        /// or in a submethod
        /// </summary>
        /// <param name="gameTime">global game time</param>
        /// <param name="menuObjects">the menu objects that should be drawn</param>
        public void drawMenu(GameTime gameTime, LayerList<LayerInterface> menuObjects)
        {
            // Draw Layer 85-100 (Menu)

            spriteBatchAll.Begin();

            foreach (DrawableObject drawableObject in menuObjects)
            {
                if (!(drawableObject is MenuButtonObject) && !(drawableObject is MenuObject) || !drawableObject.Visible) continue;
                calculateObjectSizeAndPosition(drawableObject);
                if (drawableObject.Texture != null)
                {
                    drawObjectWithTextureOrAnimation(drawableObject);
                }
            }

            if (Game1.VideoPlaying)
            {
                spriteBatchAll.Draw(Game1.videoPlayer.GetTexture(), new Rectangle(game.GraphicsDevice.Viewport.X, game.GraphicsDevice.Viewport.Y,
                                                        game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height), lerpC);
            }

            spriteBatchAll.End();
        }

        /// <summary>
        /// Draws a video texture. The video is set in Game1.
        /// </summary>
        public void drawVideo(GameTime gameTime)
        {
            updateFade(gameTime);

            spriteBatchAll.Begin();
            spriteBatchAll.Draw(Game1.videoPlayer.GetTexture(), new Rectangle(game.GraphicsDevice.Viewport.X, game.GraphicsDevice.Viewport.Y,
                                                    game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height), lerpC);
            spriteBatchAll.End();
        }

        /// <summary>
        /// Adds FPS to top right of the screen.
        /// </summary>
        /// <param name="gameTime">Current GameTime.</param>
        private void drawFPS(GameTime gameTime)
        {
            elapsedGametime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedGametime > 1.0f)
            {
                framesPerSecond = frameCounter / elapsedGametime;
                elapsedGametime -= 1f;
                frameCounter = 0;
            }
            else
            {
                frameCounter++;
            }

            spriteBatchAll.Begin();
            spriteBatchAll.DrawString(font1, Convert.ToString(framesPerSecond), new Vector2(viewPortWidth - 170, 10), Color.IndianRed);
            spriteBatchAll.End();
        }

        /// <summary>
        /// Draws a ParticleSystem with all Emitters and Orbitters.
        /// </summary>
        /// <param name="particleSystem">The ParticleSystem you want to draw.</param>
        private void drawParticleSystem(ParticleSystem particleSystem)
        {
            //draw Emitters
            foreach (Emitter e in particleSystem.EmitterList)
            {
                if(e.Visible)
                foreach (Particle p in e.ParticleList)
                {
                    if (p.Visible)
                    {
                        spriteBatchAll.Draw(e.ParticleTexture,
                            new Rectangle((int)((p.Position.X) * meterToPixel), (int)((p.Position.Y) * meterToPixel), (int)(p.Size.X * meterToPixel), (int)(p.Size.Y * meterToPixel)),
                            null, e.ParticleColor * p.Alpha, p.Rotation, new Vector2(e.ParticleTexture.Width, e.ParticleTexture.Height) / 2, SpriteEffects.None, (p.Age / p.LifeTime));

                        if(particleSystem.Layer >= 70)
                            spriteBatchHUD.Draw(e.ParticleTexture,
                                new Rectangle((int)((p.Position.X) * meterToPixel), (int)((p.Position.Y) * meterToPixel), (int)(p.Size.X * meterToPixel), (int)(p.Size.Y * meterToPixel)),
                                null, e.ParticleColor * p.Alpha, p.Rotation, new Vector2(e.ParticleTexture.Width, e.ParticleTexture.Height) / 2, SpriteEffects.None, (p.Age / p.LifeTime));
                    }
                }
            }

            //draw Orbitters
            foreach (Orbitter o in particleSystem.OrbitterList)
            {
                if (o.Visible)
                {
                    Matrix rotation = Matrix.CreateRotationZ(o.Rotation);

                    foreach (Particle p in o.getParticles())
                    {
                        if (p.Visible == true)
                        {
                            objectPosition = p.Position;
                            objectPosition = Vector2.Transform(objectPosition, rotation);
                            objectPosition += o.Position;

                            spriteBatchAll.Draw(o.ParticleTexture,
                                new Rectangle((int)((objectPosition.X - o.ParticleSize.X * 0.5f) * meterToPixel), (int)((objectPosition.Y - o.ParticleSize.Y * 0.5f) * meterToPixel),
                                (int)(o.ParticleSize.X * meterToPixel), (int)(o.ParticleSize.Y * meterToPixel)),
                                null, o.Color * p.Alpha, p.Rotation, new Vector2(o.ParticleTexture.Width, o.ParticleTexture.Height) / 2, SpriteEffects.None, 0);
                            if(particleSystem.Layer >= 70)
                                spriteBatchHUD.Draw(o.ParticleTexture,
                                    new Rectangle((int)((objectPosition.X - o.ParticleSize.X * 0.5f) * meterToPixel), (int)((objectPosition.Y - o.ParticleSize.Y * 0.5f) * meterToPixel),
                                    (int)(o.ParticleSize.X * meterToPixel), (int)(o.ParticleSize.Y * meterToPixel)),
                                    null, o.Color * p.Alpha, p.Rotation, new Vector2(o.ParticleTexture.Width, o.ParticleTexture.Height) / 2, SpriteEffects.None, 0);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws a Player.
        /// </summary>
        /// <param name="player">The Player you want to draw.</param>
        private void drawPlayer(Player player)
        {
            textureShader.Parameters["xTexture"].SetValue(player.Texture);
            textureShader.Parameters["viewMatrix"].SetValue(viewMatrix);
            textureShader.Parameters["projectionMatrix"].SetValue(projectionMatrix);
            
            //Blob
            textureShader.CurrentTechnique = textureShader.Techniques["MapTexture"];
            textureShader.Parameters["worldMatrix"].SetValue(Matrix.CreateScale(player.Size.X / 2 * meterToPixel) * Matrix.CreateTranslation(new Vector3((player.Position + player.Size / 2) * meterToPixel, 0)));
            foreach (EffectPass pass in textureShader.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, verticesCircle, 0, verticesCircle.Length, indicesCircle, 0, verticesCircle.Length - 1, VertexPositionTexture.VertexDeclaration);
            }
            //Hand
            textureShader.Parameters["worldMatrix"].SetValue(Matrix.CreateScale(player.Size.X / 2 * playerHandRatio * meterToPixel) * Matrix.CreateTranslation(new Vector3(player.HandBody.Position * meterToPixel, 0)));
            foreach (EffectPass pass in textureShader.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, verticesCircle, 0, verticesCircle.Length, indicesCircle, 0, verticesCircle.Length - 1, VertexPositionTexture.VertexDeclaration);
            }

            //Connection            
            textureShader.Parameters["worldMatrix"].SetValue(Matrix.Identity);
            createConnection(player.Position, player.Size.X / 2, player.HandBody.Position, player.Size.X * playerHandRatio);
            foreach (EffectPass pass in textureShader.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, verticesConnection, 0, verticesConnection.Length, indicesConnection, 0, 2, VertexPositionTexture.VertexDeclaration);
            }
        }

        /// <summary>
        /// Draws a grid for better orientation when placing objects. Every 1 meter is a line.
        /// </summary>
        private void drawGrid()
        {
            int width = 16, height = 9;
            //top to bottom
            for (int i = 1; i < width; i++)
            {
                DrawLine(new Vector2(i, 0) * camera.Zoom * meterToPixel, new Vector2(i, height) * camera.Zoom * meterToPixel);
            }
            //left to right
            for (int j = 1; j < height; j++)
            {
                DrawLine(new Vector2(0, j) * camera.Zoom * meterToPixel, new Vector2(width, j) * camera.Zoom * meterToPixel);
            }
        }

        /// <summary>
        /// Draws bounding boxes around every object.
        /// </summary>
        /// <param name="list">The object list.</param>
        private void drawBoundingBoxes(LayerList<LayerInterface> list)
        {
            foreach (LayerInterface l in list)
            {
                if (l is DrawableObject)
                {
                    DrawableObject drawableObject = l as DrawableObject;
                    //if (!drawableObject.Visible || drawableObject is Border) continue;
                    if (drawableObject is Border) continue;
                    //calculate size an position of object
                    calculateObjectSizeAndPosition(drawableObject);

                    objectSize.X = objectSize.X * camera.Zoom;
                    objectSize.Y = objectSize.Y * camera.Zoom;
                    objectPosition = Vector2.Transform(objectPosition, viewMatrix);

                    drawRectangle(objectPosition.X, objectPosition.Y, objectSize.X, objectSize.Y, drawableObject.Rotation, false);
                }
                else
                {
                    ParticleSystem p = l as ParticleSystem;
                    if (p.EmitterList.Count == 0) continue;
                    foreach (Emitter es in p.EmitterList)
                    {
                        EmitterShape s = es.EmitterShape;
                        if (s is EmitterRectangleShape)
                        {
                            EmitterRectangleShape e = s as EmitterRectangleShape;
                            objectSize.X = e.Width * camera.Zoom * meterToPixel;
                            objectSize.Y = e.Height * camera.Zoom * meterToPixel;
                            objectPosition = es.Position * meterToPixel;
                            objectPosition = Vector2.Transform(objectPosition, viewMatrix);

                            drawRectangle(objectPosition.X, objectPosition.Y, objectSize.X, objectSize.Y, 0f, true);
                        }
                        else
                        {
                            objectSize.X = 0.2f * camera.Zoom * meterToPixel;
                            objectSize.Y = 0.2f * camera.Zoom * meterToPixel;
                            objectPosition = es.Position * meterToPixel;
                            objectPosition = Vector2.Transform(objectPosition, viewMatrix);
                            drawRectangle(objectPosition.X, objectPosition.Y, objectSize.X, objectSize.Y, 0f, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws a DrawableObject with a texture  or, if it has one, with an animation. If it's not visible this function will do nothing.
        /// </summary>
        /// <param name="drawableObject">Object you want to draw.</param>
        private void drawObjectWithTextureOrAnimation(DrawableObject p)
        {
            if (p.FlipHorizontally == true) flipHorizontally = SpriteEffects.FlipHorizontally;
            else flipHorizontally = SpriteEffects.None;

            //calculate size and position of the object we want to draw
            calculateObjectSizeAndPosition(p);

            //draw HUD in seperate spritebatch beacuse of color mixing with mood            
            if (p is HUD)
            {
                if (p.Name == "BallIndicator")
                {
                    spriteBatchHUD.Draw(p.Texture,
                            new Rectangle((int)(objectPosition.X + objectSize.X * 0.5f), (int)(objectPosition.Y + objectSize.Y * 0.5f), (int)(objectSize.X), (int)(objectSize.Y)),
                            null, Color.Red * 0.85f, p.Rotation, new Vector2(p.Texture.Width * 0.5f, p.Texture.Height * 0.5f), flipHorizontally, 0.0f);
                }
                if (p.Texture != null)
                {
                    //texture, dest rect, source rect,  color, rotation, origin, effect, depth
                    Rectangle? source = ((HUD)p).SourceRectangle;

                    Vector2 origin = (source == null) ? new Vector2(p.Texture.Width * 0.5f, p.Texture.Height * 0.5f)
                        : new Vector2(((Rectangle)source).Width / 2, ((Rectangle)source).Height / 2);

                    spriteBatchHUD.Draw(p.Texture,
                        new Rectangle((int)(objectPosition.X + objectSize.X * 0.5f), (int)(objectPosition.Y + objectSize.Y * 0.5f), (int)(objectSize.X), (int)(objectSize.Y)),
                        source, p.BlendColor * p.Alpha, p.Rotation, origin, flipHorizontally, 0.0f);
                }
            }
            else
            {
                //if the object has a texture, draw it
                if (p.Animation == null)
                {
                    if (p.Texture != null)
                    {
                        if (p is Light)
                        {
                            Light l = p as Light;
                            spriteBatchLight.Draw(l.Texture,
                                new Rectangle((int)(objectPosition.X + objectSize.X * 0.5f), (int)(objectPosition.Y + objectSize.Y * 0.5f), (int)(objectSize.X), (int)(objectSize.Y)),
                                null, l.BlendColor * l.Alpha, l.Rotation, new Vector2(l.Texture.Width * 0.5f, l.Texture.Height * 0.5f), flipHorizontally, 0.0f);
                        }
                        else
                            //texture, dest rect, source rect,  color, rotation, origin, effect, depth
                            spriteBatchAll.Draw(p.Texture,
                                new Rectangle((int)(objectPosition.X + objectSize.X * 0.5f), (int)(objectPosition.Y + objectSize.Y * 0.5f), (int)(objectSize.X), (int)(objectSize.Y)),
                                null, p.BlendColor * p.Alpha, p.Rotation, new Vector2(p.Texture.Width * 0.5f, p.Texture.Height * 0.5f), flipHorizontally, 0.0f);
                    }
                }
                // else draw it's animation
                else
                {
                    //texture, dest rect, source rect,  color, rotation, origin, effect, depth
                    spriteBatchAll.Draw(p.Animation.SpriteStrip,
                        new Rectangle((int)(objectPosition.X), (int)(objectPosition.Y), (int)(objectSize.X), (int)(objectSize.Y)),
                        p.Animation.SourceRectangle, p.BlendColor * p.Alpha, p.Rotation, Vector2.Zero, flipHorizontally, 0.0f);
                }
            }

        }




            #endregion

        #region Helper Methods

        //store starting and ending color. provide color for linear interpolation in draw
        private Color startColor, endColor, lerpC;
        /// <summary>
        /// Timestamp in ms for fading.
        /// </summary>
        private double fadeTimeStart;
        /// <summary>
        /// Duration of the fading.
        /// </summary>
        private int fadeTime;

        /// <summary>
        /// Fades the whole screen from starting color to ending color in the given time.
        /// </summary>
        /// <param name="startColor">Starting color.</param>
        /// <param name="endColor">Ending color.</param>
        /// <param name="fadeTime">Time of the fading in ms.</param>
        public void fadeColor(Color startColor, Color endColor, int fadeTime)
        {
            this.startColor = startColor;
            this.endColor = endColor;
            this.fadeTime = fadeTime;
            this.fadeTimeStart = gameTime.TotalGameTime.TotalMilliseconds;
        }

        /// <summary>
        /// Stores time for sunrise and sunset.
        /// </summary>
        private double colorFadeTime;

        /// <summary>
        /// This will set the tinting color of the whole level so that it looks like the sun is rising.
        /// </summary>
        /// <param name="gametime">Gametime.</param>
        private void sunrise(GameTime gametime)
        {
            colorFadeTime += gametime.ElapsedGameTime.TotalSeconds;
            //increases red, then green and at the end the blue values so that the light will first appear grey, red, orange, yellow and finally white
            if (colorFadeTime >= 0.05d)
            {
                colorFadeTime = 0;
                if (mood.R <= 254)
                {
                    mood.G++;
                    mood.R += 2;
                }
                else
                    if (mood.G <= 254)
                    {
                        mood.B++;
                        mood.G++;
                    }
                    else
                        if (mood.B <= 254)
                        {
                            mood.B++;
                        }
                        //disables itself
                        else
                        {
                            mood = Color.White;
                            fadingState = 0;
                        }
            }
        }

        /// <summary>
        /// This will set the tinting color of the whole level so that it looks like the sun is setting.
        /// </summary>
        /// <param name="gametime">Gametime.</param>
        private void sunset(GameTime gametime)
        {
            colorFadeTime += gametime.ElapsedGameTime.TotalSeconds;
            //decreases blue, then green and at the end the red values so that the light will first appear white, yellow, orange, red and finally grey
            if (colorFadeTime >= 0.05d)
            {
                colorFadeTime -= 0.05d;
                if (mood.B >= 160)
                {
                    mood.B--;
                }
                else
                    if (mood.G >= 160)
                    {
                        mood.B--;
                        mood.G--;
                    }
                    else
                        if (mood.R >= 65)
                        {
                            mood.R -= 2;
                            mood.G--;
                        }
                        else
                            if (mood.B >= 10)
                            {
                                mood.B -= 2;
                                mood.G -= 2;
                                mood.R -= 2;
                            }
                            //disables itself
                            else
                            {
                                fadingState = 0;
                                TaskManager.Instance.addTask(new GraphicsTask(nightDurationTime,GraphicTask.Sunrise));
                            }
            }
        }

        /// <summary>
        /// Changes the rendersettings for different draw calls.
        /// </summary>
        /// <param name="newRenderSettings">The rendersettings for the object you want to draw next.</param>
        /// <param name="blendMode">The blendState for the draw call.</param>
        private void changeRenderSettings(RenderSettings newRenderSettings, BlendState blendMode)
        {
            if(currentRenderSettings != RenderSettings.Uninitialized)
                spriteBatchAll.End();
            currentRenderSettings = newRenderSettings;
            lastBlendMode = blendMode;

            switch (newRenderSettings)
            {
                case RenderSettings.Default:
                    spriteBatchAll.Begin(SpriteSortMode.Deferred, null, null, null, null, null, viewMatrix);                    
                    return;

                case RenderSettings.Player:
                    sobelEdge.CurrentTechnique = sobelEdge.Techniques["DarkenEdges"];
                    spriteBatchAll.Begin(SpriteSortMode.Deferred, null, null, null, null, sobelEdge, viewMatrix);
                    return;

                case RenderSettings.Particle:
                    spriteBatchAll.Begin(SpriteSortMode.FrontToBack, blendMode, null, null, null, null, viewMatrix);
                    return;
                case RenderSettings.Light:
                    spriteBatchAll.Begin(0, BlendState.AlphaBlend);
                    return;
            }
        }

        /// <summary>
        /// Higlights all players which are affected by the powerup with a color that indicates the PowerUpVersion.
        /// </summary>
        /// <param name="p">The powerup that was picked up.</param>
        public void highlightPlayer(PowerUp p)
        {
            foreach (Player player in (p.PowerUpVersion == PowerUpVersion.Positive) ? SuperController.Instance.getPlayerOfTeam(p.Owner.Team) : SuperController.Instance.getAllPlayers())
            {
                Light l = new Light("PlayerHighlight", StorageManager.Instance.getTextureByName("light_large_highlight"), player.Size * 2.5f,
                    player.Position, (p.PowerUpVersion == PowerUpVersion.Neutral) ? Color.Yellow : Color.DarkGreen, true, null);

                // Get duration for fading from SettingsManager
                int durationTime = (int)SettingsManager.Instance.get("PowerupHighlightPlayerDuration");
                l.FadingAnimation = new FadingAnimation(false, true, durationTime, true, 600);

                player.attachObject(l);

                //add light and remove it after finishing the animation
                SuperController.Instance.addGameObjectToGameInstance(l);
                TaskManager.Instance.addTask(new GameLogicTask(durationTime + 1200, l));
            }
        }

        /// <summary>
        /// Visualizes the jumpheight powerup.
        /// </summary>
        /// <param name="team">The team you want to affect.</param>
        public void playJumpHighlight(int team)
        {
            foreach (Player p in SuperController.Instance.getPlayerOfTeam(team))
            {
                SpriteAnimation sprite = new SpriteAnimation(StorageManager.Instance.getTextureByName("PowerUp_Jump"), 60, 11, 100, 100, 5, 2, true, false, true);
                FadingAnimation fading = new FadingAnimation(false, true, 7000, true, 1500);
                PassiveObject obj = new PassiveObject("", true, null, null, sprite, new Vector2(p.Size.X + 0.1f, p.Size.Y*0.7f), p.Position, 55, MeasurementUnit.Meter);

                //attach new object to player
                obj.FadingAnimation = fading;
                p.attachObject(obj);
                obj.Offset = new Vector2(0f, p.Size.Y / 2);
                obj.BlendColor = Color.DarkGreen;

                SuperController.Instance.addGameObjectToGameInstance(obj);
                TaskManager.Instance.addTask(new GameLogicTask(10000, obj));
            }
        }

        /// <summary>
        /// Processes all tasks that are sent to the Graphics by other Controllers.
        /// </summary>
        private void getTasks()
        {
            foreach (GraphicsTask task in TaskManager.Instance.GraphicTasks)
            {
                switch (task.Task)
                {
                    //take actions because a powerup task was created
                    case GraphicTask.DisplayPowerUp:
                        //this part gets executed when someone picks up a powerup
                        switch (task.PowerUpState)
                        {
                            case PowerUpState.Starting:
                                highlightPlayer(task.TriggeringPowerUp);
                                break;
                        }
                        break;

                    case GraphicTask.Sunrise:
                        fadingState = 2;

                        task.Task = GraphicTask.LightDisable;
                        task.CurrentTime = 5500;
                        task.MaximumTime = task.CurrentTime;

                        TaskManager.Instance.addTask(task);
                        break;

                    case GraphicTask.Sunset:
                        if (isNight || fadingState != 0) break;

                        fadingState = 1;

                        task.Task = GraphicTask.LightEnable;
                        task.CurrentTime = 4500;
                        task.MaximumTime = task.CurrentTime;

                        TaskManager.Instance.addTask(task);
                        break;

                    case GraphicTask.LightEnable:
                        isNight = true;
                        LightsInForest_Add();
                        break;

                    case GraphicTask.LightDisable:
                        isNight = false;
                        LightsInForest_Remove();
                        break;

                    case GraphicTask.CreateBall:
                        DrawableObject obj = SuperController.Instance.getObjectByName("BallAfterScore") as DrawableObject;
                        if (task.Team == 4)
                        {
                            obj.Position = task.Position;
                            obj.Visible = true;
                        }
                        else if (task.Team == 5)
                            obj.Visible = false;
 
                        obj.AttachedObjects.First().Visible = obj.Visible;
                        if (obj.Visible)
                        {
                            obj.Position = task.Position;
                            obj.FadingAnimation.Active = true;
                            obj.FadingAnimation.TimeSinceFadingStarted = 0;
                        }
                        break;
                }
            }

            // Clear list after executing all tasks
            TaskManager.Instance.GraphicTasks.Clear();
        }

        /// <summary>
        /// Adds a Light to the given object.
        /// </summary>
        /// <param name="obj">The object you want to attach the Light to.</param>
        public void addLightToObject(DrawableObject obj)
        {
            //only add a Light if the fading state differs from 0
            if (isNight)
            {
                Light l = new Light("forestLight", game.Content.Load<Texture2D>("Textures/Light/light_smooth"), obj.Size*4, obj.Position, Color.WhiteSmoke, true, null);
                l.Alpha = 0.8f;
                l.AttachedObject = obj;
                obj.attachObject(l);
                FadingAnimation fading = new FadingAnimation(false, false, 0, true, 500);
                l.FadingAnimation = fading;

                SuperController.Instance.addGameObjectToGameInstance(l);
            }
        }

        /// <summary>
        /// Removes a Light from the given object.
        /// </summary>
        /// <param name="obj">The object the Light is attached to.</param>
        public void removeLightFromObject(DrawableObject obj)
        {
            try
            {
                SuperController.Instance.removeGameObjectFromGameInstance(obj.uncoupleObjectByName("forestLight"));
            }
            catch (Exception e)
            {
                Logger.Instance.log(Sender.Graphics, "couldn't remove light", PriorityLevel.Priority_5);
            }
        }

        /// <summary>
        /// Adds Lights to all Players and Balls.
        /// </summary>
        private void LightsInForest_Add()
        {
            foreach (Player p in SuperController.Instance.getAllPlayers())
            {
                addLightToObject(p);
            }

            foreach (Ball b in SuperController.Instance.getAllBalls())
            {
                addLightToObject(b);
            }
        }

        /// <summary>
        /// Removes Lights from all Players and Balls.
        /// </summary>
        private void LightsInForest_Remove()
        {
            foreach (Light l in SuperController.Instance.getLightsByName("forestLight"))
            {
                SuperController.Instance.removeGameObjectFromGameInstance(l);
            }
        }

        /// <summary>
        /// Sets all values of the teamplay text so that it fades in at the right position.
        /// </summary>
        /// <param name="team">Number if team that got teamplay.</param>
        /// <param name="visible">Is the text visible?</param>
        private void setTeamplayText(int team, bool visible)
        {
            PassiveObject po;
            if (team == 1)
            {
                po = gameObjects.GetObjectByName("teamplay_t1") as PassiveObject;
            }
            else
            {
                po = gameObjects.GetObjectByName("teamplay_t2") as PassiveObject;
            }

            if (!visible)
            {
                po.Visible = false;
                return;
            }

            po.Visible = true;
            po.FadingAnimation.Active = true;
        }

        private float cos, sin, angle;
        private Vector2 blobToHand;
        private float stepAngle = MathHelper.ToRadians(10); //every 10° one vertex
        /// <summary>
        /// Creates a Circle with a given radius at a given position. Both is expected in meters.
        /// The Circle also will have a texture if the draw method uses the right shader.
        /// </summary>
        /// <param name="position">Position in meters.</param>
        /// <param name="radius">Radius in Meters.</param>
        private void createConnection(Vector2 positionBlob, float radiusBlob, Vector2 positionHand, float radiusHand)
        {
            //calculates position in pixel
            positionBlob += new Vector2(radiusBlob, radiusBlob);
            //calculate texture offset for hand
            Vector2 offset = (positionHand - positionBlob)/2;
            positionBlob *= meterToPixel;
            positionHand *= meterToPixel;

            float tempF = radiusBlob * meterToPixel;
            int deg = 70;

            //////////////////////
            //Calculate connection
            //////////////////////
            blobToHand = positionHand - positionBlob;

            //create a connection between blob and hand if hand moves out of blob
            //find orthogonal vector
            angle = MathHelper.ToDegrees((float) Math.Atan2(blobToHand.X, blobToHand.Y)) + 360;
            angle %= 360;
            
            //calculate outer position on blob for connection
            cos = (float)Math.Cos(MathHelper.ToRadians(angle + deg));
            sin = (float)Math.Sin(MathHelper.ToRadians(angle + deg));
            //if (id == 2) Console.WriteLine(angle + "  " + cos + "  " + sin);
            verticesConnection[0].Position = new Vector3(positionBlob.X + sin * tempF, positionBlob.Y + cos * tempF, 0);
            verticesConnection[0].TextureCoordinate.X = 0.5f + sin / 4;
            verticesConnection[0].TextureCoordinate.Y = 0.5f + cos / 4;

            cos = (float)Math.Cos(MathHelper.ToRadians(angle - deg));
            sin = (float)Math.Sin(MathHelper.ToRadians(angle - deg));
            verticesConnection[1].Position = new Vector3(positionBlob.X + sin * tempF, positionBlob.Y + cos * tempF, 0);
            verticesConnection[1].TextureCoordinate.X = 0.5f + sin / 4;
            verticesConnection[1].TextureCoordinate.Y = 0.5f + cos / 4;

            //calculate outer position on hand for connection
            angle = (angle + 180) % 360;
            deg += 30;
            cos = (float)Math.Cos(MathHelper.ToRadians(angle + deg));
            sin = (float)Math.Sin(MathHelper.ToRadians(angle + deg));
            verticesConnection[2].Position = new Vector3(positionHand.X + sin * (radiusHand / 2) * meterToPixel, positionHand.Y + cos * (radiusHand / 2) * meterToPixel, 0);
            verticesConnection[2].TextureCoordinate.X = 0.5f + offset.X + sin / 8;
            verticesConnection[2].TextureCoordinate.Y = 0.5f + offset.Y + cos / 8;

            cos = (float)Math.Cos(MathHelper.ToRadians(angle - deg));
            sin = (float)Math.Sin(MathHelper.ToRadians(angle - deg));
            verticesConnection[3].Position = new Vector3(positionHand.X + sin * (radiusHand / 2) * meterToPixel, positionHand.Y + cos * (radiusHand / 2) * meterToPixel, 0);
            verticesConnection[3].TextureCoordinate.X = 0.5f + offset.X + sin / 8;
            verticesConnection[3].TextureCoordinate.Y = 0.5f + offset.Y + cos / 8;

            indicesConnection[0] = indicesConnection[4] = (short)(0);
            indicesConnection[1] = indicesConnection[3] = (short)(2);
            indicesConnection[2] = (short)(1);
            indicesConnection[5] = (short)(3);

        }
        
        /// <summary>
        /// Creates a Circle with a given radius at a given position. Both is expected in meters.
        /// The Circle also will have a texture if the draw method uses the right shader.
        /// </summary>
        private void createTexturedCircle()
        {
            //sets the first vertex as center with texture coordinate also in center of texture we use
            verticesCircle[0].Position = Vector3.Zero;
            verticesCircle[0].TextureCoordinate.X = 0.5f;
            verticesCircle[0].TextureCoordinate.Y = 0.5f;

            //calculates vertices around circle depending on how smooth the circle should be (maybe optimaize with bresenham)
            for (int i = 1; i <= verticesCircle.Length-1; i++)
            {
                sin = (float)Math.Sin(stepAngle * i);
                cos = (float)Math.Cos(stepAngle * i);
                verticesCircle[i].Position = new Vector3(sin, cos, 0);
                verticesCircle[i].TextureCoordinate.X = 0.5f + sin / 2;
                verticesCircle[i].TextureCoordinate.Y = 0.5f + cos / 2;

                //sets the indices of the triangle
                indicesCircle[i * 3 - 3] = 0;
                indicesCircle[i * 3 - 2] = (i == verticesCircle.Length - 1) ? (short)1 : (short)(i + 1);
                indicesCircle[i * 3 - 1] = (short)i;               
            }
        }


        private void calculateTransformedRendertargetSizeAndPosition(Vector2 size, Vector2 position)
        {
            position.X = position.X - camera.ViewPortWidth + camera.Position.X;
            position.Y = position.Y - camera.ViewPortHeight + camera.Position.Y;

            position = position * camera.Zoom;

            position.X = position.X + camera.ViewPortWidth - camera.Position.X;
            position.Y = position.Y + camera.ViewPortHeight - camera.Position.Y;

            size.X = size.X * camera.Zoom;
            size.Y = size.Y * camera.Zoom;

            transformedPosition = position;
            transformedSize = size;
        }


        /// <summary>
        /// Calculates the right Object Size and Position depending on their measurement unit.
        /// Possible values for measurement unit are percent and meter.
        /// </summary>
        /// <param name="drawableObject">The Object of which we want to calculate the Position and Size.</param>
        private void calculateObjectSizeAndPosition(DrawableObject drawableObject)
        {
            //computes pixel from current measurement unit in percent (percent to pixel)
            if (drawableObject.Unit == MeasurementUnit.PercentOfScreen)
            {
                //Get Viewport Size in Meters
                objectSize.X = viewPortWidth * drawableObject.Size.X;
                objectSize.Y = viewPortHeight * drawableObject.Size.Y;
                objectPosition.X = viewPortWidth * drawableObject.Position.X;
                objectPosition.Y = viewPortHeight * drawableObject.Position.Y;
            }
            //computes pixel from current measurement unit in meter (meter to pixel)
            else if (drawableObject.Unit == MeasurementUnit.Meter)
            {
                objectSize = drawableObject.Size * meterToPixel;
                objectPosition = drawableObject.Position * meterToPixel;
            }
        }

        /// <summary>
        /// Has to be Implemented to Listen to the SettingsManager
        /// </summary>
        /// <param name="o">The Observable Object that sends the Message</param>
        public void notify(ObservableObject o)
        {
            //Check if we are Dealing with the right ObservableObject
            if (o is SettingsManager)
            {
                logger.log(Sender.Graphics, this.ToString() + " received a notification from the Settings Manager",PriorityLevel.Priority_2);
                this.meterToPixel = (float)((SettingsManager)o).get("meterToPixel");
                this.showBoundingBoxes = (bool)((SettingsManager)o).get("showBoundingBoxes");
                this.showGrid = (bool)((SettingsManager)o).get("showGrid");
            }
        }

        /// <summary>
        /// Draw a line.
        /// </summary>
        /// <param name="point1">Starting point.</param>
        /// <param name="point2">Ending point.</param>
        void DrawLine(Vector2 point1, Vector2 point2)
        {
            //Fill Vertices List with Coordinates of the rectangle
            vertices[0] = new VertexPositionColor(new Vector3(point1, 0), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(point2, 0), Color.Red);

            //Apply a Simple Shader so we see something
            basicEffect.CurrentTechnique.Passes[0].Apply();
            //Draw Lines
            game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 1);
        }

        /// <summary>
        /// Simple Method to draw Rectangles (oriented to xy-Axis)
        /// </summary>
        /// <param name="x">Position X of left upper Corner</param>
        /// <param name="y">Position Y of Left upper Corner</param>
        /// <param name="width">Width in Pixels</param>
        /// <param name="height">Height in Pixels</param>
        public void drawRectangle(float x, float y, float width, float height)
        {
            //Fill Vertices List with Coordinates of the rectangle
            vertices[0] = new VertexPositionColor(new Vector3(x, y, 0), Color.Green);
            vertices[1] = new VertexPositionColor(new Vector3(x + width, y, 0), Color.White);

            vertices[2] = new VertexPositionColor(new Vector3(x + width, y, 0), Color.White);
            vertices[3] = new VertexPositionColor(new Vector3(x + width, y + height, 0), Color.White);

            vertices[4] = new VertexPositionColor(new Vector3(x + width, y + height, 0), Color.White);
            vertices[5] = new VertexPositionColor(new Vector3(x , y + height, 0), Color.White);

            vertices[6] = new VertexPositionColor(new Vector3(x , y + height, 0), Color.White);
            vertices[7] = new VertexPositionColor(new Vector3(x, y, 0), Color.Green);

            //Apply a Simple Shader so we see something
            basicEffect.CurrentTechnique.Passes[0].Apply();
            //Draw Lines
            game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 4);
        }

        /// <summary>
        /// More Advanced Method to draw Rectangles
        /// </summary>
        /// <param name="x">Position X</param>
        /// <param name="y">Position Y</param>
        /// <param name="width">Width in Pixels</param>
        /// <param name="height">Height in Pixels</param>
        /// <param name="rotation">Rotation in radians</param>
        /// <param name="centerCoordinates">Boolean whether the XY Coordinates are meant for the Center, or the left upper Corner of the Rectangle</param>
        public void drawRectangle(float x, float y, float width, float height,float rotation, bool centerCoordinates)
        {
            //3 Vectors describing the Rectangle
            Vector3 pos = new Vector3(x, y, 0);
            Vector3 widthVec = new Vector3(width, 0, 0);
            Vector3 heightVec = new Vector3(0, height, 0);

            if (!centerCoordinates)
            {
                pos.X += width * 0.5f;
                pos.Y += height * 0.5f;
            }


            //Calculate Rotation Matrix
            Matrix matrix =  Matrix.CreateRotationZ(rotation);


            //Rotate "Rectangle"
            widthVec = Vector3.Transform(widthVec, matrix);
            heightVec = Vector3.Transform(heightVec, matrix);

                //Fill Vertices List with Coordinates of the rectangle
                vertices[0] = new VertexPositionColor(pos + 0.5f * (-widthVec - heightVec), Color.Green);
                vertices[1] = new VertexPositionColor(pos + 0.5f * (widthVec - heightVec), Color.White);

                vertices[2] = new VertexPositionColor(pos + 0.5f * (widthVec - heightVec), Color.White);
                vertices[3] = new VertexPositionColor(pos + 0.5f * (widthVec + heightVec), Color.White);

                vertices[4] = new VertexPositionColor(pos + 0.5f * (widthVec + heightVec), Color.White);
                vertices[5] = new VertexPositionColor(pos + 0.5f * (-widthVec + heightVec), Color.White);

                vertices[6] = new VertexPositionColor(pos + 0.5f * (-widthVec + heightVec), Color.White);
                vertices[7] = new VertexPositionColor(pos + 0.5f * (-widthVec - heightVec), Color.Green);
           
            
            //Apply a Simple Shader so we see something
            basicEffect.CurrentTechnique.Passes[0].Apply();
            //Draw Lines
            game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 4);
        }

        /// <summary>
        /// Converts a Meter Vector to a Pixel Vector
        /// </summary>
        /// <param name="vector">Vector to convert</param>
        /// <returns></returns>
        public Vector2 m2pix(Vector2 vector)
        {
            return vector * meterToPixel;
        }


        /// <summary>
        /// Converts the different Units
        /// </summary>
        /// <param name="vector">Vector to be converted</param>
        /// <param name="from">From this Unit</param>
        /// <param name="to">to this unit</param>
        /// <returns></returns>
        public Vector2 convertUnits(Vector2 vector, MeasurementUnit from, MeasurementUnit to)
        {
            Vector2 returnVector = new Vector2();

            if (from == MeasurementUnit.Meter)
            {
                if (to == MeasurementUnit.Pixel)
                {
                    returnVector = vector * meterToPixel;
                }
                else if (to == MeasurementUnit.PercentOfScreen)
                {
                   returnVector.X =  ( vector.X * meterToPixel) / this.viewPortWidth;
                   returnVector.Y = (vector.Y * meterToPixel) / this.viewPortHeight;
                }
            }
            else if (from == MeasurementUnit.PercentOfScreen)
            {
                if (to == MeasurementUnit.Pixel)
                {
                    returnVector.X = vector.X *  this.viewPortWidth;
                    returnVector.Y = vector.Y * this.viewPortHeight;
                }
                else if (to == MeasurementUnit.Meter)
                {
                    returnVector.X = (vector.X * this.viewPortWidth) / meterToPixel ;
                    returnVector.Y = (vector.Y * this.viewPortHeight) / meterToPixel;
                }

            }
            else if (from == MeasurementUnit.Pixel)
            {
                if (to == MeasurementUnit.PercentOfScreen)
                {
                    returnVector.X = vector.X / this.viewPortWidth;
                    returnVector.Y = vector.Y / this.viewPortHeight;
                }
                else if (to == MeasurementUnit.Meter)
                {
                    returnVector.X = vector.X / meterToPixel;
                    returnVector.Y = vector.X / meterToPixel;
                }

            }

            return returnVector;
        }

        /// <summary>
        /// Draws a line
        /// </summary>
        /// <param name="batch">spriteBatch to draw in</param>
        /// <param name="blank">a blank texture</param>
        /// <param name="width">width</param>
        /// <param name="color">Color</param>
        /// <param name="point1">point from</param>
        /// <param name="point2">point to</param>

        /// <summary>
        /// Does all necessary action, to bring controller back to the state after initialization
        /// </summary>
        public void reset()
        {
            mood = Color.White;
            lerpC = Color.White;
            fadeTime = 0;

            fadingState = 0;
            colorFadeTime = 0;
            TaskManager.Instance.GraphicTasks.Clear();
            isNight = false;
            ballFrame = 0;
            framesWithBall = 0;
        }

        /// <summary>
        /// Forces controller to pause, has to make sure that all timers etc. pause
        /// </summary>
        /// <param name="on">true: controller has to pause, false: controller has to work </param>
        public void pause(bool on)
        {
            paused = on;
        }

        #endregion

        #endregion
    }
}


