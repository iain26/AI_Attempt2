using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingScript : MonoBehaviour {

    public static List<Vector2> traversablePoints = new List<Vector2>();
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

    public enum PathfindSearch { dStar, aStar, BFS };
    public enum Heuristic { Euclidean, Manhattan , Chebyshev };

    delegate float HeuristicDist(float x1, float y1, float x2, float y2);
    HeuristicDist HToUse;

    public PathfindSearch pathfindSearch = PathfindSearch.aStar;
    public Heuristic heuristic;
    
    public List<Vector2> pathCheck = new List<Vector2>();

    int upAmount = 0;

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

        startPoint = traversablePoints[startIndex];

        //startPoint = gms.GetChaserGridPos();
        endPoint = traversablePoints[endIndex];

        //switch (heuristic)
        //{
        //    case Heuristic.Euclidean:
        //        HToUse = EuclideanDistanceHeuristic;
        //        break;
        //    case Heuristic.Manhattan:
        //        HToUse = ManhattanHeuristic;
        //        break;
        //    case Heuristic.Chebyshev:
        //        HToUse = ChebyshevHeuristic;
        //        break;
        //}

        switch (pathfindSearch)
        {
            case PathfindSearch.aStar:
                //pathCheck = AStarSearch(startPoint, endPoint/*, HToUse*/);
                break;
            case PathfindSearch.dStar:
                break;
            case PathfindSearch.BFS:
                pathCheck = BreadthFirstSearch(startPoint, endPoint);
                break;
        }
        //Debug.DrawLine(pathCheck[0], startPoint, Color.green, 1, false);

        //print(CheckNeighbouringPoints(traversablePoints[96]).Count);

        //print(path)
    }

    protected List<Vector2> BackTrackPath(Vector2 end)
    {
        List<Vector2> path = new List<Vector2>();
        if (end != null)
        {
            path.Add(end);
            float green = 0f;
            while (parents[end] != startPoint)
            {
                path.Add(parents[end]);
                Debug.DrawLine(end, parents[end], new Color(255f - green, green, 0),1,false);
                end = parents[end];
                green += 255f / (float)path.Count;
            }
            // Reverse the path so the start node is at index 0
            path.Reverse();
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
                    if (y > 0 && !traversablePoints.Contains(pointToCheck + new Vector2(0f, 1f))) { continue; }
                    //if (y > 0 && x == 0 && traversablePoints.Contains(pointToCheck + new Vector2(0f, -1f))) { continue; }

                    //if diagonal movement blocked then ignore
                    if (!traversablePoints.Contains(pointToCheck + new Vector2(0f, -1f)) &&
                        (!traversablePoints.Contains(pointToCheck + new Vector2(1f, 0f))) && x > 0 && y < 0) { continue; }
                    if (!traversablePoints.Contains(pointToCheck + new Vector2(0f, -1f)) &&
                        (!traversablePoints.Contains(pointToCheck + new Vector2(-1f, 0f))) && x < 0 && y < 0) { continue; }
                    if (!traversablePoints.Contains(pointToCheck + new Vector2(0f, 1f)) &&
                        (!traversablePoints.Contains(pointToCheck + new Vector2(1f, 0f))) && x > 0 && y > 0) { continue; }
                    if (!traversablePoints.Contains(pointToCheck + new Vector2(0f, 1f)) &&
                        (!traversablePoints.Contains(pointToCheck + new Vector2(-1f, 0f))) && x < 0 && y > 0) { continue; }

                    neighbours.Add(pointToCheck + new Vector2(1.5f * x, 1f * y));
                }
            }
        }
        return neighbours;
    }

    float CostToPoint(Vector2 current, Vector2 nextPoint)
    {
        float dist = Vector2.Distance(current, nextPoint);
        if (dist == 1)
            return 1f;
        if (dist == 1.5f)
            return 1.5f;
        if (dist > 1.5f)
            return 1.8f;
        return 0f;
    }

    //public List<Vector2> AStarSearch(Vector2 start, Vector2 end/*, HeuristicDist hToUse*/)
    //{

    //    parents.Clear();
    //    f.Clear();
    //    g.Clear();
    //    h.Clear();

    //    List<Vector2> openList = new List<Vector2>();
    //    List<Vector2> closedList = new List<Vector2>();

    //    openList.Add(start);

    //    while (openList.Count > 0)
    //    {

    //        ////openList.Sort();
    //        Vector2 checkFVector = startPoint;
    //        float checkF = float.MaxValue;
    //        foreach (Vector2 point in openList)
    //        {
    //            if (f.ContainsKey(point))
    //            {
    //                if (f[point] < checkF)
    //                {
    //                    checkF = f[point];
    //                    checkFVector = point;
    //                }
    //            }
    //        }

    //        Vector2 current = checkFVector;

    //        //Vector2 current = openList[0];

    //        openList.Remove(current);
    //        closedList.Add(current);

    //        if (current == end)
    //        {
    //            return BackTrackPath(end);
    //        }

    //        List<Vector2> traversableNeighbours = new List<Vector2>();
    //        traversableNeighbours = CheckNeighbouringPoints(current);

    //        int numNeighbours = traversableNeighbours.Count;
    //        //print(numNeighbours);

    //        float fConsistent = float.MaxValue;
    //        //int fIndex = 0;

    //        for (int neighbourCurrentIndex = 0; neighbourCurrentIndex < numNeighbours; ++neighbourCurrentIndex)
    //        {
    //            Vector2 neighbourCurrent = traversableNeighbours[neighbourCurrentIndex];
    //            if (closedList.Contains(neighbourCurrent))
    //            {
    //                continue;
    //            }
    //            //cost is distance as terrain is standard
    //            float hEstCostToEnd = Vector2.Distance(neighbourCurrent, end);
    //            float gCostToNeigbour = CostToPoint(current, neighbourCurrent);
    //            fConsistent = gCostToNeigbour + hEstCostToEnd;
    //            if (f.ContainsKey(neighbourCurrent)){
    //                f.Remove(neighbourCurrent);
    //                g.Remove(neighbourCurrent);
    //                h.Remove(neighbourCurrent);
    //            }
    //            f.Add(neighbourCurrent, float.MaxValue);
    //            if (fConsistent <= (f[neighbourCurrent]) || !(openList.Contains(neighbourCurrent)))
    //            {
    //                g.Add(neighbourCurrent, gCostToNeigbour);
    //                f[neighbourCurrent] = g[neighbourCurrent];
    //                h.Add(neighbourCurrent, /*hToUse*/ManhattanHeuristic(neighbourCurrent.x, neighbourCurrent.y, end.x, end.y));
    //                f[neighbourCurrent] += h[neighbourCurrent];
    //                //finish Euclidean heuristic here^
    //            }
    //            else
    //            {
    //                continue;
    //            }

    //            if (!(openList.Contains(neighbourCurrent)))
    //            {
    //                parents.Add(neighbourCurrent, current);
    //                openList.Add(neighbourCurrent);
    //            }
    //        }
    //    }

    //    return null;
    //}

    public List<Vector2> AStarSearch(Vector2 start, Vector2 end/*, HeuristicDist hToUse*/)
    {

        parents.Clear();
        f.Clear();
        g.Clear();
        h.Clear();

        List<Vector2> openList = new List<Vector2>();
        List<Vector2> closedList = new List<Vector2>();

        openList.Add(start);
        Vector2 current = start;
        while (openList.Count > 0)
        {

            ////openList.Sort();
            Vector2 checkFVector = startPoint;
            float checkF = float.MaxValue;
            Vector2 checkXVector = startPoint;
            float checkX = float.MaxValue;
            foreach (Vector2 point in openList)
            {
                //if(end.x > current.x)
                //{
                //    if(point.x < checkX)
                //    {
                //        checkX = point.x;
                //        checkXVector = point;
                //    }
                //}
                //if (end.x < current.x)
                //{
                //    if (point.x > checkX)
                //    {
                //        checkX = point.x;
                //        checkXVector = point;
                //    }
                //}
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

            //Vector2 current = openList[0];

            openList.Remove(current);
            closedList.Add(current);

            if (current == end)
            {
                return BackTrackPath(end);
            }

            List<Vector2> traversableNeighbours = new List<Vector2>();
            traversableNeighbours = CheckNeighbouringPoints(current);

            int numNeighbours = traversableNeighbours.Count;
            //print(numNeighbours);

            float fConsistent = float.MaxValue;
            //int fIndex = 0;

            for (int neighbourCurrentIndex = 0; neighbourCurrentIndex < numNeighbours; ++neighbourCurrentIndex)
            {
                Vector2 neighbourCurrent = traversableNeighbours[neighbourCurrentIndex];
                if (closedList.Contains(neighbourCurrent))
                {
                    continue;
                }
                //cost is distance as terrain is standard
                float hEstCostToEnd = Vector2.Distance(neighbourCurrent, end);
                float gCostToNeigbour = CostToPoint(current, neighbourCurrent);
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
                    h.Add(neighbourCurrent, /*hToUse*/ManhattanHeuristic(neighbourCurrent.x, neighbourCurrent.y, end.x, end.y));
                    f[neighbourCurrent] += h[neighbourCurrent];
                    //finish Euclidean heuristic here^
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
                return BackTrackPath(end);
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

        // No path has been found
        return null;
    }

    // Update is called once per frame
    void Update()
    {

        //startPoint = traversablePoints[startIndex];
        startPoint = gms.GetChaserGridPos();
        endPoint = traversablePoints[endIndex];

        if (startPoint != endPoint)
        {
            if (tempStart != startPoint)
            {
                tempStart = startPoint;
                //pathCheck = AStarSearch(startPoint, endPoint/*, ManhattanHeuristic*/);
                //Debug.DrawLine(pathCheck[0], startPoint, Color.green, 1, false);
            }
            //for (int y = 1; y < 16; y++)
            //{
            //    for (int x = 1; x < 16; x++)
            //    {
            //        //GameObject.Find("Cell x : " + x.ToString() + " y: " + y.ToString()).gameObject.transform.localPosition = new Vector3(1.5f * (x - 8), 1 * (y - 8));
            //    }
            //}
            //Vector2 change = startPoint;
            //if (Input.GetKeyDown(KeyCode.W))
            //{
            //    change = startPoint + new Vector2(0f, 1f);
            //}
            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    change = startPoint + new Vector2(0f, -1f);
            //}
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    change = startPoint + new Vector2(-1.5f, 0f);
            //}
            //if (Input.GetKeyDown(KeyCode.D))
            //{
            //    change = startPoint + new Vector2(1.5f, 0f);
            //}
            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    endIndex = Random.Range(0, traversablePoints.Count - 1);
            //    pathCheck = AStarSearch(startPoint, endPoint, ManhattanHeuristic);
            //    Debug.DrawLine(pathCheck[0], startPoint, Color.green, 1, false);
            //}
            //if (!traversablePoints.Contains(change))
            //    change = startPoint;
            //startIndex = traversablePoints.IndexOf(change);
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
