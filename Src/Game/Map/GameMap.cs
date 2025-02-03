using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

public partial class GameMap : GridMatrix<NodeData> {
	
	public GameMap(int r, int c) : base(
		r,
		c,
		MapK.DISTANCE_BETWEEN_MAP_NODES,
		MapK.MAP_X,
		MapK.MAP_Y
	) {

		ForEach(node => {
			node.data = new NodeData(node);
		});

	}

	public List<Unit> unitsOnMap = new List<Unit>();

	public void AddUnit(Unit unit) {
		unitsOnMap.Add(unit);
	}
	public void RemoveUnit(Unit unit) {
		unitsOnMap.Remove(unit);
	}
}






public partial class GameMap {  // Algorithms

	// Does not include startNode and endNode
	public GridNode<NodeData>[] GetNodesBetweenPositions((float, float) fromPos, (float, float) toPos) {
		var startNode = GetNodeByPosition(fromPos);
		var endNode = GetNodeByPosition(toPos);

		Console.WriteLine($"startNode: {startNode.ToString()}, endNode: {endNode.ToString()}");

		if (endNode.data.isBlocked) {
			return null;
		}

		if (startNode == endNode) {
			return new GridNode<NodeData>[] { };
		}

		var nodesToDestination = PathfindingAlgorithms.CalculatePathAStarInclusive<GridNode<NodeData>>(
			startNode: startNode,
			target: endNode,
			getDistanceBetweenNodes: (nodeA,nodeB) => {
				return nodeA.DistanceTo(nodeB);
			},
			getNeighbors: (node) => {
				return node.GetNeighbors().ToArray();
			}
		);

		if (nodesToDestination == null) {
			return null;
		}

		if (nodesToDestination.Length == 0) {
			return new GridNode<NodeData>[] { };
		}

		if (nodesToDestination[0] == startNode && nodesToDestination[nodesToDestination.Length - 1] == endNode) {
			if (nodesToDestination.Length == 1) { // Not sure how this can happen
				throw new Exception("How did this happen for GameMap?");
			}
			if (nodesToDestination.Length == 2) {
				return new GridNode<NodeData>[] { };
			}
		}
		var nodesExclusive = nodesToDestination.Skip(1).Take(nodesToDestination.Length - 2).ToArray();
		return nodesExclusive;
	}

	public static void Test_LerpWithNodesWithCachedPath() {
		var map = new GameMap(5, 5);

		

		void testWith((float, float) startPos, (float, float) endPos, float speed) {
			var currentPos = startPos;
			var destinationPos = endPos;
			var cachedNodes = map.GetNodesBetweenPositions(currentPos, destinationPos);

			Console.WriteLine($"Staring test from {startPos} to {endPos} speed {speed} with {cachedNodes.Length} nodes:");
			PrintNodes(cachedNodes);
			Console.WriteLine("");

			while (currentPos != destinationPos) {
				currentPos = map.LerpWithNodesWithCachedPath(
					currentPos,
					destinationPos,
					distanceTraveled: speed,
					getNodesPath: () => cachedNodes,
					setNodesPath: nodes => {
						cachedNodes = nodes;
						PrintNodes(nodes);
					}
				);
				Console.WriteLine($"New currentPos: {currentPos}");
			}
		}

		testWith((-30f, -30f), (425f, 125f), 70f);
		testWith((0f, 0f), (425f, 125f), 70f);
		testWith((-30f, -30f), (400f, 100f), 70f);
		testWith((-30f, -30f), (0f, 0f), 70f);
		testWith((-30f, -30f), (0f, 0f), 10f);
		testWith((-30f, -30f), (425f, 125f), 175f);
		testWith((-30f, -30f), (425f, 125f), 9999f);

	}

	// getNodesPath should return the nodes path fromPos toPos
	public (float, float) LerpWithNodesWithCachedPath(
		(float, float) fromPos,
		(float, float) toPos,
		float distanceTraveled,
		Func<GridNode<NodeData>[]> getNodesPath,
		Action<GridNode<NodeData>[]> setNodesPath
	) {

		var nodesPath = getNodesPath();

		if (nodesPath == null) {
			return fromPos;
		}

		if (nodesPath.Any(node => node.data.isBlocked)) {
			var updatedNodesPath = GetNodesBetweenPositions(fromPos, toPos);
			if (updatedNodesPath == null) {
				return fromPos;
			}
			setNodesPath(updatedNodesPath);
			nodesPath = getNodesPath();
		}

		if (nodesPath.Length == 0) {	// If there seems to be a direct path
			if (distanceTraveled.IsLowerThanDistanceBetweenPoints(fromPos, toPos)) {
				return fromPos.LerpTo(toPos, distanceTraveled);
			} else {
				return toPos;
			}
		}

		var distanceToFirstNode = fromPos.DistanceTo(nodesPath[0].GetPosition());
		if (distanceTraveled < distanceToFirstNode) {
			return fromPos.LerpTo(nodesPath[0].GetPosition(), distanceTraveled);
		}

		int nNodesTraveled = 0;
		float distanceLeftToTravel = distanceTraveled;
		var currentPos = fromPos;

		for (int i = 0; i < nodesPath.Length; i++) {
			var node = nodesPath[i];
			var distanceToNextNode = i == 0? distanceToFirstNode: nodesPath[i-1].DistanceTo(node);
			if (distanceLeftToTravel >= distanceToNextNode) {
				nNodesTraveled++;
				distanceLeftToTravel -= distanceToNextNode;
				currentPos = node.GetPosition();
			} else {
				break;
			}
		}

		var newNodesPath = nodesPath.Skip(nNodesTraveled).ToArray();    // All nodes from index nNodesTraveled
		setNodesPath(newNodesPath);

		if (newNodesPath.Length == 0) {
			var lastNode = nodesPath[nodesPath.Length - 1];
			var lastNodePos = lastNode.GetPosition();
			if (distanceLeftToTravel.IsGreaterEThanDistanceBetweenPoints(lastNodePos, toPos)) {
				return toPos;
			}
			return lastNodePos.LerpTo(toPos, distanceLeftToTravel);
		}

		var middleNodeA = nodesPath[nNodesTraveled-1];
		var middleNodeB = nodesPath[nNodesTraveled];
		return middleNodeA.GetPosition().LerpTo(middleNodeB.GetPosition(), distanceLeftToTravel);
	}

	public (float, float) LerpWithNodes((float, float) fromPos, (float, float) toPos, float distanceTraveled) {
		return LerpWithNodesWithCachedPath(
			fromPos,
			toPos,
			distanceTraveled,
			getNodesPath: () => GetNodesBetweenPositions(fromPos, toPos),
			setNodesPath: (val) => { }
		);
	}





	public static void PrintNodes(GridNode<NodeData>[] nodes) {
		Console.WriteLine($"Nodes: {string.Join(" -> ", nodes.Select(node => node.ToString()))}");
	}

}
