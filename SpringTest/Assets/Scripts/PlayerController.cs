using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float FlickMagnitudeThreshold = 0.3f;
    public float FlickReleaseThreshold = 0.1f;

    public float FlickPower = 5f;
    
    int _JumpPower = 0;
    Vector2 _JoystickDirection;
    public BounceController _BounceTarget;
    
    void Update()
    {
        if (!_BounceTarget)
        {
            return;
        }
        
        Vector2 newJoystickDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (DidFlick(newJoystickDirection))
        {
            TryFlickTarget(newJoystickDirection);
        }
        else if (AttemptingControl(newJoystickDirection))
        {
            TryHoldTarget(newJoystickDirection);
        }
        
        _JoystickDirection = newJoystickDirection;
    }

    bool AttemptingControl(Vector2 newJoystickDirection)
    {
        return newJoystickDirection.magnitude > FlickReleaseThreshold;
    }

    bool DidFlick(Vector2 newJoystickDirection)
    {
        return newJoystickDirection.magnitude < FlickReleaseThreshold &&
               _JoystickDirection.magnitude >= FlickReleaseThreshold;
    }

    void ResetJumpPower()
    {
        _JumpPower = 0;
    }

    void TryFlickTarget(Vector2 newJoystickDirection)
    {
        Vector2 direction = (newJoystickDirection - _JoystickDirection) *
                            (1f + (_JumpPower - 1) * 0.2f);
        direction *= FlickPower;
        if (_BounceTarget.TryFlick(direction))
        {
            _JumpPower++;
            _JumpPower = _JumpPower % 4;
        }
    }

    void TryHoldTarget(Vector2 newJoystickDirection)
    {
        _BounceTarget.TryHold(newJoystickDirection);
    }
}