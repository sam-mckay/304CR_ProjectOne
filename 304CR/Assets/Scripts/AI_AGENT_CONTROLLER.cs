using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_AGENT_CONTROLLER : MonoBehaviour
{
    //public vars
    public Vector2 startVec = new Vector2(0,0);
    public Vector2 destinationVec = new Vector2(0,0);
    public Transform pathNode;
    public Transform wallNode;
    public Transform controller;
    public bool randomGen;
    public bool isLoop;
    public bool isSeamless;
    public int width = 0;
    public int height = 0;
    public List<Vector2> walls;
    
    //private vars
    LinkedList<Location> route;
    LinkedListNode<Location> routePos;
    bool isDone = false;
    
    // Use this for initialization
    void Start ()
    {
        var grid = new SqaureGrid(width, height);
        assembleWalls(grid);
        //randomise start and destination
        if (randomGen)
        {
            bool isGenerated = false;
            int randStartX = (int)startVec.x;
            int randStartY = (int)startVec.y;
            while (!isGenerated)
            {
                if (!isSeamless)
                {
                    randStartX = Random.Range(0, width);
                    randStartY = Random.Range(0, height);
                }
                int randDestX = Random.Range(1, width);
                int randDestY = Random.Range(1, height);
                
                if(!grid.walls.Contains(new Location(randStartX, randStartY)) && !grid.walls.Contains(new Location(randDestX, randDestY)))
                {
                    
                    startVec = new Vector2(randStartX, randStartY);
                    destinationVec = new Vector2(randDestX, randDestY);
                    isGenerated = true;
                }
            }
        }
        //set locations
        Location start = new Location((int)startVec.x, (int)startVec.y);
        Location destination = new Location((int)destinationVec.x, (int)destinationVec.y);
        transform.position = new Vector3(startVec.x, startVec.y,transform.position.z);
       

        var astar = new AStar(grid, start, destination);
        route = generatePath(grid, astar, destination, start);
        drawBorder();
        drawGrid(grid, astar, route);

        routePos = route.First;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
    
    void FixedUpdate()
    {
        if (!isDone)
        {
            Move();
        }
    }
    
    void Move()
    {
        Vector3 targetPos = new Vector3(routePos.Value.x, routePos.Value.y, transform.position.z);
        Vector3 velocity = Vector3.zero;
        //transform.position = Vector3.SmoothDamp(transform.position, targetPos, 0.3f);
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.3f);
        if(Mathf.Approximately(transform.position.x,targetPos.x) && Mathf.Approximately(transform.position.y, targetPos.y))
        {
            if (routePos == route.Last && !isLoop)
            {
                isDone = true;
            }
            else if (routePos == route.Last && isLoop)
            {
                restart();
                return;
            }
            routePos = routePos.Next;
            
        }
        //this.transform.RotateAround(new Vector3(5.72f, 5.24f, 13.9f), Vector3.left, 90);
    }

    void restart()
    {
        Transform newController = (Transform)Instantiate(controller, this.transform.parent.position, Quaternion.identity);
        Transform newRoute = newController.FindChild("Ball");
        AI_AGENT_CONTROLLER newAgent = newRoute.GetComponent<AI_AGENT_CONTROLLER>();
        newAgent.isLoop = true;
        newAgent.randomGen = true;
        newAgent.isSeamless = isSeamless;
        newAgent.pathNode = pathNode;
        newAgent.wallNode = wallNode;
        newAgent.controller = controller;
        newAgent.walls = walls;
        if(isSeamless)
        {
            newAgent.startVec = new Vector2(destinationVec.x, destinationVec.y);
        }
        Destroy(this.transform.parent.gameObject);
    }

    /// <summary>
    /// Draws the map border
    /// currently hard coded - needs updating to take into account true size
    /// </summary>
    void drawBorder()
    {
        Transform wallPart;
        for(int i = 0; i < 12; i++)
        {
            wallPart = (Transform) Instantiate(wallNode, new Vector3(-1 + i, -1, 0), Quaternion.identity);
            wallPart.transform.parent = this.transform.parent;

            wallPart = (Transform)Instantiate(wallNode, new Vector3(-1 + i, width+1, 0), Quaternion.identity);
            wallPart.transform.parent = this.transform.parent;

            wallPart = (Transform)Instantiate(wallNode, new Vector3(-1, -1+i, 0), Quaternion.identity);
            wallPart.transform.parent = this.transform.parent;

            wallPart = (Transform)Instantiate(wallNode, new Vector3(height+1, -1+i, 0), Quaternion.identity);
            wallPart.transform.parent = this.transform.parent;

        }
        wallPart = (Transform) Instantiate(wallNode, new Vector3(width+1, height+1, 0), Quaternion.identity);
        wallPart.transform.parent = this.transform.parent;
    }
        
    void drawGrid(SqaureGrid grid, AStar astar, LinkedList<Location> route)
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
                    Transform wallPart = (Transform) Instantiate(wallNode, new Vector3(x, y, 0), Quaternion.identity);
                    wallPart.parent = this.transform.parent; 
                   // wallPart.RotateAround(new Vector3(5.72f, 5.24f, 13.9f), Vector3.left, 90);
                    gridString += "#|";
                }
                else if(route.Contains(currentLocation))
                {
                    Transform wallPart = (Transform)Instantiate(pathNode, new Vector3(x, y, 0), Quaternion.identity);
                    wallPart.parent = this.transform.parent;
                    // wallPart.RotateAround(new Vector3(5.72f, 5.24f, 13.9f), Vector3.left, 90);
                    gridString += "x ";
                }
                //show if part of rout
                else
                {   
                    gridString += "* ";
                }
            }
            gridString += "\n";
        }
        Debug.Log(gridString);
        
    }
    
    LinkedList<Location> generatePath(SqaureGrid grid, AStar astar, Location destination, Location start)
    {
        LinkedList<Location> newRoute = new LinkedList<Location>();
        //Positions
        Location current = destination;
        newRoute.AddFirst(current);
        while(!current.Equals(start))
        {
            current = astar.cameFrom[current];
            newRoute.AddFirst(current);
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
        return newRoute;
    }

    void assembleWalls(SqaureGrid grid)
    {
        foreach(Vector2 wall in walls)
        {
            grid.walls.Add(new Location((int)wall.x, (int)wall.y));
        }
    }
}
