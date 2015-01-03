using System;

using Microsoft.Xna.Framework;

namespace helloWorld
{
	public interface IParticleEmitter : IEntity
	{
		void Emit(GameTime gameTime);

		bool Done { get; }
	}
}
