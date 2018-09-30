using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZone : MonoBehaviour {
	public float gravityScale = 0.5f;
	private List<Rigidbody2D> bodies;

	private void Start()
	{
		bodies = new List<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		foreach (Rigidbody2D body in bodies)
		{
			body.AddForce(Physics2D.gravity * gravityScale);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		Rigidbody2D body = other.GetComponentInChildren<Rigidbody2D>();

		if (body == null)
		{
			body = other.GetComponentInParent<Rigidbody2D>();
		}

		bodies.Add(body);
		body.gravityScale = 0.0f;
		Debug.Log("body enter");
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		Rigidbody2D body = other.GetComponentInChildren<Rigidbody2D>();

		if (body == null)
		{
			body = other.GetComponentInParent<Rigidbody2D>();
		}

		bodies.Remove(body);
		body.gravityScale = 1.0f;
		Debug.Log("body exit");
	}
}
