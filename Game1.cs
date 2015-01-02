using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

namespace helloWorld
{
	public enum GameState {
		MainMenu = 0,
		Game = 1
	}

	/// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;		

		double fullCircle = 2 * Math.PI;
		float leftTurn = (float)Math.PI / -2;

		GameState state = GameState.MainMenu;
		int lives = 3;

		double turnSpeed = 0.1;
		int screenWidth = 800;
		int screenHeight = 600;

		StarField backGround;

		Texture2D titleTexture;

		Texture2D shipTexture, exhaustTexture;
		Entity ship;

		List<Texture2D> rockTextures = new List<Texture2D> ();
		List<Asteroid> asteroids = new List<Asteroid> ();

		List<Asteroid> newAsteroids = new List<Asteroid> ();
		List<Asteroid> destroyedAsteroids = new List<Asteroid> ();

		Texture2D bulletTexture;
		List<Bullet> bullets = new List<Bullet> ();
		List<Bullet> destroyedBullets = new List<Bullet> ();
		TimeSpan bulletFired;
		TimeSpan bulletFlightTime = new TimeSpan (0, 0, 3);

		List<Bullet> particles = new List<Bullet> ();
		TimeSpan particleTime = new TimeSpan (0, 0, 0, 0, 500);

		Random rnd = new Random();

		Song BackgroundMusic;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";	  

			this.Window.Title = "Hello World";
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

			SpawnShip ();
			spawnRocks ();

			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play (BackgroundMusic);
        }

		protected void spawnRocks() {
			asteroids.Clear ();
			addNewRock (50, 50);
			addNewRock (screenWidth - 50, 50);
			addNewRock (screenWidth - 50, screenHeight - 50);
			addNewRock (50, screenHeight - 50);
		}

		protected void addNewRock(double x, double y) {

			var tempRock = new Asteroid ();
			tempRock.x = x;
			tempRock.y = y;
			tempRock.dx = rnd.NextDouble () * 2.0 - 1.0;
			tempRock.dy = rnd.NextDouble () * 2.0 - 1.0;
			tempRock.dangle = rnd.NextDouble () * 0.02;
			tempRock.phase = 3;
			tempRock.texture = rockTextures[tempRock.phase];
			asteroids.Add (tempRock);
		}

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

			titleTexture = Content.Load<Texture2D> ("title.png");

			shipTexture = Content.Load<Texture2D> ("ship.png");
			bulletTexture = Content.Load<Texture2D> ("bullet.png");
			exhaustTexture = Content.Load<Texture2D> ("exhaust.png");

			rockTextures.Add (Content.Load<Texture2D> ("rock_1.png"));
			rockTextures.Add (Content.Load<Texture2D> ("rock_2.png"));
			rockTextures.Add (Content.Load<Texture2D> ("rock_3.png"));
			rockTextures.Add (Content.Load<Texture2D> ("rock_4.png"));

			var starTextures = new List<Texture2D> ();
			starTextures.Add(Content.Load<Texture2D> ("star_0.png"));
			starTextures.Add(Content.Load<Texture2D> ("star_1.png"));
			starTextures.Add(Content.Load<Texture2D> ("star_2.png"));
			starTextures.Add(Content.Load<Texture2D> ("star_3.png"));

			BackgroundMusic = Content.Load<Song> ("bg.wav");

			backGround = new StarField (rnd, screenWidth, screenHeight, starTextures);
			backGround.Init ();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
			GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
			KeyboardState keyboardState = Keyboard.GetState (PlayerIndex.One);

            // For Mobile devices, this logic will close the Game when the Back button is pressed
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
			{
				if (state == GameState.MainMenu) {
					Exit ();
				} else {
					state = GameState.MainMenu;
				}
			}

			if (state == GameState.Game) {
				if (gamePadState.ThumbSticks.Left.X < -0.1 || keyboardState.IsKeyDown(Keys.Left)) {
					ship.angle -= turnSpeed;
				}

				if (gamePadState.ThumbSticks.Left.X > 0.1 || keyboardState.IsKeyDown(Keys.Right)) {
					ship.angle += turnSpeed;
				}

				if (gamePadState.ThumbSticks.Left.Y > 0.1 || keyboardState.IsKeyDown(Keys.Up)) {
					ship.dx += Math.Cos (ship.angle) * 0.1;
					ship.dy += Math.Sin (ship.angle) * 0.1;

					for (int i = 0; i < 15; i ++) {
						var plume = new Bullet ();
						plume.x = ship.x + Math.Cos (ship.angle) * -25;
						plume.y = ship.y + Math.Sin (ship.angle) * -25;
						plume.angle = ship.angle + Math.PI + (rnd.NextDouble () * 1.5 - 0.75);
						plume.dx = Math.Cos (plume.angle) * rnd.NextDouble();
						plume.dy = Math.Sin (plume.angle) * rnd.NextDouble();
						plume.texture = exhaustTexture;
						plume.lifeTime = gameTime.TotalGameTime;
						particles.Add (plume);
					}
				}

				if (GamePad.GetState (PlayerIndex.One).Buttons.A == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Space)) {
					Shoot (gameTime);
				}
			} else {
				if (GamePad.GetState (PlayerIndex.One).Buttons.A == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Space)) {
					spawnRocks ();
					SpawnShip ();
					lives = 3;
					state = GameState.Game;
				}
			}

			bullets.RemoveAll (x => gameTime.TotalGameTime - x.lifeTime > bulletFlightTime);

			foreach (var bullet in bullets) {
				moveEntity (bullet);

				var bulletOrigin = 
					new Vector2 ((float)bullet.x, 
					             (float)bullet.y);

				var bulletCircle = new Circle (bulletOrigin, bullet.texture.Width / 2);

				foreach (var asteroid in asteroids) {
					var asteroidOrigin = 
						new Vector2 ((float)asteroid.x, 
						             (float)asteroid.y);
					var asteroidCircle = new Circle (asteroidOrigin, asteroid.texture.Width / 2);

					if (bulletCircle.Intersects (asteroidCircle)) {
						DestroyAsteroid (asteroid, bullet);
					}
				}
			}

			particles.RemoveAll (x => gameTime.TotalGameTime - x.lifeTime > particleTime);

			foreach (var particle in particles) {
				moveEntity (particle);
			}

			foreach (var asteroid in destroyedAsteroids) {
				asteroids.Remove (asteroid);
			}
			destroyedAsteroids.Clear ();

			foreach (var asteroid in newAsteroids) {
				asteroids.Add (asteroid);
			}
			newAsteroids.Clear ();

			foreach (var bullet in destroyedBullets) {
				bullets.Remove (bullet);
			}

			if (asteroids.Count == 0) {
				spawnRocks ();
			}

			moveEntity (ship);

			var shipOrigin =
				new Vector2((float)ship.x, 
				            (float)ship.y);

			var shipCircle = new Circle (shipOrigin, ship.texture.Width / 2 - 4);

			foreach (var asteroid in asteroids) {
				moveEntity (asteroid);

				var asteroidOrigin = 
					new Vector2 ((float)asteroid.x, 
					             (float)asteroid.y);

				var asteroidCircle = new Circle (asteroidOrigin, asteroid.texture.Width / 2 - 4);

				if (state == GameState.Game) {
					if (shipCircle.Intersects (asteroidCircle)) {
						ShipExplosion ();
						break;
					}
				}
			}

			backGround.Update (gameTime);

            base.Update(gameTime);
        }

		protected void DestroyAsteroid(Asteroid asteroid, Bullet bullet) {
			destroyedAsteroids.Add (asteroid);
			destroyedBullets.Add (bullet);

			if (asteroid.phase > 0) {
				for (int i = 0; i < 3; i++) {
					var newAsteroid = new Asteroid ();
					newAsteroid.x = asteroid.x;
					newAsteroid.y = asteroid.y;
					newAsteroid.dx = rnd.NextDouble () * 2.0 - 1.0;
					newAsteroid.dy = rnd.NextDouble () * 2.0 - 1.0;
					newAsteroid.dangle = rnd.NextDouble () * 0.02;
					newAsteroid.phase = asteroid.phase - 1;
					newAsteroid.texture = rockTextures[newAsteroid.phase];

					newAsteroids.Add (newAsteroid);
				}
			}
		}

		protected void Shoot(GameTime gameTime) {

			if (gameTime.TotalGameTime - bulletFired < new TimeSpan (0, 0, 0, 0, 500)) {
				return;
			}

			var bullet = new Bullet ();
			bullet.x = ship.x;
			bullet.y = ship.y;
			bullet.angle = ship.angle;
			bullet.dx = Math.Cos (bullet.angle) * 4;
			bullet.dy = Math.Sin (bullet.angle) * 4;
			bullet.texture = bulletTexture;
			bullet.lifeTime = gameTime.TotalGameTime;
			bullets.Add (bullet);

			bulletFired = gameTime.TotalGameTime;
		}

		protected void ShipExplosion() {
			lives--;

			if (lives < 0) {
				state = GameState.MainMenu;
			}

			SpawnShip ();
		}

		protected void SpawnShip() {
			if (ship == null) {
				ship = new Entity ();
			}
			ship.x = screenWidth / 2;
			ship.y = screenHeight / 2;
			ship.dx = 0;
			ship.dy = 0;
			ship.angle = leftTurn;
			ship.texture = shipTexture;
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
           	graphics.GraphicsDevice.Clear(Color.Black);
		
			spriteBatch.Begin();

			Vector2 location;
			Rectangle sourceRectangle;
			Vector2 origin;

			backGround.Draw (spriteBatch);

			foreach (var particle in particles) {
				particle.Draw (spriteBatch);
			}

			if (state == GameState.Game) {
				ship.Draw (spriteBatch);
			}

			foreach (var asteroid in asteroids) {
				asteroid.Draw (spriteBatch);
			}

			foreach (var bullet in bullets) {
				bullet.Draw (spriteBatch);
			}

			if (state == GameState.Game) {
				for (int i = 0; i < lives; i++) {
					location = new Vector2 (ship.texture.Width / 2 * i + 20, ship.texture.Height / 2);
					sourceRectangle = new Rectangle (0, 0, ship.texture.Width, ship.texture.Height);
					origin = new Vector2 (ship.texture.Width / 2, ship.texture.Height / 2);

					spriteBatch.Draw (texture: ship.texture,
					                  position: location,
					                  sourceRectangle: sourceRectangle,
					                  color: Color.White,
					                  rotation: leftTurn,
					                  origin: origin,
					                  scale: 0.5f,
					                  effect: SpriteEffects.None,
					                  depth: 1);
				}
			} else {
				location = new Vector2 (screenWidth / 2, screenHeight / 2);
				sourceRectangle = new Rectangle (0, 0, titleTexture.Width, titleTexture.Height);
				origin = new Vector2 (titleTexture.Width / 2, titleTexture.Height / 2);

				spriteBatch.Draw (texture: titleTexture,
				                  position: location,
				                  sourceRectangle: sourceRectangle,
				                  color: Color.White,
				                  rotation: 0.0f,
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

