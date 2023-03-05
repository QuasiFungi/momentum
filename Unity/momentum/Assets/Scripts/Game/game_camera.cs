// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// ensure that camera gameobject has a camera component attached else an error is thrown during compilation
[RequireComponent(typeof(Camera))]
// simple class that handles camera initializationa and tracking to  player
public class game_camera : MonoBehaviour
{
	// * for devlog only *
	public static game_camera Instance;
    // various initialization paramters for camera, can be modified in editor
    [SerializeField] private float _fieldOfView = 40f;
    [SerializeField] private float _nearClipPlane = 10f;
    [SerializeField] private float _farClipPlane = 50f;
    [SerializeField] private float _distance = -20f;
    // internal reference to camera component of this gameobject
    private Camera _camera;
    // * for devlog only *
    public bool _isFollow = true;
    public float _distanceX = 0f;
    public float _distanceY = 0f;
    // (built-in function) first function called on object initialized/spawned
    void Awake()
    {
    	// * for devlog only *
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        // get camera component attached to this gameobject
        _camera = transform.GetComponent<Camera>();
        // initialize the camera's field of view and near/far clip planes
        _camera.fieldOfView = _fieldOfView;
        _camera.nearClipPlane = _nearClipPlane;
        _camera.farClipPlane = _farClipPlane;
    }
    // (built-in function) executed every frame
    void Update()
    {
        // lock camera to player position at a fixed distance away from them
        // if (controller_player.Instance) transform.position = controller_player.Instance.Position - Vector3.forward * _distance;
    	// * for devlog only *
    	transform.position = new Vector3(_distanceX, _distanceY, _distance);
    	if (_isFollow) transform.position += controller_player.Instance.Position;
    }
    // used by damage amount display for tracking object position
    public Vector2 WorldToScreenPoint(Vector3 position)
    {
        return _camera.WorldToScreenPoint(position);
    }
    // * for devlog only *
    public float DistanceZ
    {
    	set
    	{
    		_distance = value;
    		// adjust far clip plane based on changed distance
    		_camera.farClipPlane = _farClipPlane - value;
    	}
    }
}
