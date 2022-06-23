// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// built in c# tool, allows defining lists of all types
using System.Collections.Generic;
// defines the behaviour of a fan, inherit from mechanism to allow responding to signals
public class mechanism_fan : base_mechanism
{
    // reference to animator component that handles object animation, to be assigned in editor
    [SerializeField] private Animator _anim = null;
    // reference to the trigger type collider that defines the area effected by wind from the fan, to be assigned in editor
    [SerializeField] private Transform _trigger = null;
    // the force with which to push/pull objects that enter the fan's wind hitbox, can be modified in editor
    [SerializeField] private float _force = 10f;
    // correction for fan mesh origin not being centered on actual center, can be modified in editor
    [SerializeField] private Vector3 _offsetCenter = Vector3.up * .5f;
    // referenec to the particles that visualize the direction of wind pushing/pulling objects, to be assigned in editor
    // [SerializeField] private GameObject _particleUpPull = null;
    [SerializeField] private GameObject _particleUpPush = null;
    // [SerializeField] private GameObject _particleDownPull = null;
    [SerializeField] private GameObject _particleDownPush = null;
    // flags for disabling wind particles if a side is obstructed, like if the fan is attached flat onto a surface, can be modified in editor
    [Tooltip("Top facing side unobstructed")] [SerializeField] private bool _isUp = true;
    [Tooltip("Down facing side unobstructed")] [SerializeField] private bool _isDown = true;
    // cache for the maximum distance the fan's wind effects physics objects, calculated from wind trigger collider size
    private float _distance = 5f;
    // reference to all physics objects inside wind collider, to be pushed/pulled
    private List<Rigidbody> _targets = new List<Rigidbody>();
    void Awake()
    {
        // convert the direction from local to world orientation
        _offsetCenter = transform.TransformDirection(_offsetCenter);
        // calculate the distance from fan origin to wind collider edge
        // - obtain reference to wind collider
        BoxCollider collider = _trigger.GetComponent<BoxCollider>();
        // - get colider size given in local space, and calculate simple hypotenuse since dealing with 2D motion
        // ? suck in object if collider inside trigger but origin outside
        _distance = Mathf.Sqrt(Mathf.Pow(collider.size.x / 2f, 2f) + Mathf.Pow(collider.size.y / 2, 2f));
        // enable/disable wind particles based on if the side is obstructed or not
        // _particleUpPull.SetActive(_isUp);
        _particleUpPush.SetActive(_isUp);
        // _particleDownPull.SetActive(_isDown);
        _particleDownPush.SetActive(_isDown);
    }
    // (built-in function) executed every frame
    void Update()
    {
        // if fan spin direction changed
        if (_state == _anim.GetBool("IsInvert"))
        {
            // play animation for appropriate direction
            _anim.SetBool("IsInvert", !_state);
            // if top face not obstructed
            if (_isUp)
            {
                // // if top face wind particle is pulling but wind direction is push, disable wind pull particle
                // if (_particleUpPull.activeSelf == state) _particleUpPull.SetActive(!_state);
                // if top face wind particle is pushing but wind direction is pull, disable wind push particle
                if (_particleUpPush.activeSelf != _state) _particleUpPush.SetActive(_state);
            }
            // if bottom face not obstructed
            if (_isDown)
            {
                // // if bottom face wind particle is pulling but wind direction is push, disable wind pull particle
                // if (_particleDownPull.activeSelf != state) _particleDownPull.SetActive(_state);
                // if bottom face wind particle is pushing but wind direction is pull, disable wind push particle
                if (_particleDownPush.activeSelf == _state) _particleDownPush.SetActive(!_state);
            }
        }
    }
    // (built-in function) executed when Unity updates all physics objects in the scene
    void FixedUpdate()
    {
        // objects inside wind hitbox to discard
        List<Rigidbody> toRemove = new List<Rigidbody>();
        // iterate all objects inside wind hitbox
        foreach (Rigidbody target in _targets)
            // if a valid object exists
            if (target)
            {
                // cache values for reuse
                // - object direction from fan object's origin
                Vector3 direction = target.position - _trigger.position;
                // - object's euclidean distance from the fan object's origin
                float distance = direction.magnitude;
                // ? unused value
                RaycastHit hitInfo;
                // if the path between object and fan is obstructed by certain types of objects, ignore this object
                if (Physics.Raycast(_trigger.position + _offsetCenter, direction, out hitInfo, distance, game_variables.Instance.MaskRayObstruction)) continue;
                // apply push/pull force on the object, weaken the force with distance, and invert the force direction based on the fan's spin direction
                target.AddForce(_trigger.up * (_force * (1f - distance / _distance) * (_state ? 1f : -1f)));
            }
            // object no longer exists
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