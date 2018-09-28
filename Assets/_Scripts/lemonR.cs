using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lemonR : MonoBehaviour {

	public float speed;
	private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update () {
		rb2d.velocity = Vector2.right * speed;
	}

	void OnBecameInvisible() {
		Destroy (gameObject);
	}
}
