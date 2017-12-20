using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserScript : MonoBehaviour
{
    public Rigidbody2D rb;

    public Transform groundCheck;
    float groundCheckRadius = -0.001f;
    public LayerMask whatIsGround;

    public GameManagerScript gms;

    public PathfindingScript pfs;
    [SerializeField]
    private List<Vector2> pathToTravel = new List<Vector2>();

    Vector2 start = new Vector2(-7,-4);
    Vector2 pointCurrent;

    float ForceX = 0f;
    float ForceY = 0f;
    public bool canJump = true;
    bool jumped = false;

    public GameObject Evader;

    // Use this for initialization
    void Start()
    {
        pathToTravel.Add(transform.position);
        pointCurrent = pathToTravel[0];
        whatIsGround = 1 << LayerMask.NameToLayer("Floor");
    }

    void Movement(){
        //lateral speed of player
        float vel = rb.velocity.x;
        //checks if in motion
        if (ForceX != 0)
        {
            rb.AddForce(new Vector2(ForceX, rb.velocity.y + ForceY));
        }
        if (gms.GetChaserGridPos() != gms.GetEvaderGridPos())
        {
            if (pointCurrent.x > transform.position.x)
            {
                ForceX = 8.5f;
            }
            else if (pointCurrent.x < transform.position.x)
            {
                ForceX = -8.5f;
            }
            if (pointCurrent.y > gms.GetChaserGridPos().y)
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

    // Update is called once per frame
    void Update()
    {
        if (gms.GetChaserGridPos() != gms.GetEvaderGridPos())
        {
            if (start != gms.GetChaserGridPos())
            {
                start = gms.GetChaserGridPos();
                pathToTravel = pfs.AStarSearch(gms.GetChaserGridPos(), gms.GetEvaderGridPos(), Color.red);
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
            Evader.transform.position = new Vector3(-1.5f, -1f);
        }
        canJump = GroundCheck();
        
    }
}
