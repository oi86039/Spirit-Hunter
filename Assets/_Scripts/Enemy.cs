using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //---------------------------------------------
    //All Enemies
    //---------------------------------------------

    public string enemyName;
    //Determines which enemy behavior to use.
    public int health;
    private float time;
    public float timeToNext;

    public Color defColor;
    public Color hurtColor;

    public bool onWall;
    public float sineMag;

    private Vector3 pos;

    public bool lastDir;
    public bool canTurn;
    public bool onScreen;
    public float speed;

    private Rigidbody2D rb;
    private GameObject player;
    private SpriteRenderer sprite;
    public GameObject badLemonL;
    public GameObject badLemonR;
    public GameObject smallDrop;
    public GameObject bigDrop;

    //public float shotSpeed;

    public float walkChance;
    public float jumpChance;

    public float jumpHeight;
    public float jumpTime;
    public float jumpDist;

    public float walkTime;
    public float TimeBtwShots;
    public int numShots;

    // Use this for initialization
    void Start()
    {
        pos = transform.position;

        time = timeToNext - 0.3f;
        onScreen = false;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();

        //Change shot speed
        //if (enemyName.Equals("AeroOwlman"))
        //badLemon.GetComponent<BadLemon>().speed = shotSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        CheckOnScreen();
        if (onScreen)
        { //Select enemy behavior
            if (enemyName.Equals("Owlman"))
                Owlman();
            else if (enemyName.Equals("Ninjaman"))
                Ninjaman();
            else if (enemyName.Equals("AeroOwlman"))
                AeroOwlman();
            else if (enemyName.Equals("Heavyman"))
                Heavyman();
            else if (enemyName.Equals("Roller"))
                Roller();
            else if (enemyName.Equals("Blowfish"))
                Blowfish();
            else if (enemyName.Equals("Mask"))
                Mask();
            else if (enemyName.Equals("WallTurretLeft"))
                WallTurretLeft();
            else if (enemyName.Equals("WallTurretRight"))
                WallTurretRight();
        }
        else
            rb.velocity = Vector2.zero;
        CheckDespawn();
    }

    void CheckOnScreen()
    {
        //Measure distance between player and enemy to determine if onscreen
        float distanceX = player.transform.position.x - transform.position.x;
        float distanceY = player.transform.position.y - transform.position.y;

        if (Mathf.Abs(distanceX) <= 38f && Mathf.Abs(distanceY) <= 30.5f) //Creates bounding box
            onScreen = true;
        else
            onScreen = false;
    }

    void HandleOrientation()
    {
        if (canTurn)
        {
            //Orientation (Determines animation and dashing)
            if (player.transform.position.x <= transform.position.x)
            { //If player is to left of enemy
                lastDir = false;
                sprite.flipX = true; //Reflect sprite to face left
            }
            else
            {
                lastDir = true;
                sprite.flipX = false;
            }
        }
    }

    void CheckDespawn()
    {
        //Measure distance between player and enemy to determine if onscreen
        float distanceX = player.transform.position.x - transform.position.x;

        if (distanceX >= 200f) //Creates bounding box
            Destroy(gameObject);
    }

    //-----------------Owlman Aeroman and Heavyman share AI--------------------

    void Owlman()
    {
        HandleOrientation();
        //Do action every x seconds
        time += Time.deltaTime;
        if (time >= timeToNext)
        {   //After delay, do action
            //Random number gen
            float state = Random.value;
            if (state < walkChance)
            {
                StartCoroutine(WalkForward());
            }
            else
            {
                StartCoroutine(Blaster());
            }
            time = 0.0f;
        }
    }

    void AeroOwlman()
    {
        //Track player y position and follow it
        float distX = player.transform.position.x - transform.position.x;

        //Track player y position and follow it
        if (distX < -38f) //offscreen
            rb.velocity = Vector2.zero;
        if (distX < -15f)
            rb.velocity = (Vector2.left * (speed));
        else if (distX < 0) //If player is left of enemy
            rb.velocity = (Vector2.right * (speed));
        else if (distX > 15f)
            rb.velocity = (Vector2.left * (speed));
        else if (distX > 0) //If player is right of enemy
            rb.velocity = (Vector2.right * (speed));
        else
            rb.velocity = Vector2.zero;

        HandleOrientation();
        //Do action every x seconds
        time += Time.deltaTime;
        if (time >= timeToNext)
        {
            float state = Random.value;
            if (state < walkChance)
            {
                StartCoroutine(Blaster());
            }
            time = 0.0f;
        }
    }

    void Heavyman()
    {
        HandleOrientation();
        //Do action every x seconds
        time += Time.deltaTime;
        if (time >= timeToNext)
        {   //After delay, do action
            //Random number gen
            float state = Random.value;
            if (state < walkChance)
            {
                StartCoroutine(WalkForward());
            }
            else
            {
                StartCoroutine(Blaster());
            }
            time = 0.0f;
        }
    }

    void Ninjaman()
    {
        HandleOrientation();
        //Do action every x seconds
        time += Time.deltaTime;
        if (time >= timeToNext)
        {   //After delay, do action
            //Random number gen
            float state = Random.value;
            if (state < walkChance)
            {
                StartCoroutine(WalkForward());
            }
            else if (state < jumpChance)
            {
                StartCoroutine(Jump());
            }
            else
            {
                StartCoroutine(Blaster());
            }
            time = 0.0f;
        }
    }

    void Blowfish()
    {    
        //Fire 2s, wait 2s, repeat
        time += Time.deltaTime;
        if (time >= timeToNext)
        {
            StartCoroutine(Blaster());
            time = 0.0f;
        }
    }

    void Roller()
    {
        if (lastDir)
        { // facing right
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        }
    }

    void Mask()
    {
        if (lastDir)
            pos += transform.right * Time.deltaTime * speed;
        else
            pos += transform.right * Time.deltaTime * -speed;

        transform.position = pos + transform.up * Mathf.Sin(Time.time * 4) * sineMag;
    }

    void WallTurretLeft() //Facing left
    {
        lastDir = false;
        canTurn = false; //Wall Turrets cannot turn

        //Track player y position and follow it
        float distY = player.transform.position.y - transform.position.y;

        if (onWall)
        {
            //Track player y position and follow it
            if (distY < 0) //If player is below turret
                rb.velocity = (Vector2.down * speed);
            else if (distY > 0)
                rb.velocity = (Vector2.up * speed);
            else
                rb.velocity = Vector2.zero;
        }

        else
        { //Do the opposite until on the wall
            if (distY > 0) //If player is below turret
                rb.velocity = (Vector2.down * speed);
            else if (distY < 0)
                rb.velocity = (Vector2.up * speed);
            else
                rb.velocity = Vector2.zero;
        }
        //Do action every x seconds
        time += Time.deltaTime;
        if (time >= timeToNext)
        {   //After delay, do action
            StartCoroutine(Blaster());
            time = 0.0f;
        }
    }
    void WallTurretRight()
    {
        lastDir = true;
        canTurn = false; //Wall Turrets cannot turn

        //Track player y position and follow it
        float distY = player.transform.position.y - transform.position.y;

        if (onWall)
        {
            //Track player y position and follow it
            if (distY < 0) //If player is below turret
                rb.velocity = (Vector2.down * speed);
            else if (distY > 0)
                rb.velocity = (Vector2.up * speed);
            else
                rb.velocity = Vector2.zero;
        }

        else
        { //Do the opposite until on the wall
            if (distY > 0) //If player is below turret
                rb.velocity = (Vector2.down * speed);
            else if (distY < 0)
                rb.velocity = (Vector2.up * speed);
            else
                rb.velocity = Vector2.zero;
        }
        //Do action every x seconds
        time += Time.deltaTime;
        if (time >= timeToNext)
        {   //After delay, do action
            StartCoroutine(Blaster());
            time = 0.0f;
        }
    }

    //----------------------------------------------------------------

    IEnumerator WalkForward()
    {
        float time = 0f;
        while (time < walkTime)
        {
            if (lastDir)
            { // facing right
                rb.velocity = Vector2.right * speed;
            }
            else
            {
                rb.velocity = Vector2.left * speed;
            }
            time += Time.deltaTime;
            yield return null;
        }
        //Delay before next action
        rb.velocity = Vector2.zero;
    }

    IEnumerator Blaster()
    {
        canTurn = false;
        int num = 0;
        while (num < numShots)
        {
            if (lastDir)
                Fire(badLemonR);
            else
                Fire(badLemonL);
            num++;
            yield return new WaitForSeconds(TimeBtwShots);
        }
        canTurn = true;
    }

    void Fire(GameObject projectile)
    {
        //Decide orientation/position of shot
        Vector2 pos;

        if (enemyName.Equals("Blowfish"))
            pos = new Vector2(transform.position.x, transform.position.y + 4);

        else if (lastDir)
        {
            if (enemyName.Equals("AeroOwlman"))
                pos = new Vector2(transform.position.x + 4, transform.position.y - 4);
            else
                pos = new Vector2(transform.position.x + 4, transform.position.y);
        }
        else
        {
            if (enemyName.Equals("AeroOwlman"))
                pos = new Vector2(transform.position.x - 4, transform.position.y - 4);
            else
                pos = new Vector2(transform.position.x - 4, transform.position.y);
        }
        //Fire projectile
        Instantiate(projectile, pos, projectile.transform.rotation);
    }

    IEnumerator Jump()
    {
        float jtime = 0f; //Reset timer
                          //rb.velocity = Vector2.zero; //Stop gravity and falling to jump naturally

        while (jtime < jumpTime)
        {
            jtime += Time.deltaTime;
            rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
            if (!lastDir)
            {
                rb.AddForce(Vector2.left * jumpDist);
            }
            else
            {
                rb.AddForce(Vector2.right * jumpDist);
            }
            yield return null; //Advance to next frame
        }
    }

    void TakeDamage(int damage)
    {
        StartCoroutine(HurtColor());
        health -= damage; //Will alter values based on enemy touched //Will also add damage detection to enemy script, not player script
        if (health <= 0) {
            float state = Random.value;
            if (state < 0.3)
                Instantiate(smallDrop,transform.position,smallDrop.transform.rotation);
            else if (state < 0.2)
                Instantiate(bigDrop, transform.position, bigDrop.transform.rotation);
            Destroy(gameObject);
    }
    }

    IEnumerator HurtColor()
    {
        sprite.color = hurtColor;
        yield return new WaitForSeconds(0.1f);
        sprite.color = defColor;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Lemon"))
        {
            TakeDamage(1);
        }
        if (other.gameObject.CompareTag("Charged"))
        {
            TakeDamage(10);
        }

        if (enemyName.Equals("Roller"))
        {
            if (other.gameObject.CompareTag("LeftWall"))
                lastDir = false;
            else if (other.gameObject.CompareTag("RightWall"))
                lastDir = true;
            else if (other.gameObject.CompareTag("Wall")) //Non walljump wall
                lastDir = true;
        }

        if (enemyName.Equals("WallTurret"))
        {
            if (other.gameObject.CompareTag("LeftWall"))
            {
                lastDir = false;
                onWall = true;
            }
            else if (other.gameObject.CompareTag("RightWall"))
            {
                lastDir = true;
                onWall = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Spikes") && !enemyName.Equals("Mask"))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (enemyName.Equals("WallTurret"))
        {
            if (other.gameObject.CompareTag("LeftWall"))
            {
                onWall = false;
                rb.velocity = Vector2.zero;
            }
            else if (other.gameObject.CompareTag("RightWall"))
            {
                onWall = false;
                rb.velocity = Vector2.zero;

            }
        }
    }


}
