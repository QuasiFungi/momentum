// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// defines the behaviour of a switch, inherit from signal class to allow controlling mechanisms
public class signal_switch : base_signal
{
    // reference to the switch handle object that moves, to be assigned in editor
    [SerializeField] private Transform _trigger = null;
    // to allow some transition space between active/inactive rotations, can be modified in editor
    [SerializeField] private float _buffer = 5f;
    // local reference to the rigidbody component of the switch handle, to allow control over its physics behaviour
    private Rigidbody _rb;
    // the maximum amounts the handle can swing in either direction
    private float _angleOff = 30f;
    private float _angleOn = -30f;
    // the force with which the handle swings itself to the nearest end rotation
    private float _forceFlick = .1f;
    // flag that is set to true when switch is at either end, to ensure the signal is only sent once and not every frame while switch is active/inactive
    private bool _isEnabled = false;
    // (built-in function) first function called on object initialized/spawned
    protected override void Awake()
    {
        // execute this same function defined in the parent class, before executing this one
        base.Awake();
        // store a reference to the switch handle's rigidbody component that handles physics
        _rb = _trigger.GetComponent<Rigidbody>();
        // recenter the object's center of mass to mesh origin point (default is calculated center)
        _rb.centerOfMass = Vector3.zero;
        // disable all physics based translation for the handle
        _rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        // since a switch always rotates along the camera facing axis, freeze the handle's rotation along all other axes
        _rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
    }
    // (built-in function) executed when Unity updates all physics objects in the scene
    void FixedUpdate()
    {
        // because constraints are *useless*, manually undo the handle's rotation along all other axes
        _trigger.localEulerAngles = Vector3.forward * _trigger.localEulerAngles.z;
        // convert angle from world 0:360 to local 180:-180
        // convert angle of the handle from world absolute 0:360 to local relative 180:-180
        // angle increases when going counter clockwise, and decreases when going clockwise along an axis
        float angle = Vector3.SignedAngle(_trigger.up, transform.up, Vector3.forward);
        // clamp angle if it slips above the defined maximum constraint
        if (angle > _angleOff)
        {
            // - invert the sign because relative vs absolute angle
            _trigger.localEulerAngles = Vector3.forward * -_angleOff;
            // - halt all rotational forces currently effecting object
            _rb.angularVelocity = Vector3.zero;
        }
        // clamp angle if it slips below the defined minimum constraint
        else if (angle < _angleOn)
        {
            // - invert the sign because relative vs absolute angle
            _trigger.localEulerAngles = Vector3.forward * -_angleOn;
            // - halt all rotational forces currently effecting object
            _rb.angularVelocity = Vector3.zero;
        }
        // swing the handle towards the nearest end rotation
        // - if close to but not below OFF angle, swing clockwise
        else if (angle < _angleOff && angle >= 0f) _rb.angularVelocity += Vector3.forward * -_forceFlick * Time.fixedDeltaTime;
        // - if close to but not above ON angle, swing counter clockwise
        else if (angle > _angleOn && angle <= 0f) _rb.angularVelocity += Vector3.forward * _forceFlick * Time.fixedDeltaTime;
        // handle's angle relative to ON angle, if closer than the buffer
        if (angle < -_buffer)
        {
            // and if switch is marked inactive
            if (!_isEnabled)
            {
                // trigger ON event
                SetSignal(true);
                // mark button as active
                _isEnabled = true;
            }
        }
        // handle's angle relative to OFF angle, if closer than the buffer
        else if (angle > _buffer)
        {
            // and if switch is marked inactive
            if (!_isEnabled)
            {
                // trigger OFF event
                SetSignal(false);
                // mark button as active
                _isEnabled = true;
            }
        }
        // otherwise if the switch is marked active, mark switch as inactive
        else if (_isEnabled) _isEnabled = false;
    }
}