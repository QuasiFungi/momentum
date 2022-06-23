// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// built in c# tool, allows defining lists of all types
using System.Collections.Generic;
// defines the behaviour of a dispenser, inherit from mechanism to allow responding to signals
public class mechanism_dispenser : base_mechanism
{
    // the left door of the dispenser, to be assigned in editor
    [SerializeField] private Transform _panelLeft = null;
    // the right door of the dispenser, to be assigned in editor
    [SerializeField] private Transform _panelRight = null;
    // the force with which swings itself close, can be modified in editor
    [SerializeField] private float _forceSwing = 1f;
    // push any object inside the dispenser with this force, can be modified in editor
    [SerializeField] private float _forceEject = 10f;
    // reference to the object to be dispensed, to be assigned in editor
    [SerializeField] private GameObject _dispense = null;
    // spherical area that needs to be clear for an object to be spawned, can be modified in editor
    [SerializeField] private float _radiusSpawn = 1f;
    // position at which to spawn the object to be dispensed, to be assigned in editor
    [SerializeField] private Transform _spawn = null;
    // maximum number of dispensed objects that can exist for dispenser stops working, can be modified in editor
    [SerializeField] private int _spawnLimit = 10;
    // if set to true dispenser keeps spawning objects when activated, can be modified in editor
    [SerializeField] private bool _isRepeat = false;
    // if repeat enabled, pause for this number of seconds between spawns, can be modiifed in editor
    [SerializeField] private float _delayRepeat = 1f;
    // the maximum amounts the doors can swing in either direction
    private float _angleMin = -90f;
    private float _angleMax = 0f;
    // local references to the rigidbody components of both door panels, to allow control over their physics behaviours
    private Rigidbody _rbLeft;
    private Rigidbody _rbRight;
    // reference to all physics objects inside dispenser, to be pushed out
    private List<Rigidbody> _targets = new List<Rigidbody>();
    // reference to all dispensed objects that haven't been destroyed
    private List<GameObject> _spawned = new List<GameObject>();
    // keeps track of time since last time object was spawned
    private float _spawnTimer = 0f;
    // flag that is set to true only if spawn location is unobstructed
    private bool _isDispense = false;
    // (built-in function) first function called on object initialized/spawned
    void Awake()
    {
        // throw console warning if no object assigned for dispensing
        if (_dispense == null) Debug.LogWarning(gameObject.name + ": No object/prefab assigned to dispense", transform);
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
        if (transform.right.x != 0f)
        {
            // so FreezeRotations stop external forces along LOCAL axes, but angularVelocity still treats them as GLOBAL axes...
            // hence why we need TWO free axes, instead of the logical ONE, in the case of non standard orientation
            _rbLeft.constraints |= RigidbodyConstraints.FreezeRotationZ;
            _rbRight.constraints |= RigidbodyConstraints.FreezeRotationZ;
        }
        // if the door is oriented vertically, freeze panel rotations along all other axes
        else if (transform.right.y != 0f)
        {
            // since this is the default prefab orientation, both local and global axes are identical
            _rbLeft.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _rbRight.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        // if the door is facing towards/away from camera, won't be used but just in case
        else if (transform.right.z != 0f)
        {
            _rbLeft.constraints |= RigidbodyConstraints.FreezeRotationX;
            _rbRight.constraints |= RigidbodyConstraints.FreezeRotationX;
        }
    }
    // (built-in function) executed every frame
    void Update()
    {
        // abort if no object assigned to dispense
        if (_dispense == null) return;
        // if dispenser activated
        if (_state)
        {
            // repeat spawn until OFF signal received
            if (_isRepeat)
            {
                // timer ticking
                if (_spawnTimer > 0f) _spawnTimer -= Time.deltaTime;
                // timer ticked
                else
                {
                    // reset timer, value added for spawning to be framerate independent
                    _spawnTimer += _delayRepeat;
                    // allow dispensing an object
                    _isDispense = true;
                }
            }
            // allow dispensing an object
            else _isDispense = true;
            // reached spawn limit
            if (_spawned.Count == _spawnLimit)
            {
                // objects to discard
                List<GameObject> toRemove = new List<GameObject>();
                // iterate all spawned objects
                foreach (GameObject spawn in _spawned)
                    // if object no longer exists, record invalid reference ? use int counter and null
                    if (!spawn) toRemove.Add(spawn);
                // discard invalid spawned object references
                foreach (GameObject spawn in toRemove) _spawned.Remove(spawn);
                // still at limit
                if (_spawned.Count == _spawnLimit)
                {
                    // spawn unsuccessful
                    if(!_isRepeat) _state = false;
                    // disallow dispensing an object
                    _isDispense = false;
                    // abort
                    return;
                }
            }
            // check if spawn location clear
            Collider[] colliders = Physics.OverlapSphere(_spawn.position, _radiusSpawn);
            // iterate obstructions if any exist
            foreach (Collider collider in colliders)
                // spawn loaction blocked by non interactive object
                if (collider.gameObject.layer != game_variables.Instance.LayerInteractive)
                {
                    // if repeat flag not set, dont try to spawn
                    if(!_isRepeat) _state = false;
                    // disallow dispensing an object
                    _isDispense = false;
                    // exit out of loop
                    break;
                }
            // if dispensing object copy allowed
            if (_isDispense)
            {
                // instantiate and record spawn
                // _spawned.Add(Instantiate(_dispense, _spawn.position, Quaternion.LookRotation(Vector3.forward)) as GameObject);
                // * for devlog only *
                _spawned.Add(Instantiate(_dispense, _spawn.position, Quaternion.LookRotation(Vector3.forward), transform) as GameObject);
                // if repeat flag not set, mark spawn as successful
                if(!_isRepeat) _state = false;
                // disallow dispensing an object
                _isDispense = false;
            }
        }
    }
    // (built-in function) executed when Unity updates all physics objects in the scene
    void FixedUpdate()
    {
        // because constraints are *useless*, manually undo the panel's rotations along all other axes
        _panelLeft.localEulerAngles = Vector3.right * _panelLeft.localEulerAngles.x;
        _panelRight.localEulerAngles = Vector3.right * _panelRight.localEulerAngles.x;
        // convert angles of left/right panels from world absolute 0:360 to local relative 180:-180
        // angle increases when going counter clockwise, and decreases when going clockwise along an axis
        float angleLeft = Vector3.SignedAngle(_panelLeft.forward, transform.forward, transform.right);
        float angleRight = Vector3.SignedAngle(_panelRight.forward, transform.forward, transform.right);
        // clamp angle if it slips below defined minimum constraint
        if (angleLeft < _angleMin)
        {
            // - invert the sign because relative vs absolute angle
            _panelLeft.localEulerAngles = Vector3.right * -_angleMin;
            // - halt all rotational forces currently effecting object
            _rbLeft.angularVelocity = Vector3.zero;
        }
        // clamp angle if it slips above defined maximum constraint
        else if (angleLeft > _angleMax)
        {
            // - invert the sign because relative vs absolute angle
            _panelLeft.localEulerAngles = Vector3.right * -_angleMax;
            // - halt all rotational forces currently effecting object
            _rbLeft.angularVelocity = Vector3.zero;
        }
        // clamp angle if it slips below defined minimum constraint
        if (angleRight < -_angleMax)
        {
            // - invert the sign because relative vs absolute angle
            _panelRight.localEulerAngles = Vector3.right * _angleMax;
            // - halt all rotational forces currently effecting object
            _rbRight.angularVelocity = Vector3.zero;
        }
        // clamp angle if it slips above defined maximum constraint
        else if (angleRight > -_angleMin)
        {
            // - invert the sign because relative vs absolute angle
            _panelRight.localEulerAngles = Vector3.right * _angleMin;
            // - halt all rotational forces currently effecting object
            _rbRight.angularVelocity = Vector3.zero;
        }
        // swing both panels back to their default rotations if more than slightly off center
        // - if left panel behind center, swing forwards
        if (angleLeft > _forceSwing) _rbLeft.angularVelocity += transform.right * _forceSwing * Time.fixedDeltaTime;
        // - if left panel ahead of center, swing backwards
        else if (angleLeft < -_forceSwing) _rbLeft.angularVelocity -= transform.right * _forceSwing * Time.fixedDeltaTime;
        // - if right panel behind center, swing forwards
        if (angleRight > _forceSwing) _rbRight.angularVelocity += transform.right * _forceSwing * Time.fixedDeltaTime;
        // - if right panel ahead of center, swing backwards
        else if (angleRight < -_forceSwing) _rbRight.angularVelocity -= transform.right * _forceSwing * Time.fixedDeltaTime;
        // objects inside dispenser hitbox to discard
        List<Rigidbody> toRemove = new List<Rigidbody>();
        // iterate all objects inside dispenser hitbox
        foreach (Rigidbody target in _targets)
            // if valid object exists, push object in ejection direction
            if (target) target.AddForce(transform.up * _forceEject);
            // object not longer exists
            else toRemove.Add(target);
        // discard invalid object references
        foreach (Rigidbody rb in toRemove) _targets.Remove(rb);
    }
    // (built-in function) executed when a physics object enters a trigger type collider attached to this object or a child object
    void OnTriggerEnter(Collider other)
    {
        // ignore interactive objects
        if (other.gameObject.layer == game_variables.Instance.LayerInteractive) return;
        // ignore if no rigidbody or already in list
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (!rb || _targets.Contains(rb)) return;
        // save object reference
        _targets.Add(rb);
    }
    // (built-in function) executed when a physics object exits a trigger type collider attached to this object or a child object
    void OnTriggerExit(Collider other)
    {
        // ignore interactive objects
        if (other.gameObject.layer == game_variables.Instance.LayerInteractive) return;
        // ignore if no rigidbody or already in list
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (!rb || !_targets.Contains(rb)) return;
        // discard object reference
        _targets.Remove(rb);
    }
}