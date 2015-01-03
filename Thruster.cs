using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace helloWorld
{
	public interface IParticleEmitter
	{
		void Emit(GameTime gameTime);
	}

	public class Thruster : IParticleEmitter, IEntity
	{
		public Thruster (Random rng, Texture2D texture)
		{
			particles = new List<Bullet> ();
			particleTime = new TimeSpan (0, 0, 0, 0, 500);
			this.rng = rng;
			this.texture = texture;
		}

		protected Random rng;
		protected Texture2D texture;

		protected List<Bullet> particles;
		protected TimeSpan particleTime;

		public double x { get; set; }
		public double y { get; set; }
		public double angle { get; set; }

		public void Emit(GameTime gameTime) {
			for (int i = 0; i < 15; i ++) {
				var plume = new Bullet ();
				plume.x = x;
				plume.y = y;
				plume.angle = angle + (rng.NextDouble () * 1.5 - 0.75);
				plume.dx = Math.Cos (plume.angle) * rng.NextDouble();
				plume.dy = Math.Sin (plume.angle) * rng.NextDouble();
				plume.texture = texture;
				plume.lifeTime = gameTime.TotalGameTime;
				particles.Add (plume);
			}
		}

		public void Update(GameTime gameTime) {
			particles.RemoveAll (x => gameTime.TotalGameTime - x.lifeTime > particleTime);

			foreach (var particle in particles) {
				moveEntity (particle);
			}
		}

		public void Draw(SpriteBatch spriteBatch) {
			foreach (var particle in particles) {
				particle.Draw (spriteBatch);
			}
		}

		//TODO: duplicate code
		protected void moveEntity(Entity entity) {
			entity.x += entity.dx;
			entity.y += entity.dy;
			entity.angle += entity.dangle;

			if (entity.x < 0 - entity.texture.Width / 2) {
				entity.x = 800 + entity.texture.Width / 2;
			}

			if (entity.x > 800 + entity.texture.Width / 2) {
				entity.x = 0 - entity.texture.Width / 2;
			}

			if (entity.y < 0 - entity.texture.Height / 2) {
				entity.y = 600 + entity.texture.Height / 2;
			}

			if (entity.y > 600 + entity.texture.Height / 2) {
				entity.y = 0 - entity.texture.Height / 2;
			}

			if (entity.angle > 2 * Math.PI) {
				entity.angle -= 2 * Math.PI;
			}

			if (entity.angle < 0) {
				entity.angle += 2 * Math.PI;
			}
		}
	}
}

