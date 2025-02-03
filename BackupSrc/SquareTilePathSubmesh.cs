using System;

// Creates a matrix of N_SUBMESHES_PER_TILE PathNode's connected to eachother
public class SquareTilePathSubmesh {

	public static int N_SUBMESHES_PER_TILE = 2; // Should be divisible by 2

	public SquareTileRep tile;
	public PathNode[,] nodes;

	public SquareTilePathSubmesh(SquareTileRep _tile) {
		tile = _tile;
		nodes = new PathNode[N_SUBMESHES_PER_TILE, N_SUBMESHES_PER_TILE];

		var topLeftStartPos = GetTopLeftNodeStartPosition();

		for (int i = 0; i < N_SUBMESHES_PER_TILE; i++) {
			for (int j = 0; j < N_SUBMESHES_PER_TILE; j++) {
				var nodePos = GetXYByIJFromNodeStartingPoint(topLeftStartPos, i, j);
				nodes[i, j] = new PathNode(this, nodePos.x, nodePos.y);
			}
		}

		innerAutoLinkNodes();
	}

	public void InterlinkWithSubmeshToMyRight(SquareTilePathSubmesh submesh) {
		for (int i = 0; i < N_SUBMESHES_PER_TILE; i++) {
			var myRightmostNode = nodes[i, N_SUBMESHES_PER_TILE - 1];
			var hisLeftmostNode = nodes[i, 0];
			myRightmostNode.rightNode = hisLeftmostNode;
			hisLeftmostNode.leftNode = myRightmostNode;
		}
	}

	public PathNode[] GetTopLeftHalfNodes() {
		var theNodes = new PathNode[N_SUBMESHES_PER_TILE/2];
		for (int j = 0; j < N_SUBMESHES_PER_TILE / 2; j++) {
			theNodes[j] = nodes[0, j];
		}
		return theNodes;
	}
	public PathNode[] GetTopRightHalfNodes() {
		var theNodes = new PathNode[N_SUBMESHES_PER_TILE/2];
		int index = 0;
		for (int j = N_SUBMESHES_PER_TILE / 2; j < N_SUBMESHES_PER_TILE; j++) {
			theNodes[index] = nodes[0, j];
			index++;
		}
		return theNodes;
	}
	public PathNode[] GetBottomLeftHalfNodes() {
		var theNodes = new PathNode[N_SUBMESHES_PER_TILE/2];
		for (int j = 0; j < N_SUBMESHES_PER_TILE / 2; j++) {
			theNodes[j] = nodes[N_SUBMESHES_PER_TILE - 1, j];
		}
		return theNodes;
	}
	public PathNode[] GetBottomRightHalfNodes() {
		var theNodes = new PathNode[N_SUBMESHES_PER_TILE/2];
		int index = 0;
		for (int j = N_SUBMESHES_PER_TILE / 2; j < N_SUBMESHES_PER_TILE; j++) {
			theNodes[index] = nodes[N_SUBMESHES_PER_TILE - 1, j];
			index++;
		}
		return theNodes;
	}





	void innerAutoLinkNodes() {
		for (int i = 0; i < N_SUBMESHES_PER_TILE; i++) {
			for (int j = 0; j < N_SUBMESHES_PER_TILE; j++) {
				var node = nodes[i, j];
				if (i < N_SUBMESHES_PER_TILE - 1) {
					var nodeDown = nodes[i+1, j];
					node.downNode = nodeDown;
					nodeDown.upNode = node;
				}
				if (j < N_SUBMESHES_PER_TILE - 1) {
					var nodeRight = nodes[i, j+1];
					node.rightNode = nodeRight;
					nodeRight.leftNode = node;
				}
			}
		}
	}

	public Point2D GetTopLeftNodeStartPosition() {
		var topLeftTilePos = tile.GetTopLeftPosition();
		float distanceBetweenNodes = GetDistanceBetween2NeighborNodes();
		return new Point2D(topLeftTilePos.x + distanceBetweenNodes / 2, topLeftTilePos.y + distanceBetweenNodes / 2);
	}

	public void Print() {
		for (int i = 0; i < N_SUBMESHES_PER_TILE; i++) {
			string thisLine = "";
			for (int j = 0; j < N_SUBMESHES_PER_TILE; j++) {
				thisLine += nodes[i, j].position.ToString() + " ";
			}
			Console.WriteLine(thisLine);
		}
	}




	public static float GetDistanceBetween2NeighborNodes() {
		return (float) SquareTileRep.WIDTH / (float) N_SUBMESHES_PER_TILE;
	}
	public static Point2D GetXYByIJFromNodeStartingPoint(Point2D startPoint, int i, int j) {
		float distanceBetweenNodes = GetDistanceBetween2NeighborNodes();
		float x = startPoint.x + distanceBetweenNodes * j;
		float y = startPoint.y + distanceBetweenNodes * i;
		return new Point2D(x, y);
	}
	public static void InterlinkNodeArraysUpperLower(PathNode[] nodesUpper, PathNode[] nodesLower) {
		for (int i = 0; i < nodesUpper.Length; i++) {
			nodesUpper[i].downNode = nodesLower[i];
			nodesLower[i].upNode = nodesUpper[i];
		}
	}



}
