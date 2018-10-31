using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fists : MonoBehaviour
{
    public float health;
    public float speed;
    public float slowSpeed;
    public bool lastDir;
    //true = facing right, false = facing left
    private Rigidbody2D rb2d;
    private SpriteRenderer sprite;

    public Color defColor;
    public Color hurtColor;

    public float fistTime;
    float time = 0;

    public float range;

    Primate primate;

    GameObject playerObj;
    CompletePlayerController player;

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        primate = GameObject.FindWithTag("Enemy").GetComponent<Primate>();
        playerObj = GameObject.FindWithTag("Player");
        player = playerObj.GetComponent<CompletePlayerController>();
        lastDir = primate.lastDir;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        //Fist is destroyed when time runs out
        if (time <= fistTime)
        {
            if (lastDir)
                    rb2d.velocity = Vector2.right * speed;
                else
                    rb2d.velocity = Vector2.left * speed;
            
            //boomerang!!!
            float distX = transform.position.x - primate.transform.position.x; //Distance between primate and fist (negative = left, positive = right)

            //normal horizontal fist reflect
            if (lastDir && distX > range)
            {
                lastDir = false;
            }
            else if (!lastDir && distX < -range)
            {
                lastDir = true;
            }
        }
        else
            Destroy(gameObject);
        
    }


    void TakeDamage(int damage)
    {
        health -= damage; //Will alter values based on enemy touched //Will also add damage detection to enemy script, not player script
        if (health <= 0)
            Destroy(gameObject);
        StartCoroutine(HurtColor());
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
        //Check if player is invincible
        if (other.gameObject.CompareTag("Player") && !player.invincible)
        {
            if (lastDir)
                player.lastDir = false; //Knockback player in correct orientation
            else
                player.lastDir = true;
        }
    }

}
