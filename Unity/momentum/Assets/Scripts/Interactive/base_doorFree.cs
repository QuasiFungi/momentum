// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// script that defines the behaviour for doors that swing freely
public class base_doorFree : MonoBehaviour
{
    // the left panel of the door, to be assigned in editor
    [SerializeField] private Transform _panelLeft = null;
    // the right panel of the door, to be assigned in editor
    [SerializeField] private Transform _panelRight = null;
    // the force with which swings itself close, can be modified in editor
    [SerializeField] private float _forceSwing = 1f;
    // the maximum amounts the doors can swing in either direction
    private float _angleMin = -90f;
    private float _angleMax = 90f;
    // local references to the rigidbody components of both door panels, to allow control over their physics behaviours
    private Rigidbody _rbLeft;
    private Rigidbody _rbRight;
    // (built-in function) first function called on object initialized/spawned
    void Awake()
    {
        // store references to both panel's rigidbody components that handle physics
        _rbLeft = _panelLeft.GetComponent<Rigidbody>();
        _rbRight = _panelRight.GetComponent<Rigidbody>();
        // recenter the object's center of mass to mesh origin point (default is calculated center)
        _rbLeft.centerOfMass = Vector3.zero;
        _rbRight.centerOfMass = Vector3.zero;
        // disable all physics based translation for both panels
        _rbLeft.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        _rbRight.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        // only works when object rotation is in multiples of 90 degrees, breaks otherwise, so doors can't be placed on slopes right now
        // if the door is oriented horizontally, freeze panel rotations along all other axes
        if (transform.up.x != 0f)
        {
            // so, FreezeRotations stop external forces along LOCAL axes, but angularVelocity still treats them as GLOBAL axes...
            // hence why we need TWO free axes, instead of the logical ONE, in the case of non standard orientation
            _rbLeft.constraints |= RigidbodyConstraints.FreezeRotationZ;
            _rbRight.constraints |= RigidbodyConstraints.FreezeRotationZ;
        }
        // if the door is oriented vertically, freeze panel rotations along all other axes
        else if (transform.up.y != 0f)
        {
            // since this is the default prefab orientation, both local and global axes are identical
            _rbLeft.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _rbRight.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        // if the door is facing towards/away from camera, won't be used but just in case
        else if (transform.up.z != 0f)
        {
            _rbLeft.constraints |= RigidbodyConstraints.FreezeRotationX;
            _rbRight.constraints |= RigidbodyConstraints.FreezeRotationX;
        }
    }
    // (built-in function) executed when Unity updates all physics objects in the scene
    void FixedUpdate()
    {
        // because constraints are *useless*, manually undo the panel's rotations along all other axes
        _panelLeft.localEulerAngles = Vector3.up * _panelLeft.localEulerAngles.y;
        _panelRight.localEulerAngles = Vector3.up * _panelRight.localEulerAngles.y;
        // convert angles of left/right panels from world absolute 0:360 to local relative 180:-180
        // angle increases when going counter clockwise, and decreases when going clockwise along an axis
        float angleLeft = Vector3.SignedAngle(_panelLeft.forward, transform.forward, transform.up);
        float angleRight = Vector3.SignedAngle(_panelRight.forward, transform.forward, transform.up);
        // clamp angle if it slips below defined minimum constraint
        if (angleLeft < _angleMin)
        {
            // - invert the sign because relative vs absolute angle
            _panelLeft.localEulerAngles = Vector3.up * -_angleMin;
            // - halt all rotational forces currently effecting object
            _rbLeft.angularVelocity = Vector3.zero;
        }
        // clamp angle if it slips above defined maximum constraint
        else if (angleLeft > _angleMax)
        {
            // - invert the sign because relative vs absolute angle
            _panelLeft.localEulerAngles = Vector3.up * -_angleMax;
            // - halt all rotational forces currently effecting object
            _rbLeft.angularVelocity = Vector3.zero;
        }
        // clamp angle if it slips below defined minimum constraint
        if (angleRight < _angleMin)
        {
            // - invert the sign because relative vs absolute angle
            _panelRight.localEulerAngles = Vector3.up * -_angleMin;
            // - halt all rotational forces currently effecting object
            _rbRight.angularVelocity = Vector3.zero;
        }
        // clamp angle if it slips above defined maximum constraint
        else if (angleRight > _angleMax)
        {
            // - invert the sign because relative vs absolute angle
            _panelRight.localEulerAngles = Vector3.up * -_angleMax;
            // - halt all rotational forces currently effecting object
            _rbRight.angularVelocity = Vector3.zero;
        }
        // swing both panels back to their default rotations if more than slightly off center
        // - if left panel behind center, swing forwards
        if (angleLeft > _forceSwing) _rbLeft.angularVelocity += transform.up * _forceSwing * Time.fixedDeltaTime;
        // - if left panel ahead of center, swing backwards
        else if (angleLeft < -_forceSwing) _rbLeft.angularVelocity -= transform.up * _forceSwing * Time.fixedDeltaTime;
        // - if right panel behind center, swing forwards
        if (angleRight > _forceSwing) _rbRight.angularVelocity += transform.up * _forceSwing * Time.fixedDeltaTime;
        // - if right panel ahead of center, swing backwards
        else if (angleRight < -_forceSwing) _rbRight.angularVelocity -= transform.up * _forceSwing * Time.fixedDeltaTime;
    }
}