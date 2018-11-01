using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CompletePlayerController : MonoBehaviour
{
    public Vector2 spawnPos;

    //Debug
    public bool undying;

    private int health;
    private Text healthText;
    private GameObject respawn;

    public bool invincible;
    public float invincibleTime;

    public Color defaultColor;
    public Color hurtColor;
    public Color invincibleColor;
    public Color chargingColor;
    public Color chargedColor;

    public float speed;
    private float defSpeed;
    //Default speed to be stored when changing speed
    private float moveHorizontal;
    //Walk speed
    public float jumpHeight;
    public float wallJumpHeight;
    public float wallJumpKnock;
    //Knockback from walljump
    public float wallJumpTime;
    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;
    public float gravity;
    public float wallGravity;
    public float drag;
    public float terminalVel;

    //public KeyCode dashCode;		//Store keypress
    private float dashTimeCounter;
    public float dashSpeed;
    public float dashTime;
    //Helps with temporary anti-gravity effect of the air dash
    private bool isDashing;
    public bool lastDir;
    //Last direction. True = right, False = left, default = right
    private bool canTurn;
    private bool canJump;
    private bool canDash;
    //Disable jumping if hit
    private bool onGround;
    private bool onLeftWall;
    private bool onRightWall;

    public float chargeDelay;
    public float chargeTimer;
    public float TimeToCharge;
    public bool fullCharge;
    public bool isCharging;
    private bool canFire;

    public float hitstun;
    public float knockback;

    private Rigidbody2D rb2d;
    private SpriteRenderer sprite;
    public GameObject lemon;
    public GameObject chargeLemon;

    // Use this for initialization
    void Start()
    {
        spawnPos = transform.position;

        health = 30;
        healthText = GameObject.Find("Canvas/Health").GetComponent<Text>();
        respawn = GameObject.Find("Canvas/Respawn");
        respawn.SetActive(false); //Hide respawn button until death.

        invincible = false;

        defSpeed = speed;

        Disabled(false);
        lastDir = true; //Default direction = right
        onGround = true; //Default on the ground
        onLeftWall = false;
        onRightWall = false;

        //Get and store a reference to the Rigidbody2D component so that we can access it.
        rb2d = GetComponent<Rigidbody2D>();
        gravity = rb2d.gravityScale;
        sprite = GetComponent<SpriteRenderer>();
    }


    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        HandleMovement();

        if (canJump)
            Jump();

        //Dash
        if (Input.GetKeyDown(KeyCode.Z) && !isDashing && canDash)
        { //Prevents
            StartCoroutine(Dash());
        }

        if (canFire)
            HandleBlaster();

        HealthUpdate();
    }

    void HealthUpdate()
    {
        healthText.text = "Health: \n" + health + " / 30";

        if (health <= 0 && !undying)
        {
            StartCoroutine(OnDeath());

            //Respawn();
        }
    }

    void canMove(bool y)
    {
        if (y)
        {
            speed = defSpeed;
            canTurn = true;
        }
        else
        {
            speed = 0;
            canTurn = false;
        }
    }

    void Disabled(bool y)
    {
        if (y)
        {
            canMove(false);
            canTurn = false;
            canFire = false;
            canJump = false;
            canDash = false;
            isDashing = false;
            isJumping = false;

        }
        else
        {
            canMove(true);
            canTurn = true;
            canFire = true;
            canJump = true;
            canDash = true;
        }
    }

    void HandleMovement()
    {
        //Orientation (Determines animation and dashing)
        if (Input.GetKey(KeyCode.LeftArrow) && canTurn)
        {
            lastDir = false;
            sprite.flipX = true; //Reflect sprite to face left
        }
        else if (Input.GetKey(KeyCode.RightArrow) && canTurn)
        {
            lastDir = true;
            sprite.flipX = false;
        }
        //Horizontal Movement
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        rb2d.velocity = new Vector2(moveHorizontal * speed, rb2d.velocity.y); //rb2d.velocity.y ensures smooth uninterupted jump
                                                                              //Apply drag
        if (rb2d.velocity.y < terminalVel)
        {
            rb2d.AddForce(Vector2.up * drag);
        }

    }

    void Jump()
    {

        //Short jump/hop and short wall jump
        if (Input.GetKeyDown(KeyCode.UpArrow))
        { //Originally
            if (onGround)
            {
                isJumping = true;
                jumpTimeCounter = jumpTime; //Reset counter on jump
                rb2d.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
            }
            else if (onLeftWall || onRightWall)
            {
                //lastDir = true;
                StartCoroutine(WallJump());
            }
        }
        //Long Jump
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (isJumping && jumpTimeCounter > 0)
            { //Stops when out of jump time
                rb2d.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
                jumpTimeCounter -= Time.deltaTime; //Timer limits how long one can jump
            }
            else
                isJumping = false; //Stop jump

        }
        //Check if jumping
        if (Input.GetKeyUp(KeyCode.UpArrow))  //When key is released, stop jumping immediately
            isJumping = false;


    }

    void HandleBlaster()
    {
        //Normal Shot
        if (Input.GetKeyDown(KeyCode.X))
        {
            chargeTimer = 0f;
            isCharging = false;
            //Check curr weapon
            if (GameObject.FindGameObjectsWithTag("Lemon").Length < 3)
            { //Limit to 3 projectiles on screen at a time
                Fire(lemon);
            }
        }
        //Charge Shot
        if (Input.GetKey(KeyCode.X) && !fullCharge)
        { //Stops counter from possible overflow
            chargeTimer += Time.deltaTime;
            if (chargeTimer >= chargeDelay)
            {
                isCharging = true;
                sprite.color = chargingColor;
            }
            if (chargeTimer >= TimeToCharge)
            {
                fullCharge = true;
                sprite.color = chargedColor;
            }
        }
        if (Input.GetKeyUp(KeyCode.X))
        { //Fire shot if fully charged
            if (fullCharge)
                Fire(chargeLemon);
            else if (chargeTimer >= chargeDelay)
                Fire(lemon);
            fullCharge = false;
            isCharging = false;
            sprite.color = defaultColor;
        }
    }

    void Fire(GameObject projectile)
    {
        //Decide orientation/position of shot
        Vector2 pos;
        if (lastDir)
            pos = new Vector2(transform.position.x + 2, transform.position.y);
        else
            pos = new Vector2(transform.position.x - 2, transform.position.y);
        Instantiate(projectile, pos, projectile.transform.rotation);
    }

    IEnumerator Dash()
    {
        //Prepare dash
        canTurn = false;
        isDashing = true;
        float time = 0f; //Reset timer
        canMove(false); //"Disables" effect of walking temporarily
                        //Air dash
        if (!onGround)
        {
            rb2d.velocity = Vector2.zero;
            rb2d.gravityScale = 0f; //Anti-gravity if in air
        }
        while (time < dashTime && !onLeftWall && !onRightWall) //Cancel dash if a wall is hit.
        {
            if (isJumping)
            { //Enables dash jump
                time = -0.25f;
            }
            if (lastDir)
                rb2d.AddForce(Vector2.right * dashSpeed);
            else
                rb2d.AddForce(Vector2.left * dashSpeed);
            time += Time.deltaTime;
            yield return null; //Advance to next frame
        }
        canMove(true); //Re-enables walking
                       //Check where we are
        if (onLeftWall || onRightWall)
            rb2d.gravityScale = wallGravity; //Reset gravity (in case of air dash landing)
        else
            rb2d.gravityScale = gravity; //Reset gravity (in case of air dash landing)

        canTurn = true;
        yield return new WaitForSeconds(0.3f); //Delay before second button pressed is allowed
        isDashing = false;
    }

    IEnumerator WallJump()
    {
        float time = 0f; //Reset timer
        canMove(false);
        rb2d.velocity = Vector2.zero; //Stop gravity and falling to jump naturally

        while (time < wallJumpTime)
        {
            time += Time.deltaTime;

            if (lastDir)
            {
                rb2d.AddForce(Vector2.left * wallJumpKnock);
                rb2d.AddForce(Vector2.up * wallJumpHeight);
            }
            else
            {
                rb2d.AddForce(Vector2.right * wallJumpKnock);
                rb2d.AddForce(Vector2.up * wallJumpHeight);
            }
            yield return null; //Advance to next frame
        }
        canMove(true);
    }

    IEnumerator DamageKnock(bool invin)
    {
        float time = 0f; //Reset timer
        Disabled(true);
        rb2d.velocity = Vector2.zero; //Stop gravity and falling to jump naturally

        rb2d.AddForce(Vector2.up * knockback / 50, ForceMode2D.Impulse);

        while (time < hitstun)
        {
            time += Time.deltaTime;

            if (lastDir)
            {
                rb2d.AddForce(Vector2.left * knockback);
            }
            else
            {
                rb2d.AddForce(Vector2.right * knockback);
            }
            yield return null; //Advance to next frame
        }
        time = 0f;
        while (time < hitstun - 0.6f)
        {
            time += Time.deltaTime;
            yield return null; //Advance to next frame
        }
        Disabled(false);
        sprite.color = invincibleColor;
        if (invin)
            StartCoroutine(InvincibilityFrames());

    }

    void TakeDamage(int damage, bool knockback)
    {
        if (!invincible)
        {
            sprite.color = hurtColor;
            health -= damage; //Will alter values based on enemy touched //Will also add damage detection to enemy script, not player script
            if (health <= 0)
                health = 0;
            invincible = true;
            StartCoroutine(DamageKnock(true)); //Starts invincibility frames if take damage

        }
        else if (knockback)
            StartCoroutine(DamageKnock(false)); // Knockback, but no invincibility frames
    }

    IEnumerator InvincibilityFrames()
    {
        yield return new WaitForSeconds(invincibleTime);
        invincible = false;
        sprite.color = defaultColor;

    }

    public void Respawn() {
        gameObject.SetActive(true);
        respawn.SetActive(false);
        transform.position = spawnPos;
        Disabled(false);
        sprite.color = defaultColor;
        health = 30;
    }

    IEnumerator OnDeath()
    {

        sprite.color = hurtColor;
        Disabled(true);
        rb2d.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.8f);

        Disabled(false);
        respawn.SetActive(true);
        gameObject.SetActive(true);


    }
    //----------------------------------------------------------------------------------------------------------------

    void OnCollisionEnter2D(Collision2D other)
    {
        EnterLand(other);

        if (other.gameObject.CompareTag("Spikes"))
        {//Will have to differentiate enemy and bullet types for damage sake
            TakeDamage(30, false);
        }
        if (other.gameObject.CompareTag("Small Health"))
        {
            health += 5;
            if (health >= 30)
                health = 30;
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Large Health"))
        {
            health += 10;
            if (health >= 30)
                health = 30;
            Destroy(other.gameObject);
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {//Will have to differentiate enemy and bullet types for damage sake
            TakeDamage(5, true);
        }

        if (other.gameObject.CompareTag("Primate"))
        {//Will have to differentiate enemy and bullet types for damage sake
            TakeDamage(7, false);
        }
        if (other.gameObject.CompareTag("Bad Lemon") && !invincible)
        { //Projectiles do not knock back if damaged
            TakeDamage(5, true);
        }
        if (other.gameObject.CompareTag("Bad Charged") && !invincible)
        { //Projectiles do not knock back if damaged
            TakeDamage(10, true);
        }
        if (other.gameObject.CompareTag("Fists") && !invincible)
        { //Projectiles do not knock back if damaged
            TakeDamage(6, true);
        }
        if (other.gameObject.CompareTag("Checkpoint"))
        { //Projectiles do not knock back if damaged
            spawnPos = other.gameObject.transform.position;
        }


    }

    //When jumping, leaves floor and is disallowed from continuous jumping
    void OnCollisionExit2D(Collision2D other)
    {
        ExitLand(other);
    }

    void EnterLand(Collision2D other)
    {
        //Check land
        if (other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("Spikes"))
            onGround = true;
        if (other.gameObject.CompareTag("LeftWall"))
        {
            onLeftWall = true;
            canDash = false;
            speed = 0.1f; //Prevents getting stuck on walls
            rb2d.gravityScale = wallGravity; //player descends slower on wall
        }
        if (other.gameObject.CompareTag("RightWall"))
        {
            onRightWall = true;
            canDash = false;
            speed = 0.1f; //Prevents getting stuck on walls
            rb2d.gravityScale = wallGravity; //player descends slower on wall
        }
    }

    void ExitLand(Collision2D other)
    {
        if (other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("Spikes"))
            onGround = false;
        if (other.gameObject.CompareTag("LeftWall"))
        {
            onLeftWall = false;
            canDash = true;
            speed = defSpeed; //Prevents getting stuck on walls
            rb2d.gravityScale = gravity; //player falls normally off of wall
        }
        if (other.gameObject.CompareTag("RightWall"))
        {
            onRightWall = false;
            canDash = true;
            speed = defSpeed; //Prevents getting stuck on walls
            rb2d.gravityScale = gravity; //player falls normally off of wall
        }
    }
}
