using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingScript : MonoBehaviour {

    public List<Vector2> traversablePoints = new List<Vector2>();
    public GameObject grid;

    public Vector2 startPoint;
    public int startIndex;
    public Vector2 endPoint;
    public int endIndex;

    Vector2 tempStart;

    Dictionary<Vector2, Vector2> parents = new Dictionary<Vector2, Vector2>();
    Dictionary<Vector2, bool> visited = new Dictionary<Vector2, bool>();
    Dictionary<Vector2, float> g = new Dictionary<Vector2, float>();
    Dictionary<Vector2, float> h = new Dictionary<Vector2, float>();
    Dictionary<Vector2, float> f = new Dictionary<Vector2, float>();


    Dictionary<Vector2, Vector2> parentsDstar = new Dictionary<Vector2, Vector2>();
    Dictionary<Vector2, float> gDstar = new Dictionary<Vector2, float>();
    Dictionary<Vector2, float> hDstar = new Dictionary<Vector2, float>();
    Dictionary<Vector2, float> fDstar = new Dictionary<Vector2, float>();

    public List<Vector2> pathCheck = new List<Vector2>();
    

    public GameManagerScript gms;

    // Use this for initialization
    void Start () {
        tempStart = startPoint;
        int iGridCells = grid.transform.childCount;
        for (int count = 0; count < iGridCells; count++)
        {
            if (!grid.transform.GetChild(count).gameObject.activeInHierarchy)
                traversablePoints.Add(grid.transform.GetChild(count).position);
        }
    }
    

    protected List<Vector2> BackTrackPath(Vector2 end, Color color, bool debug)
    {
        List<Vector2> path = new List<Vector2>();
        if (end != null)
        {
            path.Add(end);
            while (parents[end] != startPoint)
            {
                path.Add(parents[end]);
                if (debug)
                    Debug.DrawLine(end, parents[end], color, 1, false);
                end = parents[end];
            }
            // Reverse the path so the start node is at index 0
            path.Reverse();
            parents.Clear();
        }
        return path;
    }

    protected List<Vector2> BackTrackPathDstar(Vector2 end, Vector2 target, Color color, bool debug)
    {
        List<Vector2> path = new List<Vector2>();
        if (end != null)
        {
            while (parentsDstar[end] != endPoint)
            {
                path.Add(parentsDstar[end]);
                if (debug)
                    Debug.DrawLine(end, parentsDstar[end], color, 0.5f, false);
                end = parentsDstar[end];
            }
            // Reverse the path so the start node is at index 0
            //path.Reverse();
            path.Add(target);
            if (debug)
                Debug.DrawLine(end, target, color, 0.5f, false);
            parentsDstar.Clear();
        }
        return path;
    }


    List<Vector2> CheckNeighbouringPoints(Vector2 pointToCheck)
    {
        List<Vector2> neighbours = new List<Vector2>();
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                Vector2 neighbouringPoint = pointToCheck + new Vector2(1.5f * x, 1f * y);
                if (traversablePoints.Contains(neighbouringPoint))
                {
                    //if on current point ignore
                    if(x == 0 && y == 0) { continue; }
                    ////if searching above but there is a roof ignore
                    if (y > 0 && !traversablePoints.Contains(pointToCheck + new Vector2(0f, 1f))) { continue; }//if (y > 0 && x == 0 && traversablePoints.Contains(pointToCheck + new Vector2(0f, -2f))) { continue; }

                    neighbours.Add(pointToCheck + new Vector2(1.5f * x, 1f * y));
                }
            }
        }
        return neighbours;
    }

    float CostToPoint(Vector2 current, Vector2 nextPoint)
    {
        Vector2 dir = nextPoint - current;
        float dist = Vector2.Distance(current, nextPoint);
        float groundMultiplier = 5f;
        if(!traversablePoints.Contains(nextPoint - new Vector2(0, 1)))
        {
            groundMultiplier = 1f;
        }
        if (dist == 1)
            return 0.1f*groundMultiplier;
        if (dist == 1.5f)
              return 0.5f * groundMultiplier;
        if (dist > 1.5f)
        {
            if (dir.x > 0 && dir.y > 0 && !traversablePoints.Contains(nextPoint + new Vector2(0, 1)) && !traversablePoints.Contains(nextPoint + new Vector2(1, 0)))
            {
                return 5000f;
            }
            if (dir.x < 0 && dir.y > 0 && !traversablePoints.Contains(nextPoint + new Vector2(0, 1)) && !traversablePoints.Contains(nextPoint + new Vector2(-1, 0)))
            {
                return 5000f;
            }
            if (dir.x > 0 && dir.y < 0 && !traversablePoints.Contains(nextPoint + new Vector2(0, -1)) && !traversablePoints.Contains(nextPoint + new Vector2(1, 0)))
            {
                return 5000f;
            }
            if (dir.x < 0 && dir.y < 0 && !traversablePoints.Contains(nextPoint + new Vector2(0, -1)) && !traversablePoints.Contains(nextPoint + new Vector2(-1, 0)))
            {
                return 5000f;
            }
            if (!traversablePoints.Contains(current + new Vector2(0f, 1f)))
                return 2f * groundMultiplier;
            return 0.75f * groundMultiplier;
        }
        return 0f * groundMultiplier;
    }

    public List<Vector2> AStarSearch(Vector2 start, Vector2 end, Color color, bool showLines)
    {
        f.Clear();
        g.Clear();
        h.Clear();

        List<Vector2> openList = new List<Vector2>();
        List<Vector2> closedList = new List<Vector2>();

        openList.Add(start);
        Vector2 current = start;
        while (openList.Count > 0)
        {
            Vector2 checkFVector = startPoint;
            float checkF = float.MaxValue;
            foreach (Vector2 point in openList)
            {
                if (f.ContainsKey(point))
                {
                    if (f[point] < checkF)
                    {
                        checkF = f[point];
                        checkFVector = point;
                    }
                }
            }

            current = checkFVector;
            
            openList.Remove(current);
            closedList.Add(current);

            if (current == end)
            {
                return BackTrackPath(end, color, showLines);
            }

            List<Vector2> traversableNeighbours = new List<Vector2>();
            traversableNeighbours = CheckNeighbouringPoints(current);

            int numNeighbours = traversableNeighbours.Count;

            float fConsistent = float.MaxValue;

            for (int neighbourCurrentIndex = 0; neighbourCurrentIndex < numNeighbours; ++neighbourCurrentIndex)
            {
                Vector2 neighbourCurrent = traversableNeighbours[neighbourCurrentIndex];
                if (closedList.Contains(neighbourCurrent))
                {
                    continue;
                }
                //cost is distance as terrain is standard
                float hEstCostToEnd = Vector2.Distance(neighbourCurrent, end)*2;
                float gCostToNeigbour = CostToPoint(neighbourCurrent, current);
                fConsistent = gCostToNeigbour + hEstCostToEnd;
                if (f.ContainsKey(neighbourCurrent))
                {
                    f.Remove(neighbourCurrent);
                    g.Remove(neighbourCurrent);
                    h.Remove(neighbourCurrent);
                }
                f.Add(neighbourCurrent, float.MaxValue);
                if (fConsistent <= (f[neighbourCurrent]) || !(openList.Contains(neighbourCurrent)))
                {
                    g.Add(neighbourCurrent, gCostToNeigbour);
                    f[neighbourCurrent] = g[neighbourCurrent];
                    h.Add(neighbourCurrent, ManhattanHeuristic(neighbourCurrent.x, neighbourCurrent.y, end.x, end.y));
                    f[neighbourCurrent] += h[neighbourCurrent];
                }
                else
                {
                    continue;
                }

                if (!(openList.Contains(neighbourCurrent)))
                {
                    parents.Add(neighbourCurrent, current);
                    openList.Add(neighbourCurrent);
                }
            }
        }
        return null;
    }

    public List<Vector2> DStarSearch(Vector2 target, Vector2 pos, Color color, bool showLines)
    {
        fDstar.Clear();
        gDstar.Clear();
        hDstar.Clear();

        List<Vector2> openList = new List<Vector2>();
        List<Vector2> closedList = new List<Vector2>();

        endPoint = pos;

        openList.Add(pos);
        Vector2 current = pos;
        while (openList.Count > 0)
        {
            Vector2 checkFVector = current;
            float checkF = float.MaxValue;
            foreach (Vector2 point in openList)
            {
                if (fDstar.ContainsKey(point))
                {
                    if (fDstar[point] < checkF)
                    {
                        checkF = fDstar[point];
                        checkFVector = point;
                    }
                }
            }

            current = checkFVector;

            openList.Remove(current);
            closedList.Add(current);

            if (current == target)
            {
                return BackTrackPathDstar(target, pos, color, showLines);
            }

            List<Vector2> traversableNeighbours = new List<Vector2>();
            traversableNeighbours = CheckNeighbouringPoints(current);

            int numNeighbours = traversableNeighbours.Count;

            float fConsistent = float.MaxValue;
            for (int neighbourCurrentIndex = 0; neighbourCurrentIndex < numNeighbours; ++neighbourCurrentIndex)
            {
                Vector2 neighbourCurrent = traversableNeighbours[neighbourCurrentIndex];
                if (closedList.Contains(neighbourCurrent))
                {
                    continue;
                }
                //cost is distance as terrain is standard
                float hEstCostToStart = Vector2.Distance(target, neighbourCurrent);
                float gCostToNeigbour = CostToPoint(current, neighbourCurrent);
                fConsistent = gCostToNeigbour + hEstCostToStart;
                if (fDstar.ContainsKey(neighbourCurrent))
                {
                    fDstar.Remove(neighbourCurrent);
                    gDstar.Remove(neighbourCurrent);
                    hDstar.Remove(neighbourCurrent);
                }
                fDstar.Add(neighbourCurrent, float.MaxValue);
                if (fConsistent <= (fDstar[neighbourCurrent]) || !(openList.Contains(neighbourCurrent)))
                {
                    gDstar.Add(neighbourCurrent, gCostToNeigbour);
                    fDstar[neighbourCurrent] = gDstar[neighbourCurrent];
                    hDstar.Add(neighbourCurrent, ManhattanHeuristic(neighbourCurrent.x, neighbourCurrent.y, target.x, target.y));
                    fDstar[neighbourCurrent] += hDstar[neighbourCurrent];
                }
                else
                {
                    continue;
                }
                if (!(openList.Contains(neighbourCurrent)))
                {
                    parentsDstar.Add(neighbourCurrent, current);
                    openList.Add(neighbourCurrent);
                }
            }
        }
        return null;
    }

    public List<Vector2> BreadthFirstSearch(Vector2 start, Vector2 end)
    {
        visited.Add(start, true);

        Queue<Vector2> stack = new Queue<Vector2>();
        stack.Enqueue(start);

        while (stack.Count > 0)
        {
            Vector2 currentNode = stack.Dequeue();

            if (currentNode == end)
            {
                return BackTrackPath(end, Color.blue, true);
            }

            List<Vector2> traversableNeighbours = new List<Vector2>();
            traversableNeighbours = CheckNeighbouringPoints(currentNode);

            int numNeighbours = traversableNeighbours.Count;
            for (int connectedNodesIndex = 0; connectedNodesIndex < numNeighbours; ++connectedNodesIndex)
            {
                Vector2 connectedNode = traversableNeighbours[connectedNodesIndex];
                if (!visited.ContainsKey(connectedNode))
                {
                    visited.Add(connectedNode, true);
                    parents.Add(connectedNode, currentNode);

                    stack.Enqueue(connectedNode);
                }
            }
        }
        return null;
    }



    // Update is called once per frame
    void Update()
    {
        if (gameObject.name == "PathfinderChase")
        {
            startPoint = gms.GetChaserGridPos();
        }
        if (gameObject.name == "PathfinderEvade")
        {
            startPoint = gms.GetEvaderGridPos();
        }
        endPoint = traversablePoints[endIndex];

        if (startPoint != endPoint)
        {
            if (tempStart != startPoint)
            {
                tempStart = startPoint;
            }
        }
    }

    private float EuclideanDistanceHeuristic(float currentX, float currentY, float targetX, float targetY)
    {
        float xDist = Mathf.Abs(currentX - targetX);
        float yDist = Mathf.Abs(currentY - targetY);

        return 10 * Mathf.Sqrt((xDist * xDist + yDist * yDist) * (xDist * xDist + yDist * yDist));
    }

    private float ManhattanHeuristic(float currentX, float currentY, float targetX, float targetY)
    {
        return Mathf.Abs(currentX - targetX) + Mathf.Abs(currentY - targetY);
    }

    private float ChebyshevHeuristic(float currentX, float currentY, float targetX, float targetY)
    {
        return Mathf.Max((targetX - currentX) ,(targetY - currentY));
    }
}
