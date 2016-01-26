using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Node : MonoBehaviour
{
    public Vector3 NodePosition;
    public bool walkable;
    public LinkedList<Node> neighbours;
	// Use this for initialization
	void Start ()
    {
        //transform.position = NodePosition;
        //transform.position.Set(NodePosition.x, NodePosition.y, NodePosition.z);
        Debug.Log("Position: " + NodePosition.x);
        Debug.Log("Transform Position: " + transform.position.x);
    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    //sets the position of the node
    public void setPos(int x, int y)
    {
        NodePosition.x = x;
        NodePosition.y = y;
    }

    //sets the neighbours of this Node
    public void setNeighbours(LinkedList<Node> nodes)
    {
        if(NodePosition.x > 0 && NodePosition.y > 0)
        {
            
        }

        foreach (Node possibleNeighbour in nodes)
        {
            //if possible neighbour directly above or below this node
            if(possibleNeighbour.NodePosition.x == NodePosition.x && (possibleNeighbour.NodePosition.y == NodePosition.y +1 
                || possibleNeighbour.NodePosition.y == NodePosition.y - 1))
            {
                neighbours.AddLast(possibleNeighbour);
            }
            //if possible neighbour directly left or right of this node
            if (possibleNeighbour.NodePosition.y == NodePosition.y && (possibleNeighbour.NodePosition.x == NodePosition.x + 1
                || possibleNeighbour.NodePosition.x == NodePosition.x - 1))
            {
                neighbours.AddLast(possibleNeighbour);
            }
        }
    }
}
