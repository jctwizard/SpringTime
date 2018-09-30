using System;
using UnityEngine;

public class BounceController : MonoBehaviour
{
    public enum BounceState {IN_AIR, BOUNCE_IN, BOUNCE_PLAYER_HELD, BOUNCE_OUT}

    public Transform ViewPivot;
    public Transform View;
    public AudioSource Booing;
	public ParticleSystem JumpParticle;
	public ParticleSystem BounceParticle;

    public float RegularBounceForce = 2f;
    public float PlayerBounceForce = 5f;

    GameObject _Anchor;
    Vector2 _OutDirection;
	Rigidbody2D _Rigidbody;
    Quaternion _ParentsInitialRotation;

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
            
            Vector3 scale = ViewPivot.localScale;
            scale.y = _CompressionFactor;
			scale.x = 2 - Mathf.Clamp(CompressionFactor, 0, 2);
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
        _Anchor = new GameObject("Anchor");
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        TryBounceIn(other);
    }
    
    private void OnCollisionStay2D(Collision2D other)
    {
        TryBounceIn(other);
    }

    void Update()
    {
        switch (State)
        {
            case BounceState.IN_AIR:
                break;
            case BounceState.BOUNCE_IN:
                FlyOff(_OutDirection * RegularBounceForce);
                break;
            case BounceState.BOUNCE_PLAYER_HELD:
                SetToAnchor();
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
        
        float angle = -Vector2.SignedAngle(_OutDirection.normalized, direction.normalized);
        _Rigidbody.angularVelocity = angle * 10f;
        
        FlyOff(_OutDirection * RegularBounceForce + direction);

        return true;
    }

    void FlyOff(Vector2 direction)
    {
        //transform.SetParent(null, true);
        transform.localScale = Vector3.one;
        _Rigidbody.isKinematic = false;
        _Rigidbody.AddForce(direction, ForceMode2D.Impulse);

		if (State == BounceState.BOUNCE_PLAYER_HELD)
		{
			JumpParticle.Play();
		}
		else
		{
			BounceParticle.Play();
		}

        State = BounceState.BOUNCE_OUT;
        
        Booing.pitch = 1f + direction.magnitude * 0.2f;
        Booing.Play();
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
        
        WipeVelocities();

        float dot = Vector2.Dot((_Anchor.transform.parent.rotation * Quaternion.Inverse(_ParentsInitialRotation)) * _OutDirection , direction);
        if (dot > -0.1f)
        {
            FlyOff(_OutDirection * PlayerBounceForce);
            return false;
        }
        
        CompressionFactor = Mathf.Lerp(CompressionFactor, 1f - direction.magnitude * 0.8f, 0.1f);
        return true;
    }

    void BounceIn(Collision2D env)
    {
        State = BounceState.BOUNCE_IN;
        _Rigidbody.isKinematic = true;
    }

    void SetToAnchor()
    {
        transform.position = _Anchor.transform.position;
        transform.rotation = _Anchor.transform.rotation;
    }
    
    void TryBounceIn(Collision2D other)
    {
        if (State != BounceState.IN_AIR || other.contacts.Length == 0)
        {
            return;
        }
        
        _OutDirection = other.contacts[0].normal;
        Quaternion prevViewPivotRotation = View.rotation;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, _OutDirection);
        View.rotation = prevViewPivotRotation;
        
        //transform.SetParent(other.transform, true);
        _Anchor.transform.SetParent(other.transform, true);
        _Anchor.transform.position = transform.position;
        _Anchor.transform.rotation = transform.rotation;
        
        _ParentsInitialRotation = _Anchor.transform.parent.rotation;
        
        BounceIn(other);
    }

    void WipeVelocities()
    {
        _Rigidbody.velocity = Vector2.zero;
        _Rigidbody.angularVelocity = 0f;
    }
}
