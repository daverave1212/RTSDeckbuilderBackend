using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

public class NodeData {

	public GridNode<NodeData> gridNode;
	public bool isBlocked = false;

	public NodeData(GridNode<NodeData> gridNode) {
		this.gridNode = gridNode;
	}

}
