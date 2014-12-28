using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

namespace helloWorld
{
	/// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;		

		double fullCircle = 2 * Math.PI;

		int lives = 3;

		Texture2D shipTexture;
		Entity ship;

		Texture2D rockTexture0;
		List<Entity> asteroids = new List<Entity> ();

		double turnSpeed = 0.1;
		int screenWidth = 800;
		int screenHeight = 600;

		Random rnd = new Random();

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
            base.Initialize();

			graphics.PreferredBackBufferWidth = screenWidth;
			graphics.PreferredBackBufferHeight = screenHeight;
			graphics.IsFullScreen = false;
			graphics.ApplyChanges();

			ship = new Entity ();
			ship.x = screenWidth / 2;
			ship.y = screenHeight / 2;
			ship.dx = 0;
			ship.dy = 0;
			ship.angle = 0;
			ship.texture = shipTexture;

			addNewRock (50, 50);
			addNewRock (screenWidth - 50, 50);
			addNewRock (screenWidth - 50, screenHeight - 50);
			addNewRock (50, screenHeight - 50);
        }

		protected void addNewRock(double x, double y) {

			var tempRock = new Entity ();
			tempRock.x = x;
			tempRock.y = y;
			tempRock.dx = rnd.NextDouble () * 2.0 - 1.0;
			tempRock.dy = rnd.NextDouble () * 2.0 - 1.0;
			tempRock.dangle = rnd.NextDouble () * 0.02;
			tempRock.texture = rockTexture0;
			asteroids.Add (tempRock);
		}

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

			shipTexture = Content.Load<Texture2D> ("ship.png");
			rockTexture0 = Content.Load<Texture2D> ("rock_0.png");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
			GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            // For Mobile devices, this logic will close the Game when the Back button is pressed
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			{
				Exit();
			}

			if (gamePadState.ThumbSticks.Left.X < -0.1) {
				ship.angle += turnSpeed * gamePadState.ThumbSticks.Left.X;
			}

			if (gamePadState.ThumbSticks.Left.X > 0.1) {
				ship.angle += turnSpeed * gamePadState.ThumbSticks.Left.X;
			}

			if (gamePadState.ThumbSticks.Left.Y > 0.1) {
				ship.dx += Math.Cos(ship.angle) * 0.1;
				ship.dy += Math.Sin(ship.angle) * 0.1;
			}

			moveEntity (ship);

			var shipOrigin =
				new Vector2((float)ship.x, 
				            (float)ship.y);

			var shipCircle = new Circle (shipOrigin, ship.texture.Width / 2);

			foreach (var asteroid in asteroids) {
				moveEntity (asteroid);

				var asteroidOrigin = 
					new Vector2 ((float)asteroid.x, 
					             (float)asteroid.y);

				var asteroidCircle = new Circle (asteroidOrigin, asteroid.texture.Width / 2);

				if (shipCircle.Intersects (asteroidCircle)) {
					ShipExplosion ();
				}
			}

            base.Update(gameTime);
        }

		protected void ShipExplosion() {
			lives--;

			if (lives < 0) {
				Exit ();
			}

			ship.x = screenWidth / 2;
			ship.y = screenHeight / 2;
			ship.dx = 0;
			ship.dy = 0;
			ship.angle = 0;
		}

		protected void moveEntity(Entity entity) {
			entity.x += entity.dx;
			entity.y += entity.dy;
			entity.angle += entity.dangle;

			if (entity.x < 0 - entity.texture.Width / 2) {
				entity.x = screenWidth + entity.texture.Width / 2;
			}

			if (entity.x > screenWidth + entity.texture.Width / 2) {
				entity.x = 0 - entity.texture.Width / 2;
			}

			if (entity.y < 0 - entity.texture.Height / 2) {
				entity.y = screenHeight + entity.texture.Height / 2;
			}

			if (entity.y > screenHeight + entity.texture.Height / 2) {
				entity.y = 0 - entity.texture.Height / 2;
			}

			if (entity.angle > fullCircle) {
				entity.angle -= fullCircle;
			}

			if (entity.angle < 0) {
				entity.angle += fullCircle;
			}
		}

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
           	graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
		
			spriteBatch.Begin();

			Vector2 location = new Vector2((int)ship.x, (int)ship.y);
			Rectangle sourceRectangle = new Rectangle(0, 0, ship.texture.Width, ship.texture.Height);
			Vector2 origin = new Vector2(ship.texture.Width / 2, ship.texture.Height / 2);

			spriteBatch.Draw (texture: ship.texture,
 			                  position: location,
			                  sourceRectangle: sourceRectangle,
			                  color: Color.White,
			                  rotation: (float)ship.angle,
			                  origin: origin,
			                  scale: 1.0f,
			                  effect: SpriteEffects.None,
			                  depth: 1);

			foreach (var asteroid in asteroids) {
				location = new Vector2((int)asteroid.x, (int)asteroid.y);
				sourceRectangle = new Rectangle(0, 0, asteroid.texture.Width, asteroid.texture.Height);
				origin = new Vector2(asteroid.texture.Width / 2, asteroid.texture.Height / 2);

				spriteBatch.Draw (texture: asteroid.texture,
				                  position: location,
				                  sourceRectangle: sourceRectangle,
				                  color: Color.White,
				                  rotation: (float)asteroid.angle,
				                  origin: origin,
				                  scale: 1.0f,
				                  effect: SpriteEffects.None,
				                  depth: 1);
			}

			spriteBatch.End();
            //TODO: Drawing on the edge (ie. two blits instead of one)
            
            base.Draw(gameTime);
        }
    }
}

