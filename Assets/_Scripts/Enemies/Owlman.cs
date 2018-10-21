using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Owlman : MonoBehaviour
{
	public int health;
	private float time;
	public float timeToNext;

	public bool lastDir;
	public bool canTurn;
	public bool onScreen;
	public float speed;

	public float shotSpeed;

	private Rigidbody2D rb;
	private GameObject player;
	private SpriteRenderer sprite;
	public GameObject badLemon;

	public float walkChance;

	public float walkTime;
	public float TimeBtwShots;
	public int numShots;


	// Use this for initialization
	void Start ()
	{
		time = timeToNext - 1;
		onScreen = false;
		rb = GetComponent<Rigidbody2D> ();
		player = GameObject.FindGameObjectWithTag ("Player"); 
		sprite = GetComponent<SpriteRenderer> ();

		//Change shot speed
		badLemon.GetComponent<BadLemon>().enemyName = "Owlman";
		badLemon.GetComponent<BadLemon>().speed = shotSpeed;
	}

	// Update is called once per frame
	void Update ()
	{
		CheckOnScreen ();
		if (onScreen) { //Select enemy behavior
			if(canTurn)
			HandleOrientation ();
			//Do action every x seconds
			time += Time.deltaTime;
			if (time >= timeToNext) {	//After delay, do action
				//Random number gen
				float state = Random.value;
				if (state < walkChance) {
					StartCoroutine (WalkForward ());
				} else {
					StartCoroutine (Blaster ());
				}
				time = 0.0f;
			}
		}
	}

	void CheckOnScreen ()
	{
		//Measure distance between player and enemy to determine if onscreen
		float distanceX = player.transform.position.x - transform.position.x;
		float distanceY = player.transform.position.y - transform.position.y;

		if (Mathf.Abs (distanceX) <= 38f && Mathf.Abs (distanceY) <= 30.5f) //Creates bounding box
			onScreen = true;
		else
			onScreen = false;
	}

	void HandleOrientation ()
	{
		//Orientation (Determines animation and dashing)
		if (player.transform.position.x <= transform.position.x) { //If player is to left of enemy
			lastDir = false;
			sprite.flipX = true; //Reflect sprite to face left
		} else {
			lastDir = true;
			sprite.flipX = false;
		}
	}

	IEnumerator WalkForward ()
	{
		float time = 0f;
		while (time < walkTime) {
			if (lastDir) { // facing right
				rb.velocity = Vector2.right * speed;
			} else {
				rb.velocity = Vector2.left * speed;
			}
			time += Time.deltaTime;
			yield return null;
		}
		//Delay before next action
		rb.velocity = Vector2.zero;
	}
		
	IEnumerator Blaster ()
	{
		canTurn = false;
		int num = 0;
		while (num < numShots) {
			Fire (badLemon);
			num++;
			yield return new WaitForSeconds (TimeBtwShots);
		}
		canTurn = true;
	}

	void Fire (GameObject projectile)
	{
		//Decide orientation/position of shot
		Vector2 pos;
		if (lastDir) {
			pos = new Vector2 (transform.position.x + 4, transform.position.y);
		} else {
			pos = new Vector2 (transform.position.x - 4, transform.position.y);
		}
		Instantiate (projectile, pos, projectile.transform.rotation);
	}
		
	//--------------------------------------------------------------------
	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.CompareTag ("Lemon")) {
			TakeDamage (1);
		}
		if (other.gameObject.CompareTag ("Charged")) {
			TakeDamage (10);
		}
	}

	void TakeDamage (int damage)
	{
		health -= damage; //Will alter values based on enemy touched //Will also add damage detection to enemy script, not player script
		if (health <= 0)
			Destroy (gameObject);
	}
}
