using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Primate : MonoBehaviour
{
    private Text healthText;

    public bool fistDiag; //Chooses between horizonal and diagonal fist shot
    //Determines which enemy behavior to use.
    public int health;
    private float time;
    public float timeToNext;
    public float angryTimeToNext;

    public Color defColor;
    public Color hurtColor;
    public Color angryColor;

    public bool lastDir;
    public bool canTurn;
    public bool onScreen;
    public float speed;
    public float angrySpeed;

    private Rigidbody2D rb;
    private GameObject player;
    private SpriteRenderer sprite;
    public GameObject fist;

    public float shotSpeed;
    public float angryShotSpeed;

    public float chargeChance;
    public float fistChance;
    public float fistDiagChance;
    public float jumpChance;

    public float jumpHeight;
    public float jumpTime;
    public float jumpDist;

    public float chargeTime;

    public float TimeBtwFists;
    public int numFists;

    // Use this for initialization
    void Start()
    {
        healthText = GameObject.Find("Canvas/Boss Health").GetComponent<Text>();

        time = timeToNext - 1;
        onScreen = false;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();

        //Change shot speed
        //if (enemyName.Equals("AeroOwlman"))
        fist.GetComponent<Fists>().speed = shotSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        CheckOnScreen();
        if (onScreen)
            PrimateBehav();
        else
            rb.velocity = Vector2.zero;
        HealthUpdate();
    }

    void CheckOnScreen()
    {
        //Measure distance between player and enemy to determine if onscreen
        float distanceX = player.transform.position.x - transform.position.x;
        float distanceY = player.transform.position.y - transform.position.y;

        if (Mathf.Abs(distanceX) <= 76f && Mathf.Abs(distanceY) <= 60f) //Creates bounding box
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

    void PrimateBehav()
    {

        if (health < 35)
        {
            defColor = angryColor;
            timeToNext = angryTimeToNext;
        }
        HandleOrientation();
        //Do action every x seconds
        time += Time.deltaTime;
        if (time >= timeToNext)
        {   //After delay, do action
            //Random number gen
            float state = Random.value;
            //Charge
            if (state < chargeChance)
            {
                StartCoroutine(Charge());
            }
            //Jump
            else if (state < jumpChance)
            {
                StartCoroutine(Jump());
            }
            //Fire
            else if (state < fistChance)
            {
                StartCoroutine(FireFists());
            }
            //FireDiag
            time = 0.0f;
        }
    }
    //----------------------------------------------------------------

    IEnumerator Charge()
    {
        float time = 0f;
        while (time < chargeTime)
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

    IEnumerator FireFists()
    {
        canTurn = false;
        int num = 0;
        while (num < numFists)
        {
            Fire(fist);
            num++;
            yield return new WaitForSeconds(TimeBtwFists);
        }
        canTurn = true;
    }

    void Fire(GameObject projectile)
    {
        //Decide orientation/position of shot
        Vector2 pos;

        if (lastDir)
            pos = new Vector2(transform.position.x + 4, transform.position.y);
        else
            pos = new Vector2(transform.position.x - 4, transform.position.y);

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
        health -= damage; //Will alter values based on enemy touched //Will also add damage detection to enemy script, not player script
        if (health <= 0)
            Destroy(gameObject);
        StartCoroutine(HurtColor());
    }

    void HealthUpdate()
    {
        healthText.text = "Boss\nHealth: \n" + health + " / 70";
    }

            IEnumerator HurtColor() {
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
    }

}
