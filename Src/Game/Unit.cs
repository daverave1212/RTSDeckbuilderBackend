using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public partial class Unit { // Stats
	public float speed = 70f;
	public float sightRadius = 150f;
}



public partial class Unit { // Base

	public GameMap gameMap;

	public Unit(GameMap gameMap, (float, float) onPosition) {
		this.gameMap = gameMap;
		gameMap.AddUnit(this);
		SetPosition(onPosition);
	}


	public void Tick(float deltaTime, bool isDebug=false) {
		if (isMoving) {
			TickMovement(deltaTime, isDebug);
		}
	}


}






public partial class Unit { // Movement

	bool isMoving = false;
	(float, float) destinationPosition;
	
	public (float, float) upcomingPosition;	// To send this inbetween ticks to client
	public GridNode<NodeData>[] currentNodesToDestination;

	public void OrderMove((float, float) toPosition) {	// Setup everything for movement
		destinationPosition = toPosition;
		upcomingPosition = GetPosition();
		isMoving = true;
		currentNodesToDestination = gameMap.GetNodesBetweenPositions(GetPosition(), toPosition);

		var isNoPathAvailable = currentNodesToDestination == null;
		if (isNoPathAvailable) {
			isMoving = false;
		}
	}



	void TickMovement(float deltaTime, bool isDebug=false) {
		if (!isMoving) {
			throw new Exception("TickMovement should not be called if unit is not moving");
		}

		SetPosition(upcomingPosition);
		Console.WriteLine($"Moved to: {GetPosition()}");

		if (GetPosition() == destinationPosition) {
			isMoving = false;
			return;
		}

		upcomingPosition = gameMap.LerpWithNodesWithCachedPath(
			fromPos: GetPosition(),
			toPos: destinationPosition,
			distanceTraveled: speed * deltaTime / 1000,
			getNodesPath: () => currentNodesToDestination,
			setNodesPath: (nodes) => { currentNodesToDestination = nodes; }
		);

		var willNotMove = upcomingPosition == GetPosition();
		if (willNotMove) {
			isMoving = false;
		}
		Console.WriteLine($"  upcomingPosition: {upcomingPosition}");

	}

}







public partial class Unit { // Position

	(float, float) position = (0f, 0f);

	public void SetPosition((float, float) pos) {
		position = pos;
	}
	public (float, float) GetPosition() {
		return position;
	}



	public float DistanceTo(Unit unit) {
		return GetPosition().DistanceTo(unit.GetPosition());
	}
	public bool IsWithinDistanceOf(Unit unit, float distance) {
		return distance.IsLowerEThanDistanceBetweenPoints(GetPosition(), unit.GetPosition());
	}




	public List<Unit> GetUnitsNearby() {
		var unitsNearby = new List<Unit>();
		foreach (var unit in gameMap.unitsOnMap) {
			if (unit == this) {
				continue;
			}
			if (IsWithinDistanceOf(unit, sightRadius)) {
				unitsNearby.Add(unit);
			}
		}
		return unitsNearby;
	}

	public Unit GetUnitNearby() {
		var unitsNearby = GetUnitsNearby();
		if (unitsNearby.Count == 0) {
			return null;
		}
		var closestUnit = unitsNearby[0];
		var closestDistance = DistanceTo(closestUnit);
		foreach (var unit in unitsNearby) {
			var distanceToThisUnit = DistanceTo(unit);
			if (distanceToThisUnit < closestDistance) {
				closestUnit = unit;
				closestDistance = distanceToThisUnit;
			}
		}
		return closestUnit;
	}

}




public static class UnitAlgorithms {

	

}




public partial class Unit {	// Tests

	public static void Test_Unit() {
		var gameMap = new GameMap(5, 5);

		Utils.DoEveryAsync(1000, () => {
			foreach (var unit in gameMap.unitsOnMap) {
				unit.Tick(1000);
			}
		});

		var unit = new Unit(gameMap, (-30f, -30f));
		unit.OrderMove((425, 125));
	}

}
