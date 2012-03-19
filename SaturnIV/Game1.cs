 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using TomShane.Neoforce.Controls;

namespace SaturnIV
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        actionMenuClass aMenu;        
        public CameraNew ourCamera;
        Vector3 defaultCameraOffset = new Vector3(10, 5000, 0);
        public static GraphicsDeviceManager graphics;
        SpriteFont spriteFont;
        SpriteFont spriteFontSmall;
        Random rand;
        public GraphicsDevice device;
        public Viewport viewport;
        KeyboardState oldkeyboardState;
        public static MouseState mouseStateCurrent, mouseStatePrevious;
        bool isRclicked, isLclicked, isRdown, isLdown, isSelected;
        int thisTeam;
        public static bool isTacmap;
        float mapScrollSpeed = 380f;
        Vector3 farpoint, nearpoint = new Vector3(0, 0, 0);
        Vector3 activeFoundryPos;
        SpriteBatch spriteBatch;
        HelperClass helperClass;
        double lastKeyPressTime;
        gameServer gServer;
        gameClient gClient;
        Texture2D rectTex, shipRec, selectRecTex, dummyTex, planetTexture;
        Texture2D transCircleGreen, orangeTarget;
        MessageClass messageClass;
        public Vector2 systemMessagePos = new Vector2(55, 30);
        public StringBuilder messageBuffer = new StringBuilder();
        public static List<string> messageLog = new List<string>();
        public Vector3[] plyonOffset;
        Line3D fLine;
        public Rectangle selectionRect;
        public List<Texture2D> objectThumbs = new List<Texture2D>();
        Ray currentMouseRay;
        HealthBarClass bar;
        Color shipColor;
        EditModeComponent editModeClass;
        public List<newShipStruct> activeShipList = new List<newShipStruct>();
        public List<squadClass> squadList = new List<squadClass>();
        squadClass thisSquad;
        public NPCManager npcManager;
        public PlayerManager playerManager;
        public static newShipStruct playerShip = new newShipStruct();
        public ModelManager modelManager;
        public WeaponsManager weaponsManager;
        public SystemClass systemManager;
        public BuildManager buildManager;
        public static List<shipData> shipDefList = new List<shipData>();
        public List<weaponData> weaponDefList = new List<weaponData>();
        public SerializerClass serializerClass;
        public RandomNames rNameList = new RandomNames();
        public string tmpShipName;
        ExplosionClass ourExplosion;
        public PlanetManager planetManager;
        public Effect effect;
        public Matrix cameraTarget;
        public Vector3 cameraTargetVec3;
        int typeSpeed = 125;
        SkySphere skySphere;
        RenderStarfield starField;
        VertexDeclaration vertexDeclaration, quadVertexDecl;
        ControlPanelClass cPanel = new ControlPanelClass();
        public float gameSpeed = 10.0f;
        //public Camera ourCamera;
        bool isEditMode = false;
        bool isServer = false;
        bool isClient = false;
        bool isChat = false;
        bool isDebug = false;
        bool isInvalidArea = false;
        bool isSystemMap = false;
        bool showBuildMenu = false;
        bool showActionMenu = false;
        bool inAMenu = false;
        public static bool drawTextbox = false;
        newShipStruct potentialTarget;
        int screenX, screenY, screenCenterX, screenCenterY;
        ParticleSystem projectileTrailParticles;
        public string chatMessage;
        Vector3 isRight;
        guiClass Gui;
        RadarClass radar;
        renderTriangle firingArc = new renderTriangle();
        Vector3 isFacing;
        Double loopTimer = -1;
        Double currentTime;
        Model systemMapSphere;

        // Define Hud Components
        Texture2D centerHUD;
        Texture2D targetTracker;
        public static Dictionary<string, Model> modelDictionary = new Dictionary<string, Model>();
        DrawQuadClass drawQuad;
        BasicEffect quadEffect;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            ConfigureGraphicsManager();
            Content.RootDirectory = "Content";
            //var models = Content.LoadContent<Model>("Models");
        }

        protected override void Initialize()
        {
            helperClass = new HelperClass();
            //ourCamera = new CameraNew(screenCenterX, screenCenterY);
            //ourCamera.ResetCamera();
            ourCamera = new CameraNew();
            ourCamera.ResetCamera();
            cameraTargetVec3 = Vector3.Zero;
            CameraNew.offsetDistance = new Vector3(-2400, 3200, 250);
            rand = new Random();
            ////////////TODO: Add your initialization logic here
            spriteBatch = new SpriteBatch(GraphicsDevice);
            serializerClass = new SerializerClass();
            messageClass = new MessageClass();
            bar = new HealthBarClass(this);
            bar.Initialize();
            activeShipList = new List<newShipStruct>();
            ////////////Initialize Manager Classes
            modelManager = new ModelManager(this);
            modelManager.Initialize();
            playerManager = new PlayerManager(this);
            playerManager.Initialize();
            npcManager = new NPCManager(this);
            npcManager.Initialize();
            weaponsManager = new WeaponsManager(this);
            weaponsManager.Initialize();
            systemManager = new SystemClass();
            buildManager = new BuildManager();
            ////////////Initalize Starfield
            starField = new RenderStarfield(this);
            InitializeStarFieldEffect();
            //firingArc = new renderTriangle();
            editModeClass = new EditModeComponent(this);
            editModeClass.Initialize(modelManager);
            ourExplosion = new ExplosionClass();
            ourExplosion.initExplosionClass(this);
            Gui = new guiClass();
            fLine = new Line3D(GraphicsDevice);
            skySphere = new SkySphere(this);
            planetManager = new PlanetManager(this);
            planetManager.Initialize();
            ////////////Mousey Stuff
            this.IsMouseVisible = true;
            Mouse.SetPosition(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
            //mouseStateCurrent = Mouse.GetState();
            ////////////Network Stuff
            gServer = new gameServer();
            gClient = new gameClient();
            /// quadClass init
            /// 
            drawQuad = new DrawQuadClass();
            quadEffect = new BasicEffect(GraphicsDevice,null);
            quadVertexDecl = new VertexDeclaration(graphics.GraphicsDevice,
               VertexPositionNormalTexture.VertexElements);            
            ////////////Random Stuff             
            projectileTrailParticles = new ProjectileTrailParticleSystem(this, Content);
            aMenu = new actionMenuClass();
            ////////////Add Components
            
            Components.Add(projectileTrailParticles);
            Components.Add(editModeClass);
            base.Initialize();

        }

        public void ConfigureGraphicsManager()
        {
            screenX = 1280;
            screenY = 1024;
            graphics.PreferredBackBufferWidth = screenX;
            graphics.PreferredBackBufferHeight = screenY;
            screenCenterX = screenX / 2;
            screenCenterY = screenY / 2;
            graphics.IsFullScreen = false;
        }

        protected override void LoadContent()
        {
            device = graphics.GraphicsDevice;
            viewport = device.Viewport;
            spriteFont = this.Content.Load<SpriteFont>("MedFont");
            spriteFontSmall = this.Content.Load<SpriteFont>("SmallFont");
            loadMetaData();
            Gui.initalize(this, ref shipDefList);
            cPanel.LoadPanel(Content, spriteBatch);
            rectTex = this.Content.Load<Texture2D>("textures//SelectionBox");
            dummyTex = this.Content.Load<Texture2D>("textures//dummy");
            shipRec = this.Content.Load<Texture2D>("textures//missiletrack");
            selectRecTex = this.Content.Load<Texture2D>("textures//SelectionBox");
            centerHUD = this.Content.Load<Texture2D>("textures//HUD/centertarget");
            targetTracker = this.Content.Load<Texture2D>("textures//HUD/target_track");
            systemMapSphere = this.Content.Load<Model>("models//planet");
            planetTexture = this.Content.Load<Texture2D>("textures/planettexture1");
            transCircleGreen = this.Content.Load<Texture2D>("Models/tacmap_items/transplanegreen");
            orangeTarget = this.Content.Load<Texture2D>("Models/tacmap_items/orange_target");
            aMenu.initalize(this, ref shipDefList);
            skySphere.LoadSkySphere(this);
            starField.LoadStarFieldAssets(this);
            planetManager.generatSpaceObjects(1);
            // Init Player ship
            playerShip = EditModeComponent.spawnNPC(Vector3.Zero, ref shipDefList, "player1", 2, 0, false);
            playerShip.modelRotation *= Matrix.CreateRotationY(MathHelper.ToRadians(90));
        }

        private void loadMetaData()
        {
            /// <summary>
            /// Load Ship/Unit Data from XML            
            /// </summary>
            /// <param name="ShipData">Provide Reference to List to populate</param>
            serializerClass.loadMetaData(ref shipDefList, ref weaponDefList, ref rNameList);
            foreach (shipData thisShip in shipDefList)
            {
                if (!modelDictionary.ContainsKey(thisShip.FileName))
                {
                    modelDictionary.Add(thisShip.FileName, Content.Load<Model>(thisShip.FileName));
                    //messageClass.messageLog.Add("Loading " + thisShip.FileName);
                }
            }

            foreach (weaponData thisWeapon in weaponDefList)
            {
                if (!modelDictionary.ContainsKey(thisWeapon.FileName))
                {
                    modelDictionary.Add(thisWeapon.FileName, Content.Load<Model>(thisWeapon.FileName));
                    //SystemLogClass.messageLog.Add("Loading " + thisWeapon.assetString);
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            currentTime = gameTime.TotalGameTime.TotalMilliseconds;
            processInput(gameTime);
            cameraTarget = Matrix.CreateWorld(cameraTargetVec3, Vector3.Forward, Vector3.Up);
            if (buildManager.buildQueueList.Count > 0)
                buildManager.updateBuildQueue(ref shipDefList, ref activeShipList, currentTime);
            if (isEditMode || isSystemMap)
            {
                ourCamera.ResetCamera();
                CameraNew.offsetDistance = new Vector3(0, 20000, 250);
                CameraNew.currentCameraMode = CameraNew.CameraMode.orbit;
                ourCamera.Update(cameraTarget, isEditMode);
                this.IsMouseVisible = true;
                //editModeClass.Update(gameTime, currentMouseRay, mouse3dVector, ref activeShipList, isLclicked, isRclicked, isLdown,
                //    ref npcManager, ourCamera, ref viewport);
                Gui.update(mouseStateCurrent, mouseStatePrevious);
            }
            else
            {
                aMenu.update(mouseStateCurrent, mouseStatePrevious,buildManager,isLclicked,activeFoundryPos);
                CameraNew.offsetDistance = new Vector3(0, 20000, 250);
                CameraNew.currentCameraMode = CameraNew.CameraMode.orbit;
                ourCamera.Update(cameraTarget, isEditMode);
                updateObjects(gameTime);
            }
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                    Mouse.SetPosition(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);            

            if (weaponsManager.activeWeaponList.Count > 0)
                helperClass.CheckForCollision(gameTime, ref activeShipList, ref weaponsManager.activeWeaponList,
                    ref ourExplosion, ref gServer);

            if (isServer)
                gServer.update(ref activeShipList);
            if (isClient)
                gClient.Update(ref activeShipList, ref npcManager);

            if (selectionRect != Rectangle.Empty)
                RectangleSelect(activeShipList, viewport, ourCamera.projectionMatrix, ourCamera.viewMatrix,
                    selectionRect);

            base.Update(gameTime);
        }

        protected void updateObjects(GameTime gameTime)
        {
            double currentTime = gameTime.TotalGameTime.TotalMilliseconds;
            if (loopTimer < 0)
                loopTimer = currentTime;
            if (currentTime - loopTimer > 500)
            {
                foreach (newShipStruct thisShip in activeShipList)
                {
                    loopTimer = currentTime;
                    npcManager.performAI(gameTime, ref weaponsManager, ref projectileTrailParticles, ref weaponDefList, thisShip, activeShipList, 0, null);
                }
                loopTimer = currentTime;
            }
            foreach (newShipStruct thisShip in activeShipList)
                npcManager.updateShipMovement(gameTime, gameSpeed, thisShip, ourCamera, false);

            //playerManager.updateShipMovement(gameTime, gameSpeed, Keyboard.GetState(), playerShip, ourCamera);
            weaponsManager.Update(gameTime, gameSpeed, ourExplosion);
        }

        protected void processInput(GameTime gameTime)
        {
            if (showBuildMenu && aMenu.medRec.Intersects(new Rectangle((int)mouseStateCurrent.X, (int)mouseStateCurrent.Y, 5, 5)))
                inAMenu = true;
            else
                inAMenu = false;
            KeyboardState keyboardState = Keyboard.GetState();
            mouseStateCurrent = Mouse.GetState();
            Vector3 myMouse3dVector = mouse3dVector;


            if (mouseStateCurrent.RightButton == ButtonState.Pressed &&
                mouseStatePrevious.RightButton == ButtonState.Released)
            {
                isRclicked = true;
                isRdown = false;
            }
            else if (mouseStateCurrent.RightButton == ButtonState.Pressed &&
                    mouseStatePrevious.RightButton == ButtonState.Pressed)
            {
                isRclicked = false;
                isRdown = true;
            }
            else
            {
                isRclicked = false;
                isRdown = false;
            }
            if (mouseStateCurrent.LeftButton == ButtonState.Pressed &&
                mouseStatePrevious.LeftButton == ButtonState.Released)
            {
                isLclicked = true;
                isLdown = false;
            }
            else
                isLclicked = false;
                if (!inAMenu)
                {
                    if (mouseStateCurrent.LeftButton == ButtonState.Pressed &&
                             mouseStatePrevious.LeftButton == ButtonState.Pressed)
                    {
                        isLclicked = false;
                        isLdown = true;
                        if (!isEditMode)
                        {
                            isSelected = true;
                            selectionRect = selectRectangle(mouseStateCurrent, myMouse3dVector);
                        }
                    }
                    else
                    {
                        isLclicked = false;
                        isLdown = false;
                        selectionRect = Rectangle.Empty;
                    }

                    if (isRclicked && isEditMode && !EditModeComponent.isSelecting)
                    {
                        isInvalidArea = false;
                        potentialTarget = null;
                        foreach (newShipStruct thisShip in activeShipList)
                            if (EditModeComponent.checkIsSelected(myMouse3dVector, thisShip.modelBoundingSphere))
                            {
                                isInvalidArea = true;
                                potentialTarget = thisShip;
                                break;
                            }
                        if (!isInvalidArea)
                        {
                            int rLen = rNameList.capitalShipNames.Count;
                            int i = rand.Next(0, rLen);
                            tmpShipName = rNameList.capitalShipNames[2];
                            rNameList.capitalShipNames.Remove(tmpShipName);
                            messageClass.sendSystemMsg(spriteFont, spriteBatch, tmpShipName + " Added", systemMessagePos);
                            newShipStruct newShip = EditModeComponent.spawnNPC(myMouse3dVector, ref shipDefList,
                                tmpShipName, Gui.thisShip, Gui.thisFaction, false);
                            activeShipList.Add(newShip);
                        }
                    }

                    if (isEditMode && guiClass.inGui)
                    {
                        if (Gui.loadThisScenario != null && mouseStateCurrent.LeftButton == ButtonState.Released)
                        {
                            serializerClass.loadScenario(Gui.loadThisScenario, ref activeShipList, ref shipDefList);
                            Gui.loadThisScenario = null;
                        }
                    }

                    if (isRclicked && !isEditMode && isSelected)
                    {
                        isInvalidArea = false;
                        //potentialTarget = null;
                        foreach (newShipStruct thisShip in activeShipList)
                            if (EditModeComponent.checkIsSelected(myMouse3dVector, thisShip.modelBoundingSphere))
                            {
                                isInvalidArea = true;
                                break;
                            }

                        foreach (newShipStruct thisShip in activeShipList)
                            if (thisShip.isSelected)
                            {
                                if (potentialTarget != null && potentialTarget.team != thisShip.team)
                                    thisShip.currentTarget = potentialTarget;
                                thisShip.currentDisposition = disposition.moving;
                                thisShip.targetPosition = myMouse3dVector;
                                thisShip.wayPointPosition = myMouse3dVector;
                            }
                    }
                }
            if (isChat) typeSpeed = 50;
            else
                typeSpeed = 100;

            if (currentTime - lastKeyPressTime > typeSpeed)
            {                
                if (keyboardState.IsKeyDown(Keys.F1) && !isEditMode && !isChat)
                {
                    isEditMode = true;
                    string msg = "Edit Mode";
                    messageClass.sendSystemMsg(spriteFont, spriteBatch, msg, systemMessagePos);
                     //CameraNew.zoomFactor = 8.0f;
                }
                else if (keyboardState.IsKeyDown(Keys.F1) && isEditMode && !isChat)
                    isEditMode = false;

                // Chat Mode Handler //
                if (keyboardState.IsKeyDown(Keys.C))
                {
                    if (isChat)
                    {
                        isChat = false;
                        string msg = "Chat Off";
                        messageClass.sendSystemMsg(spriteFont, spriteBatch, msg, systemMessagePos);
                    }
                    else
                    {
                        isChat = true;
                        string msg = "Chat On";
                        messageClass.sendSystemMsg(spriteFont, spriteBatch, msg, systemMessagePos);
                    }
                }
                else if (isChat)
                {
                    string pmsg = chatMessage;
                    chatMessage += helperClass.UpdateInput();
                    string msg = chatMessage;
                    if (pmsg != msg)
                        messageClass.sendSystemMsg(spriteFont, spriteBatch, msg, systemMessagePos);
                }

                if (isChat && keyboardState.IsKeyDown(Keys.Enter))// && !oldkeyboardState.IsKeyDown(Keys.Enter))
                {
                    if (isClient)
                        gClient.SendChat(chatMessage);
                    if (isServer)
                        gServer.SendChat(chatMessage);
                    messageClass.sendSystemMsg(spriteFont, spriteBatch, chatMessage, systemMessagePos);
                    chatMessage = "";
                    //systemMessagePos.Y += 10;
                }

                // Edit mode save Handler
                if (keyboardState.IsKeyDown(Keys.F10) && isEditMode)
                {
                    drawTextbox = true;
                    ControlPanelClass.textBoxActions = TextBoxActions.SaveScenario;
                    //serializerClass.exportSaveScenario(activeShipList, "plan1");
                }
                if (keyboardState.IsKeyDown(Keys.F11) && isEditMode)
                {
                    drawTextbox = true;
                    ControlPanelClass.textBoxActions = TextBoxActions.LoadScenario;
                    //serializerClass.exportSaveScenario(activeShipList, "plan1");
                }

                // Turn on/off Server/Client Mode
                if (keyboardState.IsKeyDown(Keys.F2) && !isServer)
                {
                    isServer = true;
                    isClient = false;
                    thisTeam = 0;
                    string msg = "Network: Server Mode";
                    messageClass.sendSystemMsg(spriteFont, spriteBatch, msg, systemMessagePos);
                    gServer.initializeServer();
                }
                else if (keyboardState.IsKeyDown(Keys.F3) && !isServer && !isClient)
                {
                    isServer = false;
                    isClient = true;
                    thisTeam = 1;
                    string msg = "Network:Client Mode";
                    messageClass.sendSystemMsg(spriteFont, spriteBatch, msg, systemMessagePos);
                    gClient.initializeNetwork("127.0.0.1");
                }
                lastKeyPressTime = currentTime;
            }

            if (keyboardState.IsKeyDown(Keys.F12) && !oldkeyboardState.IsKeyDown(Keys.F12) && !isDebug && !isChat)
            {
                MessageClass.messageLog.Add("Debug Mode On");
                isDebug = true;
            }
            else if (keyboardState.IsKeyDown(Keys.F12) && !oldkeyboardState.IsKeyDown(Keys.F12) && isDebug && !isChat)
            {
                MessageClass.messageLog.Add("Debug Mode Off");
                isDebug = false;
            }
          
                if (keyboardState.IsKeyDown(Keys.Escape) &&
                   !oldkeyboardState.IsKeyDown(Keys.Escape))
                {
                    selectionRect = Rectangle.Empty;
                    inAMenu = false;
                    showBuildMenu = false;
                }

                if (keyboardState.IsKeyDown(Keys.Space) &&
                    !oldkeyboardState.IsKeyDown(Keys.Space))
                {
                    showActionMenu = false;
                    showBuildMenu = false;
                    foreach (newShipStruct thisShip in activeShipList)
                    if (thisShip.isSelected)
                    {
                        if (thisShip.objectClass != ClassesEnum.Foundry)
                            showActionMenu = true;
                        else
                        {
                            showBuildMenu = true;
                            activeFoundryPos = thisShip.modelPosition;
                        }
                    }
                }
                if (!isChat)
                {
            /// Pan Camera
            /// 
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    cameraTargetVec3.X += -200f * CameraNew.zoomFactor;
                    //roll = 0.023f;
                }
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    cameraTargetVec3.X += 200f * CameraNew.zoomFactor;
                }
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    cameraTargetVec3.Z += 200f * CameraNew.zoomFactor;
                }
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    cameraTargetVec3.Z += -200f * CameraNew.zoomFactor;
                }


                // T will form a squad of all selected ships
                if (keyboardState.IsKeyDown(Keys.T) &&
                    !oldkeyboardState.IsKeyDown(Keys.T))
                {
                    squadClass createSquad = new squadClass();
                    createSquad.squadmate = new List<newShipStruct>();
                    createSquad.squadOrders = SquadDisposition.tight;
                    squadList.Add(createSquad);

                    bool isLeader = false;
                    foreach (newShipStruct thisShip in activeShipList)
                        if (thisShip.isSelected && thisShip.squadNo < 0)
                        {
                            createSquad.squadmate.Add(thisShip);
                            if (!isLeader)
                            {
                                MessageClass.messageLog.Add(thisShip.objectAlias + " Squad Formed");
                                thisShip.isSquadLeader = true;
                                squadList[squadList.Count - 1].leader = thisShip;
                            }
                            thisShip.squadNo = 0;
                            squadList[squadList.Count - 1].squadmate.Add(thisShip);
                            isLeader = true;
                        }
                }
            }
            mouseStatePrevious = mouseStateCurrent;
            oldkeyboardState = keyboardState;
        }
        private void InitializeStarFieldEffect()
        {
            vertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionColor.VertexElements);
            Matrix wvp = (Matrix.CreateScale(5.0f) * Matrix.CreateFromQuaternion(Quaternion.Identity) *
                Matrix.CreateTranslation(Vector3.Zero)) * ourCamera.viewMatrix * ourCamera.projectionMatrix;
            Matrix worldMatrix = Matrix.CreateTranslation(GraphicsDevice.Viewport.Width / 4f - 150,
                GraphicsDevice.Viewport.Height / 4f - 50, 0);
        }

        protected override void Draw(GameTime gameTime)
        {
            float time = (float)gameTime.TotalGameTime.TotalMilliseconds / 100.0f;
            graphics.GraphicsDevice.Clear(Color.Black);
            /// Draw system Map if systemMap mode is selected!
            skySphere.DrawSkySphere(this, ourCamera);
            starField.DrawStars(this, ourCamera);
            planetManager.DrawPlanets(gameTime, ourCamera.viewMatrix, ourCamera.projectionMatrix, ourCamera);
            drawMainObjects(gameTime);
            /// Draw tacitcal Circle Element
            //drawQuad.DrawQuad(quadVertexDecl, quadEffect, ourCamera.viewMatrix, ourCamera.projectionMatrix, tacPlaneQuad, orangeTarget);
            helperClass.DrawFPS(gameTime, device, spriteBatch, spriteFont);
            DrawHUDTargets(gameTime);
             spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            if (isEditMode) editModeClass.Draw(gameTime, ref activeShipList, ourCamera, spriteBatch);
            if (isEditMode) Gui.drawGUI(spriteBatch, spriteFont);            

            /// We need to fix the selection rectangle in case one of its dimensions is negative                
            /// 
            Rectangle r = new Rectangle(selectionRect.X, selectionRect.Y, selectionRect.Width, selectionRect.Height);
            if (r.Width < 0)
            {
                r.Width = -r.Width;
                r.X -= r.Width;
            }
            if (r.Height < 0)
            {
                r.Height = -r.Height;
                r.Y -= r.Height;
            }
            /// End Rectangle Select Crap
            ///
            spriteBatch.Draw(selectRecTex, r, Color.White);
            if (showBuildMenu) aMenu.drawGUI(spriteBatch, spriteFont, buildManager);
            spriteBatch.End();
            if (drawTextbox && ControlPanelClass.textBoxActions == TextBoxActions.SaveScenario)
                cPanel.drawTextbox(spriteBatch, "Scenario: ", new Vector2(screenX / 2 - 50, screenY / 2 - 50),
                activeShipList, serializerClass,300,25);
            messageClass.sendSystemMsg(spriteFont, spriteBatch, null, systemMessagePos);
            base.Draw(gameTime);
        }

        private void drawMainObjects(GameTime gameTime)
        {
            foreach (newShipStruct npcship in activeShipList)
            {
                modelManager.DrawModel(ourCamera, modelDictionary[npcship.objectFileName], npcship.worldMatrix, shipColor, true);
                if (isDebug)
                    debug(npcship);
                spriteBatch.Begin();
                if (npcship.team > 0)                
                    spriteBatch.Draw(orangeTarget, new Rectangle((int)npcship.screenCords.X-24, (int)npcship.screenCords.Y-24, 48, 48), Color.Red);
                else
                    spriteBatch.Draw(orangeTarget, new Rectangle((int)npcship.screenCords.X - 24, (int)npcship.screenCords.Y - 24, 48, 48), Color.Blue);                
                spriteBatch.End();

            }
            modelManager.DrawModel(ourCamera, modelDictionary[playerShip.objectFileName], playerShip.worldMatrix, Color.Blue, true);
            projectileTrailParticles.SetCamera(ourCamera.viewMatrix, ourCamera.projectionMatrix);
            foreach (weaponStruct theList in weaponsManager.activeWeaponList)
            {
                if (theList.isProjectile)
                    modelManager.DrawModel(ourCamera, modelDictionary[theList.objectFileName], theList.worldMatrix, Color.White, true);
                else
                    weaponsManager.DrawLaser(device, ourCamera.viewMatrix, ourCamera.projectionMatrix, theList.objectColor, theList);
            }
            ourExplosion.DrawExp(gameTime, ourCamera, GraphicsDevice);
        }

        private void DrawHUDTargets(GameTime gameTime)
        {
            bool isDone = false;
            StringBuilder messageBuffer = new StringBuilder();
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            Vector2 fontPos = new Vector2(0, 0);
            foreach (newShipStruct enemy in activeShipList)
            {
                StringBuilder buffer = new StringBuilder();
                if (enemy.isSelected && !isDone)
                {
                    isDone = true;
                    int starty = 600;
                    int timerIndex = 0;
                    double hbarValue;
                    int hbarwidth = 100;
                    buffer = new StringBuilder();
                    buffer.AppendLine(enemy.objectAlias + "\n" + enemy.objectClass + "\n" + enemy.currentDisposition);
                    spriteBatch.DrawString(spriteFont, buffer.ToString(), new Vector2(1000, starty - 70), Color.White);

                    foreach (WeaponModule thisMod in enemy.weaponArray)
                    {
                        buffer = new StringBuilder();
                        buffer.AppendLine(thisMod.weaponType + "");
                        spriteBatch.DrawString(spriteFont, buffer.ToString(), new Vector2(1100, starty - 18), Color.Green);
                        foreach (Vector4 thisWeapon in thisMod.ModulePositionOnShip)
                        {
                            double currentTime = gameTime.TotalGameTime.TotalMilliseconds - enemy.regenTimer[timerIndex];
                            if (enemy.regenTimer[timerIndex] == 0)
                                currentTime = 0;
                            hbarValue = currentTime / weaponDefList[(int)thisMod.weaponType].regenTime * 100;
                            timerIndex++;
                            hbarValue = hbarValue / hbarwidth * 100;
                            if (hbarValue > hbarwidth) hbarValue = hbarwidth;
                            bar.DrawHbar(gameTime, spriteBatch, Color.Red, 1100, starty, hbarwidth, 10, hbarwidth);
                            bar.DrawHbar(gameTime, spriteBatch, Color.LightBlue, 1100, starty, hbarwidth, 10, (int)hbarValue);
                            starty += 20;
                        }
                        starty += 10;
                    }
                }
                if (!isEditMode)
                {
                    switch (enemy.team)
                    {
                        case 0:
                            shipColor = Color.Blue;
                            break;
                        case 1:
                            shipColor = Color.Green;
                            break;
                    }
                    if (enemy.isSelected)
                        shipColor = Color.White;
                    fontPos = new Vector2(enemy.screenCords.X, enemy.screenCords.Y - 45);
                    if (enemy.isSelected)
                        buffer.AppendFormat("[" + enemy.objectAlias + "]");
                    if (isDebug)
                    {                       
                        buffer.AppendFormat("[" + enemy.currentDisposition + "]");
                        buffer.AppendFormat("[" + enemy.angleOfAttack + "]");
                        buffer.AppendFormat("[" + enemy.isEvading + "]");
                        buffer.AppendFormat("[" + enemy.currentTargetLevel + "]");
                        if (enemy.currentTarget != null)
                        {
                            buffer.AppendFormat("[" + enemy.currentTarget.objectAlias + "]");
                            buffer.AppendFormat("[" + Vector3.Distance(enemy.currentTarget.modelPosition, enemy.modelPosition) + "]");
                        }
                    }
                    spriteBatch.DrawString(spriteFontSmall, buffer.ToString(), fontPos, shipColor);
                }
            }
            spriteBatch.DrawString(spriteFont, messageBuffer.ToString(), new Vector2(0, 0), Color.White);
            spriteBatch.End();
            DrawHUD(gameTime);
        }

        private void DrawHUD(GameTime gameTime)
        {
            StringBuilder messageBuffer = new StringBuilder();
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            spriteBatch.DrawString(spriteFont, messageBuffer.ToString(), new Vector2(0, 0), Color.White);
            messageBuffer = new StringBuilder();
            messageBuffer.AppendFormat("\nZoomFactor {0} ", CameraNew.zoomFactor);
            messageBuffer.AppendFormat("\nCamera Mode " + CameraNew.currentCameraMode);
            if (potentialTarget != null)
                messageBuffer.AppendFormat("\nClicked On " + potentialTarget);
            messageBuffer.AppendFormat("\nMenu CurrentSlection: " + guiClass.currentSelection);
            spriteBatch.DrawString(spriteFont, messageBuffer.ToString(), new Vector2(screenCenterX +
                                    (screenX / 6) - 150, screenCenterY + (screenY / 3)), Color.White);
            messageBuffer = new StringBuilder();
            spriteBatch.DrawString(spriteFont, messageBuffer.ToString(), systemMessagePos, Color.Yellow);
            //spriteBatch.Draw(centerHUD, new Vector2(screenCenterX - 149, screenCenterY - 155), Color.Wheat);

            StringBuilder buffer = new StringBuilder();
            buffer.AppendFormat("Ninja76");
            spriteBatch.DrawString(spriteFont, buffer.ToString(), new Vector2(playerShip.screenCords.X - 16, playerShip.screenCords.Y - 16), Color.Gray);
            spriteBatch.End();
        }

        // Rectangle Drag N Drop routines //
        public Rectangle selectRectangle(MouseState mouseState, Vector3 mouse3d)
        {
            if (selectionRect == Rectangle.Empty)
                selectionRect = new Rectangle((int)mouseState.X, (int)mouseState.Y, 0, 0);
            else if (selectionRect != Rectangle.Empty)
            {
                selectionRect.Width = mouseState.X - selectionRect.X;
                selectionRect.Height = mouseState.Y - selectionRect.Y;
            } //else
            return selectionRect;
        }

        public void RectangleSelect(List<newShipStruct> objectsList, Viewport viewport, Matrix projection, Matrix view, Rectangle selectionRect)
        {
            foreach (newShipStruct o in objectsList)
            {
                Vector3 screenPos = viewport.Project(o.modelPosition, projection, view, Matrix.Identity);
                if (selectionRect.Contains((int)screenPos.X, (int)screenPos.Y) && o.team == 0)
                {
                    o.isSelected = true;                
                }
                else
                {
                    o.isSelected = false;
                }
            }
        }

        Vector3 mouse3dVector
        {
            get
            {
                MouseState mouseState = Mouse.GetState();
                Vector2 ms = new Vector2(mouseState.X,mouseState.Y);
                //  Unproject the screen space mouse coordinate into model space 
                //  coordinates. Because the world space matrix is identity, this 
                //  gives the coordinates in world space.
                Viewport vp = GraphicsDevice.Viewport;
                //  Note the order of the parameters! Projection first.
                Vector3 pos1 = vp.Unproject(new Vector3(ms.X, ms.Y, 0), ourCamera.projectionMatrix, ourCamera.viewMatrix, Matrix.Identity);
                Vector3 pos2 = vp.Unproject(new Vector3(ms.X, ms.Y, 1), ourCamera.projectionMatrix, ourCamera.viewMatrix, Matrix.Identity);
                Vector3 dir = Vector3.Normalize(pos2 - pos1);
                Vector3 ppos = Vector3.Zero;
                //  If the mouse ray is aimed parallel with the world plane, then don't 
                //  intersect, because that would divide by zero.
                if (dir.Y != 0)
                {
                    Vector3 x = pos1 - dir * (pos1.Y / dir.Y);
                    ppos = x;
                    return ppos;
                }
                return Vector3.Zero;
            }
        }

        public void debug(newShipStruct npcship)
        {
            fLine.Draw(npcship.modelPosition, npcship.targetPosition, Color.Blue, ourCamera.viewMatrix, ourCamera.projectionMatrix);
                               foreach (BoundingFrustum bf in npcship.moduleFrustum)                        
                                 BoundingFrustumRenderer.Render(bf, device, ourCamera.viewMatrix, ourCamera.projectionMatrix, Color.White);
            //                   BoundingFrustumRenderer.Render(npcship.portFrustum, device, ourCamera.viewMatrix, ourCamera.projectionMatrix, Color.White);
            //                    BoundingFrustumRenderer.Render(npcship.starboardFrustum, device, ourCamera.viewMatrix, ourCamera.projectionMatrix, Color.White);

            //Update all Weapon Module Firing Frustums           
            //Update all Weapon Module Firing Frustums
            int moduleCount = 0;
            foreach (WeaponModule thisWeapon in npcship.weaponArray)
            {
                for (int i = 0; i < thisWeapon.ModulePositionOnShip.Count() - 1; i++)
                {
                    switch ((int)thisWeapon.ModulePositionOnShip[i].W)
                    {
                        case 0:
                            isFacing = npcship.Direction;
                            isRight = npcship.modelRotation.Forward;
                            break;
                        case 1:
                            isFacing = -npcship.Direction;
                            isRight = npcship.modelRotation.Backward;
                            break;
                        case 2:
                            isFacing = -npcship.modelRotation.Right;
                            isRight = npcship.modelRotation.Forward;
                            break;
                        case 3:
                            isFacing = npcship.modelRotation.Right;
                            isRight = -npcship.modelRotation.Forward;
                            break;
                    }
                    Vector3 tVec = npcship.modelPosition;
                    tVec.X = npcship.modelPosition.X + thisWeapon.ModulePositionOnShip[i].X;
                    tVec.Z = npcship.modelPosition.Z + thisWeapon.ModulePositionOnShip[i].Z;

                    fLine.Draw(tVec, tVec + (isFacing * 15000), Color.Blue, ourCamera.viewMatrix, ourCamera.projectionMatrix);
                    moduleCount++;
                }
            }

        }
    }

}
