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

	Owlman e;
	Ninjaman e1;
	Aeroman e2;
	Heavyman e3;
	Extendo e4;
	WallTurret e5;

	GameObject playerObj;
	CompletePlayerController player;

	// Use this for initialization
	void Start ()
	{
		rb2d = GetComponent<Rigidbody2D> ();
		FindEnemy ();
		onScreen = true;
		playerObj = GameObject.FindWithTag ("Player");
		player = playerObj.GetComponent<CompletePlayerController> ();
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
		//Check if on screen

		//Check if player is invincible
		if (other.gameObject.CompareTag ("Player") && !player.invincible) {
			if (lastDir)
				player.lastDir = false; //Knockback player in correct orientation
			else
				player.lastDir = true;
			Destroy (gameObject);
		}
	}

	void FindEnemy ()
	{
		if (enemyName.Equals ("Owlman")) {
			e = GameObject.FindWithTag ("Enemy").GetComponent<Owlman> ();
			lastDir = e.lastDir;
		} else if (enemyName.Equals ("Ninjaman")) {
			e1 = GameObject.FindWithTag ("Enemy").GetComponent<Ninjaman> ();
			lastDir = e1.lastDir;
		} else if (enemyName.Equals ("Aeroman")) {
			e2 = GameObject.FindWithTag ("Enemy").GetComponent<Aeroman> ();
			lastDir = e2.lastDir;
		} else if (enemyName.Equals ("Heavyman")) {
			e3 = GameObject.FindWithTag ("Enemy").GetComponent<Heavyman> ();
			lastDir = e3.lastDir;
		} else if (enemyName.Equals ("Extendo")) {
			e4 = GameObject.FindWithTag ("Enemy").GetComponent<Extendo> ();
			lastDir = e4.lastDir;
		} else if (enemyName.Equals ("WallTurret")) {
			e5 = GameObject.FindWithTag ("Enemy").GetComponent<WallTurret> ();
			lastDir = e5.lastDir;
		}
	}
}
