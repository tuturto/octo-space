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
		public Thruster (Random rng, Texture2D texture, IScreenMovement movement)
		{
			particles = new List<Bullet> ();
			particleTime = new TimeSpan (0, 0, 0, 0, 500);
			this.rng = rng;
			this.texture = texture;
			this.movement = movement;
		}

		protected Random rng;
		protected Texture2D texture;
		protected IScreenMovement movement;

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
				movement.MoveEntity (particle);
			}
		}

		public void Draw(SpriteBatch spriteBatch) {
			foreach (var particle in particles) {
				particle.Draw (spriteBatch);
			}
		}
	}
}

