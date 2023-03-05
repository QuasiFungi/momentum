// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// base class for objects like crates windows etc
public class base_breakable : MonoBehaviour
{
    // damage threshold before object is destroyed
    [SerializeField] protected float _health = 1f;
    // keep track of current health
    protected float _healthInst;
    // limit maximum movement speed to prevent physics glitches
    protected float _speedTerminal;
    // induce/observe physics behaviours
    protected Rigidbody _rb;
    // reference to special object that is a broken version of this object
    public GameObject _fracture = null;
    // specify specific rotation for fractured object version on spawn
    [Tooltip("Sequence Z X Y")] public Vector3 _rotationFracture = Vector3.zero;
    // fracture copies this object's rotation if set to true
    [Tooltip("Use parent rotation")] public bool _parentRotationFracture = false;
    // optional effect to spawn when object destroyed (like an explosion for barrels)
    [Tooltip("Optional destroy effect")] public GameObject _effectDestroy = null;
    // reference to the material used by this object
    private Material _material;
    // for keeping track of the material color as it flashes white
    private Color _tintColor;
    // determines how quickly the white fades away, larger values means shorter flash
    private float _tintFadeSpeed = 4f;
    // (built-in function) first function called on object initialized/spawned
    void Awake()
    {
        // mark current health as full
        _healthInst = _health;
        // maximum movement speed limited to 50 units per second ? use global constant
        _speedTerminal = 50f;
        // store reference to object's rigidbody component that handles physics
        _rb = GetComponent<Rigidbody>();
        // get reference to material instance used by this object
        _material = GetComponent<MeshRenderer>().material;
        // if the material uses the flash effect shader, get its default tint color otherwise use any color with the alpha set to zero
        _tintColor = _material.HasProperty("_Tint") ? _material.GetColor("_Tint") : new Color(0f, 0f, 0f, 0f);
    }
    // (built-in function) executed every frame
    void Update()
    {
        // if the damage tint is showing
        if (_tintColor.a > 0f)
        {
            // fade out the color transparency slightly
            _tintColor.a = Mathf.Clamp01(_tintColor.a - _tintFadeSpeed * Time.deltaTime);
            // apply the updated tint color
            _material.SetColor("_Tint", _tintColor);
        }
        // this object does not move
        if (!_rb) return;
        // cap current movement speed if it exceeds specified limit ? use global constant
        if (Speed > _speedTerminal) _rb.velocity = _rb.velocity.normalized * _speedTerminal;
    }
    // called by other objects that collide with this object, like the player, returns true if object health reaches zero
    public bool ModifyHealthInst(float value)
    {
        // apply received change to current health
        _healthInst = Mathf.Clamp(_healthInst + value, 0f, _health);
        // register the damage amount to the damage display indicator
        feedback_damage.Instance.Register(transform, value);
        // make the damage flash visible
        _tintColor.a = 1f;
        // object is damaged enough to break
        if (_healthInst <= 0f)
        {
            // destroy this object
	        Destroy();
	        // inform the belligerent of their success
	        return true;
	    }
        // inform the belligerent of their failure
	    return false;
    }
    // called by external events, like an explosion
    public void AddForce(Vector3 force)
    {
        // apply force on this object for this frame only
        _rb.AddForce(force, ForceMode.Impulse);
    }
    // handles how this object should be destroyed
    protected void Destroy()
    {
        // spawn the specified destruction effect
        if (_effectDestroy != null) Instantiate(_effectDestroy, transform.position, transform.rotation);
        // spawn the fracture object for this object
        if (_fracture != null) Instantiate(_fracture, transform.position, _parentRotationFracture ? transform.rotation : Quaternion.Euler(_rotationFracture.x, _rotationFracture.y, _rotationFracture.z));
        // delete this gameobject from the scene
        Destroy(gameObject);
    }
    // regions are good for organization since they can be minimzed and make it easier to read lengthy code
    #region Properties
    // shorthand for getting this object's movement speed
    protected float Speed
    {
        // provide the object's physics speed
        get { return _rb.velocity.magnitude; }
    }
    // object's maximum health, visible to all other classes
    public float Health
    {
        // provide the specified maximum health
        get { return _health; }
    }
    // object's current health, visible to all other classes
    public float HealthInst
    {
        // provide the current health
        get { return _healthInst; }
    }
    #endregion
}
