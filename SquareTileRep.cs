using System;
using System.Numerics;

public partial class SquareTileRep {

	public static float HEIGHT = 100;
	public static float WIDTH = 100;

	GameMapRep gameMapRep;

	public Point2D position;
	public int i;
	public int j;
	public SquareTilePathSubmesh tilePathSubmesh;

	public SquareTileRep(GameMapRep gameMapRep, int i, int j) {
		this.gameMapRep = gameMapRep;
		this.i = i;
		this.j = j;
		position = CalculateCenterXYFromIJ(gameMapRep, i, j);
		tilePathSubmesh = new SquareTilePathSubmesh(this);
	}

	public Point2D GetTopLeftPosition() {
		return new Point2D(position.x - WIDTH / 2, position.y - HEIGHT / 2);
	}

	public Point2D CalculateCenterXYFromIJ(GameMapRep gameMapRep, int i, int j) {
		float iOffsetCoef;
		float jOffsetCoef;
		if (gameMapRep.nCols % 2 == 0) {
			jOffsetCoef = j - (float) gameMapRep.nCols / 2 + 0.5f;
		} else {
			jOffsetCoef = j - (gameMapRep.nCols / 2);
		}
		if (gameMapRep.nRows % 2 == 0) {
			iOffsetCoef = i - (float) gameMapRep.nRows / 2 + 0.5f;
		} else {
			iOffsetCoef = i - (gameMapRep.nRows / 2);
		}
		float x = jOffsetCoef * SquareTileRep.WIDTH;
		var isRowOffsetToRight = i % 2 == 0;
		if (isRowOffsetToRight) {
			x += SquareTileRep.WIDTH / 2;
		}
		float y = iOffsetCoef * SquareTileRep.HEIGHT;
		return new Point2D(x, y);
	}

	public PathNode GetNodeAt(int i, int j) {
		return tilePathSubmesh.nodes[i, j];
	}

	
}




public partial class SquareTileRep {

	public void PrintNodes() {
		for (int i = 0; i < SquareTilePathSubmesh.N_SUBMESHES_PER_TILE; i++) {
			string line = "";
			for (int j = 0; j < SquareTilePathSubmesh.N_SUBMESHES_PER_TILE; j++) {
				line += tilePathSubmesh.nodes[i, j].ToString() + " ";
			}
			Console.WriteLine(line);
		}
	}
	public override string ToString() {
		return position.ToString();
	}
	public string ToStringIndices() {
		return $"({i},{j})";
	}


}
