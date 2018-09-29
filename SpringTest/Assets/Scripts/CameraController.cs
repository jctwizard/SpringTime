using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform Follow;

	void Update()
	{
		Vector3 pos = transform.position;
		pos.x = Mathf.Lerp(pos.x, Follow.position.x, 0.1f);
		transform.position = pos;
	}
}
