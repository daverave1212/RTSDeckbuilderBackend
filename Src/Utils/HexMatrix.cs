using System;
using System.Linq;

public class HPoint {
	public float X;
	public float Y;

	public HPoint(float x, float y) {
		X = x;
		Y = y;
	}

	public static bool IsPointInTriangle(HPoint p, HPoint p0, HPoint p1, HPoint p2) {
		var s = (p0.X - p2.X) * (p.Y - p2.Y) - (p0.Y - p2.Y) * (p.X - p2.X);
		var t = (p1.X - p0.X) * (p.Y - p0.Y) - (p1.Y - p0.Y) * (p.X - p0.X);

		if ((s < 0) != (t < 0) && s != 0 && t != 0)
			return false;

		var d = (p2.X - p1.X) * (p.Y - p1.Y) - (p2.Y - p1.Y) * (p.X - p1.X);
		return d == 0 || (d < 0) == (s + t <= 0);
	}
}


public class HexDirections {
	public const int Left = 0;
	public const int TopLeft = 1;
	public const int TopRight = 2;
	public const int Right = 3;
	public const int BottomRight = 4;
	public const int BottomLeft = 5;

	public static int GetOpposite(int dir) {
		switch (dir) {
			case Left: return Right;
			case TopLeft: return BottomRight;
			case TopRight: return BottomLeft;
			case Right: return Left;
			case BottomRight: return TopLeft;
			case BottomLeft: return TopRight;
		}
		throw new Exception($"Unknown direction for GetOpposite {dir}");
	}

}

public class HexNode<T> {

	public HexMatrix<T> matrix;
	public int i;
	public int j;

	public T value;
	public HexNode<T>[] neighbors = new HexNode<T>[6];

	public HexNode(HexMatrix<T> m, int _i, int _j) {
		matrix = m;
		i = _i;
		j = _j;
	}

	public void DoubleLinkWith(int direction, int _i, int _j) {
		var nextNode = matrix.Get(_i, _j);
		neighbors[direction] = nextNode;
		nextNode.neighbors[HexDirections.GetOpposite(direction)] = this;
	}
	public void TryDoubleLinkWith(int direction, int _i, int _j) {
		var isInBounds = matrix.IsInBounds(_i, _j);
		if (!isInBounds) {
			return;
		}
		DoubleLinkWith(direction, _i, _j);
	}

	public (float, float) GetPosition() {
		float x = matrix.x + j * matrix.distanceBetweenNodes;
		if (i % 2 == 0) {
			x -= matrix.distanceBetweenNodes / 2;
		}
		float y = matrix.y - i * matrix.GetHeightBetweenNodes();
		return (x, y);
	}

	public string ToString() {
		var (x, y) = GetPosition();
		return $"({i}, {j}; x:{x} y:{y})";
	}

}


public class HexMatrix<T> {

	public HexNode<T>[,] matrix;
	public int nRows;
	public int nCols;
	public float distanceBetweenNodes = 1;
	public float x = 0;
	public float y = 0;

	public bool IsInBounds(int i, int j) {
		return (i >= 0 && i < nRows && j >= 0 && j < nCols);
	}

	public HexMatrix(int r, int c, float radius = 10, float x = 0, float y = 0) {
		nRows = r;
		nCols = c;
		this.x = x;
		this.y = y;
		distanceBetweenNodes = radius;
		matrix = new HexNode<T>[nRows, nCols];
		for (int i = 0; i < nRows; i++) {
			for (int j = 0; j < nCols; j++) {
				matrix[i, j] = new HexNode<T>(this, i, j);
			}
		}

		linkNodes();
	}

	public HexNode<T> Get(int i, int j) {
		return matrix[i, j];
	}

	void linkNodes() {
		// Link horizontally	[  ][  ][  ][  ]
		for (int i = 0; i < nRows; i++) {
			for (int j = 0; j < nCols - 1; j++) {
				var node = matrix[i, j];
				var nextNode = matrix[i, j+1];
				node.neighbors[HexDirections.Right] = nextNode;
				nextNode.neighbors[HexDirections.Left] = node;
			}
		}

		// Link vertically
		for (int i = 1; i < nRows - 1; i++) {
			for (int j = 0; j < nCols; j++) {
				var node = matrix[i, j];
				if (i % 2 == 0) {
					node.TryDoubleLinkWith(HexDirections.TopLeft, i - 1, j);
					node.TryDoubleLinkWith(HexDirections.TopRight, i - 1, j + 1);
					node.TryDoubleLinkWith(HexDirections.BottomLeft, i + 1, j);
					node.TryDoubleLinkWith(HexDirections.BottomRight, i + 1, j + 1);
				}
			}
		}
	}

	public float GetHexPerpendicularRadius() {
		return distanceBetweenNodes / 2f;
	}
	public float GetHexRadius() {
		var perpendicularRadius = GetHexPerpendicularRadius();
		var thirtyDegrees = (float) Math.PI * 30f / 180.0f;
		var radius = perpendicularRadius / Math.Cos((double) thirtyDegrees);
		return (float) radius;
	}
	public float GetHeightBetweenNodes() {
		return (float) GetHexRadius() * 1.5f;
	}

	public (int, int) GetNodeIJByPosition(float mouseX, float mouseY) {
		var radius = GetHexRadius();
		var startY = y + radius;
		var yOffset = -(mouseY - startY);   // Offset (increasing positively) from top

		var totalHeight = radius + (nRows - 1) * GetHeightBetweenNodes() + radius;

		if (yOffset < 0 || y > totalHeight) {
			return (-1, -1);
		}

		// Try to find the i (only valid if j is later also found)
		int i = -1;

		float hexUpperSectionHeight = radius + radius / 2; // From top-most corner to intersection with hex below
		int iSection = (int) Math.Floor(yOffset / hexUpperSectionHeight);
		float yOffsetInSection = yOffset % hexUpperSectionHeight;

		bool isIAmbiguous = false;
		if (yOffsetInSection > radius / 2) {
			i = iSection;   // Unambiguous middle section
		} else {
			isIAmbiguous = true;
		}

		// Try to find j
		int j = -1;
		var perpendicularRadius = GetHexPerpendicularRadius();
		if (isIAmbiguous == false) {
			var clearXOffset = (iSection % 2 == 0) ?
				(mouseX - x - perpendicularRadius) :
				(mouseX - x - perpendicularRadius - perpendicularRadius);
			if (clearXOffset < 0 || clearXOffset > distanceBetweenNodes * nCols) {
				return (i, -1);
			}
			j = (int) Math.Floor(clearXOffset / (perpendicularRadius * 2));

			return (i, j);
		}

		// Find i and j if i is ambiguous
		var xOffset = mouseX - x - perpendicularRadius - perpendicularRadius;
		float rect2TrianglesHeight = radius / 2;            // Height of the strip
		float rect2TrianglesWidth = perpendicularRadius;    // Width of a rect |/|\|/|\...
		int nRects = 2 * nCols + 1;

		if (xOffset < 0 || xOffset > nRects * rect2TrianglesWidth) {
			return (-1, -1);
		}

		int whichRect = (int) Math.Floor(xOffset / rect2TrianglesWidth);
		float yOffsetInRect = yOffsetInSection;
		float xOffsetInRect = xOffset % rect2TrianglesWidth;

		// Points for the square (offsets only)
		var pMouse = new HPoint(xOffsetInRect, yOffsetInRect);
		var pTopLeft = new HPoint(0f, 0f);
		var pTopRight = new HPoint(rect2TrianglesWidth, 0f);
		var pBottomRight = new HPoint(rect2TrianglesWidth, rect2TrianglesHeight);
		var pBottomLeft = new HPoint(0f, rect2TrianglesHeight);
		if (iSection % 2 == 1) {    // ;/|\|/|\|/;
			if (whichRect % 2 == 0) {
				if (HPoint.IsPointInTriangle(pMouse, pTopLeft, pTopRight, pBottomLeft)) {
					j = whichRect / 2 - 1;
					i = iSection - 1;
				} else {
					j = whichRect / 2;
					i = iSection;
				}
			} else {
				if (HPoint.IsPointInTriangle(pMouse, pTopLeft, pBottomLeft, pBottomRight)) {
					j = whichRect / 2;
					i = iSection;
				} else {
					j = whichRect / 2;
					i = iSection - 1;
				}
			}
		} else {                    // ;\|/|\|/;
			if (whichRect % 2 == 0) {
				if (HPoint.IsPointInTriangle(pMouse, pTopLeft, pBottomLeft, pBottomRight)) {
					j = whichRect / 2 - 1;
					i = iSection;
				} else {
					j = whichRect / 2;
					i = iSection - 1;
				}
			} else {
				if (HPoint.IsPointInTriangle(pMouse, pTopLeft, pTopRight, pBottomLeft)) {
					j = whichRect / 2;
					i = iSection - 1;
				} else {
					j = whichRect / 2;
					i = iSection;
				}
			}
		}

		return (i, j);
	}
}

