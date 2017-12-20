using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaderScript : MonoBehaviour {

    public Rigidbody2D rb;

    public Transform groundCheck;
    float groundCheckRadius = -0.001f;
    public LayerMask whatIsGround;

    public GameManagerScript gms;

    public PathfindingScript pfs;
    [SerializeField]
    private List<Vector2> pathToTravel = new List<Vector2>();
    Vector2 start;
    Vector2 pointCurrent;


    GameObject nextCollect;

    bool deposit = false;

    float ForceX = 0f;
    float ForceY = 0f;
    public bool canJump = true;
    bool jumped = false;

    public List<GameObject> collectables = new List<GameObject>();

    void Start()
    {
        pathToTravel.Add(transform.position);
        pointCurrent = pathToTravel[0];
        whatIsGround = 1 << LayerMask.NameToLayer("Floor");
        float distCollect = float.MaxValue;
        foreach (GameObject drop in collectables)
        {
            if (distCollect > Vector2.Distance(transform.position, drop.transform.position))
            {
                distCollect = Vector2.Distance(transform.position, drop.transform.position);
                nextCollect = drop;
            }
        }
    }

    void Movement()
    {
        //lateral speed of player
        float vel = rb.velocity.x;
        //checks if in motion
        if (ForceX != 0)
        {
            rb.AddForce(new Vector2(ForceX, rb.velocity.y + ForceY));
        }
        if (gms.GetEvaderGridPos() != new Vector2(nextCollect.transform.position.x, nextCollect.transform.position.y))
        {
            if (pointCurrent.x > transform.position.x)
            {
                ForceX = 8.5f;
            }
            else if (pointCurrent.x < transform.position.x)
            {
                ForceX = -8.5f;
            }
            if (pointCurrent.y > gms.GetEvaderGridPos().y)
            {
                if (canJump)
                {
                    if (!jumped)
                    {
                        jumped = true;
                        StartCoroutine(Wait());
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        rb.AddForce(new Vector2(0f, 300f));
                    }
                }
            }
        }
        else
        {
            float dist = Vector2.Distance(new Vector2(nextCollect.transform.position.x, nextCollect.transform.position.y), transform.position);
            if (dist < 0.5f)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                ForceX = 0;
            };
        }
        if (rb.velocity.y > 0)
        {
            rb.gravityScale = 1;
        }
        else
        {
            rb.gravityScale = 1.15f;
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    bool GroundCheck()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        jumped = false;
        yield return 0;
    }

    void Update()
    {
        float distCollect = float.MaxValue;
        foreach (GameObject drop in collectables)
        {
            if (distCollect > Vector2.Distance(transform.position, drop.transform.position))
            {
                distCollect = Vector2.Distance(transform.position, drop.transform.position);
                nextCollect = drop;
            }
        }
        //if (!deposit)
        {
            if (gms.GetEvaderGridPos() != new Vector2(nextCollect.transform.position.x, nextCollect.transform.position.y))
            {
                print(start);
                if (start != gms.GetEvaderGridPos())
                {
                    start = gms.GetEvaderGridPos();
                    pathToTravel = pfs.AStarSearch(gms.GetEvaderGridPos(), new Vector2(nextCollect.transform.position.x, nextCollect.transform.position.y), Color.blue);
                    pointCurrent = pathToTravel[0];
                    float dist = Vector2.Distance(pointCurrent, transform.position);
                    if (dist < 0.15f)
                    {
                        pointCurrent = pathToTravel[0];
                    }
                }
            }
            else
            {
                collectables.Remove(nextCollect);
                nextCollect.SetActive(false);
                deposit = true;
                print("Collectable Reached!!!");
            }
        }
        //else
        //{
        //    if (gms.GetEvaderGridPos() != new Vector2(-1.5f, -1f))
        //    {
        //        if (start != gms.GetEvaderGridPos())
        //        {
        //            start = gms.GetEvaderGridPos();
        //            pathToTravel = pfs.AStarSearch(gms.GetEvaderGridPos(), new Vector2(-1.5f, -1f), Color.blue);
        //            pointCurrent = pathToTravel[0];
        //            float dist = Vector2.Distance(pointCurrent, transform.position);
        //            if (dist < 0.15f)
        //            {
        //                pointCurrent = pathToTravel[0];
        //            }
        //        }
        //    }
        //    else
        //    {
        //        deposit = false;
        //        print("Deposit Reached!!!");
        //    }
        //}
        canJump = GroundCheck();
    }
}
