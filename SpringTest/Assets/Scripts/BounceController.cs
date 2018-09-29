using System;
using UnityEngine;

public class BounceController : MonoBehaviour
{
    public enum BounceState {IN_AIR, BOUNCE_IN, BOUNCE_PLAYER_HELD, BOUNCE_OUT}

    public Transform ViewPivot;
    public Transform View;
    public AudioSource Booing;

    Rigidbody2D _Rigidbody;
    Vector2 _OutDirection;

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
    
    BounceState _State;
    public BounceState State
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
        if (other.contacts.Length == 0)
        {
            return;
        }
        
        _OutDirection = other.contacts[0].normal;
        Quaternion prevViewPivotRotation = View.rotation;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, _OutDirection);
        View.rotation = prevViewPivotRotation;
        
        switch (other.gameObject.tag)
        {
            case "Env":
                if (State == BounceState.IN_AIR)
                {
                    BounceIn(other);
                }
                break;
        }
    }

    void Update()
    {
        switch (State)
        {
            case BounceState.IN_AIR:
                break;
            case BounceState.BOUNCE_IN:
                _Rigidbody.isKinematic = false;
                _Rigidbody.AddForce(_OutDirection * 5f, ForceMode2D.Impulse);
                State = BounceState.BOUNCE_OUT;
                break;
            case BounceState.BOUNCE_OUT:
                CompressionFactor = CompressionFactor * 2;

                if (CompressionFactor > 1f)
                {
                    CompressionFactor = 1f;
                    State = BounceState.IN_AIR;
                }
                break;
        }
    }
    
    public bool TryFlick(Vector2 direction)
    {
        if (_State != BounceState.BOUNCE_PLAYER_HELD)
        {
            return false;
        }
        
        _Rigidbody.isKinematic = false;
        float angle = -Vector2.SignedAngle(_OutDirection.normalized, direction.normalized);
        
        _Rigidbody.AddForce(_OutDirection * 5f + direction, ForceMode2D.Impulse);
        _Rigidbody.angularVelocity = angle * 10f;
        
        State = BounceState.BOUNCE_OUT;

        Booing.pitch = 1f + direction.magnitude * 0.2f;
        Booing.Play();

        return true;
    }

    public bool TryHold(Vector2 direction)
    {
        if (State == BounceState.BOUNCE_IN)
        {
            State = BounceState.BOUNCE_PLAYER_HELD;
        }
        
        if (State != BounceState.BOUNCE_PLAYER_HELD)
        {
            return false;
        }
        
        CompressionFactor = Mathf.Lerp(CompressionFactor, 1f - direction.magnitude * 0.8f, 0.1f);
        return true;
    }

    void BounceIn(Collision2D env)
    {
        State = BounceState.BOUNCE_IN;
        WipeVelocities();
        _Rigidbody.isKinematic = true;
    }

    void WipeVelocities()
    {
        _Rigidbody.velocity = Vector2.zero;
        _Rigidbody.angularVelocity = 0f;
    }
}
