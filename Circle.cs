using Microsoft.Xna.Framework;

public struct Circle
{
	public Circle(Vector2 origin, int radius)
	: this()
	{
		Origin = origin;
		Radius = radius;
	}

	public int Radius { get; private set; }
	public Vector2 Origin { get; private set; }

	public bool Intersects(Circle circle)
	{
		return Vector2.Distance(circle.Origin, Origin) < Radius + circle.Radius;
	}
}
