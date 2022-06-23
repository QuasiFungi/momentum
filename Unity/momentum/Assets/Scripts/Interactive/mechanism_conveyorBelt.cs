// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// ensure that this object has a rigidbody component attached else an error is thrown during compilation
[RequireComponent(typeof(Rigidbody))]
// defines the behaviour of a conveyor belt, inherit from mechanism to allow responding to signals
public class mechanism_conveyorBelt : base_mechanism
{
    // the force with which to push physics objects that touch the belt, can be modified in editor
    [SerializeField] private float _forcePush = 5f;
    // reference to the renderer component of the belt object, since it is a separate mesh from the frame, to be assigned in editor
    [SerializeField] private Renderer _renderer = null;
    // the speed at which to move the belt texture, creates the illusion of movement, can be modified in editor
    [SerializeField] private float _speedScroll = .1f;
    // reference to physics component of the conveyor belt
    private Rigidbody _rb = null;
    // the default direction in which to push objects
    private Vector3 _directionPush = Vector3.right;
    // (built-in function) first function called on object initialized/spawned
    void Awake()
    {
        // store reference to object's rigidbody component that handles physics
        _rb = GetComponent<Rigidbody>();
        // convert the push direction from absolute to relative, allows belts to work at an angle
        _directionPush = transform.TransformDirection(_directionPush);
    }
    // (built-in function) executed every frame
    void Update()
    {
        // scroll texture in push direction, reversing direction based on current state
        _renderer.material.mainTextureOffset = new Vector2(_state ? 0f : .5f, _speedScroll * (_state ? -1f : 1f) * Time.time);
    }
    // (built-in function) executed when Unity updates all physics objects in the scene
    void FixedUpdate()
    {
        // record current position
        Vector3 position = _rb.position;
        // teleport to slightly away from push position
        _rb.position -= _directionPush * (_state ? 1f : -1f) * _forcePush * Time.fixedDeltaTime;
        // perform physics movement back towards the original position, taking all contacting objects with itself
        _rb.MovePosition(position);
    }
}