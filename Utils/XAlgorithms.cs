using System;

class XAlgorithms {
	public static T[] CalculatePathAStarInclusive<T>(T startNode, T target, Func<T, T, float> getDistanceBetweenNodes, Func<T, T[]> getNeighbors) {
		var nodeManager = new AStarNodeManager<T>(getDistanceBetweenNodes);

		nodeManager.CalculateStartingHeuristicsForNodeToTarget(startNode, target);
		nodeManager.QueueNodeToVisit(startNode);

		while (nodeManager.IsQueueEmpty() == false) {
			var currentNode = nodeManager.PopNodeToVisit();
			Console.WriteLine($"At node {currentNode}; distance: {getDistanceBetweenNodes(currentNode, target)}");

			if (getDistanceBetweenNodes(currentNode, target) == 0) {
				return nodeManager.GetNodesPathToNode(currentNode);
			}

			foreach (var neighborNode in getNeighbors(currentNode)) {
				if (nodeManager.IsNodeAlreadyVisited(neighborNode)) {
					continue;
				}
				nodeManager.CalculateHeuristicsForNodeFromNodeToTarget(neighborNode, currentNode, target);
				nodeManager.QueueNodeToVisit(neighborNode);
			}
		}

		Console.WriteLine("Found nothing noooo");
		return new T[0];
	}
}
class AStarNodeManager<T> {

	List<AStarNode<T>> nodesToVisitOrderedQueue;
	Dictionary<T, AStarNode<T>> nodesData;

	Func<T, T, float> getDistanceBetweenNodes;

	public AStarNodeManager(Func<T, T, float> getDistanceBetweenNodes) {
		nodesData = new Dictionary<T, AStarNode<T>>();
		nodesToVisitOrderedQueue = new List<AStarNode<T>>();

		this.getDistanceBetweenNodes = getDistanceBetweenNodes;
	}

	public AStarNode<T> GetNodeData(T obj) {
		if (nodesData.ContainsKey(obj) == false) {
			nodesData[obj] = new AStarNode<T>(obj);
		}
		return nodesData[obj];
	}

	public void CalculateStartingHeuristicsForNodeToTarget(T t, T target) {
		GetNodeData(t).CalculateStartingHeuristics(target, getDistanceBetweenNodes);
	}
	public void CalculateHeuristicsForNodeFromNodeToTarget(T t, T fromNode, T target) {
		GetNodeData(t).CalculateHeuristics(GetNodeData(fromNode), target, getDistanceBetweenNodes);
	}
	public void QueueNodeToVisit(T t) {
		Utils.InsertInOrderedQueue(nodesToVisitOrderedQueue, GetNodeData(t), aStarNode => aStarNode.totalEstimatedDistance);
	}
	public T PopNodeToVisit() {
		var node = nodesToVisitOrderedQueue[0].t;
		nodesToVisitOrderedQueue.RemoveAt(0);
		return node;
	}
	public bool IsQueueEmpty() {
		return nodesToVisitOrderedQueue.Count == 0;
	}
	public bool IsNodeAlreadyVisited(T t) {
		return GetNodeData(t).isVisited;
	}
	public T[] GetNodesPathToNode(T t) {
		var list = new List<T>();
		T currentNode = t;
		while (GetNodeData(currentNode).nodeGotHereFrom != null) {
			list.Add(currentNode);
			currentNode = GetNodeData(currentNode).nodeGotHereFrom.t;
		}
		list.Reverse();
		return list.ToArray();
	}




}
class AStarNode<T> {

	public bool isVisited = false;
	public float distanceSoFar = 0;
	public float minimumDistanceToTarget = 0;
	public float totalEstimatedDistance = 0;
	public AStarNode<T> nodeGotHereFrom = null;

	public T t;

	public AStarNode(T _t) {
		t = _t;
	}

	public void CalculateHeuristics(AStarNode<T> fromNode, T target, Func<T, T, float> getDistanceBetweenNodes) {
		distanceSoFar = fromNode.distanceSoFar + getDistanceBetweenNodes(fromNode.t, t);
		minimumDistanceToTarget = getDistanceBetweenNodes(fromNode.t, target);
		totalEstimatedDistance = distanceSoFar + minimumDistanceToTarget;
		nodeGotHereFrom = fromNode;
		isVisited = true;
	}
	public void CalculateStartingHeuristics(T target, Func<T, T, float> getDistanceBetweenNodes) {
		distanceSoFar = 0;
		minimumDistanceToTarget = getDistanceBetweenNodes(this.t, target);
		totalEstimatedDistance = distanceSoFar + minimumDistanceToTarget;
		isVisited = true;
	}
}

