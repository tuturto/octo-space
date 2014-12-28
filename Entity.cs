using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace helloWorld
{
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
}