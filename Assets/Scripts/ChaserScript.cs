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

    Vector2 start;
    Vector2 pointCurrent;

    float ForceX = 0f;
    float ForceY = 0f;
    public bool canJump = true;
    bool jumped = false;

    bool started = false;

    public GameObject Evader;

    public bool ShowLines = true;

    // Use this for initialization
    void Start()
    {
        pathToTravel.Add(transform.position);
        pointCurrent = pathToTravel[0];
        whatIsGround = 1 << LayerMask.NameToLayer("Chaser");
        StartCoroutine(JustStarted());
    }

    IEnumerator JustStarted()
    {
        yield return new WaitForSeconds(1f);
        started = true;
        yield return 0;
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
                        float jumpThres = 1.4f;
                        Vector2 dir = new Vector2(gms.GetChaserGridPos().x - pointCurrent.x, gms.GetChaserGridPos().y - pointCurrent.y);
                        if (dir.x > 0 || dir.x < 0 && dir.y > 0 || dir.y < 0)
                        {
                            jumpThres = 1.9f;
                        }
                        float Dist = Vector2.Distance(transform.position, pointCurrent);
                        if (Dist < jumpThres)
                        {
                            jumped = true;
                            StartCoroutine(Wait());
                            rb.velocity = new Vector2(0, rb.velocity.y);
                            rb.AddForce(new Vector2(0f, 300f));
                        }
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
        bool evaderSafe = gms.accessiblePointsChaser.Contains(gms.GetEvaderGridPos());
        if (evaderSafe)
        {
            if (gms.GetChaserGridPos() != gms.GetEvaderGridPos())
            {
                if (start != gms.GetChaserGridPos())
                {
                    start = gms.GetChaserGridPos();
                    pathToTravel = pfs.DStarSearch(gms.GetChaserGridPos(), gms.GetEvaderGridPos(), Color.red, ShowLines);
                    pointCurrent = pathToTravel[0];
                }
            }
            else
            {
                if (started)
                {
                    print("Target Reached!!!");
                    Evader.transform.position = new Vector3(-1.5f, -1f);
                    Evader.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
                }
            }
        }
        else
        {
            if (gms.GetChaserGridPos() != new Vector2(9f, 6f))
            {
                if (start != gms.GetChaserGridPos())
                {
                    start = gms.GetChaserGridPos();
                    pathToTravel = pfs.DStarSearch(gms.GetChaserGridPos(), new Vector2(9f, 6f), Color.white, ShowLines);
                    pointCurrent = pathToTravel[0];
                }
            }
            else
            {
                if (started)
                {
                    print("Target Reached!!!");
                }
            }
        }
        canJump = GroundCheck();
        
    }
}
