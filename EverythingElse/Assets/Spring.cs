using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour {

	public float contractSpeed;
	public float springSpeed;
	public Vector3 contractScale;
	public float springForce;
	public AudioClip springSound;
	public int playerNumber;

	private bool colliding = false;
	private List<GameObject> collidingObjects = new List<GameObject>();
	private Vector3 initialScale;
	private bool contracting = false;

	// Use this for initialization
	void Start () {
		initialScale = transform.localScale;
	}

	// Update is called once per frame
	void Update () {


		if (Input.GetKey(KeyCode.Space) || (new Vector2(Input.GetAxis("Horizontal" + playerNumber), Input.GetAxis("Vertical" + playerNumber)).magnitude > 0.9f))
		{
			contracting = true;
			transform.localScale = Vector3.Lerp(transform.localScale, contractScale, Time.deltaTime * contractSpeed);
		}
		else if (contracting)
		{
			contracting = false;

			if (colliding)
			{
				float springDirection = 1.0f;

				if (Mathf.Repeat(transform.eulerAngles.z, 360.0f) > 90.0f && Mathf.Repeat(transform.eulerAngles.z, 360.0f) < 270.0f)
				{
					springDirection = -1.0f;
				}

				GetComponent<AudioSource>().PlayOneShot(springSound);

				GetComponent<Rigidbody2D>().AddForce(transform.up * springForce * springDirection, ForceMode2D.Impulse);
			}
		}
		else
		{
			transform.localScale = Vector3.Lerp(transform.localScale, initialScale, Time.deltaTime * springSpeed);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Unused" && collision.gameObject.GetComponent<Rigidbody2D>() && collision.gameObject.GetComponent<Rigidbody2D>().isKinematic == false)
		{
			Spring newSpring = collision.gameObject.AddComponent<Spring>();
			newSpring.contractSpeed = contractSpeed;
			newSpring.contractScale = contractScale;
			newSpring.springSpeed = springSpeed;
			newSpring.springForce = springForce;
			newSpring.springSound = springSound;
			newSpring.playerNumber = playerNumber;

			collision.gameObject.tag = "Player";

			tag = "Used";

			if (newSpring.GetComponent<AudioSource>() == null)
			{
				AudioSource newAudio = newSpring.gameObject.AddComponent<AudioSource>();
				newAudio.playOnAwake = false;
			}

			transform.localScale = initialScale;

			Destroy(this);
		}
		else if (collidingObjects.Contains(collision.gameObject) == false)
		{
			collidingObjects.Add(collision.gameObject);
			colliding = true;
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collidingObjects.Contains(collision.gameObject) == true)
		{
			collidingObjects.Remove(collision.gameObject);

			if (collidingObjects.Count <= 0)
			{
				colliding = false;
			}
		}
	}
}
