using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_AGENT_CONTROLLER : MonoBehaviour
{
    public Vector2 startVec = new Vector2(0,0);
    public Vector2 destinationVec = new Vector2(0,0);
    public GameObject pathNode;
    public GameObject wallNode;

    public List<Vector2> walls;
	// Use this for initialization
	void Start ()
    {
        var grid = new SqaureGrid(10, 10);
        Location start = new Location((int)startVec.x, (int)startVec.y);
        Location destination = new Location((int)destinationVec.x, (int)destinationVec.y);
        assembleWalls(grid);
        var astar = new AStar(grid, start, destination);
        LinkedList<Location> route = generatePath(grid, astar, destination, start);
        drawGrid(grid, astar, route);
        
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    static void drawGrid(SqaureGrid grid, AStar astar, LinkedList<Location> route)
    {
        string gridString = "";
        for (var y = 0; y < 10; y++)
        {
            for (var x = 0; x < 10; x++)
            {
                Location currentLocation = new Location(x, y);
                Location locationPtr = currentLocation;
                if (!astar.cameFrom.TryGetValue(currentLocation, out locationPtr))
                {
                    locationPtr = currentLocation;
                }
                //show coloured blocks
                if (grid.walls.Contains(currentLocation))
                {
                    //show wall at current pos
                    gridString += "#|";
                }
                else if(route.Contains(currentLocation))
                {
                    gridString += "x ";
                }
                /*
                else if (locationPtr.x == x + 1)
                {
                    //
                    gridString += "->";
                }
                else if (locationPtr.x == x - 1)
                {
                    //
                    gridString += "<-";
                }
                else if (locationPtr.y == y + 1)
                {
                    //
                    gridString += "^ ";
                }
                else if (locationPtr.y == y - 1)
                {
                    //
                    gridString += "v ";
                }
                */
                //show if part of rout
                else
                {
                    //Instantiate(pathNode, new Vector3(x, y, 0), Quaternion.identity);
                    //Debug.Log("X: " + x + " Y: " + y + "*");
                    gridString += "* ";
                }
            }
            gridString += "\n";
        }
        Debug.Log(gridString);
        
    }
    
    LinkedList<Location> generatePath(SqaureGrid grid, AStar astar, Location destination, Location start)
    {
        LinkedList<Location> route = new LinkedList<Location>();
        //Positions
        Location current = destination;
        route.AddFirst(current);
        while(!current.Equals(start))
        {
            current = astar.cameFrom[current];
            route.AddFirst(current);
        }

        //DEBUG
        //Print Route
        /*
        Debug.Log("ROUTE: COUNT: "+route.Count);
        LinkedListNode<Location> routeNode = route.First;
        for (int i = 0; i < route.Count; i++)
        {
            Debug.Log("X: " + routeNode.Value.x + " Y: " + routeNode.Value.y);
            routeNode = routeNode.Next;
        }
        */
        return route;
    }

    void assembleWalls(SqaureGrid grid)
    {
        foreach(Vector2 wall in walls)
        {
            grid.walls.Add(new Location((int)wall.x, (int)wall.y));
        }
    }
}
