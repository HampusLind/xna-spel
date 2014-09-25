using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace mittspel
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //textur till spelet
        Texture2D tankgTexture, tankrTexture, backgroundTexture, holeTexture, hole2Texture, hole3Texture, shotTexture, shot2Texture, wscreengTexture,
            wscreenrTexture;
        //position i x och y led för bilden
        Vector2 tankgPosition = new Vector2(100, 200);
        Vector2 tankrPosition = new Vector2(500, 200);
        Vector2 holePosition = new Vector2(315, 200);
        Vector2 hole2Position = new Vector2(-10000, -10000);
        Vector2 hole3Position = new Vector2(-20000, -20000);
        Vector2 shotPosition = new Vector2(-5000, -5000);
        Vector2 shot2Position = new Vector2(-800, -800);
        Vector2 wscreengPosition = new Vector2(-8000, -8000);
        Vector2 wscreenrPosition = new Vector2(-6500, -5810);
        //för att skriva text på skärmen
        SpriteFont gamefont;
        SpriteFont gamefont2;
        //hämta färg data från varje sprite
        Color[] tankgTextureData;
        Color[] tankrTextureData;
        Color[] holeTextureData;
        Color[] hole2TextureData;
        Color[] hole3TextureData;
        Color[] shotTextureData;
        Color[] shot2TextureData;
        //kolla krock
        Color tankgColor = Color.White;
        Color tankrColor = Color.White;
        //spara gamla tank green position
        Vector2 tankgOldPosition;
        Vector2 tankrOldPosition;
        //heltalsvariabler som håller koll på poängen
        int pointsg;
        int pointsr;
        // variabel för att spara en ljudeffekt
        SoundEffect explosion;
        //musik till spelet
        Song gameMusic;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //starta spelet i fullscreen
            this.graphics.IsFullScreen = true;
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
            // TODO: Add your initialization logic here
            //sätter poäng till 3 och 6
            pointsg = 3;
            pointsr = 3;
            base.Initialize();
        }

        //egen metod för kollisionshantering
        static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
            Rectangle rectangleB, Color[] dataB)
        {
            //hitta de delar av rektanglarna som överlappar
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);
           //nu tar vi och kollar varje pixel om de är transparenta och icke överlappande
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++) 
                {
                    Color colorA = dataA[(x - rectangleA.Left) +
                        (y - rectangleA.Top) * rectangleA.Width];

                    Color colorB = dataB[(x - rectangleB.Left) +
                        (y - rectangleB.Top) * rectangleB.Width];

                    //kolla om dessa två är icke transparenta
                    //i så fall har vi en kollision
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        return true; 
                   }
                }
            }
            return false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            #region laddar saker
            tankgTexture = Content.Load<Texture2D>("tank green");
            tankrTexture = Content.Load<Texture2D>("tank red");
            //laddar in skott
            shotTexture = Content.Load<Texture2D>("shot");
            shot2Texture = Content.Load<Texture2D>("shot2");
            //ladda in vår font
            gamefont = Content.Load<SpriteFont>("gamefont");
            gamefont2 = Content.Load<SpriteFont>("gamefont2");
            //ladda in vinstskärmar
            wscreengTexture = Content.Load<Texture2D>("winscreeng");
            wscreenrTexture = Content.Load<Texture2D>("winscreenr");
            //hämta färgdatat från texturerna
            tankgTextureData =
                new Color[tankgTexture.Width * tankgTexture.Height];
            tankgTexture.GetData(tankgTextureData);

            tankrTextureData = new Color[tankrTexture.Width * tankrTexture.Height];
            tankrTexture.GetData(tankrTextureData);

            shotTextureData =
               new Color[shotTexture.Width * shotTexture.Height];
            shotTexture.GetData(shotTextureData);

            shot2TextureData =
               new Color[shot2Texture.Width * shot2Texture.Height];
            shot2Texture.GetData(shot2TextureData);

            backgroundTexture = Content.Load<Texture2D>("arenaback");
            holeTexture = Content.Load<Texture2D>("hole");
            holeTextureData =
                new Color[holeTexture.Width * holeTexture.Height];
            holeTexture.GetData(holeTextureData);

            hole2Texture = Content.Load<Texture2D>("hole2");
            hole2TextureData =
                new Color[hole2Texture.Width * hole2Texture.Height];
            hole2Texture.GetData(hole2TextureData);

            hole3Texture = Content.Load<Texture2D>("hole3");
            hole3TextureData =
                new Color[hole3Texture.Width * hole3Texture.Height];
            hole3Texture.GetData(hole3TextureData);
            #endregion
            #region musik och ljudeffekter
            //ladda in ljudeffekt
            explosion = Content.Load<SoundEffect>("explosion-02");
            //ladda in musik
            gameMusic = Content.Load<Song>("C&C Red alert 2 music (Fortification)");
            //musiken startar direkt när spelet börjar
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(gameMusic);
            #endregion
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            //tangentbord
            KeyboardState kState = Keyboard.GetState();
            //avsluta med escape
            if (kState.IsKeyDown(Keys.Escape))
                this.Exit();
            #region tank green styrning
            //spara gamla positionen på tank green
            tankgOldPosition = tankgPosition;
            // om man trycker ner A 
            if (kState.IsKeyDown(Keys.A))
            {
                //tank green går åt vänster då minskar x värdet med 3 
                tankgPosition.X -= 3;
            }
            // om man trycker ner D 
            if (kState.IsKeyDown(Keys.D))
            {
                //tank green går åt höger då ökar x värdet med 3 
                tankgPosition.X += 3;
            }
            // om man trycker ner W 
            if (kState.IsKeyDown(Keys.W))
            {
                //tank green går uppåt då minskar y värdet med 3 
                tankgPosition.Y -= 3;
            }
            // om man trycker ner S 
            if (kState.IsKeyDown(Keys.S))
            {
                //tank green går neråt då ökar y värdet med 3 
                tankgPosition.Y += 3;
            }

            //om man trycker på G
            if (kState.IsKeyDown(Keys.G))
            {
                //laddar
                shotPosition.Y = tankgPosition.Y + 48;
                shotPosition.X = tankgPosition.X + tankgTexture.Width;
            }

            //om man trycker på H
            if (kState.IsKeyDown(Keys.H))
            {
                //skjuter
                shotPosition.X += 7;
            }

            //kolla om skottet har hamnat utanför skärmen om så flytta tillbaka
            if (shotPosition.X < 0)
                shotPosition.X = -5000;
            if (shotPosition.Y < 0)
                shotPosition.Y = -1000;
            //för lång åt höger?
            if (shotPosition.X + shotTexture.Width > this.Window.ClientBounds.Width)
                shotPosition.X = -5000;
          

            if (tankgPosition.X + tankgTexture.Width < 0)
                tankgPosition.X = this.Window.ClientBounds.Width;
            if (tankgPosition.Y + tankgTexture.Height < 0)
                tankgPosition.Y = this.Window.ClientBounds.Height;
            //för lång åt höger?
            if (tankgPosition.X > this.Window.ClientBounds.Width)
                tankgPosition.X = 0;
            //för lång ner?
            if (tankgPosition.Y  > this.Window.ClientBounds.Height)
                tankgPosition.Y = 0;
            #endregion
            #region tank red styrning
            //spara gamla positionen
            tankrOldPosition = tankrPosition;
            // om man trycker ner vänster piltangent 
            if (kState.IsKeyDown(Keys.Left))
            {
                //tank red går åt vänster då minskar x värdet med 3 
                tankrPosition.X -= 3;
            }
            // om man trycker ner höger piltangent 
            if (kState.IsKeyDown(Keys.Right))
            {
                //tank red går åt höger då ökar x värdet med 3 
                tankrPosition.X += 3;
            }
            // om man trycker ner upp piltangent 
            if (kState.IsKeyDown(Keys.Up))
            {
                //tank red går uppåt då minskar y värdet med 3 
                tankrPosition.Y -= 3;
            }
            // om man trycker ner ner piltangenten 
            if (kState.IsKeyDown(Keys.Down))
            {
                //tank red går neråt då ökar y värdet med 3 
                tankrPosition.Y += 3;
            }

            //om man trycker på K
            if (kState.IsKeyDown(Keys.K))
            {
                //laddar
                shot2Position.Y = tankrPosition.Y + 48;
                shot2Position.X = tankrPosition.X -50;
            }

            //om man trycker på L
            if (kState.IsKeyDown(Keys.L))
            {
                //skjuter
                shot2Position.X -= 7;
            }

            //kolla om skottet har hamnat utanför skärmen om så flytta tillbaka
            if (shot2Position.X < 0)
                shot2Position.X = -2000;
            if (shot2Position.Y < 0)
                shot2Position.Y = -7000;
            //för lång åt höger?
            if (shot2Position.X + shot2Texture.Width > this.Window.ClientBounds.Width)
                shot2Position.X = -1500;
           

            //utanför skärmen?
            if (tankrPosition.X + tankrTexture.Width < 0)
                tankrPosition.X = this.Window.ClientBounds.Width;
            if (tankrPosition.Y + tankgTexture.Height < 0)
                tankrPosition.Y = this.Window.ClientBounds.Height;
            //för lång åt höger?
            if (tankrPosition.X > this.Window.ClientBounds.Width)
                tankrPosition.X = 0;
            //för lång ner?
            if (tankrPosition.Y > this.Window.ClientBounds.Height)
                tankrPosition.Y = 0;

            #endregion
            #region kollisioner
            //ta reda på om det är en kollision
            Rectangle tankgRectangle =
                new Rectangle((int)tankgPosition.X, (int)tankgPosition.Y,
                   tankgTexture.Width, tankgTexture.Height);

            Rectangle tankrRectangle =
                 new Rectangle((int)tankrPosition.X, (int)tankrPosition.Y,
                   tankrTexture.Width, tankrTexture.Height);
            tankgColor = Color.White;
            //anropa kollisionsmetoden
            if(IntersectPixels(tankgRectangle, tankgTextureData,
                tankrRectangle, tankrTextureData))
            {
               //tanks knuffar inte varandra vid kollision
                tankrPosition = tankrOldPosition;
                tankgPosition = tankgOldPosition;  
            }
            Rectangle holeRectangle =
                new Rectangle((int)holePosition.X, (int)holePosition.Y,
                    holeTexture.Width, holeTexture.Height);
            if (IntersectPixels(tankgRectangle, tankgTextureData,
                holeRectangle, holeTextureData))
            {
                //poängen minska med 1
                pointsg -= 1;
                //flytta på hole
                holePosition = new Vector2(-5000, -6000);
                hole2Position = new Vector2(315, 75);
            }
            Rectangle hole2Rectangle =
               new Rectangle((int)hole2Position.X, (int)hole2Position.Y,
                   hole2Texture.Width, hole2Texture.Height);
            if (IntersectPixels(tankgRectangle, tankgTextureData,
                hole2Rectangle, hole2TextureData))
            {
                //poäng minskar med 1
                pointsg -= 1;
                hole2Position = new Vector2(-1000, -1000);
                hole3Position = new Vector2(315, 90);
            }

            Rectangle hole3Rectangle =
               new Rectangle((int)hole3Position.X, (int)hole3Position.Y,
                   hole3Texture.Width, hole3Texture.Height);
            if (IntersectPixels(tankgRectangle, tankgTextureData,
                hole3Rectangle, hole3TextureData))
            {
                //poäng minskar med 1
                pointsg -= 1;
                //this.Exit();
            }

            if (IntersectPixels(tankrRectangle, tankrTextureData,
               holeRectangle, holeTextureData))
            {
                pointsr -= 1;
                holePosition = new Vector2(-7000, -6000);
                hole2Position = new Vector2(315, 123);
            }

            if (IntersectPixels(tankrRectangle, tankrTextureData,
              hole2Rectangle, hole2TextureData))
            {
                pointsr -= 1;
                hole2Position = new Vector2(-6000, 0);
                hole3Position = new Vector2(315, 60);
            }

            if (IntersectPixels(tankrRectangle, tankrTextureData,
              hole3Rectangle, hole3TextureData))
            {
                pointsr -= 1;
            }

            //skott
            Rectangle shotRectangle =
              new Rectangle((int)shotPosition.X, (int)shotPosition.Y,
                  shotTexture.Width, shotTexture.Height);
            if (IntersectPixels(tankrRectangle, tankrTextureData,
                shotRectangle, shotTextureData))
            {
                //poäng minskar med 1
                pointsr -= 1;
                shotPosition.X = -2000;
                explosion.Play(1.0f, 0.0f, 0.0f);
                tankrColor = Color.Black;
            }
            Rectangle shot2Rectangle =
                new Rectangle((int)shot2Position.X, (int)shot2Position.Y,
                    shot2Texture.Width, shot2Texture.Height);
            if (IntersectPixels(tankgRectangle, tankgTextureData,
                shot2Rectangle, shot2TextureData))
            {
                //poäng för grön minskar med 1
                pointsg -= 1;
                shot2Position.X = -1863;
                explosion.Play(1.0f, 0.0f, 0.0f);
                tankgColor = Color.Red;
            }

           
            #endregion
            //vinnarskärmar
            //vinstskärm grön
            if (pointsr == 0)
            {
                wscreengPosition = new Vector2(100, 90);
            }
            //vinstskärm röd
            if (pointsg == 0)
            {
                wscreenrPosition = new Vector2(100, 90);
               
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
           
            //nu börjar jag rita på skärmen
            spriteBatch.Begin();
            #region ritar upp saker
            //rita ut bakgrund först annars täcker den det andra
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0,
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height), Color.White);
            //rita ut hole
            spriteBatch.Draw(holeTexture, holePosition, Color.White);
            //rita ut hole 2
            spriteBatch.Draw(hole2Texture, hole2Position, Color.White);
            //rita ut hole 3
            spriteBatch.Draw(hole3Texture, hole3Position, Color.White);
            //ritar ut tank green
            spriteBatch.Draw(tankgTexture, tankgPosition, tankgColor);
            //rita ut tank red
            spriteBatch.Draw(tankrTexture, tankrPosition, Color.White);
            //rita ut skott
            spriteBatch.Draw(shotTexture, shotPosition, Color.White);
            spriteBatch.Draw(shot2Texture, shot2Position, Color.White);
            //rita ut vinnarskärmar
            spriteBatch.Draw(wscreengTexture, wscreengPosition, Color.White);
            spriteBatch.Draw(wscreenrTexture, wscreenrPosition, Color.White);
            //skriv text
            spriteBatch.DrawString(gamefont, "points: " + pointsg.ToString() , new Vector2(100, 10), Color.White);
            spriteBatch.DrawString(gamefont2, "Points: " + pointsr.ToString(), new Vector2(500, 10), Color.White);
            //avsluta ritandet
            #endregion
            spriteBatch.End();
           

            base.Draw(gameTime);
        }
    }
}
