using System;
using System.Collections.Generic;
using System.Globalization;

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
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;		

		float leftTurn = (float)Math.PI / -2;

		GameState state = GameState.MainMenu;
		int lives = 3;
        int score = 0;

		double turnSpeed = 0.1;
		int screenWidth = 1920;
		int screenHeight = 1080;

		IScreenMovement movement;

		StarField backGround;

		Texture2D titleTexture;

		Texture2D shipTexture, exhaustTexture;
		Entity ship;

		List<Texture2D> rockTextures = new List<Texture2D> ();
		List<Asteroid> asteroids = new List<Asteroid> ();

		List<Asteroid> newAsteroids = new List<Asteroid> ();
		List<Asteroid> destroyedAsteroids = new List<Asteroid> ();

		Texture2D bulletTexture;
        List<Particle> bullets = new List<Particle> ();
        List<Particle> destroyedBullets = new List<Particle> ();
		TimeSpan bulletFired;
		TimeSpan bulletFlightTime = new TimeSpan (0, 0, 3);

		Thruster thruster;

		List<IParticleEmitter> particles = new List<IParticleEmitter> ();

        Dictionary<Char, Texture2D> numbers = new Dictionary<Char, Texture2D> ();

		Random rng = new Random();

		Song BackgroundMusic;

        public Main()
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
			graphics.IsFullScreen = true;
			graphics.ApplyChanges();

			SpawnShip ();
			spawnRocks ();

			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play (BackgroundMusic);
			MediaPlayer.Volume = 0.5f;
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
			tempRock.dx = rng.NextDouble () * 2.0 - 1.0;
			tempRock.dy = rng.NextDouble () * 2.0 - 1.0;
			tempRock.dangle = (rng.NextDouble () * 2 - 1) * 0.02;
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

			titleTexture = Content.Load<Texture2D> ("title");

			shipTexture = Content.Load<Texture2D> ("ship");
			bulletTexture = Content.Load<Texture2D> ("bullet");
			exhaustTexture = Content.Load<Texture2D> ("exhaust");

			rockTextures.Add (Content.Load<Texture2D> ("rock_1"));
			rockTextures.Add (Content.Load<Texture2D> ("rock_2"));
			rockTextures.Add (Content.Load<Texture2D> ("rock_3"));
			rockTextures.Add (Content.Load<Texture2D> ("rock_4"));

			var starTextures = new List<Texture2D> ();
			starTextures.Add(Content.Load<Texture2D> ("star_0"));
			starTextures.Add(Content.Load<Texture2D> ("star_1"));
			starTextures.Add(Content.Load<Texture2D> ("star_2"));
			starTextures.Add(Content.Load<Texture2D> ("star_3"));

            numbers ['0'] = Content.Load<Texture2D> ("0");
            numbers ['1'] = Content.Load<Texture2D> ("1");
            numbers ['2'] = Content.Load<Texture2D> ("2");
            numbers ['3'] = Content.Load<Texture2D> ("3");
            numbers ['4'] = Content.Load<Texture2D> ("4");
            numbers ['5'] = Content.Load<Texture2D> ("5");
            numbers ['6'] = Content.Load<Texture2D> ("6");
            numbers ['7'] = Content.Load<Texture2D> ("7");
            numbers ['8'] = Content.Load<Texture2D> ("8");
            numbers ['9'] = Content.Load<Texture2D> ("9");

			BackgroundMusic = Content.Load<Song> ("bg");

			backGround = new StarField (rng, screenWidth, screenHeight, starTextures);
			backGround.Init ();

			movement = new DonutSpace(screenWidth, screenHeight);
			thruster = new Thruster (rng, exhaustTexture, movement);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
			GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
			KeyboardState keyboardState = Keyboard.GetState();

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

					thruster.x = ship.x + Math.Cos (ship.angle) * -25;
					thruster.y = ship.y + Math.Sin (ship.angle) * -25;
					thruster.angle = ship.angle + Math.PI;

					thruster.Emit (gameTime);
				}

				if (GamePad.GetState (PlayerIndex.One).Buttons.A == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Space)) {
					Shoot (gameTime);
				}
			} else {
				if (GamePad.GetState (PlayerIndex.One).Buttons.A == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Space)) {
					spawnRocks ();
					SpawnShip ();
					lives = 3;
                    score = 0;
					state = GameState.Game;
				}
			}

			bullets.RemoveAll (x => gameTime.TotalGameTime - x.SpawnTime > bulletFlightTime);

			foreach (var bullet in bullets) {
				movement.MoveEntity (bullet);

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
						DestroyAsteroid (asteroid, bullet, gameTime);
					}
				}
			}

			thruster.Update (gameTime);

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

			particles.RemoveAll (x => x.Done);
			foreach (var emitter in particles) {
				emitter.Update (gameTime);
			}

			if (asteroids.Count == 0) {
				spawnRocks ();
			}

			movement.MoveEntity (ship);

			var shipOrigin =
				new Vector2((float)ship.x, 
				            (float)ship.y);

			var shipCircle = new Circle (shipOrigin, ship.texture.Width / 2 - 4);

			foreach (var asteroid in asteroids) {
				movement.MoveEntity (asteroid);

				var asteroidOrigin = 
					new Vector2 ((float)asteroid.x, 
					             (float)asteroid.y);

				var asteroidCircle = new Circle (asteroidOrigin, asteroid.texture.Width / 2 - 4);

				if (state == GameState.Game) {
					if (shipCircle.Intersects (asteroidCircle)) {
						ShipExplosion (gameTime);
						break;
					}
				}
			}

			backGround.Update (gameTime);

            base.Update(gameTime);
        }

        protected void DestroyAsteroid(Asteroid asteroid, Particle bullet, GameTime gameTime) {
            score++;

            destroyedAsteroids.Add (asteroid);
			destroyedBullets.Add (bullet);

			if (asteroid.phase > 0) {
				for (int i = 0; i < 3; i++) {
					var newAsteroid = new Asteroid ();
					newAsteroid.x = asteroid.x;
					newAsteroid.y = asteroid.y;
					newAsteroid.dx = rng.NextDouble () * 2.0 - 1.0;
					newAsteroid.dy = rng.NextDouble () * 2.0 - 1.0;
					newAsteroid.dangle = rng.NextDouble () * 0.02;
					newAsteroid.phase = asteroid.phase - 1;
					newAsteroid.texture = rockTextures[newAsteroid.phase];

					newAsteroids.Add (newAsteroid);
				}
			}

            var explosion = new Explosion (rng, bulletTexture, movement) {
                ParticleCount = 10,
                ParticleSpeed = 4,
                ParticleLifeTime = new TimeSpan (0, 0, 0, 0, 250),
                EmitterLifeTime = new TimeSpan(0, 0, 0, 0, 10),
                x = bullet.x,
                y = bullet.y
            };

            explosion.Emit (gameTime);
            particles.Add (explosion);
		}

		protected void Shoot(GameTime gameTime) {

			if (gameTime.TotalGameTime - bulletFired < new TimeSpan (0, 0, 0, 0, 500)) {
				return;
			}

            var bullet = new Particle ()
            {
                x = ship.x,
                y = ship.y,
                angle = ship.angle,
                dx = Math.Cos (ship.angle) * 4,
                dy = Math.Sin (ship.angle) * 4,
                texture = bulletTexture,
                SpawnTime = gameTime.TotalGameTime
            };

			bullets.Add (bullet);

			bulletFired = gameTime.TotalGameTime;
		}

		protected void ShipExplosion(GameTime gameTime) {

			var explosion = new Explosion (rng, bulletTexture, movement) {
                ParticleCount = 15,
                ParticleSpeed = 4,
                ParticleLifeTime = new TimeSpan (0, 0, 0, 0, 750),
                EmitterLifeTime = new TimeSpan(0, 0, 0, 0, 50),
				x = ship.x,
				y = ship.y
			};

			explosion.Emit (gameTime);
			particles.Add (explosion);

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

			thruster.Draw (spriteBatch);

			if (state == GameState.Game) {
				ship.Draw (spriteBatch);
			}

			foreach (var asteroid in asteroids) {
				asteroid.Draw (spriteBatch);
			}

			foreach (var bullet in bullets) {
				bullet.Draw (spriteBatch);
			}

			foreach (var emitter in particles) {
				emitter.Draw (spriteBatch);
			}

			if (state == GameState.Game) {
				for (int i = 0; i < lives; i++) {
					location = new Vector2 (ship.texture.Width / 2 * i + 20, ship.texture.Height / 2);
					sourceRectangle = new Rectangle (0, 0, ship.texture.Width, ship.texture.Height);
					origin = new Vector2 (ship.texture.Width / 2, ship.texture.Height / 2);

                    spriteBatch.Draw(texture: ship.texture,
                                      position: location,
                                      sourceRectangle: sourceRectangle,
                                      color: Color.White,
                                      rotation: leftTurn,
                                      origin: origin,
                                      scale: new Vector2(0.5f));
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
				                  origin: origin);
			}

            var scoreStr = score.ToString (CultureInfo.InvariantCulture);
            int index = 0;
            foreach (var chr in scoreStr) {
                location = new Vector2 (25 * index + 85, 12);
                sourceRectangle = new Rectangle (0, 0, 25, 25);
                origin = new Vector2 (0, 0);
                spriteBatch.Draw (texture: numbers[chr],
                                  position: location,
                                  sourceRectangle: sourceRectangle,
                                  color: Color.White,
                                  rotation: 0.0f,
                                  origin: origin);
                index++;
            }

			spriteBatch.End();
            //TODO: Drawing on the edge (ie. two blits instead of one)
            
            base.Draw(gameTime);
        }
    }
}

