using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace helloWorld
{
	public interface IEntity
	{
		void Update (GameTime gameTime);
		void Draw (GameTime gameTime);
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
	}

	public class Asteroid : Entity
	{
		public int phase { get; set; }
	}

	public class Bullet : Entity
	{
		public TimeSpan lifeTime { get; set; }
	}

	public class Star
	{
		public double distance { get; set; }
		public double angle { get; set; }
		public double speed { get; set; }

		public Texture2D texture { get; set; }
	}
}
