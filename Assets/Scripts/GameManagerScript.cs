using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    public Transform Chaser;
    private Vector2 chaserPointLocation;

    public Transform Evader;
    private Vector2 evaderPointLocation;


    public List<Vector2> collectable = new List<Vector2>();

    public List<Vector2> accessiblePointsEvader = new List<Vector2>();
    public List<Vector2> accessiblePointsChaser = new List<Vector2>();

    public PathfindingScript pfsEvader;
    public PathfindingScript pfsChaser;

    // Use this for initialization
    void Start () {
        accessiblePointsEvader = pfsEvader.traversablePoints;
        accessiblePointsChaser = pfsChaser.traversablePoints;
    }

    Vector2 GetNearestPoint(Vector2 pos)
    {
        float y = Mathf.Round(pos.y);
        float rounded = Mathf.Round(pos.x / 1.5f);
        float x  = rounded * 1.5f;
        return new Vector2(x , y);
    }
	

    void SetChaserGridPos()
    {
        Vector2 chaserCheckV = GetNearestPoint(Chaser.position);
        if (accessiblePointsChaser.Contains(chaserCheckV))
            chaserPointLocation = GetNearestPoint(Chaser.position);
    }

    void SetEvaderGridPos()
    {
        Vector2 evaderCheckV = GetNearestPoint(Evader.position);
        if (accessiblePointsEvader.Contains(evaderCheckV))
            evaderPointLocation = GetNearestPoint(Evader.position);
    }

    public Vector2 GetChaserGridPos()
    {
        return chaserPointLocation;
    }

    public Vector2 GetEvaderGridPos()
    {
        return evaderPointLocation;
    }

    // Update is called once per frame
    void Update () {
        SetChaserGridPos();
        SetEvaderGridPos();
    }
}
