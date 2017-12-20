using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    public Transform Chaser;
    private Vector2 chaserPointLocation;

    public Transform Evader;
    private Vector2 evaderPointLocation = new Vector2(9,2);


    public List<Vector2> collectable = new List<Vector2>();

    List<Vector2> accessiblePoints = new List<Vector2>();

    public PathfindingScript pfs;

    // Use this for initialization
    void Start () {
        accessiblePoints = pfs.traversablePoints;
        //evaderPointLocation = new Vector2(1.5F, 3.0F);
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
        if (accessiblePoints.Contains(chaserCheckV))
            chaserPointLocation = GetNearestPoint(Chaser.position);
    }

    void SetEvaderGridPos()
    {
        Vector2 evaderCheckV = GetNearestPoint(Evader.position);
        if (accessiblePoints.Contains(evaderCheckV))
            evaderPointLocation = GetNearestPoint(Evader.position);
    }

    public Vector2 GetChaserGridPos()
    {
        //SetChaserGridPos();
        return chaserPointLocation;
    }

    public Vector2 GetEvaderGridPos()
    {
        //SetEvaderGridPos();
        return evaderPointLocation;
    }

    // Update is called once per frame
    void Update () {
        SetChaserGridPos();
        SetEvaderGridPos();
    }
}
