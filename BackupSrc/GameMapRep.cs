using System;
using System.Numerics;


/*
 *				[  ][  ][  ][  ]					[  ][  ][  ][  ]
 *			  [  ][  ][  ][  ]					  [  ][  ][  ][  ]
 *				[  ][  ][  ][  ]					[  ][  ][  ][  ]
 *			  [  ][  ][  ][  ]					  [  ][  ][  ][  ]
 *				[  ][  ][  ][  ]
 * 
 * */



public class GameMapRep
{

	public static void Test_GameMapRep() {
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


		PathNode[] res = PathfindingAlgorithms.CalculatePathAStarInclusive<PathNode>(
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


	public SquareTileRep[,] matrix;
	public int nRows;
	public int nCols;

	public GameMapRep(int r, int c)	{
		nRows = r;
		nCols = c;
		matrix = new SquareTileRep[nRows, nCols];
		for (int i = 0; i < nRows; i++) {
			for (int j = 0; j < nCols; j++) {
				matrix[i, j] = new SquareTileRep(this, i, j);
			}
		}
		outerLinkSubmeshNodes();
	}

	void outerLinkSubmeshNodes() {
		// Link horizontally	[  ][  ][  ][  ]
		for (int i = 0; i < nRows - 1; i++) {
			for (int j = 0; j < nCols - 1; j++) {
				var tile = matrix[i, j];
				var submesh = tile.tilePathSubmesh;
				
				var tileToMyRight = matrix[i, j+1];
				var submeshToMyRight = tileToMyRight.tilePathSubmesh;
				submesh.InterlinkWithSubmeshToMyRight(submeshToMyRight);
			}
		}

		// Link vertically
		for (int i = 0; i < nRows - 1; i++) {
			for (int j = 0; j < nCols; j++) {
				var tile = matrix[i, j];
				var myBottomLeftNodes = tile.tilePathSubmesh.GetBottomLeftHalfNodes();
				var myBottomRightNodes = tile.tilePathSubmesh.GetBottomRightHalfNodes();
				var isRowBelowOffsetToLeft = i % 2 == 0;
				if (isRowBelowOffsetToLeft) {																		//   [  ]
					var lowerLeftTile = matrix[i+1, j];																// [  ]
					var leftTileTopRightNodes = lowerLeftTile.tilePathSubmesh.GetTopRightHalfNodes();
					SquareTilePathSubmesh.InterlinkNodeArraysUpperLower(myBottomLeftNodes, leftTileTopRightNodes);
					if (j < nCols - 1) {																			//	 [  ]
						var lowerRightTile = matrix[i+1, j+1];														//	...[  ]
						var rightTileTopLeftNodes = lowerRightTile.tilePathSubmesh.GetTopLeftHalfNodes();
						SquareTilePathSubmesh.InterlinkNodeArraysUpperLower(myBottomRightNodes, rightTileTopLeftNodes);
					}
				} else {																							//	  [  ]
					var lowerRightTile = matrix[i+1, j];															//      [  ]
					var rightTileTopLeftNodes = lowerRightTile.tilePathSubmesh.GetTopLeftHalfNodes();
					SquareTilePathSubmesh.InterlinkNodeArraysUpperLower(myBottomRightNodes, rightTileTopLeftNodes);
					if (j > 0) {																					//	  [  ]
						var lowerLeftTile = matrix[i+1, j-1];														//  [  ]
						var leftTileTopRightNodes = lowerLeftTile.tilePathSubmesh.GetTopRightHalfNodes();
						SquareTilePathSubmesh.InterlinkNodeArraysUpperLower(myBottomLeftNodes, leftTileTopRightNodes);
					}
				}
			}
		}



		
	}

	public void Print() {
		for (int i = 0; i < nRows; i++) {
			string thisLine = "";
			for (int j = 0; j < nRows; j++) {
				thisLine += matrix[i, j].ToString() + " ";
			}
			Console.WriteLine(thisLine);
		}
	}
	public void PrintIndices() {
		for (int i = 0; i < nRows; i++) {
			string thisLine = "";
			for (int j = 0; j < nRows; j++) {
				thisLine += matrix[i, j].ToStringIndices() + " ";
			}
			Console.WriteLine(thisLine);
		}
	}

	public void PrintPathNodes() {
		for (int row = 0; row < nRows; row++) {
			string[] lines = new string[SquareTilePathSubmesh.N_SUBMESHES_PER_TILE];
			var isRowOffsetToRight = row % 2 == 0;
			for (int col = 0; col < nRows; col++) {
				var tile = matrix[row, col];
				for (int i = 0; i < SquareTilePathSubmesh.N_SUBMESHES_PER_TILE; i++) {
					for (int j = 0; j < SquareTilePathSubmesh.N_SUBMESHES_PER_TILE; j++) {
						var node = tile.tilePathSubmesh.nodes[i, j];
						lines[i] += node.ToString() + " ";
					}
				}
			}

			foreach (var line in lines) {
				Console.WriteLine((isRowOffsetToRight ? "                " : "") + line);
			}
		}
	}

}
