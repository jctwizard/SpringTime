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
        GameObject followObject = GameObject.FindGameObjectWithTag(followTag);
        Vector3 followPosition = followObject.transform.position;
        float scale = 1;
        if (followObject.GetComponent<ObjectSize>())
        {
            scale = followObject.GetComponent<ObjectSize>().size;
        }
        else
        {
            Debug.Log(followObject.gameObject.name + "doesn't have a size assigned");
        }
        StartCoroutine(cameraSizeLerp(scale));

        Vector3 newPosition = Vector3.Lerp(transform.position, followPosition, Time.deltaTime * followSpeed);
        newPosition.z = transform.position.z;
        transform.position = newPosition;
    }

    IEnumerator cameraSizeLerp(float scale)
    {
        float i = 0;
        while (i<1)
        {
            i += Time.deltaTime * 0.25f;
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, initialSize * scale, i);
            yield return new WaitForEndOfFrame();
        }
    }
}
