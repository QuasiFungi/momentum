// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// ensure that this object has a rigidbody component attached else an error is thrown during compilation
[RequireComponent(typeof(Rigidbody))]
// main Singleton class that handles the player's response to input, collisions, etc
public class controller_player : MonoBehaviour
{
    // allow access to the player controller from any script
    public static controller_player Instance;
    // reference to physics component of player
    private Rigidbody _rb;
    // temporary, the longest distance for a swipe to be detected, longer swipes are scaled down
    private float _sensitivityDash = 100f;
    // the amount of force with which the player dashes
    private float _speedDash = 15f;
    // the rate of speed reduction when braking
    private float _dragBrake = .2f;
    // the fastest speed the player is allowed to move at
    private float _speedTerminal = 30f;
    // player speeds lower than this are zeroed, prevents sliding when no input is supplied
    private float _drag = 1f;
    // is set to true when input is held down, used for toggling braking particle effect
    private bool _isHold = false;
    // reference to particle effect used to visualize braking
    protected GameObject _effectBrake;
    // reference to particle effect used to visualize dashing
    protected GameObject _effectDash;
    // amount of damage applied to objects on collision, this value is scaled to the player's current speed
    protected float _damage;
    // only speeds higher than this value produce the dash particle effect
    private float _thresholdEffect = 10f;
    // * for devlog only *
    public bool _isDemo = false;
    // (built-in function) first function called on object initialized/spawned
    void Awake()
    {
        // initialize a Singleton to ensure only one player can exist at a time, discarding any duplicates
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        // bind the brake EVENT of input manager class to the brake FUNCTION of this class
        manager_input.Instance.OnStartBrake += Brake;
        // bind the dash EVENT of input manager class to the dash FUNCTION of this class
        manager_input.Instance.OnStartDash += Dash;
        // store reference to object's rigidbody component that handles physics
        _rb = GetComponent<Rigidbody>();
        // local variable for preparing the brake and dash particle effects
        GameObject temp;
        // load the brake effect from the project's resources folder, type casted as a gameobject
        temp = Resources.Load("Particle/EffectBrake") as GameObject;
        // initialize a copy of the loaded effect as child of player object
        _effectBrake = Instantiate(temp, transform, false);
        // disable the effect until when the brake input is provided
        _effectBrake.SetActive(false);
        // only load the dash effect from the project's resources folder, type casted as a gameobject
        _effectDash = Resources.Load("Particle/EffectDash") as GameObject;
        // set amount of damage applied when player speed is 1 unit per second
        _damage = .1f;
    }
    // (built-in function) executed when Unity updates all physics objects in the scene
    void FixedUpdate()
    {
        // * for devlog only *
        if (Input.GetKey("w")) _rb.AddForce(Vector3.up * _speedDash);
        if (Input.GetKey("s")) _rb.AddForce(Vector3.down * _speedDash);
        if (Input.GetKey("a")) _rb.AddForce(Vector3.left * _speedDash);
        if (Input.GetKey("d")) _rb.AddForce(Vector3.right * _speedDash);
        // limit the current player speed if it exceeds the provided terminal speed
        if (Speed > _speedTerminal) _rb.velocity = Direction * _speedTerminal;
        // if braking, apply reduction to player speed
        if (_isHold) AddDrag(_dragBrake);
        // constantly reduce player speed to simulate air/surface friction
        if (Speed > 0f) _rb.velocity -= Direction * _drag * Time.fixedDeltaTime;
    }
    // (built-in function) executed every frame
    void Update()
    {
        // full brake if speed insignifantly small
        if (Speed < _drag * Time.deltaTime) _rb.velocity = Vector3.zero;
        // * for devlog only *
        if (!_isDemo)
        // enable/disable brake effect based on whether brake input detected or not
        _effectBrake.SetActive(_isHold);
    }
    // executed when brake input detected by input manager
    public void Brake()
    {
        // mark brake as started
        _isHold = true;
    }
    // executed when dash input detected by input manager
    public void Dash()
    {
        // calculate pointer displacement between press and release to determine swipe direction
        Vector2 direction = manager_input.Instance.PositionDrag - manager_input.Instance.PositionPress;
        // scale down displacement based on input sensitivity
        direction /= _sensitivityDash;
        // (memory vs computation) cache calculated distance in local variable for reuse
        float distance = direction.magnitude;
        // cap maximum swipe distance to 1 meaning a full powered dash, a lower value means a weaker dash
        if (distance > 1f) direction = direction.normalized;
        // * for devlog only *
        if (!_isDemo)
        // spawn dash effect if calculated dash speed is above threshold, measured relative to dash speed (vs current speed)
        if (distance * _speedDash >= _thresholdEffect) Instantiate(_effectDash, _rb.position, Quaternion.LookRotation(direction));
        // execute dash, apply force in swipe direction scaled to dash speed
        AddForce(direction * _speedDash);
        // mark brake as ended
        _isHold = false;
    }
    // handles physics forces applied internally to this object
    private void AddForce(Vector3 force, bool mode = false)
    {
        _rb.AddForce(force, mode ? ForceMode.Force : ForceMode.Impulse);
    }
    // handles physics drag forces applied internally to this object
    private void AddDrag(float force)
    {
        _rb.velocity -= Speed > force ? Direction * force : _rb.velocity * Time.fixedDeltaTime;
    }
    // (built-in function) executed when this object collides with another collider in the scene
    void OnCollisionEnter(Collision other)
    {
        // * for devlog only *
        if (!_isDemo)
        {
            // abort if somehow collide with itself
            if (other.transform.parent == transform) return;
            // detected impact with surface type based on tag, spawn appropriate particle effect
            if (other.transform.tag == "Concrete")
                Instantiate(game_variables.Instance.ParticleDust, other.contacts[0].point, Quaternion.FromToRotation(Vector3.up, other.contacts[0].normal));
            else if (other.transform.tag == "Metal")
                Instantiate(game_variables.Instance.ParticleSpark, other.contacts[0].point, Quaternion.FromToRotation(Vector3.up, other.contacts[0].normal));
            else if (other.transform.tag == "Glass")
                Instantiate(game_variables.Instance.ParticleShard, other.contacts[0].point, Quaternion.FromToRotation(Vector3.up, other.contacts[0].normal));
            else if (other.transform.tag == "Wood")
                Instantiate(game_variables.Instance.ParticleSplinter, other.contacts[0].point, Quaternion.FromToRotation(Vector3.up, other.contacts[0].normal));
        }
        // no damage if slow ? consider direction
        if (Speed < 1f) return;
        // apply damage if collided object is of type breakable (defined via layers)
        if (other.gameObject.layer == game_variables.Instance.LayerBreakable)
            other.transform.GetComponent<base_breakable>()?.ModifyHealthInst(-_damage * Mass * Speed);
    }
    // regions are good for organization since they can be minimzed and make it easier to read lengthy code
    #region Properties
    // shorthand, used internally for calculations
    private float Speed
    {
        get { return _rb.velocity.magnitude; }
    }
    // shorthand, used internally for calculations
    private float Mass
    {
        get { return _rb.mass; }
    }
    // shorthand, used internally for calculations
    private Vector3 Direction
    {
        get { return _rb.velocity.normalized; }
    }
    // used by camera to track to player
    public Vector3 Position
    {
        get { return _rb.position; }
    }
    #endregion
}
