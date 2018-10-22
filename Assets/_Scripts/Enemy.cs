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

	public bool lastDir;
    public bool canTurn;
	public bool onScreen;
	public float speed;

	private Rigidbody2D rb;
	private GameObject player;
	private SpriteRenderer sprite;
	public GameObject badLemon;

	public float shotSpeed;

	public float walkChance;
	public float jumpChance;
	public float backChance;

	public float jumpHeight;
	public float jumpTime;
	public float jumpDist;

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
        if (enemyName.Equals("AeroOwlman"))
		badLemon.GetComponent<BadLemon>().speed = shotSpeed;
	}

	// Update is called once per frame
	void Update ()
	{
		CheckOnScreen ();
		if (onScreen) { //Select enemy behavior
			if (enemyName.Equals ("Owlman"))
				Owlman ();
			else if (enemyName.Equals ("Ninjaman"))
				Ninjaman ();
			else if (enemyName.Equals ("AeroOwlman"))
				AeroOwlman ();
			else if (enemyName.Equals ("Heavyman"))
				Heavyman ();
			else if (enemyName.Equals ("Roller"))
				Roller ();
			else if (enemyName.Equals ("Extendo"))
				Extendo ();
			else if (enemyName.Equals ("Mask"))
				Mask ();
			else if (enemyName.Equals ("WallTurret"))
				WallTurret ();
		}
        CheckDespawn();
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

    //------------------------------------------
    //Owlman
    //------------------------------------------

    void Owlman ()
	{
		HandleOrientation ();
		//Do action every x seconds
		time += Time.deltaTime;
		if (time >= timeToNext) {	//After delay, do action
			//Random number gen
			float state = Random.value;
			if (state < walkChance) {
				StartCoroutine (WalkForward (true));
			} else {
				StartCoroutine (Blaster ());
			}
			time = 0.0f;
		}
	}

	//------------------------------------------
	//Ninjaman
	//------------------------------------------

	void Ninjaman ()
	{
		HandleOrientation ();
		//Do action every x seconds
		time += Time.deltaTime;
		if (time >= timeToNext) {	//After delay, do action
			//Random number gen
			float state = Random.value;
			if (state < walkChance) {
				StartCoroutine (WalkForward (true));
			}
			else if (state < jumpChance) {
				StartCoroutine (Jump ());
			} else {
				StartCoroutine (Blaster ());
			}
			time = 0.0f;
		}
	}

	void AeroOwlman ()
	{
		HandleOrientation ();
		//Do action every x seconds
		time += Time.deltaTime;
		if (time >= timeToNext) {	//After delay, do action
			//Random number gen
			float state = Random.value;
			if (state < walkChance) {
				StartCoroutine (WalkForward (true));
			} else if (state < backChance) {
				StartCoroutine (WalkBackward ());
			} else {
				StartCoroutine (Blaster ());
			}
			time = 0.0f;
		}
	}

	void Heavyman ()
	{
	}

	void Roller ()
	{
        if (lastDir)
        { // facing right
            rb.velocity = Vector2.right * speed;
        }
        else
        {
            rb.velocity = Vector2.left * speed;
        }
    }

	void Mask ()
	{
	}

	void Extendo ()
	{
	}

	void WallTurret ()
	{
	}

	IEnumerator WalkForward (bool stop)
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
        if(stop)
		rb.velocity = Vector2.zero;
	}

	IEnumerator WalkBackward ()
	{
		float time = 0f;
		while (time < walkTime) {
			if (lastDir) { // facing right
				rb.velocity = Vector2.left * speed;
			} else {
				rb.velocity = Vector2.right * speed;
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
        while (num < numShots)
        {
            Fire(badLemon);
            num++;
            yield return new WaitForSeconds(TimeBtwShots);
        }
        canTurn = true;
	}

	void Fire (GameObject projectile)
	{
		//Decide orientation/position of shot
		Vector2 pos;
		if (lastDir) {
			if (enemyName.Equals ("AeroOwlman"))
				pos = new Vector2 (transform.position.x + 4, transform.position.y - 4);
			else
				pos = new Vector2 (transform.position.x + 4, transform.position.y);
		} else {
			if (enemyName.Equals ("AeroOwlman"))
				pos = new Vector2 (transform.position.x - 4, transform.position.y - 4);
			else
				pos = new Vector2 (transform.position.x - 4, transform.position.y);
		}
        if (enemyName.Equals("AeroOwlman"))
            Instantiate (projectile, pos, projectile.transform.rotation);
        else
            Instantiate(projectile, pos, projectile.transform.rotation);
    }

    IEnumerator Jump ()
	{
		float jtime = 0f; //Reset timer
		//rb.velocity = Vector2.zero; //Stop gravity and falling to jump naturally

		while (jtime < jumpTime) {
			jtime += Time.deltaTime;
            rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
            if (!lastDir) {
				rb.AddForce (Vector2.left * jumpDist);
			} else {
				rb.AddForce (Vector2.right * jumpDist);
			}
			yield return null; //Advance to next frame
		}
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

        if (enemyName.Equals("Roller")) {
            if (other.gameObject.CompareTag("LeftWall"))
                lastDir = true;
            if (other.gameObject.CompareTag("RightWall"))
                lastDir = false;
        }
	}

	void TakeDamage (int damage)
	{
		health -= damage; //Will alter values based on enemy touched //Will also add damage detection to enemy script, not player script
		if (health <= 0)
			Destroy (gameObject);
	}
}
