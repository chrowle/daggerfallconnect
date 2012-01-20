﻿// Project:         Ruins of Hill Deep - Playground Build
// Description:     Test environment for Ruins of Hill Deep development.
// Copyright:       Copyright (C) 2012 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    http://code.google.com/p/daggerfallconnect/

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DeepEngine.Core;
using DeepEngine.Components;
using DeepEngine.GameStates;
using DeepEngine.World;
using DeepEngine.Player;
using DeepEngine.UserInterface;
#endregion

namespace RoHD_Playground.GameStates
{

    /// <summary>
    /// Title screen interface.
    /// </summary>
    public interface ITitleScreenState : IGameState { }

    /// <summary>
    /// Title screen class.
    /// </summary>
    public sealed class TitleScreen : GameState, ITitleScreenState
    {

        #region Fields

        const string titleText = "Ruins of Hill Deep";
        const string versionText = "Playground Build 1.0";
        const string startMenuText = "Start";
        const string exitMenuText = "Exit";

        Color clearColor = Color.Transparent;
        Color skyDark = Color.Black;
        Color skyLight = new Color(64, 32, 32);

        Scene scene;
        Song song;

        SpriteFont titleFont;
        SpriteFont consoleFont;
        SpriteFont menuFont;
        SpriteFont menuFont2;

        float cloudTime = 0;
        float cloudSpeed = 5.0f;

        InterfaceManager gui;
        MenuItemScreenComponent startMenuItem;
        MenuItemScreenComponent exitMenuItem;

        public event EventHandler OnStartClicked;
        public event EventHandler OnExitClicked;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="core">Engine core.</param>
        /// <param name="game">Game object.</param>
        public TitleScreen(DeepCore core, Game game)
            : base(core, game)
        {
        }

        #endregion

        #region GameState Overrides

        protected override void LoadContent()
        {
            // Create scene
            scene = new Scene(core);
            scene.Camera.Position = new Vector3(33.0625f, 4.4375f, 89.8125f);
            scene.Camera.Transform(327, 0, Vector3.Zero);
            scene.Camera.Update();
            core.ActiveScene = scene;

            // Disable core input handling
            core.Input.ActiveDevices = Input.DeviceFlags.None;

            core.Renderer.ClearColor = clearColor;

            // Set day/night mode for window textures
            core.MaterialManager.Daytime = false;

            // Set climate
            DFLocation.ClimateSettings climateSettings = MapsFile.DefaultClimateSettings;
            climateSettings.ClimateType = DFLocation.ClimateBaseType.Swamp;
            climateSettings.SceneryArchive = 510;

            // Create level entity
            WorldEntity level = new WorldEntity(core.ActiveScene);

            // Create block component
            DaggerfallBlockComponent block = new DaggerfallBlockComponent(core, core.ActiveScene);
            block.LoadBlock("CASTAA26.RMB", climateSettings);
            level.Components.Add(block);

            // Attach block flats
            AddBlockFlats(level, block);

            // Create directional lights
            Color lightColor = new Color(100, 100, 200);
            WorldEntity directionalLight = new WorldEntity(core.ActiveScene);
            directionalLight.Components.Add(new LightComponent(core, Vector3.Normalize(Vector3.Down + Vector3.Right), lightColor, 0.60f));
            directionalLight.Components.Add(new LightComponent(core, Vector3.Normalize(Vector3.Forward + Vector3.Left), lightColor, 0.90f));

            // Create fireflies in courtyard
            for (int i = 0; i < 20; i++)
            {
                Entities.Firefly firefly = new Entities.Firefly(scene);
                firefly.Matrix = Matrix.CreateTranslation(62.5f, 3.125f, 43.75f);
            }

            // Create fireflies in left of courtyard
            for (int i = 0; i < 10; i++)
            {
                Entities.Firefly firefly = new Entities.Firefly(scene);
                firefly.Matrix = Matrix.CreateTranslation(31.25f, 3.125f, 43.75f);
            }

            // Create fireflies near camera
            for (int i = 0; i < 10; i++)
            {
                Entities.Firefly firefly = new Entities.Firefly(scene);
                firefly.Matrix = Matrix.CreateTranslation(37.5f, 3.125f, 75f);
            }

            // Load songs
            song = Game.Content.Load<Song>("Songs/DanGoodale_DF-11");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(song);

            // Load fonts
            titleFont = Game.Content.Load<SpriteFont>("Fonts/TitleFont");
            consoleFont = Game.Content.Load<SpriteFont>("Fonts/ConsoleFont");
            menuFont = Game.Content.Load<SpriteFont>("Fonts/MenuFont");
            menuFont2 = Game.Content.Load<SpriteFont>("Fonts/MenuFont2");

            // Create gui manager
            gui = new InterfaceManager(core);
            gui.SetMargins(Margin.All, 20);

            // Create title text
            TextItemScreenComponent title = new TextItemScreenComponent(core, titleFont, titleText);
            title.HorizontalAlignment = HoriztonalTextAlignment.Right;
            title.OutlineColor = Color.Gray;
            title.EnableOutline = true;

            // Create version text
            TextItemScreenComponent version = new TextItemScreenComponent(core, consoleFont, versionText);
            version.HorizontalAlignment = HoriztonalTextAlignment.Right;
            version.TextColor = Color.Gold;

            // Start menu item
            startMenuItem = new MenuItemScreenComponent(core, menuFont2, startMenuText);
            startMenuItem.HorizontalAlignment = HoriztonalTextAlignment.Right;
            startMenuItem.TextColor = Color.White;
            startMenuItem.OutlineColor = Color.Goldenrod;

            // Exit menu item
            exitMenuItem = new MenuItemScreenComponent(core, menuFont2, exitMenuText);
            exitMenuItem.HorizontalAlignment = HoriztonalTextAlignment.Right;
            exitMenuItem.TextColor = Color.White;
            exitMenuItem.OutlineColor = Color.Gold;

            // Add everything to gui manager
            gui.Components.Add(title);
            gui.Components.Add(version);
            gui.Components.Add(startMenuItem);
            gui.Components.Add(exitMenuItem);

            // Set positions of gui items
            version.OffsetFrom(title, Sides.Bottom, 0);
            startMenuItem.OffsetFrom(version, Sides.Bottom, 50);
            exitMenuItem.OffsetFrom(startMenuItem, Sides.Bottom, 30);

            // Wire up menu events
            startMenuItem.OnMouseEnter += new EventHandler(StartMenuItem_OnMouseEnter);
            startMenuItem.OnMouseLeave += new EventHandler(StartMenuItem_OnMouseLeave);
            exitMenuItem.OnMouseEnter += new EventHandler(ExitMenuItem_OnMouseEnter);
            exitMenuItem.OnMouseLeave += new EventHandler(ExitMenuItem_OnMouseLeave);
            startMenuItem.OnMouseClick += new EventHandler(StartMenuItem_OnMouseClick);
            exitMenuItem.OnMouseClick += new EventHandler(ExitMenuItem_OnMouseClick);

            base.Game.IsMouseVisible = true;
        }

        protected override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            // Update GUI components
            gui.Update(gameTime.ElapsedGameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw
            core.Draw(false);

            // Draw sky dome
            core.DrawSkyDome(skyLight, skyDark, 0.3f, cloudTime, true);
            cloudTime += cloudSpeed * core.DeltaTime;

            // Present
            core.Present();

            // Draw GUI components
            gui.Draw();
        }

        #endregion

        #region Scene Building

        /// <summary>
        /// Attach flats component to an entity.
        /// </summary>
        /// <param name="entity">Entity to attach billboards.</param>
        /// <param name="block">Block to get flats from.</param>
        private void AddBlockFlats(BaseEntity entity, DaggerfallBlockComponent block)
        {
            // Exit if no flats
            if (block.BlockFlats.Count == 0)
                return;

            // Add flats to component
            foreach (var flat in block.BlockFlats)
            {
                // Get position
                Vector3 position = new Vector3(flat.Position.X, flat.Position.Y, flat.Position.Z);

                // Add billboard component
                DaggerfallBillboardComponent billboard = new DaggerfallBillboardComponent(core, flat);
                billboard.Matrix = block.Matrix * Matrix.CreateTranslation(position);
                entity.Components.Add(billboard);

                // Add a light commponent for each billboard light source
                if (flat.Archive == 210)
                {
                    position.Y += billboard.Size.Y;
                    LightComponent lightComponent = new LightComponent(core, block.Matrix.Translation + position, 750f, Color.White, 1.1f);
                    entity.Components.Add(lightComponent);
                }
            }
        }

        #endregion

        #region Menu Events

        private void StartMenuItem_OnMouseClick(object sender, EventArgs e)
        {
            if (OnStartClicked != null)
                OnStartClicked(this, null);
        }

        private void ExitMenuItem_OnMouseClick(object sender, EventArgs e)
        {
            if (OnExitClicked != null)
                OnExitClicked(this, null);
        }

        private void StartMenuItem_OnMouseEnter(object sender, EventArgs e)
        {
            startMenuItem.EnableOutline = true;
        }

        private void StartMenuItem_OnMouseLeave(object sender, EventArgs e)
        {
            startMenuItem.EnableOutline = false;
        }

        private void ExitMenuItem_OnMouseEnter(object sender, EventArgs e)
        {
            exitMenuItem.EnableOutline = true;
        }

        private void ExitMenuItem_OnMouseLeave(object sender, EventArgs e)
        {
            exitMenuItem.EnableOutline = false;
        }

        #endregion

    }

}