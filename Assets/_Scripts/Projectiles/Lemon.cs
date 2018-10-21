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

		CheckOnScreen ();
		if (!onScreen)
			Destroy (gameObject);
	}

	void CheckOnScreen ()
	{
		//Measure distance between player and enemy to determine if onscreen
		float distanceX = player.transform.position.x - transform.position.x;
		float distanceY = player.transform.position.y - transform.position.y;

		if (Mathf.Abs (distanceX) <= 39f && Mathf.Abs (distanceY) <= 30.5f) //Creates bounding box
			onScreen = true;
		else
			onScreen = false;
	}


	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.CompareTag ("Enemy")) { //Prevents none from being launched

			//If we hit an enemy, Do damage
			Destroy (gameObject);
		}
	}
}
