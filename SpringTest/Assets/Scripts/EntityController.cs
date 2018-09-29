using System;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    public enum EntityState {IN_AIR, BOUNCE_IN, BOUNCE_OUT}

    public Transform ViewPivot;
    public Transform View;
    public AudioSource Booing;

    Rigidbody2D _Rigidbody;
    Vector3 _OutDirection;

    Vector2 _JoystickDirection;
    int JumpPower = 0;
    
    float _CompressionFactor = 1f;
    public float CompressionFactor
    {
        get { return _CompressionFactor; }
        set
        {
            if (_CompressionFactor == value)
            {
                return;
            }
            
            _CompressionFactor = value;
            
            Time.timeScale = _CompressionFactor;
            Vector3 scale = ViewPivot.localScale;
            scale.y = _CompressionFactor;
            ViewPivot.localScale = scale;
        }
    }
    
    EntityState _State;
    public EntityState State
    {
        get { return _State; }
        set
        {
            if (_State == value)
            {
                return;
            }
            
            _State = value;
        }
    }
    
    void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        _OutDirection = other.contacts[0].normal;
        Quaternion prevViewPivotRotation = View.rotation;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, _OutDirection);
        View.rotation = prevViewPivotRotation;
        
        switch (other.gameObject.tag)
        {
            case "Env":
                if (State == EntityState.IN_AIR)
                {
                    BounceIn(other);
                }
                break;
        }
    }

    void Update()
    {
        Vector2 newJoystickDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        bool spring = false;
        if (_JoystickDirection.magnitude >= 0.9f && newJoystickDirection.magnitude < 0.9f)
        {
            spring = true;
        }
        
        switch (State)
        {
            case EntityState.IN_AIR:
                break;
            case EntityState.BOUNCE_IN:
                CompressionFactor = Mathf.Lerp(CompressionFactor, 1f - newJoystickDirection.magnitude * 0.8f, 0.1f);

                if (spring)
                {
                    BounceOut();
                }
                break;
            case EntityState.BOUNCE_OUT:
                CompressionFactor = CompressionFactor * 2;

                if (CompressionFactor > 1f)
                {
                    CompressionFactor = 1f;
                    State = EntityState.IN_AIR;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        _JoystickDirection = newJoystickDirection;
    }

    void BounceOut()
    {
        _Rigidbody.isKinematic = false;
        Vector2 bounceDirection = _OutDirection * 5f;
        Vector2 customDirection = -_JoystickDirection.normalized * 5f;
        float angle = -Vector2.SignedAngle(_OutDirection.normalized, -_JoystickDirection.normalized);
        
        _Rigidbody.AddForce((bounceDirection + customDirection) * (1f + (JumpPower - 1) * 0.2f), ForceMode2D.Impulse);
        _Rigidbody.angularVelocity = angle * 10f;
        
        /*
        Debug.LogFormat("up: {0}, -jd: {1}, angle: {2}",
            _OutDirection.normalized,
            -_JoystickDirection.normalized,
            Vector2.SignedAngle(_OutDirection.normalized, -_JoystickDirection.normalized));
            */
        
        State = EntityState.BOUNCE_OUT;

        Booing.pitch = 1f + (JumpPower - 1f) * 0.2f;
        Booing.Play();
    }

    void BounceIn(Collision2D env)
    {
        if (_JoystickDirection.magnitude < 0.1f)
        {
            ResetJumpPower();
            _Rigidbody.AddForce(env.contacts[0].normal * 5f, ForceMode2D.Impulse);
            return;
        }

        JumpPower++;
        if (JumpPower > 3)
        {
            JumpPower = 1;
        }
        
        State = EntityState.BOUNCE_IN;
        WipeVelocities();
        _Rigidbody.isKinematic = true;
    }

    void ResetJumpPower()
    {
        JumpPower = 0;
    }

    void WipeVelocities()
    {
        _Rigidbody.velocity = Vector2.zero;
        _Rigidbody.angularVelocity = 0f;
    }
}
