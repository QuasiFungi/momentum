// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// defines the behaviour of a turn type windup, inherit from signal class to allow controlling mechanisms
public class signal_windupTurn : base_signal
{
    // determines which direction the windup needs to be spun to be activated, can be modiifed in editor
    [SerializeField] private bool _isClockwise = false;
    // total rotation needed to activate the windup, can be modified in editor
    [SerializeField] private float _angleOn = 360f;
    // the force with which the windup spins itself back to the default rotation
    [SerializeField] private float _forceUnwind = 1f;
    // local reference to the rigidbody component of the windup, to allow control over its physics behaviour
    private Rigidbody _rb;
    // records total difference in angle relative to the starting angle
    private float _angleDelta = 0f;
    // records windup's rotation in the previous frame, to be compared to the angle in current frame
    private float _angleCache = 0f;
    // flag that is set to true when the windup is active, to ensure the signal is only sent once and not every frame while button is active/inactive
    private bool _isEnabled = false;
    // (built-in function) first function called on object initialized/spawned
    protected override void Awake()
    {
        // execute this same function defined in the parent class, before executing this one
        base.Awake();
        // store a reference to the windup's rigidbody component that handles physics
        _rb = GetComponent<Rigidbody>();
    }
    // (built-in function) executed when Unity updates all physics objects in the scene
    void FixedUpdate()
    {
        // windup's direction in last frame
        Vector3 from = Quaternion.AngleAxis(_angleCache, Vector3.forward) * Vector3.up;
        // windup's direction in this frame
        Vector3 to = Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.forward) * Vector3.up;
        // record total change in angle relative to starting angle
        _angleDelta += Vector3.SignedAngle(to, from, Vector3.forward);
        // if windup is set ot spin clockwise
        if (_isClockwise)
        {
            // if windup slips below starting angle
            if (_angleDelta < 0f)
            {
                // - clamp rotation to lowest allowed value
                transform.eulerAngles = Vector3.zero;
                // - halt all rotational forces currently effecting object
                _rb.angularVelocity = Vector3.zero;
                // and if windup is marked inactive
                if (!_isEnabled)
                {
                    // trigger OFF event
                    SetSignal(false);
                    // mark windup as active
                    _isEnabled = true;
                }
            }
            // if windup's rotation is inbetween start and ON angles
            else if (_angleDelta > 0f && _angleDelta < _angleOn)
            {
                // unwind back towards starting position
                _rb.angularVelocity += Vector3.forward * _forceUnwind * Time.fixedDeltaTime;
                // and if windup is marked active, mark windup as inactive
                if (_isEnabled) _isEnabled = false;
            }
            // if windup's rotation overshoots beyond the ON angle
            else if (_angleDelta >= _angleOn)
            {
                // - clamp rotation to maximum allowed value, rounded off to under 360, invert the sign because relative vs absolute angle
                transform.eulerAngles = Vector3.forward * (-_angleOn % 360f);
                // - halt all rotational forces currently effecting object
                _rb.angularVelocity = Vector3.zero;
                // and if windup marked inactive
                if (!_isEnabled)
                {
                    // trigger ON event
                    SetSignal(true);
                    // mark windup as active
                    _isEnabled = true;
                }
            }
        }
        // if windup is set ot spin counter clockwise
        else
        {
            // if windup slips below starting angle
            if (_angleDelta > 0f)
            {
                // - clamp rotation to lowest allowed value
                transform.eulerAngles = Vector3.zero;
                // - halt all rotational forces currently effecting object
                _rb.angularVelocity = Vector3.zero;
                // and if windup is marked inactive
                if (!_isEnabled)
                {
                    // trigger OFF event
                    SetSignal(false);
                    // mark windup as active
                    _isEnabled = true;
                }
            }
            // if windup's rotation is inbetween start and ON angles
            else if (_angleDelta < 0f && _angleDelta > -_angleOn)
            {
                // unwind back towards starting position
                _rb.angularVelocity -= Vector3.forward * _forceUnwind * Time.fixedDeltaTime;
                // and if windup is marked active, mark windup as inactive
                if (_isEnabled) _isEnabled = false;
            }
            // if windup's rotation overshoots beyond the ON angle
            else if (_angleDelta <= -_angleOn)
            {
                // - clamp rotation to maximum allowed value, rounded off to under 360, invert the sign because relative vs absolute angle
                transform.eulerAngles = Vector3.forward * (_angleOn % 360f);
                // - halt all rotational forces currently effecting object
                _rb.angularVelocity = Vector3.zero;
                // and if windup marked inactive
                if (!_isEnabled)
                {
                    // trigger ON event
                    SetSignal(true);
                    // mark windup as active
                    _isEnabled = true;
                }
            }
        }
        // record current rotation for comparision to rotation in next frame
        _angleCache = transform.eulerAngles.z;
    }
}