using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour {

    [SerializeField]
    private int r;
    [SerializeField]
    private int c;
    [SerializeField]
    private Vector2 gS;

    [SerializeField]
    private Sprite cellSprite;
    [SerializeField]
    private Vector2 cellOffset;

    public Vector2 cellSize;
    private Vector2 cellScale;

    // Use this for initialization
    void Start () {
        InitialiseCells();
	}

    void InitialiseCells()
    {
        GameObject cellObject = new GameObject();
        cellObject.name = "original";
        cellObject.AddComponent<SpriteRenderer>().sprite = cellSprite;
        cellSize = cellSprite.bounds.size;
        Vector2 newCellSize = new Vector2(gS.x / (float)c, gS.y / (float)r);

        cellScale.x = newCellSize.x / cellSize.x;
        cellScale.y = newCellSize.y / cellSize.y;

        cellSize = newCellSize;

        cellObject.transform.localScale = cellScale;

        cellOffset.x = (int)-(gS.x / 2) + cellSize.x / 2;
        cellOffset.y = (int)-(gS.y / 2) + cellSize.y / 2;

        for (int row = 0; row < r; row++)
        {
            for (int col = 0; col < c; col++)
            {
                Vector2 pos = new Vector2(col * cellSize.x + cellOffset.x, row * cellSize.y + cellOffset.y);
                GameObject currentCell = Instantiate(cellObject, pos, Quaternion.identity) as GameObject;
                currentCell.name = "Cell x : " + (col+1).ToString() + " y: " + (row+1).ToString();
                currentCell.transform.parent = transform;
            }
        }
    }

    // Update is called once per frame
    void Update () {
       
    }
}
