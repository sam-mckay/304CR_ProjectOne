using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface WeightedGraph<L>
{
    int cost(Location A, Location B);
    IEnumerable<Location> Neighbours(Location currentLocation);
}

public struct Location
{
    public readonly int x, y;
    public Location(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

//To Represent the map
public class SqaureGrid : WeightedGraph<Location>
{

    public static readonly Location[] DIRS = new[]
    {
        new Location(1,0),
        new Location(0,-1),
        new Location(-1,0),
        new Location(0,1)
    };

    int width, height;
    public HashSet<Location> walls = new HashSet<Location>();
    public HashSet<Location> forests = new HashSet<Location>();

    public SqaureGrid(int w, int h)
    {
        width = w;
        height = h;
    }

    bool inBounds(Location currentLocation)
    {
        if(0 <= currentLocation.x && currentLocation.y < width &&
            0 <= currentLocation.y && currentLocation.y < height)
        {
            return true;
        }
        return false;
    }

    bool passable(Location currentLocation)
    {
        if(walls.Contains(currentLocation))
        {
            return false;
        }
        return true;
    }

    public int cost(Location A, Location B)
    {
        //add weighting here
        //i.e. water, long grass, hills etc
        if(forests.Contains(B) || forests.Contains(A))
        {
            Debug.Log("RETURNING FOREST:"+ PlayerPrefs.GetInt(SaveManager.forestCost));
            return PlayerPrefs.GetInt(SaveManager.forestCost);
        }
        return 1;
    }

    public IEnumerable<Location> Neighbours(Location currentLocation)
    {
        foreach (var dir in DIRS)
        {
            Location next = new Location(currentLocation.x + dir.x, currentLocation.y + dir.y);
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
        var frontier = new PriorityQueue<int, Location>();
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
                break;
            }

            foreach (var next in grid.Neighbours(current))
            {
                //Debug.Log("A STAR FOREACH LOOP");
                int newCost = costSoFar[current] + grid.cost(current, next);
                //Debug.Log("CONTAINS KEY TEST");
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    //Debug.Log("CONTAINS KEY");
                    costSoFar[next] = newCost;
                    int priority = newCost + distCalc(next, destination);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                    //Debug.Log("END A STAR LOOP");
                }
            }
        }
    }

    
}
