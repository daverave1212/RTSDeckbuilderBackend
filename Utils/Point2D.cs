using System;

public struct Point2D {

	public float x;
	public float y;

	public Point2D(float _x, float _y) {
		x = _x;
		y = _y;
	}

	public Point2D Plus(float _x, float _y) {
		var newPoint = new Point2D(x + _x, y + _y);
		return newPoint;
	}

	public float DistanceToPoint(Point2D p) {
		return (float) Math.Sqrt(Math.Pow(p.x - x, 2) + Math.Pow(p.y - y, 2));
	}

	public override string ToString() {
		return $"(y:{padNumber(y)},x:{padNumber(x)})";
	}

	static string padNumber(float number) {
		string str = number.ToString();
		if (number >= 0) {
			str = " " + str;
		}
		if (Math.Abs(number) < 100) {
			str = " " + str;
		}
		if (Math.Abs(number) < 10) {
			str = " " + str;
		}
		return str;
	}
}
