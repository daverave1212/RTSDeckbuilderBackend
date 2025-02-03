using System;

public static class ExtensionUtils {

	public static float DistanceTo(this (float, float) pointA, (float, float) pointB) {
		var (ax, ay) = pointA;
		var (bx, by) = pointB;
		return (float) Math.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));
	}

	public static bool IsLowerThanDistanceBetweenPoints(
		this float distance,
		(float, float) pointA,
		(float, float) pointB
	) {
		var (ax, ay) = pointA;
		var (bx, by) = pointB;

		var distanceBetweenPointsNoSqr = Math.Pow(ax - bx, 2) + Math.Pow(ay - by, 2);

		return Math.Pow(distance, 2) < distanceBetweenPointsNoSqr;
	}

	public static bool IsLowerEThanDistanceBetweenPoints(
		this float distance,
		(float, float) pointA,
		(float, float) pointB
	) {
		var (ax, ay) = pointA;
		var (bx, by) = pointB;

		var distanceBetweenPointsNoSqr = Math.Pow(ax - bx, 2) + Math.Pow(ay - by, 2);

		return Math.Pow(distance, 2) <= distanceBetweenPointsNoSqr;
	}

	public static bool IsGreaterEThanDistanceBetweenPoints(
		this float distance,
		(float, float) pointA,
		(float, float) pointB
	) {
		var (ax, ay) = pointA;
		var (bx, by) = pointB;

		var distanceBetweenPointsNoSqr = Math.Pow(ax - bx, 2) + Math.Pow(ay - by, 2);

		return Math.Pow(distance, 2) >= distanceBetweenPointsNoSqr;
	}

	public static (float, float) LerpTo(
		this (float, float) pointA,
		(float, float) pointB,
		float distance
	) {
		var (ax, ay) = pointA;
		var (bx, by) = pointB;

		var dx = bx - ax;
		var dy = by - ay;
		var distanceAB = pointA.DistanceTo(pointB);

		var unitDX = dx / distanceAB;
		var unitDY = dy / distanceAB;

		var resultX = ax + unitDX * distance;
		var resultY = ay + unitDY * distance;

		return (resultX, resultY);
	}


}

