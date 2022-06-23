// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// defines the behaviour of a button, inherit from signal class to allow controlling mechanisms
public class signal_button : base_signal
{
    // position of the button when OFF, to be assigned in editor
    [SerializeField] private Vector3 _positionOff = Vector3.zero;
    // position of the button when ON, to be assigned in editor
    [SerializeField] private Vector3 _positionOn = Vector3.down;
    // deactivate the button when it is this % of the way between ON and OFF positions, can be modified in editor
    [Tooltip("Distance % from OFF position at which to consider the button inactive")] [Range(0,1)] [SerializeField] private float _thresholdOff = .5f;
    // activate the button when it is this % of the way between ON and OFF positions, can be modified in editor
    [Tooltip("Distance % from ON position at which to consider the button active")] [Range(0,1)] [SerializeField] private float _thresholdOn = .5f;
    // button remains enabled if this is set to true, otherwise return to OFF position, can be modified in editor
    [SerializeField] private bool _isOneway = true;
    // when true, the button remains active/inactive until pressed again, otherwise only active when pressed, can be modified in editor
    [Tooltip("Only change signal value on activate")] [SerializeField] private bool _isToggle = true;
    // force with which button returns to OFF position, can be modified in editor
    [SerializeField] private float _forceOff = 1f;
    // reference to the button panel object that moves, to be assigned in editor
    [SerializeField] private Transform _trigger = null;
    // local reference to the rigidbody component of the button panel, to allow control over its physics behaviour
    private Rigidbody _rb;
    // relative direction to which to push the button panel for it to reach the OFF position
    private Vector3 _directionOff = Vector3.up;
    // cache total distance between end positions
    private float _distance = 0f;
    // distance % at which to consider the button unpressed
    private float _distanceOff = .1f;
    // distance % at which to consider the button pressed
    private float _distanceOn = .1f;
    // flag that is set to true when button is active, to ensure the signal is only sent once and not every frame while button is active/inactive
    private bool _isEnabled = false;
    // (built-in function) first function called on object initialized/spawned
    protected override void Awake()
    {
        // execute this same function defined in the parent class, before executing this one
        base.Awake();
        // // ? get rb component of child named trigger
        // _trigger = transform.GetChild("trigger");
        // store a reference to the button panel's rigidbody component that handles physics
        _rb = _trigger.GetComponent<Rigidbody>();
        // direction of OFF position relative to ON position
       _directionOff = _positionOff - _positionOn;
        // cache total distance between end positions for reuse
        _distance = _directionOff.magnitude;
        // deactivate the button when at this % of the total distance
        _distanceOff = _distance * _thresholdOff;
        // activate the button when at this % of the total distance
        _distanceOn = _distance * _thresholdOn;
        // convert the direction from local to world orientation
        _directionOff = transform.TransformDirection(_directionOff.normalized);
        // modify/set physics constraints based on orientation
        // - disable all physics based rotation of the panel
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        // only works when object rotation is in multiples of 90 degrees, breaks otherwise, so doors can't be placed on slopes right now
        // if the button is oriented horizontally, freeze panel movement along all other axes
        if (_directionOff.x != 0f) _rb.constraints |= RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        // if the button is oriented vertically, freeze panel movement along all other axes
        else if (_directionOff.y != 0f) _rb.constraints |= RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        // if the button is facing towards/away from camera, won't be used but just in case
        else if (_directionOff.z != 0f) _rb.constraints |= RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
    }
    // (built-in function) executed when Unity updates all physics objects in the scene
    void FixedUpdate()
    {
        // cache panel's current distance from ON position, for reuse
        float distance = Vector3.Distance(_trigger.localPosition, _positionOn);
        // if the button panel is sufficiently close to the ON position
        if (distance <= _distanceOn)
        {
            // and if button is inactive
            if (!_isEnabled)
            {
                // trigger ON event but
                // if type TOGGLE then (if type INVERT then same event else invert) else trigger ON event
                SetSignal(_isToggle ? (_isInvert ? State : !State) : true);
                // mark button as active
                _isEnabled = true;
            }
        }
        // if the button panel is sufficiently close to the OFF position
        else if (distance >= (_distance - _distanceOff))
        {
            // and if button is marked active, mark button as inactive
            if (_isEnabled) _isEnabled = false;
        }
        // if not at OFF position
        if (distance < _distance)
        {
            // if button is not ONEWAY or not ON, then push button towards OFF position
            if (!_isOneway || distance > _distanceOn) _rb.AddForce(_directionOff * _forceOff);
        }
        // if button panel overshoots beyond OFF position
        else if (distance > _distance)
        {
            // halt panel movement
            _rb.velocity = Vector3.zero;
            // snap panel to OFF position
            _trigger.localPosition = _positionOff;
            // trigger OFF event, if not type TOGGLE
            if (!_isToggle) SetSignal(false);
        }
    }
}