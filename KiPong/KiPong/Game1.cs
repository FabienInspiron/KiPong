namespace KiPong
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Kinect;
    using System.Collections.Generic;

    public enum Difficulty { EASY, MEDIUM, HARD };

    public enum Side { LEFT, RIGHT, TOP, BOTTOM };

    /// <summary>
    /// Menu Principale
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        /* -- GAME ELEMENT -- */
        public static GameStates gamestate;
        private Menu PlayingMenu, ModeMenu, DifficultyMenu, PauseMenu, EndMenu;
        private Jeu jeu;
        private bool IsOnePlayer, IsKinectMode;

        /* -- SCREEN -- */
        private KinectInput kinectInput;
        private KeyboardInput keyboardInput;
        private int screenWidth;
        public int ScreenWidth
        { get { return screenWidth; } }
        private int screenHeight;
        public int ScreenHeight
        { get { return screenHeight; } }
        private Rectangle screen;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;

        /* AIDES */
        private Aide aideMenuKeyboard, aideMenuKinect, aideJeu;

        public enum GameStates
        {
            ModeMenu,
            PlayingMenu,
            DifficultyMenu,
            Running,
            PauseMenu,
            EndMenu
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // r�cup�re la taille exacte de l'ecran
            //screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            screenHeight = 600;
            screenWidth = 800;
            keyboardInput = new KeyboardInput(this);
            keyboardInput.IsHoldable = false;
            ModeMenu = new MenuKeyboard(this, "Mode de jeu", new List<string>() { "Clavier", "Kinect", "Quitter" }, keyboardInput);
            aideMenuKeyboard = new Aide(this, "aideMenuKinectImg", "aideMenuKinectTxt");
            gamestate = GameStates.ModeMenu;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            screen = new Rectangle(0, 0, screenWidth, screenHeight);

            IsKinectMode = false;

            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>("Font");
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            if(IsKinectMode)
                kinectInput.UnloadContent();
            Content.Unload();
        }

        /// <summary>
        /// Lance le jeu
        /// </summary>
        /// <param name="d">Difficult�e du jeu</param>
        private void Jouer(Difficulty d)
        {
            if (IsKinectMode)
            {
                jeu = new JeuKinect(this, d, IsOnePlayer, kinectInput);
            }
            else
            {
                jeu = new JeuKeyboard(this, d, IsOnePlayer, keyboardInput);
            }
        }

        /// <summary>
        /// Creer les menus
        /// </summary>
        private void SetMenus()
        {
            if (IsKinectMode)
            {
                kinectInput = new KinectInput(this);
                IsOnePlayer = true;
                PlayingMenu = new MenuKinect(this, "KiPong", new List<string>() { "1 joueur", "2 joueurs" }, kinectInput);
                DifficultyMenu = new MenuKinect(this, "Difficultes", new List<string>() { "Facile", "Medium", "Difficile" }, kinectInput);
                PauseMenu = new MenuKinect(this, "Pause", new List<string>() { "Reprendre", "Menu", "Quitter" }, kinectInput);
                EndMenu = new MenuKinect(this, "", new List<string>() { "Menu", "Quitter" }, kinectInput);
            }
            else
            {
                PlayingMenu = new MenuKeyboard(this, "KiPong", new List<string>() { "1 joueur", "2 joueurs" }, keyboardInput);
                DifficultyMenu = new MenuKeyboard(this, "Difficultes", new List<string>() { "Facile", "Medium", "Difficile" }, keyboardInput);
                PauseMenu = new MenuKeyboard(this, "Pause", new List<string>() { "Reprendre", "Menu", "Quitter" }, keyboardInput);
                EndMenu = new MenuKeyboard(this, "", new List<string>() { "Menu", "Quitter" }, keyboardInput);
            }
        }

        /// <summary>
        /// M�thode appel� par le syst�me de jeu en boucle
        /// </summary>
        /// <param name="gameTime"></param>
        ///
        protected override void Update(GameTime gameTime)
        {
            keyboardInput.Update();

            // Si on en en mode kinect et quelle n'est pas pr�te on retourne
            if (IsKinectMode)
            {
                if ((kinectInput.ReadyForOne && IsOnePlayer)
                    || (kinectInput.ReadyForTwo && !IsOnePlayer))
                {
                    kinectInput.Update();
                }
                else
                {
                    base.Update(gameTime);
                    return;
                }
            }

            #region Playing
            if (gamestate == GameStates.Running)
            {
                jeu.Update();
                if (jeu.Finish)
                {
                    EndMenu.Title = jeu.getMessage();
                    keyboardInput.IsHoldable = false;
                    gamestate = GameStates.EndMenu;
                }
                if (!IsKinectMode ? keyboardInput.Pause() : kinectInput.Pause())
                {
                    keyboardInput.IsHoldable = false;
                    gamestate = GameStates.PauseMenu;
                }
            }
            #endregion Playing
            #region ModeMenu
            else if (gamestate == GameStates.ModeMenu)
            {
                ModeMenu.Update();
                // Lors de la selection
                if (ModeMenu.Valid)
                {
                    gamestate = GameStates.PlayingMenu;
                    if (ModeMenu.Iterator == 0)
                    {
                        IsKinectMode = false;
                    }
                    else if (ModeMenu.Iterator == 1)
                    {
                        IsKinectMode = true;
                    }
                    else if (ModeMenu.Iterator == 2)
                    {
                        this.Exit();
                    }
                    SetMenus();
                    //ModeMenu.Iterator = 0;
                }
            }
            #endregion ModeMenu
            #region Menu Jouer
            else if (gamestate == GameStates.PlayingMenu)
            {
                PlayingMenu.Update();
                // Lors de la selection
                if (PlayingMenu.Valid)
                {
                    gamestate = GameStates.DifficultyMenu;
                    if (PlayingMenu.Iterator == 0)
                    {
                        IsOnePlayer = true;
                    }
                    else if (PlayingMenu.Iterator == 1)
                    {
                        IsOnePlayer = false;
                    }
                    PlayingMenu.Iterator = 0;
                }
                if (PlayingMenu.Back)
                {
                    gamestate = GameStates.ModeMenu;
                    IsKinectMode = false;
                }
            }
            #endregion Menu Jouer
            #region DifficultyMenu
            else if (gamestate == GameStates.DifficultyMenu)
            {
                DifficultyMenu.Update();

                // Lors de la selection
                if (DifficultyMenu.Valid)
                {
                    gamestate = GameStates.Running;
                    keyboardInput.IsHoldable = true;
                    if (DifficultyMenu.Iterator == 0)
                    {
                        Jouer(Difficulty.EASY);
                    }
                    else if (DifficultyMenu.Iterator == 1)
                    {
                        Jouer(Difficulty.MEDIUM);
                    }
                    else if (DifficultyMenu.Iterator == 2)
                    {
                        Jouer(Difficulty.HARD);
                    }
                    DifficultyMenu.Iterator = 0;
                }
                if (DifficultyMenu.Back)
                {
                    gamestate = GameStates.PlayingMenu;
                }
            }
            #endregion DifficultyMenu
            #region PauseMenu
            else if (gamestate == GameStates.PauseMenu)
            {
                PauseMenu.Update();

                // Lors de la selection
                if (PauseMenu.Valid)
                {
                    if (PauseMenu.Iterator == 0)
                    {
                        keyboardInput.IsHoldable = true;
                        jeu.SetAfterPause();
                        gamestate = GameStates.Running;
                    }
                    else if (PauseMenu.Iterator == 1)
                    {
                        gamestate = GameStates.PlayingMenu;
                    }
                    else if (PauseMenu.Iterator == 2)
                    {
                        this.Exit();
                    }
                    PauseMenu.Iterator = 0;
                }
                if (PauseMenu.Back)
                {
                    keyboardInput.IsHoldable = true;
                    jeu.SetAfterPause();
                    gamestate = GameStates.Running;
                }
            }
            #endregion PauseMenu
            #region EndMenu
            else if (gamestate == GameStates.EndMenu)
            {
                EndMenu.Update();

                // Lors de la selection
                if (EndMenu.Valid)
                {
                    if (EndMenu.Iterator == 0)
                    {
                        gamestate = GameStates.PlayingMenu;
                    }
                    else if (EndMenu.Iterator == 1)
                    {
                        this.Exit();
                    }
                    EndMenu.Iterator = 0;
                }
            }
            #endregion EndMenu

            base.Update(gameTime);
        }

        /// <summary>
        /// M�thode appeler r�guli�rement par le syst�me de jeu
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            if (gamestate == GameStates.Running)
            {
                jeu.Draw(spriteBatch, font);
            }
            else if (gamestate == GameStates.ModeMenu)
            {
                ModeMenu.Draw(spriteBatch, font);
            }
            else if (gamestate == GameStates.PlayingMenu)
            {
                PlayingMenu.Draw(spriteBatch, font);
            }
            else if (gamestate == GameStates.DifficultyMenu)
            {
                DifficultyMenu.Draw(spriteBatch, font);
            }
            else if (gamestate == GameStates.PauseMenu)
            {
                PauseMenu.Draw(spriteBatch, font);
            }
            else if (gamestate == GameStates.EndMenu)
            {
                EndMenu.Draw(spriteBatch, font);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }


        #region Helpers
        public void DrawStringAtCenter(SpriteBatch sb, String text, Color color)
        {
            Utils.DrawStringAtCenter(sb, font, screen, text, color);
        }
        #endregion Helpers
    }
}