using UnityEngine;
using System.Collections;

public class CompletePlayerController : MonoBehaviour
{
	//Floating point variable to store the player's movement speed.
	public float speed;
	public float jumpHeight;
	public float dashSpeed;
	//public float dashTimer;	//Helps with temporary anti-gravity effect of the air dash
	public float gravity;

	public bool lastDir;
	//Last direction. True = right, False = left, default = right

	public bool onGround;
	public bool onLeftWall;
	public bool onRightWall;

	//Store a reference to the Rigidbody2D component required to use 2D Physics.
	private Rigidbody2D rb2d;
	private SpriteRenderer sprite;
	public GameObject lemonL;
	public GameObject lemonR;
   
	// Use this for initialization
	void Start ()
	{
		lastDir = true; //Default direction = right
		onGround = true; //Default on the ground
		onLeftWall = false;
		onRightWall = false;

		//Get and store a reference to the Rigidbody2D component so that we can access it.
		rb2d = GetComponent<Rigidbody2D> ();
		sprite = GetComponent<SpriteRenderer> ();
	}

	//FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
	void FixedUpdate ()
	{
		//Orientation (Determines animation and dashing)
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			lastDir = false;
			sprite.flipX = true; //Reflect sprite to face left
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			lastDir = true;
			sprite.flipX = false;
		}

		//Movement (Left and Right)
		float moveHorizontal = Input.GetAxis ("Horizontal");
		Vector2 movement = new Vector2 (moveHorizontal, 0);
		rb2d.velocity = (movement * speed);

		//Jumping and Wall Jumping
		if (Input.GetKey (KeyCode.UpArrow)) {
			//if(onGround)
				rb2d.AddForce (Vector2.up * jumpHeight, ForceMode2D.Impulse);
		//Wall-Jumping
			 if (onLeftWall) {
				//lastDir = true;
				rb2d.AddForce (Vector2.right * 0.5f * jumpHeight, ForceMode2D.Impulse);
				rb2d.AddForce (Vector2.up * jumpHeight, ForceMode2D.Impulse);
			} else if (onRightWall) {
				//lastDir = false;
				rb2d.AddForce (Vector2.left * 0.5f * jumpHeight, ForceMode2D.Impulse);
				rb2d.AddForce (Vector2.up * jumpHeight, ForceMode2D.Impulse);
			}
		}
		//Dashing (Using Z)
		//if (onGround && Input.GetKeyDown (KeyCode.Z)) { //Just for Ground dashing
		if (Input.GetKey (KeyCode.Z)) {
			if (lastDir) //if facing right
				rb2d.AddForce (Vector2.right * dashSpeed, ForceMode2D.Impulse);
			else
				rb2d.AddForce (Vector2.left * dashSpeed, ForceMode2D.Impulse);
		}

		/*
		//Air Dashing
		//Dashing (Using Z)
		if (!onGround && Input.GetKeyDown (KeyCode.Z)) {
			if (lastDir)
				rb2d.AddForce (Vector2.right * dashSpeed, ForceMode2D.Impulse);
			else
				rb2d.AddForce (Vector2.left * dashSpeed, ForceMode2D.Impulse);
		}
		*/

		//Blasting
		if (Input.GetKeyDown (KeyCode.X)) {
			if (lastDir) //if facing right
				Instantiate(lemonR, transform.position, lemonR.transform.rotation);
			else
				Instantiate(lemonL, transform.position, lemonL.transform.rotation);
		}
	}
	//When on the ground, jumping is allowed
	void OnCollisionEnter2D (Collision2D other)
	{
		if (other.gameObject.CompareTag ("Floor"))
			onGround = true;
		if (other.gameObject.CompareTag ("LeftWall")) {
			onLeftWall = true;
			rb2d.gravityScale = 0.75f * gravity; //player descends slower on wall
		}
		if (other.gameObject.CompareTag ("RightWall")) {
			onRightWall = true;
			rb2d.gravityScale = 0.75f * gravity; //player descends slower on wall
		}
	}

	//When jumping, leaves floor and is disallowed from continuous jumping
	void OnCollisionExit2D (Collision2D other)
	{
		if (other.gameObject.CompareTag ("Floor"))
			onGround = false;
		if (other.gameObject.CompareTag ("LeftWall")) {
			onLeftWall = false;
			rb2d.gravityScale = gravity; //player falls normally off of wall
		}
		if (other.gameObject.CompareTag ("RightWall")) {
			onRightWall = false;
			rb2d.gravityScale = gravity; //player falls normally off of wall
		}
	}
}
