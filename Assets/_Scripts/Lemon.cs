using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lemon : MonoBehaviour {

	public float speed;
	public bool lastDir; //true = facing right, false = facing left
	private Rigidbody2D rb2d;

	CompletePlayerController player;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D>();
		player = GameObject.FindWithTag ("Player").GetComponent<CompletePlayerController>();
		lastDir = player.lastDir;
	}
	
	// Update is called once per frame
	void Update () {
		if (lastDir)
			rb2d.velocity = Vector2.right * speed;
		else
			rb2d.velocity = Vector2.left * speed;
	}

	void OnBecameInvisible() {
		Destroy (gameObject);
	}
}
