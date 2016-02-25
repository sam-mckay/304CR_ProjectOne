using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface WeightedGraph<Location>
{
    int cost(Location A, Location B);
    IEnumerable<Location> Neighbours(Location currentLocation);
}

public struct Location
{
    public readonly int x;
    public readonly int y;
    public Location(int w, int h)
    {
        this.x = w;
        this.y = h;
    }
}

//To Represent the map
public class SqaureGrid : WeightedGraph<Location>
{
    //used to locate neighbours
    public static readonly Location[] DIRS = new[]
    {
        new Location(1,0),
        new Location(0,-1),
        new Location(-1,0),
        new Location(0,1)
    };

    int width;
    int height;
    int forestCost;
    int roadCost; 
    //for weighting nodes
    public HashSet<Location> walls = new HashSet<Location>();
    public HashSet<Location> forests = new HashSet<Location>();
    public HashSet<Location> roads = new HashSet<Location>();

    //construtor for map grid
    public SqaureGrid(int w, int h)
    {
        //init vars
        width = w;
        height = h;
        forestCost = PlayerPrefs.GetInt(SaveManager.forestCost);
        roadCost = PlayerPrefs.GetInt(SaveManager.roadCost);
    }

    //check whether the Location is on the grid
    // useful for checkin edge cases as map does not support looping
    bool inBounds(Location currentLocation)
    {
        if(0 <= currentLocation.x && currentLocation.y < width &&
            0 <= currentLocation.y && currentLocation.y < height)
        {
            return true;
        }
        return false;
    }

    //check if terrain can be travelled over 
    //could be expanded to support more than just walls
    //i.e. lava, holes 
    bool passable(Location currentLocation)
    {
        if(walls.Contains(currentLocation))
        {
            return false;
        }
        return true;
    }

    //returns the cost of a node dependent on its weight
    public int cost(Location A, Location B)
    {
        if(forests.Contains(B) || forests.Contains(A))
        {
            Debug.Log("RETURNING FOREST:"+ PlayerPrefs.GetInt(SaveManager.forestCost));
            return forestCost;
        }
        if (roads.Contains(B) || roads.Contains(A))
        {
            Debug.Log("RETURNING ROAD:" + PlayerPrefs.GetInt(SaveManager.roadCost));
            return roadCost;
        }
        //standard cost
        return 5;
    }

    //returns each valid neighbour
    public IEnumerable<Location> Neighbours(Location currentLocation)
    {
        foreach (var dir in DIRS)
        {
            Location next = new Location(currentLocation.x + dir.x, currentLocation.y + dir.y);
            //if next is on the map and is a walkable i.e. not a wall
            if (inBounds(next) && passable(next))
            {
                yield return next;
            }
        }
    }
}

public class AStar : MonoBehaviour
{
    public Dictionary<Location, Location> cameFrom = new Dictionary<Location, Location>();
    public Dictionary<Location, int> costSoFar = new Dictionary<Location, int>();

    static public int distCalc(Location A, Location B)
    {
        return Math.Abs(A.x - B.x) + Math.Abs(A.y - B.y);
    }

       
    public AStar(SqaureGrid grid, Location startPos, Location destination)
    {
        //Debug.Log("START A STAR");
        //setup variables
        PriorityQueue<int, Location> frontier = new PriorityQueue<int, Location>();
        frontier.Enqueue(startPos, 0);
        cameFrom[startPos] = startPos;
        costSoFar[startPos] = 0;

        //main loop
        //while frontier is not empty
        //Debug.Log("START A STAR LOOP");
        while (frontier.count > 0)
        {
            //get first Node in queue
            Location current = frontier.Dequeue();
            //check if current Node is the Destination Node
            if (current.Equals(destination))
            {
                break;//Destination reached stop searching
            }

            foreach (var next in grid.Neighbours(current))
            {
                // calculate cost to neighbour
                int newCost = costSoFar[current] + grid.cost(current, next);
                // if its cheaper to get here from this node than previous routes or 
                //  we haven't checked this neighbour
                if (newCost < costSoFar[next] || !costSoFar.ContainsKey(next))
                {
                    //update costSoFar
                    costSoFar[next] = newCost;
                    //calculate priority
                    int priority = distCalc(next, destination) + newCost;
                    //add to frontier
                    frontier.Enqueue(next, priority);
                    //sent current node as the previous node
                    //allows us to work our way back through cameFrom to find the route 
                    cameFrom[next] = current;
                }
            }
        }
    }

    
}
