using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Media;
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
		float leftTurn = (float)Math.PI / -2;

		int lives = 3;

		double turnSpeed = 0.1;
		int screenWidth = 800;
		int screenHeight = 600;

		Texture2D shipTexture;
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

		Texture2D starTexture;
		List<Texture2D> starTextures = new List<Texture2D> ();
		List<Star> stars = new List<Star> ();
		double starDistance;
		double starAngle;
		double starTurnSpeed = 0.02;
		TimeSpan starTimer;
		TimeSpan starPeriod = new TimeSpan (0, 0, 5);

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

			starDistance = Math.Sqrt (screenWidth * screenWidth + screenHeight * screenHeight);

			graphics.PreferredBackBufferWidth = screenWidth;
			graphics.PreferredBackBufferHeight = screenHeight;
			graphics.IsFullScreen = false;
			graphics.ApplyChanges();

			ship = new Entity ();
			ship.x = screenWidth / 2;
			ship.y = screenHeight / 2;
			ship.dx = 0;
			ship.dy = 0;
			ship.angle = leftTurn;
			ship.texture = shipTexture;

			addNewRock (50, 50);
			addNewRock (screenWidth - 50, 50);
			addNewRock (screenWidth - 50, screenHeight - 50);
			addNewRock (50, screenHeight - 50);

			for (int i = 0; i < 800; i++) {
				var star = new Star ();
				star.distance = rnd.NextDouble() * starDistance;
				star.angle = rnd.NextDouble() * fullCircle;
				star.speed = rnd.NextDouble () * 3 + 1;
				star.texture = starTexture;
				stars.Add (star);
			}

			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play (BackgroundMusic);
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

			shipTexture = Content.Load<Texture2D> ("ship.png");
			bulletTexture = Content.Load<Texture2D> ("bullet.png");

			rockTextures.Add (Content.Load<Texture2D> ("rock_1.png"));
			rockTextures.Add (Content.Load<Texture2D> ("rock_2.png"));
			rockTextures.Add (Content.Load<Texture2D> ("rock_3.png"));
			rockTextures.Add (Content.Load<Texture2D> ("rock_4.png"));

			starTexture = Content.Load<Texture2D> ("star.png");

			starTextures.Add(Content.Load<Texture2D> ("star_0.png"));
			starTextures.Add(Content.Load<Texture2D> ("star_1.png"));
			starTextures.Add(Content.Load<Texture2D> ("star_2.png"));
			starTextures.Add(Content.Load<Texture2D> ("star_3.png"));

			BackgroundMusic = Content.Load<Song> ("bg.wav");
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

			if (GamePad.GetState (PlayerIndex.One).Buttons.A == ButtonState.Pressed) {
				Shoot (gameTime);
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

				if (shipCircle.Intersects (asteroidCircle)) {
					ShipExplosion ();
				}
			}

			if (gameTime.TotalGameTime - starTimer > starPeriod) {
				var direction = rnd.Next (-1, 1);
				starTurnSpeed = 0.02 * direction;
				starTimer = gameTime.TotalGameTime;
			}

			starAngle += starTurnSpeed;

			if (starAngle > fullCircle) {
				starAngle -= fullCircle;
			}

			if (starAngle < 0) {
				starAngle += fullCircle;
			}

			foreach (var star in stars) {
				star.distance += star.speed;

				if (star.distance > starDistance) {
					star.distance = rnd.NextDouble() * 150;
				}
			}

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
				Exit ();
			}

			ship.x = screenWidth / 2;
			ship.y = screenHeight / 2;
			ship.dx = 0;
			ship.dy = 0;
			ship.angle = leftTurn;
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

		protected Texture2D getStarTexture(Star star) {
			var period = starDistance / 10;
			if (star.distance < period) {
				return starTextures [0];
			}

			if (star.distance < period * 2) {
				return starTextures [1];
			}

			if (star.distance < period * 3) {
				return starTextures [2];
			}

			return starTextures [3];
		}

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
			var center = new Vector2 (screenWidth / 2, screenHeight / 2);

           	graphics.GraphicsDevice.Clear(Color.Black);
		
			spriteBatch.Begin();

			Vector2 location;
			Rectangle sourceRectangle;
			Vector2 origin;

			foreach (var star in stars) {

				location = new Vector2 ((float)(Math.Sin (star.angle + starAngle) * star.distance + center.X),
				                        (float)(Math.Cos (star.angle + starAngle) * star.distance + center.Y));

				sourceRectangle = new Rectangle(0, 0, star.texture.Width, star.texture.Height);
				origin = new Vector2(star.texture.Width / 2, star.texture.Height / 2);

				spriteBatch.Draw (texture: getStarTexture(star),
				                  position: location,
				                  sourceRectangle: sourceRectangle,
				                  color: Color.White,
				                  rotation: 0.0f,
				                  origin: origin,
				                  scale: 1.0f,
				                  effect: SpriteEffects.None,
				                  depth: 1);
			}

			location = new Vector2((int)ship.x, (int)ship.y);
			sourceRectangle = new Rectangle(0, 0, ship.texture.Width, ship.texture.Height);
			origin = new Vector2(ship.texture.Width / 2, ship.texture.Height / 2);

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

			foreach (var bullet in bullets) {
				location = new Vector2((int)bullet.x, (int)bullet.y);
				sourceRectangle = new Rectangle(0, 0, bullet.texture.Width, bullet.texture.Height);
				origin = new Vector2(bullet.texture.Width / 2, bullet.texture.Height / 2);

				spriteBatch.Draw (texture: bullet.texture,
				                  position: location,
				                  sourceRectangle: sourceRectangle,
				                  color: Color.White,
				                  rotation: (float)bullet.angle,
				                  origin: origin,
				                  scale: 1.0f,
				                  effect: SpriteEffects.None,
				                  depth: 1);
			}

			for (int i = 0; i < lives; i++) {
				location = new Vector2(ship.texture.Width / 2 * i + 20, ship.texture.Height / 2);
				sourceRectangle = new Rectangle(0, 0, ship.texture.Width, ship.texture.Height);
				origin = new Vector2(ship.texture.Width / 2, ship.texture.Height / 2);

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

			spriteBatch.End();
            //TODO: Drawing on the edge (ie. two blits instead of one)
            
            base.Draw(gameTime);
        }
    }
}

