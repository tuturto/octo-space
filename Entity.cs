using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace helloWorld
{
	public interface IEntity
	{
		void Update (GameTime gameTime);
		void Draw (SpriteBatch spriteBatch);
	}
	
	public class Entity
	{
		public Entity ()
		{
		}

		public double x { get; set; }
		public double y { get; set; }

		public double dx { get; set; }
		public double dy { get; set; }

		public double angle { get; set; }
		public double dangle { get; set; }

		public Texture2D texture { get; set; }

		public void Draw(SpriteBatch spriteBatch) {
			var location = new Vector2((int)x, (int)y);
			var sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
			var origin = new Vector2(texture.Width / 2, texture.Height / 2);

			spriteBatch.Draw (texture: texture,
			                  position: location,
			                  sourceRectangle: sourceRectangle,
			                  color: Color.White,
			                  rotation: (float)angle,
			                  origin: origin,
			                  scale: 1.0f,
			                  effect: SpriteEffects.None,
			                  depth: 1);
		}
	}

	public class Asteroid : Entity
	{
		public int phase { get; set; }
	}

	public class Particle : Entity
	{
		public TimeSpan SpawnTime { get; set; }
        public TimeSpan LifeTime { get; set; }
	}

	public class Star
	{
		public double distance { get; set; }
		public double angle { get; set; }
		public double speed { get; set; }
	}
}
