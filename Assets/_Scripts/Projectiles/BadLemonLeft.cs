using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadLemonLeft : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb2d;
    public bool onScreen;

    GameObject playerObj;
    CompletePlayerController player;

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        onScreen = true;
        playerObj = GameObject.FindWithTag("Player");
        player = playerObj.GetComponent<CompletePlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.velocity = Vector2.left * speed;
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
        //Check if player is invincible
        if (other.gameObject.CompareTag("Player") && !player.invincible)
        {
            player.lastDir = true; //Knockback player in correct orientation
            Destroy(gameObject);
        }
    }

}
