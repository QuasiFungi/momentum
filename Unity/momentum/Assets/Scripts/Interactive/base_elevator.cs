// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// script that defines the behaviour for broken elevators
public class base_elevator : MonoBehaviour
{
    // the position of the platform when it is fully lifted, to be assigned in editor
    [SerializeField] private Vector3 _positionOut = Vector3.zero;
    // the position of the platform when it is fully pushed down, to be assigned in editor
    [SerializeField] private Vector3 _positionIn = Vector3.down;
    // the force with which the platform tries to lift, to be assigned in editor
    [SerializeField] private float _forceLift = 1f;
    // reference to the platform gameobject of the elevator, to be assigned in editor
    [SerializeField] private Transform _trigger = null;
    // local reference to the rigidbody components of the platform, to allow control over its physics behaviours
    private Rigidbody _rb;
    // the maximum distance the platform can be pushed down
    private float _distance = 1f;
    // the direction in which the platform lifts
    private Vector3 _directionOut = Vector3.up;
    // used to counter minor inaccuracies when calculating distances that prevent the platform from lifting
    private float _buffer = .01f;
    // (built-in function) first function called on object initialized/spawned
    void Awake()
    {
        // store references to the platform's rigidbody component that handle physics
        _rb = _trigger.GetComponent<Rigidbody>();
        // calculate the relative direction the panel is to lift in
        _directionOut = _positionOut - _positionIn;
        // store the total distance between the in/out positions of the platform
        _distance = _directionOut.magnitude;
        // convert the direction from local to world orientation
        _directionOut = transform.TransformDirection(_directionOut.normalized);
        // modify/set physics constraints based on orientation
        // - disable all physics based rotation of the platform
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        // only works when object rotation is in multiples of 90 degrees, breaks otherwise, so doors can't be placed on slopes right now
        // if the elevator is oriented horizontally, freeze platform movement along all other axes
        if (_trigger.up.x != 0f) _rb.constraints |= RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        // if the elevator is oriented vertically, freeze platform movement along all other axes
        else if (_trigger.up.y != 0f) _rb.constraints |= RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        // if the elevator is facing towards/away from camera, won't be used but just in case
        else if (_trigger.up.z != 0f) _rb.constraints |= RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
    }
    // (built-in function) executed when Unity updates all physics objects in the scene
    void FixedUpdate()
    {
        // cache platform's current distance from in/out positions, for reuse
        float distanceOut = Vector3.Distance(_trigger.localPosition, _positionOut);
        float distanceIn = Vector3.Distance(_trigger.localPosition, _positionIn);
        // if platform overshoot / undershoot, with buffer for physics inaccuracies
        if (distanceOut + distanceIn > _distance + _buffer)
        {
            // halt platform movement
            _rb.velocity = Vector3.zero;
            // snap to nearest end position (in/out)
            _trigger.localPosition = distanceOut < distanceIn ? _positionOut : _positionIn;
        }
        // otherwuse, if platform currently inbetween end positions
        else if (distanceIn <= _distance)
            // move towards OUT position
            _rb.AddForce(_directionOut * _forceLift);
    }
}