using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    public Transform Chaser;
    private Vector2 chaserPointLocation;


    public Vector2 evaderPointLocation = new Vector2(7.5f,-2f);

    List<Vector2> accessiblePoints = new List<Vector2>();

	// Use this for initialization
	void Start () {
        accessiblePoints = PathfindingScript.traversablePoints;
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

    }
}
