﻿
using System.Text.Json;

public class Program {
	
	static void Main(string[] args) {
		Console.WriteLine("MAIN");

		var gameServer = new GameServer();
		gameServer.StartServerAndDontWait();
		gameServer.CreateLiveGame("asdf");


		while (true) { }
	}

	static void StartMap() {
		SquareTilePathSubmesh.N_SUBMESHES_PER_TILE = 4;
		var map = new GameMapRep(1, 1);

		map.PrintIndices();
		Console.WriteLine("");
		map.Print();
		Console.WriteLine("");
		Console.WriteLine("");

		//map.matrix[0, 0].PrintNodes();
		//Console.WriteLine("");
		//map.matrix[0, 1].PrintNodes();
		//Console.WriteLine("");

		map.PrintPathNodes();
		Console.WriteLine("");
		Console.WriteLine("");

		var tile = map.matrix[0,0];
		var fromNode = tile.GetNodeAt(0, 0);
		var toNode = tile.GetNodeAt(3, 3);

		var upNode = tile.GetNodeAt(0, 1);
		var leftNode = tile.GetNodeAt(1, 0);
		Console.WriteLine(upNode.downNode.ToString());
		Console.WriteLine(leftNode.rightNode.ToString());

		Console.WriteLine("");
		Console.WriteLine("");


		PathNode[] res = XAlgorithms.CalculatePathAStarInclusive<PathNode>(
			fromNode,
			toNode,
			getDistanceBetweenNodes: (nodeA, nodeB) => nodeA.GetDistanceToNode(nodeB),
			getNeighbors: node => node.GetAvailableNeighbors()
		);


		//var result = PathNode.CalculatePathAStar(fromNode, toNode);
		foreach (var node in res) {
			Console.WriteLine(node.ToString());
		}
	}
}