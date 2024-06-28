using System;

public class UnitRep {

	LiveGame liveGame;

	public float speedPerSecond = 100;
	public Point2D position = new Point2D(0, 0);

	List<Point2D> currentMovementPath;

	public UnitRep(LiveGame liveGame) {
		this.liveGame = liveGame;
		TestInConstructor();
	}
	
	void TestInConstructor() {
		currentMovementPath = new List<Point2D>(new Point2D[] { new Point2D(50, 0), new Point2D(50, 50) });
		liveGame.AddUnit(this);
	}

	public void Update() {
		MovePerCurrentMovementPath();
	}

	void MovePerCurrentMovementPath() {
		if (currentMovementPath == null) {
			return;
		}
		if (currentMovementPath.Count == 0) {
			return;
		}
		var targetPoint = currentMovementPath[0];
		var didReachPoint = MoveTowardsPoint(targetPoint);
		if (didReachPoint) {
			currentMovementPath.RemoveAt(0);
		}
	}
	bool MoveTowardsPoint(Point2D point) {
		Console.WriteLine($"Now at {position}");
		if (position.DistanceToPoint(point) <= GetSpeedPerUpdate()) {   // Todo: speed remainder
			return true;
		}
		var xDelta = point.x - position.x;
		var yDelta = point.y - position.y;
		var angleRadians = Math.Atan2(yDelta, xDelta);
		var angleDegrees = angleRadians * (180 / Math.PI);
		var xSpeed = (float) Math.Cos(angleDegrees) * GetSpeedPerUpdate();
		var ySpeed = (float) Math.Sin(angleDegrees) * GetSpeedPerUpdate();
		position.x += xSpeed;
		position.y += ySpeed;
		return false;
	}

	float GetSpeedPerUpdate() {
		return speedPerSecond / (1000 / LiveGame.TICK_RATE);
	}
}
