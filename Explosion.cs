using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace helloWorld
{
	public class Explosion : IParticleEmitter
	{
		public Explosion (Random rng, Texture2D texture, IScreenMovement movement)
		{
			particles = new List<Bullet> ();

			this.rng = rng;
			this.texture = texture;
			this.movement = movement;
		}

		protected TimeSpan startTime = TimeSpan.MinValue;

		protected Random rng;
		protected Texture2D texture;
		protected IScreenMovement movement;

		protected List<Bullet> particles;

		public double x { get; set; }
		public double y { get; set; }

        public int ParticleCount { get; set; }
        public int ParticleSpeed { get; set; }
        public TimeSpan EmitterLifeTime { get; set; }
        public TimeSpan ParticleLifeTime { get; set; }

		public void Emit(GameTime gameTime) {
			startTime = gameTime.TotalGameTime;

			SpawnShrapnel (gameTime);
		}

		private void SpawnShrapnel(GameTime gameTime) {
			for (int i = 0; i < ParticleCount; i ++) {
				var sharpnel = new Bullet ();
				sharpnel.x = x;
				sharpnel.y = y;
				sharpnel.angle = rng.NextDouble () * 2 * Math.PI;
				sharpnel.dx = ParticleSpeed * Math.Cos (sharpnel.angle) * rng.NextDouble();
                sharpnel.dy = ParticleSpeed * Math.Sin (sharpnel.angle) * rng.NextDouble();
				sharpnel.texture = texture;
				sharpnel.lifeTime = gameTime.TotalGameTime;
				particles.Add (sharpnel);
			}
		}

		public bool Done {
			get {
				return particles.Count == 0;
			}
		}

		public void Update(GameTime gameTime) {
			if (gameTime.TotalGameTime - startTime < EmitterLifeTime) {
				SpawnShrapnel (gameTime);
			}

			particles.RemoveAll (x => gameTime.TotalGameTime - x.lifeTime > ParticleLifeTime);

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

