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
    private List<Vector2> pathToTravel = new List<Vector2>();

    Vector2 start;
    Vector2 pointCurrent;

    float ForceX = 0f;
    float ForceY = 0f;
    public bool canJump = true;
    bool jumped = false;

    // Use this for initialization
    void Start()
    {
        pathToTravel.Add(transform.position);
        pointCurrent = pathToTravel[0];
        whatIsGround = 1 << LayerMask.NameToLayer("Floor");
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
    }

    private void FixedUpdate()
    {
        Movement();
        if (gms.GetEvaderGridPos() != gms.collectable[0])
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
            float dist = Vector2.Distance(gms.GetEvaderGridPos(), transform.position);
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

    // Update is called once per frame
    void Update()
    {
        if (canJump)
        {
            //rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        if (gms.GetEvaderGridPos() != gms.collectable[0])
        {
            if (start != gms.GetEvaderGridPos())
            {
                start = gms.GetEvaderGridPos();
                pathToTravel = pfs.AStarSearch(gms.GetEvaderGridPos(), gms.collectable[0], Color.green);
                print(gms.GetEvaderGridPos());
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
            print("Target Reached!!!");
        }
        canJump = GroundCheck();

        if (Input.GetKey(KeyCode.Space))
        {
            if (canJump)
                rb.AddForce(new Vector2(0, 350f));
        }
    }
}
