using System;
using System.Collections;
using System.Collections.Generic;

public class TestGridMatrix {
	public static void Test(string[] args) {
		// float y = 0.35f;
		// float distanceBetweenNodes = 0.10f;
		// int nNodes = 4;

		// int i = nNodes - (int) (y / distanceBetweenNodes) - 1;
		// Console.WriteLine(i);


		var matrix = new GridMatrix<int>(4, 4, 0.10f, 0.1f, 0.1f);
		var x = 0.17f;
		var y = 0.051f;
		(var iy, var jx) = matrix.GetNodeIJByPosition((x, y));
		// Console.WriteLine((iy, jx));

		Console.WriteLine(matrix.Get(0, 2).DistanceTo(matrix.Get(1, 0)));
	}
}

public class GridMatrix<T> {
	public GridNode<T>[,] matrix;
	public int nRows = 0;
	public int nCols = 0;
	public float distanceBetweenNodes = 10f;
	public float distanceBetweenDiagonalNodes;
	public float x = 0f;
	public float y = 0f;

	public bool IsInBounds(int yIndex, int xIndex) {
		return (yIndex >= 0 && yIndex < nRows && xIndex >= 0 && xIndex < nCols);
	}

	public GridMatrix(int r, int c, float distBetweenNodes = 10, float x = 0, float y = 0) {
		nRows = r;
		nCols = c;
		this.x = x;
		this.y = y;
		distanceBetweenNodes = distBetweenNodes;
		distanceBetweenDiagonalNodes = (float) Math.Sqrt(2) * distanceBetweenNodes;
		matrix = new GridNode<T>[nRows, nCols];
		for (int iy = 0; iy < nRows; iy++) {    // Bottom -> Up
			for (int jx = 0; jx < nCols; jx++) {
				matrix[iy, jx] = new GridNode<T>(this, iy, jx);
			}
		}
	}

	public GridNode<T> Get(int iy, int jx) {
		return matrix[iy, jx];
	}

	public void ForEach(Action<int, int> forEachFunc) {
		for (int iy = 0; iy < nRows; iy++) {
			for (int jx = 0; jx < nCols; jx++) {
				forEachFunc(iy, jx);
			}
		}
	}
	public void ForEach(Action<GridNode<T>> forEachFunc) {
		for (int iy = 0; iy < nRows; iy++) {
			for (int jx = 0; jx < nCols; jx++) {
				var node = matrix[iy,jx];
				forEachFunc(node);
			}
		}
	}
	public void ForEach(Action<int, int, GridNode<T>> forEachFunc) {
		for (int iy = 0; iy < nRows; iy++) {
			for (int jx = 0; jx < nCols; jx++) {
				var node = matrix[iy,jx];
				forEachFunc(iy, jx, node);
			}
		}
	}

	public (int, int) GetNodeIJByPosition((float, float) position) {
		(float posX, float posY) = position;
		float xOffset = posX - x + distanceBetweenNodes / 2;
		float yOffset = posY - y + distanceBetweenNodes / 2;

		float nodeSize = distanceBetweenNodes;

		int jx = (int) Math.Floor(xOffset / nodeSize);
		int iy = (int) Math.Floor(yOffset / nodeSize);

		return (iy, jx);
	}

	public GridNode<T> GetNodeByPosition((float, float) position) {
		(int i, int j) = GetNodeIJByPosition(position);
		return matrix[i, j];
	}

}

public class GridNode<T> {

	public GridMatrix<T> matrix;
	public int i;
	public int j;
	public T data;

	public GridNode(GridMatrix<T> gridMatrix, int _i, int _j) {
		matrix = gridMatrix;
		i = _i;
		j = _j;
	}

	public (float, float) GetPosition() {
		var x = matrix.x + matrix.distanceBetweenNodes * j;
		var y = matrix.y + matrix.distanceBetweenNodes * i;
		return (x, y);
	}

	public string ToString() {
		var (x, y) = GetPosition();
		return $"({i}, {j}; x:{x} y:{y})";
	}

	public List<GridNode<T>> GetNeighbors() {
		var neighbors = new List<GridNode<T>>();
		if (i > 0) {
			neighbors.Add(matrix.Get(i - 1, j));
		}
		if (i < matrix.nRows - 1) {
			neighbors.Add(matrix.Get(i + 1, j));
		}
		if (j > 0) {
			neighbors.Add(matrix.Get(i, j - 1));
		}
		if (j < matrix.nCols - 1) {
			neighbors.Add(matrix.Get(i, j + 1));
		}
		return neighbors;
	}

	public List<GridNode<T>> GetExtendedNeighbors() {
		var neighbors = GetNeighbors();
		if (i > 0) {
			if (j > 0) {
				neighbors.Add(matrix.Get(i - 1, j - 1));
			}
			if (j < matrix.nCols - 1) {
				neighbors.Add(matrix.Get(i - 1, j + 1));
			}
		}
		if (i < matrix.nRows - 1) {
			if (j > 0) {
				neighbors.Add(matrix.Get(i + 1, j - 1));
			}
			if (j < matrix.nCols - 1) {
				neighbors.Add(matrix.Get(i + 1, j + 1));
			}
		}
		return neighbors;
	}

	public float DistanceTo(GridNode<T> node) {
		if (node.i == i && node.j == j) {
			return 0;
		}
		int iDist = Math.Abs(node.i - i);
		int jDist = Math.Abs(node.j - j);

		if (iDist == 0 && jDist == 1) {
			return matrix.distanceBetweenNodes;
		}
		if (iDist == 1 && jDist == 0) {
			return matrix.distanceBetweenNodes;
		}
		if (iDist == 1 && jDist == 1) {
			return matrix.distanceBetweenDiagonalNodes;
		}

		return _DistanceBetween(GetPosition(), node.GetPosition());
	}



	static float _DistanceBetween((float, float) pointA, (float, float) pointB) {
		var (ax, ay) = pointA;
		var (bx, by) = pointB;
		return (float) Math.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));
	}

}

