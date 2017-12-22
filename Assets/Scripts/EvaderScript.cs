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
    //[SerializeField]
    public List<Vector2> pathToTravel = new List<Vector2>();

    Vector2 start;
    Vector2 pointCurrent;

    GameObject nextCollect;
    bool deposit = false;
    Vector2 destination;

    float ForceX = 0f;
    float ForceY = 0f;
    public bool canJump = true;
    bool jumped = false;
    bool waited = false;

    public Transform Chaser;

    public List<GameObject> collectables = new List<GameObject>();

    public bool ShowLines = true;

    void Start()
    {
        pathToTravel.Add(transform.position);
        pointCurrent = pathToTravel[0];
        whatIsGround = 1 << LayerMask.NameToLayer("Evader");
        float distCollect = float.MaxValue;
        foreach (GameObject drop in collectables)
        {
            if (distCollect > Vector2.Distance(transform.position, drop.transform.position))
            {
                distCollect = Vector2.Distance(transform.position, drop.transform.position);
                nextCollect = drop;
                destination = nextCollect.transform.position;
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
        if(ForceX > 0)
        {
            if(vel > ForceX)
            {
                rb.velocity = new Vector2(ForceX, rb.velocity.y);
            }
        }
        if (ForceX < 0)
        {
            if (vel < ForceX)
            {
                rb.velocity = new Vector2(ForceX, rb.velocity.y);
            }
        }
        if (gms.GetEvaderGridPos() != destination)
        {
            if (pointCurrent.x > transform.position.x)
            {
                ForceX = 8.0f;
            }
            else if (pointCurrent.x < transform.position.x)
            {
                ForceX = -8.0f;
            }
            if (pointCurrent.y > gms.GetEvaderGridPos().y)
            {
                if (canJump)
                {
                    if (!jumped)
                    {
                        float jumpThres = 1.4f;
                        Vector2 dir = new Vector2(gms.GetEvaderGridPos().x - pointCurrent.x, gms.GetEvaderGridPos().y - pointCurrent.y);
                        if (dir.x > 0 || dir.x < 0 && dir.y > 0 || dir.y < 0)
                        {
                            jumpThres = 1.9f;
                        }
                        float Dist = Vector2.Distance(transform.position, pointCurrent);
                        if (Dist < jumpThres)
                        {
                            jumped = true;
                            StartCoroutine(Wait());
                            rb.velocity = new Vector2(0,0);
                            rb.AddForce(new Vector2(0f, 300f));
                        }
                    }
                }
            }
        }
        else
        {
            float dist = Vector2.Distance(destination, transform.position);
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
        yield return new WaitForSeconds(0.25f);
        jumped = false;
        yield return 0;
    }

    IEnumerator RespawnDrop(GameObject drop)
    {
        yield return new WaitForSeconds(16f);
        drop.SetActive(true);
        collectables.Add(drop);
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
            if (!deposit)
            {
                destination = nextCollect.transform.position;
                if (gms.GetEvaderGridPos() != destination)
                {
                    if (start != gms.GetEvaderGridPos())
                    {
                        start = gms.GetEvaderGridPos();
                    if (pfs.traversablePoints.Contains(destination))
                    {
                        pathToTravel = pfs.DStarSearch(gms.GetEvaderGridPos(), destination, Color.cyan, ShowLines);
                    }
                    pointCurrent = pathToTravel[0];
                    }
                }
                else
                {
                    collectables.Remove(nextCollect);
                    nextCollect.SetActive(false);
                    StartCoroutine(RespawnDrop(nextCollect));
                    deposit = true;
                    print("Collectable Reached!!!");
                }
            }
            else
            {
                destination = new Vector2(-1.5f, -1f);
                if (gms.GetEvaderGridPos() != destination)
                {
                    if (start != gms.GetEvaderGridPos())
                    {
                        start = gms.GetEvaderGridPos();
                        pathToTravel = pfs.DStarSearch(gms.GetEvaderGridPos(), destination, Color.magenta, ShowLines);
                        pointCurrent = pathToTravel[0];
                    }
                }
                else
                {
                    deposit = false;
                    print("Deposit Reached!!!");
                }
            }
        
            canJump = GroundCheck();
    }
}
