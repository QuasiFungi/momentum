// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// used for exploding fragments of fractured versions of breakable objects
public class Explode : MonoBehaviour
{
    // to randomize explosions apply a random force between this range to each fragment, can be modified in editor
    [SerializeField] private float _forceMin = 0f;
    [SerializeField] private float _forceMax = 1f;
    // specifies the radius of effect of the explosion
    [SerializeField] private float _radius = 5f;
    // when true all physics objects within a radius experience an explosion force, otherwise only the fragments do
    [SerializeField] private bool _isExternal = true;
    // optional damage to apply onto nearby objects when exploding, scaled weaker with distance
    [SerializeField] private float _damage = 2f;
    // time till the fracture fragments are deleted, to free memory
    protected float _delay = 5f;
    // stores references to all fragments of this object, used for fading fragments away before deletion
    protected Transform[] _fragments;
    // because blender models are imported at x100 scale for some reason, this value corrects the scaling when fading away fragments
    protected float _scaleCorrection = 100f;
    // (built-in function) first function called on object initialized/spawned
    void Awake()
    {
        // initialize based on the number of fragments this object has
        _fragments = new Transform[transform.childCount];
    }
    // (built-in function) called after the awake function is executed for all objects
    void Start()
    {
        // local variable cache for reuse, rather than redeclare per loop
        Rigidbody rb = null;
        // if apply explosion force to all nearby objects
        if (_isExternal)
        {
            // get reference to all nearby objects
            Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
            // iterate each object
            foreach (Collider collider in colliders)
            {
                // check if object is physics based, ignore otherwise
                rb = collider.GetComponent<Rigidbody>();
                if (rb == null) continue;
                // apply a random force to the object
                rb.AddExplosionForce(Random.Range(_forceMin, _forceMax), transform.position, _radius);
                // if explosion can damage objects
                if (_damage == 0f) continue;
                // and if object is of type breakable
                if (collider.gameObject.layer == game_variables.Instance.LayerBreakable)
                    // apply damage, scaling it weaker for objects further away
                    collider.transform.GetComponent<base_breakable>()?.ModifyHealthInst(-_damage * (1f - (rb.position - transform.position).magnitude / _radius));
            }
        }
        // initialize counter for fragments
        int counter = 0;
        // iterate each fragment
        foreach (Transform fragment in transform)
        {
            // store a reference to the fragment's rigidbody component that handles physics
            rb = fragment.GetComponent<Rigidbody>();
            // ignore if physics component not assigned
            if (rb == null) continue;
            // if self contained explosion, apply a random force to the object
            if (!_isExternal) rb.AddExplosionForce(Random.Range(_forceMin, _forceMax), transform.position, _radius);
            // set the fragment to delete after a delay
            Destroy(fragment.gameObject, _delay);
            // record a reference the fragment, for use later when scaling down
            _fragments[counter] = fragment;
            // onto the next fragment
            counter++;
        }
    }
    // (built-in function) executed every frame
    void Update()
    {
        // count down the self destruct timer
        _delay -= Time.deltaTime;
        // if timer fully ticked, delete the explosion object
        if (_delay < 0f) Destroy(gameObject);
        // if nearing deletion
        else if (_delay < 1f)
            // iterate all fragments
            for (int i = _fragments.Length - 1; i > -1; i--)
                // reduce each fragment's scale based on time left till deletion, accounting for import scale correction
                _fragments[i].localScale = Vector3.one * _delay * _scaleCorrection;
    }
    // (built-in function) used to render things in editor view for debugging purposes, when this object is selected
    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}