// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// base class for objects like crates windows etc
public class base_breakable : MonoBehaviour
{
    // damage threshold before object is destroyed
    [SerializeField] protected float _health = 1f;
    //
    [SerializeField] protected int _cells = 1;
    // keep track of current health
    protected float[] _healthInst;
    //
    protected int _cellID;
    //
    protected float _healthCell;
    //
    [SerializeField] protected float _rateRegen = 1f;
    //
    [SerializeField] protected float _timeRegen = 1f;
    //
    protected float _timerRegen = 0f;
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
    //
    [SerializeField] private MeshRenderer _renderer = null;
    // reference to the material used by this object
    protected Material _material = null;
    // for keeping track of the material color as it flashes white
    private Color _tintColor;
    // determines how quickly the white fades away, larger values means shorter flash
    private float _tintFadeSpeed = 4f;
    //
    [SerializeField] protected Vector2 _offset = Vector2.zero;
    // (built-in function) first function called on object initialized/spawned
    protected virtual void Awake()
    {
        // mark current health as full
        //_healthInst = _health;
        //
        _healthInst = new float[_cells];
        //
        _cellID = _cells - 1;
        //
        _healthCell = _health / _cells;
        //
        for (int i = _cellID; i > -1; i--) _healthInst[i] = _healthCell;
        // maximum movement speed limited to 30 units per second ? use global constant
        _speedTerminal = 30f;
        // store reference to object's rigidbody component that handles physics
        _rb = GetComponent<Rigidbody>();
        // get reference to material instance used by this object
        if(_renderer == null) _material = GetComponent<MeshRenderer>().material;
        else _material = _renderer.material;
        // if the material uses the flash effect shader, get its default tint color otherwise use any color with the alpha set to zero
        _tintColor = _material.HasProperty("_Tint") ? _material.GetColor("_Tint") : new Color(0f, 0f, 0f, 0f);
    }
    // (built-in function) executed every frame
    protected virtual void Update()
    {
        // if the damage tint is showing
        if (_tintColor.a > 0f)
        {
            // fade out the color transparency slightly
            _tintColor.a = Mathf.Clamp01(_tintColor.a - _tintFadeSpeed * Time.deltaTime);
            // apply the updated tint color
            _material.SetColor("_Tint", _tintColor);
        }
        //
        if (_timerRegen > 0f) _timerRegen -= Time.deltaTime;
        //
        else if (_healthInst[_cellID] < _healthCell)
        {
            _healthInst[_cellID] += _rateRegen * Time.deltaTime;
            //
            if (_healthInst[_cellID] > _healthCell) _healthInst[_cellID] = _healthCell;
            //
            else if (_healthInst[_cellID] <= 0f) Destroy();
        }
        // this object does not move
        if (!_rb) return;
        // cap current movement speed if it exceeds specified limit ? use global constant
        if (Speed > _speedTerminal) _rb.velocity = _rb.velocity.normalized * _speedTerminal;
    }
    // called by other objects that collide with this object, like the player, returns true if object health reaches zero ? use bool for hurt/restore
    public bool ModifyHealthInst(float value, bool isSubtract = true)
    {
        // safety
        value = Mathf.Abs(value);
        //
        if (value == 0f) return false;
        // register the damage amount to the damage display indicator
        feedback_damage.Instance.Register(transform, value);
        // apply received change to current health
        //_healthInst = Mathf.Clamp(_healthInst + value, 0f, _health);
        //
        int cellID = _cellID;
        // received damage
        if (isSubtract)
        {
            // make the damage flash visible
            _tintColor.a = 1f;
            //
            HurtOnCell();
            //
            while (value > 0f)
            {
                // current cell can tank damage
                if (_healthInst[_cellID] > value)
                {
                    _healthInst[_cellID] -= value;
                    value = 0f;
                }
                // damage is more than what current cell can handle
                else
                {
                    value -= _healthInst[_cellID];
                    _healthInst[_cellID] = 0f;
                    if (_cellID > 0) _cellID--;
                    else value = 0f;
                }
            }
            //
            _timerRegen = _timeRegen;
            //
	        if (cellID != _cellID) CellOnHurt();
            //
            feedback_health.Instance.Register(transform, _cells, HealthNormalized());
        }
        // object is damaged enough to break
        if (_cellID == 0 && _healthInst[_cellID] <= 0f)
        {
            // destroy this object
	        Destroy();
	        // inform the belligerent of their success
	        return true;
	    }
        // inform the belligerent of their failure
	    return false;
    }
    // optional special behaviour for when health in some cell on hurt
    protected virtual void CellOnHurt()
    {
        // crate
        // - decal crack
        // entity
        // - mesh damage
        // - AI alter: desperate, malfunction, flee, surrender, feint, etc
    }
    // optional special behaviour for hurt when health in some cell
    protected virtual void HurtOnCell()
    {
        //print(gameObject.name + "\tcritical health");
    }
    // called by external events, like an explosion
    //public void AddForce(Vector3 force)
    //{
        // apply force on this object for this frame only
        //_rb.AddForce(force, ForceMode.Impulse);
    //}
    // handles physics forces applied internally to this object
    public virtual void AddForce(Vector3 force, bool mode = false)
    {
        _rb.AddForce(force, mode ? ForceMode.Force : ForceMode.Impulse);
    }
    // handles how this object should be destroyed
    protected virtual void Destroy()
    {
        // spawn the specified destruction effect
        if (_effectDestroy != null) Instantiate(_effectDestroy, transform.position, transform.rotation);
        // spawn the fracture object for this object
        if (_fracture != null) Instantiate(_fracture, transform.position, _parentRotationFracture ? transform.rotation : Quaternion.Euler(_rotationFracture.x, _rotationFracture.y, _rotationFracture.z));
        // delete this gameobject from the scene
        Destroy(gameObject);
    }
    //
    private float[] HealthNormalized()
    {
        float[] health = new float[_cells];
        for (int i = _cells - 1; i > -1; i--) health[i] = _healthInst[i] / _healthCell;
        return health;
    }
    // regions are good for organization since they can be minimzed and make it easier to read lengthy code
    #region Properties
    // shorthand for getting this object's movement speed
    //protected float Speed
    public float Speed
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
    //public float HealthInst
    //{
        // provide the current health
        //get { return _healthInst; }
    //}
    #endregion
}
