using UnityEngine;
using System.Collections;

public class CompletePlayerController : MonoBehaviour
{
	public float speed;
	//Walk speed
	public float jumpHeight;
	public float wallJumpHeight;
	public float wallJumpKnock;
	public float wallJumpTime;
	private float jumpTimeCounter;
	public float jumpTime;
	public bool isJumping;
	public float gravity;
	public float wallGravity;
	public float drag;
	public float terminalVel;

	//public KeyCode dashCode;		//Store keypress
	private float dashTimeCounter;
	public float dashSpeed;
	public float dashTime;
	//Helps with temporary anti-gravity effect of the air dash
	public bool isDashing;
	public bool lastDir;
	//Last direction. True = right, False = left, default = right
	public bool canTurn;
	public bool onGround;
	public bool onLeftWall;
	public bool onRightWall;

	private Rigidbody2D rb2d;
	private SpriteRenderer sprite;
	public GameObject lemon;
	// Use this for initialization
	void Start ()
	{
		canTurn = true;
		lastDir = true; //Default direction = right
		onGround = true; //Default on the ground
		onLeftWall = false;
		onRightWall = false;

		//Get and store a reference to the Rigidbody2D component so that we can access it.
		rb2d = GetComponent<Rigidbody2D> ();
		gravity = rb2d.gravityScale;
		sprite = GetComponent<SpriteRenderer> ();
	}


	//FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
	void FixedUpdate ()
	{
//////////////////////////////////////////////////////
//Horizontal Movement
//////////////////////////////////////////////////////

		//Orientation (Determines animation and dashing)
		if (Input.GetKey (KeyCode.LeftArrow) && canTurn) {
			lastDir = false;
			sprite.flipX = true; //Reflect sprite to face left
		} else if (Input.GetKey (KeyCode.RightArrow) && canTurn) {
			lastDir = true;
			sprite.flipX = false;
		}
		//Horizontal Movement
		float moveHorizontal = Input.GetAxisRaw ("Horizontal");
		rb2d.velocity = new Vector2 (moveHorizontal * speed, rb2d.velocity.y); //rb2d.velocity.y ensures smooth uninterupted jump
		//Apply drag
		if (rb2d.velocity.y < terminalVel) {
			rb2d.AddForce (Vector2.up * drag);
		}
//////////////////////////////////////////////////////
//Jumping and Wall Jumping
//////////////////////////////////////////////////////
		 
		//Short jump/hop and short wall jump
		if (Input.GetKeyDown (KeyCode.UpArrow)) { //Originally
			if (onGround) {
				isJumping = true;
				jumpTimeCounter = jumpTime; //Reset counter on jump
				rb2d.AddForce (Vector2.up * jumpHeight, ForceMode2D.Impulse);
			} else if (onLeftWall || onRightWall) {
				//lastDir = true;
				StartCoroutine (WallJump ());
			} 
		}
		//Long Jump
		if (Input.GetKey (KeyCode.UpArrow)) {
			if (isJumping && jumpTimeCounter > 0) { //Stops when out of jump time
				rb2d.AddForce (Vector2.up * jumpHeight, ForceMode2D.Impulse);
				jumpTimeCounter -= Time.deltaTime; //Timer limits how long one can jump
			} else
				isJumping = false; //Stop jump
			
		}
		//Check if jumping
		if (Input.GetKeyUp (KeyCode.UpArrow))  //When key is released, stop jumping immediately
			isJumping = false;
		

//////////////////////////////////////////////////////
//Dashing and Air Dashing
//////////////////////////////////////////////////////

		if (Input.GetKeyDown (KeyCode.Z) && !isDashing) { //Prevents
			StartCoroutine (Dash ());
		}

//////////////////////////////////////////////////////
//Blaster
//////////////////////////////////////////////////////
		 
		//Normal Shot
		if (Input.GetKeyDown (KeyCode.X)) {
			//Check curr weapon

			if (GameObject.FindGameObjectsWithTag ("Projectile").Length < 3) { //Limit to 3 projectiles on screen at a time
				Vector2 pos;
				if (lastDir)
					pos = new Vector2 (transform.position.x + 2, transform.position.y);
				else
					pos = new Vector2 (transform.position.x - 2, transform.position.y);
				Instantiate (lemon, pos, lemon.transform.rotation);
			}
		}


	}

	IEnumerator Dash ()
	{
		//Prepare dash
		canTurn = false;
		isDashing = true;
		float time = 0f; //Reset timer
		speed /= 1000f;	//"Disables" effect of walking temporarily
		//Air dash
		if (!onGround) {
			rb2d.velocity = Vector2.zero;
			rb2d.gravityScale = 0f; //Anti-gravity if in air
		}
		while (time < dashTime) {
			if (isJumping) { //Enables dash jump
				time = -0.25f;
			} 
			if (lastDir)
				rb2d.AddForce (Vector2.right * dashSpeed);
			else
				rb2d.AddForce (Vector2.left * dashSpeed);
			time += Time.deltaTime;
			yield return null; //Advance to next frame
		}
		speed *= 1000f; //Re-enables walking
		//Check where we are
		if (onLeftWall || onRightWall)
			rb2d.gravityScale = wallGravity; //Reset gravity (in case of air dash landing)
		else
			rb2d.gravityScale = gravity; //Reset gravity (in case of air dash landing)

		canTurn = true;
		yield return new WaitForSeconds (0.3f); //Delay before second button pressed is allowed
		isDashing = false;
	}

	IEnumerator WallJump ()
	{
		float time = 0f; //Reset timer
		speed /= 1000f;	//"Disables" effect of walking temporarily
		rb2d.velocity = Vector2.zero; //Stop gravity and falling to jump naturally

		while (time < wallJumpTime) {
			time += Time.deltaTime;
		
			if (lastDir) {
				rb2d.AddForce (Vector2.left * wallJumpKnock);
				rb2d.AddForce (Vector2.up * wallJumpHeight);
			} else {
				rb2d.AddForce (Vector2.right * wallJumpKnock);
				rb2d.AddForce (Vector2.up * wallJumpHeight);
			}
			yield return null; //Advance to next frame
		}
		speed *= 1000f;	//"Disables" effect of walking temporarily

	}


	//////////////////////////////////////////////////////
	//OnCollisionEnter
	//////////////////////////////////////////////////////

	//When on the ground, jumping is allowed
	void OnCollisionEnter2D (Collision2D other)
	{
		if (other.gameObject.CompareTag ("Floor"))
			onGround = true;
		if (other.gameObject.CompareTag ("LeftWall")) {
			onLeftWall = true;
			speed /= 5; //Prevents getting stuck on walls
			rb2d.gravityScale = wallGravity; //player descends slower on wall
		}
		if (other.gameObject.CompareTag ("RightWall")) {
			onRightWall = true;
			speed /= 5; //Prevents getting stuck on walls
			rb2d.gravityScale = wallGravity; //player descends slower on wall
		}
	}

	//////////////////////////////////////////////////////
	//OnCollisionExit
	//////////////////////////////////////////////////////

	//When jumping, leaves floor and is disallowed from continuous jumping
	void OnCollisionExit2D (Collision2D other)
	{
		if (other.gameObject.CompareTag ("Floor"))
			onGround = false;
		if (other.gameObject.CompareTag ("LeftWall")) {
			onLeftWall = false;
			speed *= 5; //Prevents getting stuck on walls
			rb2d.gravityScale = gravity; //player falls normally off of wall
		}
		if (other.gameObject.CompareTag ("RightWall")) {
			onRightWall = false;
			speed *= 5; //Prevents getting stuck on walls
			rb2d.gravityScale = gravity; //player falls normally off of wall
		}
	}
}
