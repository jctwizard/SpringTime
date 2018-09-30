using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float Speed;

    void Update()
    {
        transform.rotation *= Quaternion.Euler(0f, 0f, Time.deltaTime * Speed);
    }
}
