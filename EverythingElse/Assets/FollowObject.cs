using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
	public string followTag;
	public float followSpeed;

	private float initialSize;

	void Start ()
	{
		initialSize = GetComponent<Camera>().orthographicSize;	
	}
	
	void Update ()
	{
		Vector3 followPosition = Vector3.zero;

		GameObject[] followObjects = GameObject.FindGameObjectsWithTag(followTag);

		if (followObjects.Length > 0)
		{
			foreach (GameObject followObject in followObjects)
			{
				followPosition += followObject.transform.position;
			}

			followPosition /= followObjects.Length;

			float separationDistance = 0.0f;

			foreach (GameObject followObject in followObjects)
			{
				float distance = Vector3.Distance(followObject.transform.position, followPosition);

				if (distance > separationDistance) 
				{
					separationDistance = distance;
				}
			}

			Vector3 newPosition = Vector3.Lerp(transform.position, followPosition, Time.deltaTime * followSpeed);
			newPosition.z = transform.position.z;
			transform.position = newPosition;
			GetComponent<Camera>().orthographicSize = initialSize + separationDistance;
		}
	}
}
