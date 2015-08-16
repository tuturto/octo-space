using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace helloWorld
{
	public class Thruster : IParticleEmitter
	{
		public Thruster (Random rng, Texture2D texture, IScreenMovement movement)
		{
            particles = new List<Particle> ();
			particleTime = new TimeSpan (0, 0, 0, 0, 500);
			this.rng = rng;
			this.texture = texture;
			this.movement = movement;
		}

		protected Random rng;
		protected Texture2D texture;
		protected IScreenMovement movement;

		protected List<Particle> particles;
		protected TimeSpan particleTime;

		public double x { get; set; }
		public double y { get; set; }
		public double angle { get; set; }

		public void Emit(GameTime gameTime) {
			for (int i = 0; i < 15; i ++) {
                var plume = new Particle ();
				plume.x = x;
				plume.y = y;
				plume.angle = angle + (rng.NextDouble () * 1.5 - 0.75);
				plume.dx = Math.Cos (plume.angle) * rng.NextDouble();
				plume.dy = Math.Sin (plume.angle) * rng.NextDouble();
				plume.texture = texture;
				plume.SpawnTime = gameTime.TotalGameTime;
                plume.LifeTime = new TimeSpan (0, 0, 0, 0, rng.Next(250, 1000));
				particles.Add (plume);
			}
		}

		public bool Done {
			get {
				return false;
			}
		}

		public void Update(GameTime gameTime) {
			particles.RemoveAll (x => gameTime.TotalGameTime - x.SpawnTime > x.LifeTime);

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

