// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// main Singleton class that handles miscellaneous variables used everywhere
public class game_variables : MonoBehaviour
{
    // allow access to the player controller from any script
    public static game_variables Instance;
    // references to particle effects based on each surface type
    private GameObject _particleDust;
    private GameObject _particleSpark;
    private GameObject _particleShard;
    private GameObject _particleSplinter;
    // (built-in function) first function called on object initialized/spawned
    void Awake()
    {
        // initialize a Singleton to ensure only one player can exist at a time, discarding any duplicates
        if (Instance) Destroy(gameObject);
        else Instance = this;
        // load all surface collision particle effects from the project's resources folder, type casted as a gameobject
        _particleDust = Resources.Load("Particle/ParticleDust") as GameObject;
        _particleSpark = Resources.Load("Particle/ParticleSpark") as GameObject;
        _particleShard = Resources.Load("Particle/ParticleShard") as GameObject;
        _particleSplinter = Resources.Load("Particle/ParticleSplinter") as GameObject;
    }
    // regions are good for organization since they can be minimzed and make it easier to read lengthy code
    // allow access to all loaded surface collision particle effects
    #region Particles
    public GameObject ParticleDust
    {
        get { return _particleDust; }
    }
    public GameObject ParticleSpark
    {
        get { return _particleSpark; }
    }
    public GameObject ParticleShard
    {
        get { return _particleShard; }
    }
    public GameObject ParticleSplinter
    {
        get { return _particleSplinter; }
    }
    #endregion
    // regions are good for organization since they can be minimzed and make it easier to read lengthy code
    // used when trying to detect if an object belongs to a specific category, like for damaging only breakable objects
    #region Layers
    public int LayerBreakable
    {
        get { return LayerMask.NameToLayer("Breakable"); }
    }
    public int LayerInteractive
    {
        get { return LayerMask.NameToLayer("Interactive"); }
    }
    public int LayerPlayer
    {
        get { return LayerMask.NameToLayer("Player"); }
    }
    #endregion
    // regions are good for organization since they can be minimzed and make it easier to read lengthy code
    // used by physics casts (rays/spheres/etc) to detect groups of layers, like for obstruction in fan's wind
    #region  Masks
    public LayerMask MaskRayObstruction
    {
        get { return LayerMask.GetMask("Default", "Interactive"); }
        // get { return LayerMask.GetMask("Default"); }
    }
    #endregion
}