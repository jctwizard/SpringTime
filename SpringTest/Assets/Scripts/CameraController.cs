using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform Follow;

	void Update()
	{
		Vector3 pos = transform.position;
		pos = Vector3.Lerp(pos, Follow.position, 0.1f);
		pos.z = transform.position.z;
		transform.position = pos;
	}
}
