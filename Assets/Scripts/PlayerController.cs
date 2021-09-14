using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;

    [SerializeField] private bool combatEnabled = true;
    public float inputTimer, attack1Radius;
    public int attack1Damage = 40;
    [SerializeField] private Transform attack1HitBoxPos;
    [SerializeField] private LayerMask Damageable;

    private enum State {idle,run,jump,fall,climbi,attack1}
    private State state = State.idle;
    private string currentAnimaton;
    const string Player_Run = "Run";
    const string Player_Idle = "Idle";
    const string Player_Jump = "Jump";
    const string Player_Fall = "Fall";
    const string Player_Climb = "Climb";
    const string Player_Attack1 = "Attack1";
    
    public Transform groundCheck;
    public Transform wallCheck;
    public Transform ledgeCheck;
    public bool isGrounded;
    public bool isTouchingWall;
    public bool isWallSliding;
    public bool isTouchingLedge;
    public bool canClimbLedge = false; 
    public bool ledgeDetected;
    public bool isFacingRight = true;
    public bool isDashing;
    private bool canFlip = true;
    public bool gotInput;
    private bool isAttacking, isFirstAttack; 

    [SerializeField]private LayerMask Ground;
    [SerializeField]private float speed = -5f;
    private float jumpforce = 10f;
    public float facingDirection;
    public float dashTimeLeft; 
    private float lastImageXpos;
    private float lastDash = -100f;
    private int skull = 0;
    private int fireSkull = 0;

    public float ledgeClimbXOffset1 = 0f,ledgeClimbYOffset1 = 0f,ledgeClimbXOffset2 = 0f,ledgeClimbYOffset2 = 0f;

    public Vector2 ledgePosBot;
    public Vector2 ledgePos1;
    public Vector2 ledgePos2;

    public float groundCheckRadius;
    public float hDirection;    
    public float wallCheckDistance;
    public float dashTime, dashSpeed, distanceBetweenImages, dashCoolDown = -100;
    [SerializeField] private Text skullText;
    public float lastInputTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();

    }
    private void Update()
    {
        CheckAttacks();
        Movement();
        CheckLedgeClimb();
        anim.SetInteger("state", (int)state);
        VelocityState();
    }
    private void FixedUpdate()
    {   
        CheckDash(); 
        ApplyMovement();
        CheckSurroundings();
        FinalCollisionCheck();

    }
    private void Movement()
    {

        if(Input.GetButtonDown("Fire1"))
        {
            if(combatEnabled && !isAttacking && (currentAnimaton != "Jump") && (currentAnimaton != "Fall"))
            {
            gotInput = true;
            isAttacking = false;
            lastInputTime = Time.time; 
            }
        }

        hDirection = Input.GetAxisRaw("Horizontal");

        if (isFacingRight && hDirection > 0 && !canClimbLedge && !isAttacking)
        {
            if (isGrounded) 
           {
            ChangeAnimationState(Player_Run);
           } 
            Flip();
        }

        else if(!isFacingRight && hDirection < 0 && !canClimbLedge && !isAttacking)
        {
           if (isGrounded) 
           {
            ChangeAnimationState(Player_Run);
           } 
            Flip();
            
        }
        if (Input.GetButtonDown("Dash") ) 
        {
            if(Time.time >= (lastDash + dashCoolDown))
            AttempToDash();
        }
        if(Input.GetButtonDown("Jump") && isGrounded && !isAttacking)
        {
            rb.velocity = new Vector2(rb.velocity.x,jumpforce);
            ChangeAnimationState(Player_Jump);
        }
    }

    private void VelocityState()
    {   
        if (canClimbLedge)
          {     {
                ChangeAnimationState(Player_Climb);
                }
          }
        else if (rb.velocity.y < -.2)
        {
                ChangeAnimationState(Player_Fall);

        }

        else if (currentAnimaton == "Jump")
        {   
            if (rb.velocity.y < .1f)
            {
                ChangeAnimationState(Player_Fall);
            }
        }
        else if (Mathf.Abs(rb.velocity.x) > 2 && isGrounded && !canClimbLedge && !isAttacking)
        {
            ChangeAnimationState(Player_Run);

        }
        else if (isAttacking) 
        {
                ChangeAnimationState(Player_Attack1);
        }
        else 
        {
            if(isGrounded)
            {
            ChangeAnimationState("Idle");
            }
        }
        
    }
    private void CheckSurroundings()
    {

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, Ground);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, Ground);
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, Ground);
        
        if(isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePosBot = wallCheck.position;
        }
        if (isTouchingWall && isTouchingLedge)
        {
            ledgeDetected = false;

        }
        if (!isTouchingWall && !isTouchingLedge)
        {
            ledgeDetected = false;

        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
    if(collision.tag == "Collectable")
        {
            Destroy(collision.gameObject);
            skull += 1;
            skullText.text = skull.ToString();
        }

    else if (collision.tag == "Collectable1")
    {
            Destroy(collision.gameObject);
            skull += 5;
            skullText.text = skull.ToString();
    }
    } 
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y) );
        Gizmos.DrawLine(ledgeCheck.position, new Vector2(ledgeCheck.position.x + wallCheckDistance, ledgeCheck.position.y) );
        Gizmos.DrawWireSphere(attack1HitBoxPos.position,attack1Radius);
    }
    private void FinalCollisionCheck()
    {
        // Get the velocity
        Vector2 hDirection = new Vector2(rb.velocity.x * Time.fixedDeltaTime, 0.1f);
     
        // Get bounds of Collider
        var topRight = new Vector2(coll.bounds.max.x, coll.bounds.max.y);
        var bottomLeft = new Vector2(coll.bounds.min.x, coll.bounds.min.y);
 
        // Move collider in direction that we are moving
        topRight += hDirection;
        bottomLeft += hDirection;
            
        // Check if the body's current velocity will result in a collision
        if (Physics2D.OverlapArea(bottomLeft, topRight, Ground))
        {
            // If so, stop the movement
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }   
    private void CheckLedgeClimb()
    {
        if(ledgeDetected && !canClimbLedge &&  ((currentAnimaton == "Jump") || currentAnimaton == "Fall") )
        {
            canClimbLedge = true;
            canFlip = false;
            if (isFacingRight)
            {
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }
            else
            {
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }
        }
        if(isGrounded)
        {
            canClimbLedge = false;
        }
        if (canClimbLedge)
        {
            transform.position = ledgePos1;
        }
    }
    private void Flip()
    {
        if (canFlip)
        {
        {
            transform.Rotate(0.0f, 180.0f, 0.0f);
            isFacingRight = !isFacingRight;

        }
        if (isFacingRight)
        {
            facingDirection = -1;
        }
        if (!isFacingRight)
        {
            facingDirection = 1;
        }}
    }
    public void FinishLedgeClimb()
    {
        canClimbLedge = false;
        transform.position = ledgePos2;
        ledgeDetected = false;
        ChangeAnimationState("Idle");
        canFlip = true;
    }
    private void ApplyMovement()
    {
        if(isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x , rb.velocity.y);
        }
        else if (!isGrounded && hDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x , rb.velocity.y);
        }
        else 
        {
            rb.velocity = new Vector2(speed * hDirection, rb.velocity.y);
        }
    }
    private void AttempToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
        
        AfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }
    private void CheckDash()
    {
        if (isDashing)
        {   
            if(dashTimeLeft <=0 || isTouchingWall)
            {
            isDashing = false; 
            }
            if(dashTimeLeft > 0)
            rb.velocity = new Vector2(dashSpeed * facingDirection, rb.velocity.y);
            dashTimeLeft -= Time.deltaTime; 

            if(Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
            {
                AfterImagePool.Instance.GetFromPool();
                lastImageXpos = transform.position.x;
            }
        }
    }
    public void StopClimb()
        {
            rb.velocity = new Vector2(0,0);
            if(Input.GetButtonDown("up"))
            {
                CheckLedgeClimb();
            }
            else if (Input.GetButtonDown("down"))
            {
                rb.velocity = new Vector2(0,-20);
            }
            else
            {
                rb.velocity = new Vector2(0,0);

            }
        }
    public void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimaton == newAnimation) return;

        anim.Play(newAnimation);
        currentAnimaton = newAnimation;
    }
    public void CheckAttacks()
{
    if(gotInput)
    {   
        if(!isAttacking && (Time.time >= lastInputTime + inputTimer))
        {   
            gotInput = false;
            isAttacking = true;
            isFirstAttack = !isFirstAttack;
        }

    }
}
    private void CheckAttackHitBox()
{
    Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, Damageable);

    foreach (Collider2D enemy in detectedObjects)
    {
        enemy.GetComponent<Enemy>().TakeDamage(attack1Damage);
        Debug.Log("Hit" + enemy.name);
    }
}
    public void FinistAttack1()
    {
        isAttacking = false;
    }
}



