using System;
using System.Numerics;
using System.Xml.Linq;

public partial class PathNode {

	public SquareTilePathSubmesh submesh;

	public PathNode upNode;
	public PathNode rightNode;
	public PathNode downNode;
	public PathNode leftNode;

	public Point2D position;

	public bool isBlocked = false;

	public PathNode(SquareTilePathSubmesh _submesh, float x, float y) {
		submesh = _submesh;
		position = new Point2D(x, y);
	}

	public float GetDistanceToNode(PathNode otherNode) {
		return Math.Abs(position.x - otherNode.position.x) + Math.Abs(position.y - otherNode.position.y);
	}
	public PathNode[] GetAvailableNeighbors() {
		var neighborsSoFar = new List<PathNode>();
		void addNodeIfOk(PathNode node) {
			if (node != null && node.isBlocked == false && node.isVisited == false) neighborsSoFar.Add(node);
		}
		addNodeIfOk(upNode);
		addNodeIfOk(rightNode);
		addNodeIfOk(downNode);
		addNodeIfOk(leftNode);
		return neighborsSoFar.ToArray();
	}

	public override string ToString() {
		return position.ToString();
		//return submesh.tile.ToString();
	}
	public string ToStringNeighbors() {
		var n = "null";
		return $"(up:{(upNode != null ? upNode : n)},right:{(rightNode != null? rightNode: n)},down:{(downNode != null? downNode: n)},left:{(leftNode != null? leftNode: n)})";
	}
}



public partial class PathNode {

	public bool isVisited = false;

	public float distanceSoFar = 0;
	public float minimumDistanceToTarget = 0;
	public float totalEstimatedDistance = 0;
	
	public void Reset() {
		isVisited = false;
	}

	public void CalculateHeuristics(PathNode fromNode, PathNode target) {
		distanceSoFar = fromNode.distanceSoFar + SquareTilePathSubmesh.GetDistanceBetween2NeighborNodes();
		minimumDistanceToTarget = GetDistanceToNode(target);
		totalEstimatedDistance = distanceSoFar + minimumDistanceToTarget;
	}
	public void CalculateStartingHeuristics(PathNode target) {
		distanceSoFar = 0;
		minimumDistanceToTarget = GetDistanceToNode(target);
		totalEstimatedDistance = distanceSoFar + minimumDistanceToTarget;
	}

	

	public static string CalculatePathAStar(PathNode startNode, PathNode target) {
		Console.WriteLine($"Starting A* from {startNode} to {target}");
		List<PathNode> nodesToVisitOrderedQueue = new List<PathNode>();

		startNode.CalculateStartingHeuristics(target);
		nodesToVisitOrderedQueue.Add(startNode);

		while (nodesToVisitOrderedQueue.Count > 0) {
			var currentNode = nodesToVisitOrderedQueue[0];
			//Console.WriteLine($"Looping node {currentNode.ToString()}");
			if (currentNode == target) {
				return $"Finished and found! Node is: {currentNode}";
			}
			nodesToVisitOrderedQueue.RemoveAt(0);

			var nodesToVisit = currentNode.GetAvailableNeighbors();
			//Console.WriteLine($"  Neighbors: {currentNode.ToStringNeighbors()}");
			foreach (var node in nodesToVisit) {
				//Console.WriteLine($"  At neighbor {node.ToString()}");
				node.isVisited = true;
				node.CalculateHeuristics(currentNode, target);
				Utils.InsertInOrderedQueue(nodesToVisitOrderedQueue, node, node => node.totalEstimatedDistance);
			}
		}
		return "Nothing found unfortunately. Sad day for humanity.";
	}
}
