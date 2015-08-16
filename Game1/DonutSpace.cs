using System;

namespace helloWorld
{
	public interface IScreenMovement
	{
		void MoveEntity(Entity entity);
	}

	public class DonutSpace : IScreenMovement
	{
		public DonutSpace (int screenWidth, int screenHeight)
		{
			this.screenWidth = screenWidth;
			this.screenHeight = screenHeight;
		}

		protected int screenWidth;
		protected int screenHeight;
		protected float fullCircle = (float)(2 * Math.PI);

		public void MoveEntity(Entity entity) {
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
	}
}
