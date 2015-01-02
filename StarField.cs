using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace helloWorld
{
	public class StarField : IEntity
	{
		public StarField (Random rng, int screenWidth, int screenHeight, 
		                  List<Texture2D> textures)
		{
			this.rng = rng;
			this.screenWidth = screenWidth;
			this.screenHeight = screenHeight;
			starDistance = Math.Sqrt (screenWidth * screenWidth + screenHeight * screenHeight);
			starTextures = textures;
		}

		private Random rng;
		int screenWidth, screenHeight;

		private List<Texture2D> starTextures;
		private List<Star> stars = new List<Star> ();
		private double starDistance;
		private double starAngle;
		private double starTurnSpeed = 0.02;
		private TimeSpan starTimer;
		private TimeSpan starPeriod = new TimeSpan (0, 0, 5);

		public void Init() {
			for (int i = 0; i < 800; i++) {
				var star = new Star ();
				star.distance = rng.NextDouble() * starDistance;
				star.angle = rng.NextDouble() * 2 * Math.PI;
				star.speed = rng.NextDouble () * 3 + 1;
				stars.Add (star);
			}
		}

		public void Update(GameTime gameTime) {
			if (gameTime.TotalGameTime - starTimer > starPeriod) {
				var direction = rng.Next (-1, 1);
				starTurnSpeed = 0.02 * direction;
				starTimer = gameTime.TotalGameTime;
			}

			starAngle += starTurnSpeed;

			if (starAngle > 2 * Math.PI) {
				starAngle -= 2 * Math.PI;
			}

			if (starAngle < 0) {
				starAngle += 2 * Math.PI;
			}

			foreach (var star in stars) {
				star.distance += star.speed;

				if (star.distance > starDistance) {
					star.distance = rng.NextDouble() * 150;
				}
			}
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {

			var center = new Vector2 (screenWidth / 2, screenHeight / 2);

			foreach (var star in stars) {

				var texture = getStarTexture (star);
				var location = new Vector2 ((float)(Math.Sin (star.angle + starAngle) * star.distance + center.X),
				                        (float)(Math.Cos (star.angle + starAngle) * star.distance + center.Y));

				var sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
				var origin = new Vector2(texture.Width / 2, texture.Height / 2);

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
	}
}

