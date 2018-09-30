using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float FlickMagnitudeThreshold = 0.3f;
    public float FlickReleaseThreshold = 0.1f;

    public float FlickPower = 10f;

    public LineRenderer Trajectory;

	public string AxisSuffix = "";

    int _JumpPower = 0;
    Vector2 _JoystickDirection;
    public BounceController _BounceTarget;

    void Update()
    {
        if (!_BounceTarget)
        {
            return;
        }

        Vector2 newJoystickDirection = Vector2.zero;
        if (Input.GetJoystickNames().Length > 0)
        {
            if (Input.GetJoystickNames()[0].ToLower().Contains("hori") ||
                Input.GetJoystickNames()[0].ToLower().Contains("wireless"))
            {
                newJoystickDirection = new Vector2(Input.GetAxis("HorizontalDS4" + AxisSuffix), Input.GetAxis("VerticalDS4" + AxisSuffix));
            }
            else
            {
                newJoystickDirection = new Vector2(Input.GetAxis("Horizontal" + AxisSuffix), Input.GetAxis("Vertical" + AxisSuffix));
            }
        }

        Trajectory.enabled = _BounceTarget.State == BounceController.BounceState.BOUNCE_PLAYER_HELD;
        Trajectory.SetPosition(0, _BounceTarget.transform.position - _BounceTarget.transform.up * 0.5f);
        Trajectory.SetPosition(1, _BounceTarget.transform.position - (Vector3)newJoystickDirection * 6);

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
        Vector2 direction = (newJoystickDirection - _JoystickDirection).normalized *
                            (1f + (_JumpPower - 1 * 0.1f));
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