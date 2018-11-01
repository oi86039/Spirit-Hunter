using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lemon : MonoBehaviour
{
	public float speed;
	public bool lastDir;
	public bool onScreen;
	//true = facing right, false = facing left
	private Rigidbody2D rb2d;

	GameObject playerObj;
	CompletePlayerController player;

	// Use this for initialization
	void Start ()
	{
		onScreen = true;
		rb2d = GetComponent<Rigidbody2D> ();
		playerObj = GameObject.FindWithTag ("Player");
		player = playerObj.GetComponent<CompletePlayerController> ();
		lastDir = player.lastDir;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (lastDir)
			rb2d.velocity = Vector2.right * speed;
		else
			rb2d.velocity = Vector2.left * speed;

		
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.CompareTag ("Enemy")) { //Prevents none from being launched

			//If we hit an enemy, Do damage
			Destroy (gameObject);
		}
        if (other.gameObject.CompareTag("Fists"))
        {
            //If we hit an enemy, Do damage
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("MainCamera"))
            Destroy(gameObject);

        //if (other.gameObject.CompareTag("LeftWall") || other.gameObject.CompareTag("RightWall"))
          //  Destroy(gameObject);
    }

}
