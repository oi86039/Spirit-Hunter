using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadLemon : MonoBehaviour
{
    public float speed;
    public bool lastDir;
    //true = facing right, false = facing left
    private Rigidbody2D rb2d;
    public bool onScreen;
    public string enemyName;

    Enemy enemy;

    GameObject playerObj;
    CompletePlayerController player;

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        enemy = GameObject.FindWithTag("Enemy").GetComponent<Enemy>();
        enemyName = enemy.enemyName;
        onScreen = true;
        playerObj = GameObject.FindWithTag("Player");
        player = playerObj.GetComponent<CompletePlayerController>();
        lastDir = enemy.lastDir;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyName.Equals("AeroOwlman"))
        {
            if (lastDir)
                rb2d.velocity = Vector2.right * speed + Vector2.down * (speed-15f);
            else
                rb2d.velocity = Vector2.left * speed + Vector2.down * (speed-15f);
        }
        else if (enemyName.Equals("Blowfish"))
        {
            rb2d.velocity = Vector2.up * speed;
        }
        else
        {
            if (lastDir)
                rb2d.velocity = Vector2.right * speed;
            else
                rb2d.velocity = Vector2.left * speed;

        }
        CheckOnScreen();
        if (!onScreen)
            Destroy(gameObject);
    }

    void CheckOnScreen()
    {
        //Measure distance between player and enemy to determine if onscreen
        float distanceX = player.transform.position.x - transform.position.x;
        float distanceY = player.transform.position.y - transform.position.y;

        if (Mathf.Abs(distanceX) <= 39f && Mathf.Abs(distanceY) <= 30.5f) //Creates bounding box
            onScreen = true;
        else
            onScreen = false;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        //Check if on screen

        //Check if player is invincible
        if (other.gameObject.CompareTag("Player") && !player.invincible)
        {
            if (lastDir)
                player.lastDir = false; //Knockback player in correct orientation
            else
                player.lastDir = true;
            Destroy(gameObject);
        }
    }

}
