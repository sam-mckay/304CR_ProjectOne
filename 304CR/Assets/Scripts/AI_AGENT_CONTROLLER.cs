using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AI_AGENT_CONTROLLER : MonoBehaviour
{
    //public vars
    public Vector2 startVec = new Vector2(0,0);
    public Vector2 destinationVec = new Vector2(0,0);
    public Transform pathNode;
    public Transform wallNode;
    public Transform forestNode;
    public Transform visitedNode;
    public Transform controller;
    public float speed;
    public bool randomGen;
    public bool isLoop;
    public bool isSeamless;
    public int width = 0;
    public int height = 0;
    public List<Vector2> walls;
    public List<Vector2> forests;
    public List<Vector2> roads;
    public bool isDone = true;
    //private vars
    LinkedList<Location> route;
    LinkedListNode<Location> routePos;
    Camera fpCamera;
    float distance = 0;
    Vector3 previousPos;
    Slider movementSlider;
    // Use this for initialization
    void Start ()
    {
        var grid = new SqaureGrid(width, height);
        assembleWalls(grid);
        assembleForests(grid);
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
        transform.position = new Vector3(startVec.x, transform.position.y, startVec.y);
       

        var astar = new AStar(grid, start, destination);
        route = generatePath(grid, astar, destination, start);
        drawBorder();
        drawGrid(grid, astar, route);

        routePos = route.First;
        distance = 1.2f;

        //set first person camera in active
        fpCamera = this.transform.FindChild("Camera").GetComponent<Camera>();
        fpCamera.gameObject.SetActive(false);
        //set movement slider
        movementSlider = GameObject.FindGameObjectWithTag("movementSlider").GetComponent<Slider>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        speed = movementSlider.value;
	}
    
    void FixedUpdate()
    {
        Camera firstPersonCamera = this.transform.FindChild("Camera").GetComponent<Camera>();
        if (firstPersonCamera != null)
        {
            Vector3 lookAtPos = transform.position + transform.forward;
            firstPersonCamera.transform.LookAt(lookAtPos);
        }
        if (!isDone)
        {
            
            distance += speed * Time.deltaTime;
            Move();
        }
        
    }
    
    void Move()
    {
        Vector3 targetPos = new Vector3(routePos.Value.x, transform.position.y, routePos.Value.y);
        Vector3 velocity = Vector3.zero;
        
        
        transform.position = Vector3.Lerp(previousPos, targetPos, distance);
        transform.LookAt(targetPos);
        if (distance>=1)
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
            distance = 0;
            previousPos = targetPos;
        }
    }

    public void restart()
    {
        Transform newController = (Transform)Instantiate(controller, this.transform.parent.position, Quaternion.identity);
        Transform newRoute = newController.FindChild("Ball");
        Camera newCamera = newRoute.FindChild("Camera").GetComponent<Camera>();
        AI_AGENT_CONTROLLER newAgent = newRoute.GetComponent<AI_AGENT_CONTROLLER>();
        newCamera.transform.localPosition = this.transform.FindChild("Camera").localPosition;
        newCamera.gameObject.tag = "fpCamera";
        newRoute.gameObject.tag = "aiAgent";
        newAgent.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        newAgent.isLoop = true;
        newAgent.randomGen = true;
        newAgent.isSeamless = isSeamless;
        newAgent.pathNode = pathNode;
        newAgent.wallNode = wallNode;
        newAgent.forestNode = forestNode;
        newAgent.visitedNode = visitedNode;
        newAgent.speed = speed;
        newAgent.controller = controller;
        newAgent.walls = walls;
        newAgent.forests = forests;
        if (isSeamless)
        {
            newAgent.startVec = new Vector2(destinationVec.x, destinationVec.y);
        }
        newAgent.isDone = false;
        Destroy(this.transform.parent.gameObject);
    }

    public Camera getCamera()
    {
        return fpCamera;
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
            //bottom
            wallPart = (Transform) Instantiate(wallNode, new Vector3(-1 + i, 0, -1), Quaternion.identity);
            wallPart.transform.parent = this.transform.parent;

            wallPart = (Transform)Instantiate(wallNode, new Vector3(-1 + i, 0, width), Quaternion.identity);
            wallPart.transform.parent = this.transform.parent;

            wallPart = (Transform)Instantiate(wallNode, new Vector3(-1, 0, -1 + i), Quaternion.identity);
            wallPart.transform.parent = this.transform.parent;

            wallPart = (Transform)Instantiate(wallNode, new Vector3(height+1, 0, -1 + i), Quaternion.identity);
            wallPart.transform.parent = this.transform.parent;

        }
    }
        
    void drawGrid(SqaureGrid grid, AStar astar, LinkedList<Location> route)
    {
        string gridString = "";
        for (var y = 0; y <= 10; y++)
        {
            for (var x = 0; x <= 10; x++)
            {
                Location currentLocation = new Location(x, y);
                Location locationPtr = currentLocation;
                if (!astar.cameFrom.TryGetValue(currentLocation, out locationPtr))
                {
                    locationPtr = currentLocation;
                }
                if (grid.forests.Contains(currentLocation))
                {
                    //show forest at current pos
                    Transform wallPart = (Transform)Instantiate(forestNode, new Vector3(x, 0, y), Quaternion.identity);
                    wallPart.parent = this.transform.parent;
                    gridString += "FF";
                }
                //show coloured blocks
                if (route.Contains(currentLocation))
                {
                    Transform wallPart = (Transform)Instantiate(pathNode, new Vector3(x, 0, y), Quaternion.identity);
                    wallPart.parent = this.transform.parent;
                    gridString += "x ";
                }
                else if (grid.walls.Contains(currentLocation))
                {
                    //show wall at current pos
                    Transform wallPart = (Transform) Instantiate(wallNode, new Vector3(x, 0, y), Quaternion.identity);
                    wallPart.parent = this.transform.parent; 
                    gridString += "# ";
                }
                else if(astar.cameFrom.ContainsValue(currentLocation))
                {
                    Transform wallPart = (Transform)Instantiate(visitedNode, new Vector3(x, 0, y), Quaternion.identity);
                    wallPart.parent = this.transform.parent;
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

    void assembleForests(SqaureGrid grid)
    {
        foreach (Vector2 forest in forests)
        {
            Location currentLocation = new Location((int)forest.x, (int)forest.y);
            if (!grid.walls.Contains(currentLocation))
            {
                grid.forests.Add(new Location((int)forest.x, (int)forest.y));
            }            
        }
    }

    void assembleRoads(SqaureGrid grid)
    {
        foreach (Vector2 road in forests)
        {
            Location currentLocation = new Location((int)road.x, (int)road.y);
            if (!grid.walls.Contains(currentLocation))
            {
                grid.roads.Add(new Location((int)road.x, (int)road.y));
            }
        }
    }
}
