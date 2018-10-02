using UnityEngine;
using System.Collections;

public class CompletePlayerController : MonoBehaviour
{
	public float speed;
	//Walk speed
	public float jumpHeight;

	private float jumpTimeCounter;
	public float jumpTime;

	public float dashSpeed;
	//public float dashTimer;	//Helps with temporary anti-gravity effect of the air dash

	public bool lastDir;
	//Last direction. True = right, False = left, default = right

	public bool onGround;
	public bool onLeftWall;
	public bool onRightWall;
	public bool isJumping;
	//If holding up arrow.

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
		float moveHorizontal = Input.GetAxisRaw ("Horizontal");
		rb2d.velocity = new Vector2 (moveHorizontal * speed, rb2d.velocity.y); //rb2d.velocity.y ensures smooth uninterupted jump

		//Jumping and Wall Jumping
		if (Input.GetKeyDown (KeyCode.UpArrow)) { //Short hop and short wall jump
			if (onGround) {
				isJumping = true;
				jumpTimeCounter = jumpTime; //Reset counter on jump
				rb2d.AddForce (Vector2.up * jumpHeight, ForceMode2D.Impulse);
			}
			//Wall-Jumping
			if (onLeftWall) {
				//lastDir = true;
				rb2d.velocity = Vector2.zero; //Cancel out gravity
				rb2d.AddForce (Vector2.right * 2 * jumpHeight, ForceMode2D.Impulse); //Bounceback from wall.
				rb2d.AddForce (Vector2.up * 0.75f * jumpHeight, ForceMode2D.Impulse);
			} else if (onRightWall) {
				//lastDir = false;
				rb2d.velocity = Vector2.zero;
				rb2d.AddForce (Vector2.left * 2 * jumpHeight, ForceMode2D.Impulse);
				rb2d.AddForce (Vector2.up * 0.75f * jumpHeight, ForceMode2D.Impulse);
			}
		}
		if (Input.GetKey (KeyCode.UpArrow) && isJumping) { //Long Jump and Wall-Eject
			if (jumpTimeCounter > 0) { //Stops when out of jump time
				rb2d.AddForce (Vector2.up * jumpHeight, ForceMode2D.Impulse);
				jumpTimeCounter -= Time.deltaTime; //Timer limits how long one can jump
			} else {isJumping = false;}
			//Wall-Jumping
			if (onLeftWall) {
				//lastDir = true;
				rb2d.velocity = Vector2.zero; //Cancel out gravity
				rb2d.AddForce (Vector2.right * 2 * jumpHeight, ForceMode2D.Impulse); //Bounceback from wall.
				rb2d.AddForce (Vector2.up * 0.75f * jumpHeight, ForceMode2D.Impulse);
			} else if (onRightWall) {
				//lastDir = false;
				rb2d.velocity = Vector2.zero;
				rb2d.AddForce (Vector2.left * 2 * jumpHeight, ForceMode2D.Impulse);
				rb2d.AddForce (Vector2.up * 0.75f * jumpHeight, ForceMode2D.Impulse);
			}
		}
		if(Input.GetKeyUp(KeyCode.UpArrow)){
			isJumping = false;
		}
		//Dashing (Using Z)
		if (Input.GetKey (KeyCode.Z)) {
			if (lastDir) //if facing right
				rb2d.AddForce (Vector2.right * dashSpeed, ForceMode2D.Impulse);
			else
				rb2d.AddForce (Vector2.left * dashSpeed, ForceMode2D.Impulse);
		}
	
		//Blasting
		if (Input.GetKeyDown (KeyCode.X)) {
			if (lastDir) //if facing right
				Instantiate (lemonR, new Vector2 (transform.position.x + 2, transform.position.y), lemonR.transform.rotation);
			else
				Instantiate (lemonL, new Vector2 (transform.position.x - 2, transform.position.y), lemonL.transform.rotation);
		}
	}

	//When on the ground, jumping is allowed
	void OnCollisionEnter2D (Collision2D other)
	{
		if (other.gameObject.CompareTag ("Floor"))
			onGround = true;
		if (other.gameObject.CompareTag ("LeftWall")) {
			onLeftWall = true;
			speed /= 5; //Prevents getting stuck on walls
			rb2d.gravityScale *= 0.40f; //player descends slower on wall
		}
		if (other.gameObject.CompareTag ("RightWall")) {
			onRightWall = true;
			speed /= 5; //Prevents getting stuck on walls
			rb2d.gravityScale *= 0.40f; //player descends slower on wall
		}
	}

	//When jumping, leaves floor and is disallowed from continuous jumping
	void OnCollisionExit2D (Collision2D other)
	{
		if (other.gameObject.CompareTag ("Floor"))
			onGround = false;
			
		if (other.gameObject.CompareTag ("LeftWall")) {
			onLeftWall = false;
			speed *= 5; //Prevents getting stuck on walls
			rb2d.gravityScale /= 0.40f; //player falls normally off of wall
		}
		if (other.gameObject.CompareTag ("RightWall")) {
			onRightWall = false;
			speed *= 5; //Prevents getting stuck on walls
			rb2d.gravityScale /= 0.40f; //player falls normally off of wall
		}
	}
}
