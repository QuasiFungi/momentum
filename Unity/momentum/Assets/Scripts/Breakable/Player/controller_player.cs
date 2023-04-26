// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// ensure that this object has a rigidbody component attached else an error is thrown during compilation
[RequireComponent(typeof(Rigidbody))]
// main Singleton class that handles the player's response to input, collisions, etc
//public class controller_player : MonoBehaviour
public class controller_player : base_breakable
{
    // allow access to the player controller from any script
    public static controller_player Instance;
    // reference to physics component of player
    //private Rigidbody _rb;
    // temporary, the longest distance for a swipe to be detected, longer swipes are scaled down ? use global constants
    private float _sensitivityDash = 50f;
    // temporary, the longest distance for a brake to be detected, longer inputs are ignored ? use global constants
    private float _sensitivityBrake = 200f;
    // the amount of force with which the player dashes
    private float _speedDash = 10f;
    // the rate of speed reduction when braking
    private float _dragBrake = 1f;
    // the fastest speed the player is allowed to move at ? use global constant
    //private float _speedTerminal = 30f;
    // player speeds lower than this are zeroed, prevents sliding when no input is supplied
    private float _drag = 1f;
    // is set to true when input is held down, used for toggling braking particle effect
    private bool _isHold = false;
    // reference to particle effect used to visualize braking
    protected GameObject _effectBrake;
    // reference to particle effect used to visualize dashing
    protected GameObject _effectDash;
    // reference to particle effect used to visualize stall
    protected GameObject _effectStall;
    // reference to particle effect used to visualize boost
    protected GameObject _effectBoost;
    // amount of damage applied to objects on collision, this value is scaled to the player's current speed
    protected float _damage;
    // only speeds higher than this value produce the dash particle effect
    private float _thresholdEffect = 10f;
    // * for devlog only *
    public bool _isDemo = false;
    // reference to the trail renderer component ? should be private or protected
    public TrailRenderer _trail = null;
    // for recording the current player velocity at the start of each physics update cycle
    private Vector3 _cacheVelocity = Vector3.zero;
    // time frame for stall mechanic to be triggered
    private float _timeStall = 1f;
    // time passed since stall mechanic became valid
    private float _timerStall = 0f;
    // to make sure that the stall mechanic is used once per valid condition, true means unused
    private bool _flagStall = true;
    // records the player velocity whenever brake is triggered
    private Vector3 _velocityStall = Vector3.zero;
    // total time for boost mechanic to be charged and executed
    private float _timeBoost = 1f;
    // current charge level of the boost
    private float _timerBoost = 0f;
    // to make sure that the boost mechanic is used once per valid condition, true means unused
    private bool _flagBoost = false;
    // the increase in force with which the player dashes when the boost mechanic is triggered
    private float _speedBoost = 10f;
    // the rate of rotation when using the steer mechanic
    private float _speedSteer = 1f;
    //
    private int _counterCollision = 0;
    // (built-in function) first function called on object initialized/spawned
    protected override void Awake()
    {
        base.Awake();
        // initialize a Singleton to ensure only one player can exist at a time, discarding any duplicates
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        // bind the brake EVENT of input manager class to the brake FUNCTION of this class
        manager_input.Instance.OnStartBrake += Brake;
        // bind the dash EVENT of input manager class to the dash FUNCTION of this class
        manager_input.Instance.OnStartDash += Dash;
        // store reference to object's rigidbody component that handles physics
        _rb = GetComponent<Rigidbody>();
        // local variable for preparing the brake and boost particle effects
        GameObject temp;
        // load the brake effect from the project's resources folder, type casted as a gameobject
        temp = Resources.Load("Particle/EffectBrake") as GameObject;
        // initialize a copy of the loaded effect as child of player object
        _effectBrake = Instantiate(temp, transform, false);
        // disable the effect until when the brake input is provided
        _effectBrake.SetActive(false);
        // only load the dash effect from the project's resources folder, type casted as a gameobject
        _effectDash = Resources.Load("Particle/EffectDash") as GameObject;
        // only load the stall effect from the project's resources folder, type casted as a gameobject
        _effectStall = Resources.Load("Particle/EffectStall") as GameObject;
        // load the boost effect from the project's resources folder, type casted as a gameobject
        temp = Resources.Load("Particle/EffectBoost") as GameObject;
        // initialize a copy of the loaded effect as child of player object
        _effectBoost = Instantiate(temp, transform, false);
        // disable the effect until when the boost is charged
        _effectBoost.SetActive(false);
        // set amount of damage applied when player speed is 1 unit per second
        _damage = .1f;
    }
    // (built-in function) executed when Unity updates all physics objects in the scene
    void FixedUpdate()
    {
        // record current velocity
        _cacheVelocity = Velocity;
        // * for devlog only *
        if (Input.GetKey("w")) _rb.AddForce(Vector3.up * _speedDash);
        if (Input.GetKey("s")) _rb.AddForce(Vector3.down * _speedDash);
        if (Input.GetKey("a")) _rb.AddForce(Vector3.left * _speedDash);
        if (Input.GetKey("d")) _rb.AddForce(Vector3.right * _speedDash);
        // limit the current player speed if it exceeds the provided terminal speed
        if (Speed > _speedTerminal) _rb.velocity = Direction * _speedTerminal;
        // if braking, apply reduction to player speed
        if (_effectBrake.activeSelf) AddDrag(_dragBrake);
        // constantly reduce player speed to simulate air/surface friction
        if (Speed > 0f) _rb.velocity -= Direction * _drag * Time.fixedDeltaTime;
    }
    // (built-in function) executed every frame
    protected override void Update()
    {
        // full brake if speed insignifantly small
        if (Speed < _drag * Time.deltaTime) _rb.velocity = Vector3.zero;
        // * for devlog only *
        if (!_isDemo)
        // enable/disable brake effect based on whether brake input detected and pointer in brake radius
        _effectBrake.SetActive(_isHold && (manager_input.Instance.PositionDrag - manager_input.Instance.PositionPress).magnitude < _sensitivityBrake);
        // tick stall mechanic timer if active
        if (_timerStall > 0f) _timerStall -= Time.deltaTime;
        // currently braking
        if (_effectBrake.activeSelf)
        {
            // using stall mechanic is possible
            if (_flagStall)
            {
                // set the stall timer
                _timerStall = _timeStall;
                // mark stall as having been attempted
                _flagStall = false;
                // record current player velocity
                _velocityStall = Velocity;
            }
            // charge up the boost if remaining
            if (_timerBoost < _timeBoost) _timerBoost += Time.deltaTime;
            // boost fully charged
            else if (_timerBoost > _timeBoost)
            {
                // cap the boost charge timer
                _timerBoost = _timeBoost;
                // mark boost as charged
                _flagBoost = true;
            }
        }
        // not braking
        else
        {
            // stall mechanic already attempted
            if (!_flagStall)
            {
                // reset the stall mechanic timer
                _timerStall = 0f;
                // mark stall mechanic as usable
                _flagStall = true;
            }
            // drain boost charge if any
            if (_timerBoost > 0f) _timerBoost -= Time.deltaTime;
            // boost fully discharged
            else if (_timerBoost < 0f)
            {
                // cap the boost charge timer
                _timerBoost = 0f;
                // mark boost as discharged
                _flagBoost = false;
            }
        }
        //
        _effectBoost.SetActive(_flagBoost);
        // calculate pointer displacement between press and current to determine drag direction
        Vector3 direction = manager_input.Instance.PositionDrag - manager_input.Instance.PositionPress;
        // convert displacement into rotation in degrees along the forward axis
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // subtract the brake vector from the displacement
        direction -= direction.normalized * _sensitivityBrake;
        // scale down displacement based on dash input sensitivity
        direction /= _sensitivityDash;
        // cap maximum drag distance to 1 meaning a full drag
        if (direction.magnitude > 1f) direction = direction.normalized;
        // configure the touch input display to the current input status
        feedback_touch.Instance.SetDrag(!_effectBrake.activeSelf, angle, direction.magnitude);
        // valid conditions for the steer mechanic
        if (_isHold && !_effectBrake.activeSelf && Speed > 0f)
            // rotate the current velocity based on drag direction and steer speed
            Velocity = Vector3.RotateTowards(Direction, direction.normalized, _speedSteer * Time.deltaTime, 0f) * Speed;
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
        // check if pointer is outside brake radius, ignore dash otherwise
        if (direction.sqrMagnitude > (direction.normalized * _sensitivityBrake).sqrMagnitude)
        {
            // calculate pointer displacement from outside brake radius
            direction -= direction.normalized * _sensitivityBrake;
            // scale down displacement based on dash input sensitivity
            direction /= _sensitivityDash;
            // (memory vs computation) cache calculated distance in local variable for reuse
            float distance = direction.magnitude;
            // cap maximum swipe distance to 1 meaning a full powered dash, a lower value means a weaker dash
            if (distance > 1f) direction = direction.normalized;
            // * for devlog only *
            if (!_isDemo)
            // spawn dash effect if calculated dash speed is above threshold, measured relative to dash speed (vs current speed)
            if (distance * _speedDash >= _thresholdEffect) Instantiate(_effectDash, _rb.position, Quaternion.LookRotation(direction));
            // boost condition is valid
            if (_flagBoost)
            {
                // execute dash, apply force in swipe direction scaled to the boosted dash speed
                AddForce(direction * (_speedDash + _speedBoost));
                // mark boost charge as consumed
                _flagBoost = false;
            }
            // execute dash, apply force in swipe direction scaled to the dash speed
            else AddForce(direction * _speedDash);
        }
        // mark brake as ended
        _isHold = false;
    }
    // handles physics forces applied internally to this object
    //private void AddForce(Vector3 force, bool mode = false)
    //{
        //_rb.AddForce(force, mode ? ForceMode.Force : ForceMode.Impulse);
    //}
    // handles physics drag forces applied internally to this object
    private void AddDrag(float force)
    {
        _rb.velocity -= Speed > force ? Direction * force : _rb.velocity * Time.fixedDeltaTime;
    }
    // (built-in function) executed when this object collides with another collider in the scene
    void OnCollisionEnter(Collision other)
    {
        //
        _counterCollision++;
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
        // get the collided objects rigidbody
        Rigidbody rb = other.transform.GetComponent<Rigidbody>();
        // object is a valid recipent for stall mechanic to be triggered
        if (rb && !rb.isKinematic && _timerStall > 0f)
        {
            // apply player momentum before brake start onto the colliding object
            rb.AddForce(_velocityStall * Mass);
            // halt player movement
            Velocity = Vector3.zero;
            //
            Instantiate(_effectStall, other.contacts[0].point, Quaternion.FromToRotation(Vector3.back, other.contacts[0].normal));
        }
        // no damage if slow ? consider direction
        if (SpeedCache < 1f) return;
        // apply damage if collided object is of type breakable (defined via layers)
        if (other.gameObject.layer == game_variables.Instance.LayerBreakable)
        {
            // get the colliding objects component that handles custom collision mechanics
            base_breakable temp = other.transform.GetComponent<base_breakable>();
            // if the object is destroyed on impact, reapply the players velocity from the last physics update cycle (preserve momentum)
            if (temp && temp.ModifyHealthInst(_damage * Mass * SpeedCache) && Speed > 0f) Velocity = _cacheVelocity;
        }
    }
    protected override void Destroy()
    {
        // bind the brake EVENT of input manager class
        manager_input.Instance.OnStartBrake -= Brake;
        // bind the dash EVENT of input manager class
        manager_input.Instance.OnStartDash -= Dash;
        //
        base.Destroy();
    }
    // regions are good for organization since they can be minimzed and make it easier to read lengthy code
    #region Properties
    // shorthand, used for calculations
    //public float Speed
    //{
        //get { return _rb.velocity.magnitude; }
    //}
    // shorthand, used internally for calculations
    // * public for devlog only *
    public float Mass
    {
        get { return _rb.mass; }
        // * for devlog only *
    	set { _rb.mass = value; }
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
    // * for devlog only *
    public float SpeedDash
    {
    	set { _speedDash = value; }
    }
    public float SpeedTerminal
    {
    	set { _speedTerminal = value; }
    }
    public float DragBrake
    {
    	set { _dragBrake = value; }
    }
    public float Size
    {
    	set { transform.localScale = Vector3.one * value; }
    }
    public float Trail
    {
    	set { _trail.time = value; }
    }
    public float Damage
    {
    	set { _damage = value; }
    }
    public float Drag
    {
    	set { _drag = value; }
    }
    private Vector3 Velocity
    {
        get { return _rb.velocity; }
        set { _rb.velocity = value; }
    }
    private float SpeedCache
    {
        get { return _cacheVelocity.magnitude; }
    }
    public float Stall
    {
    	set { _timeStall = value; }
    }
    public float Boost
    {
    	set { _timeBoost = value; }
    }
    public float Steer
    {
    	set { _speedSteer = value; }
    }
    public float SensitivityBrake
    {
        get { return _sensitivityBrake; }
        set { _sensitivityBrake = value; }
    }
    public float SensitivityDash
    {
        get { return _sensitivityDash; }
        set { _sensitivityDash = value; }
    }
    public float SpeedBoost
    {
    	set { _speedBoost = value; }
    }
    public int CounterCollision
    {
        get { return _counterCollision; }
    }
    #endregion
}
