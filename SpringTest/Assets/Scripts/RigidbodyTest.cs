using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		//GetComponent<Rigidbody2D>().isKinematic = true;
		GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown("space"))
		{
			Debug.LogFormat("appling force");
			GetComponent<Rigidbody2D>().AddForce(Vector2.right * 10f, ForceMode2D.Impulse);
		}
		
	}
}
