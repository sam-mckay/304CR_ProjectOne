using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface WeightedGraph<L>
{
    int Cost(Node A, Node B);

}

public class AStar : MonoBehaviour
{
    public Node[] walls;
    public Vector2 mapSize;
    public LinkedList<Node> nodes;

	// Use this for initialization
	void Start ()
    {
	    for(int x = 0; x < mapSize.x; x++)
        {
            for(int y = 0; y < mapSize.y; y++)
            {
                Node newNode = new Node();
                newNode.setPos(x, y);
                nodes.AddLast(newNode);
            }
        }
        LinkedListNode<Node> currentNode = nodes.First;
        
        for (int i = 0; i < nodes.Count; i++)
        {
            currentNode.Value.setNeighbours(nodes);
            currentNode = currentNode.Next;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void findPath(Node startPos, Node destination)
    {
        //setup variables
        PriorityQueue<int, Node> frontier = new PriorityQueue<int, Node>();
        frontier.Enqueue(startPos,0);
        Node[] cameFrom = {};
        int[] costSoFar = {};
        cameFrom[0] = null;
        costSoFar[0] = 0;

        //main loop
        //while frontier is not empty
        while(!frontier.isEmpty())
        {
            //get first Node in queue
            Node current = frontier.Dequeue();
            //check if current Node is the Destination Node
            if(current == destination)
            {
                break;
            }

            for(LinkedListNode<Node> currentNode = current.neighbours.First; currentNode != current.neighbours.Last; currentNode = currentNode.Next)
            {
                //int newCost = costSoFar[]
            }
        }
    }
}
