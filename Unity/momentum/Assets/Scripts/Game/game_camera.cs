// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// ensure that camera gameobject has a camera component attached else an error is thrown during compilation
[RequireComponent(typeof(Camera))]
// simple class that handles camera initializationa and tracking to  player
public class game_camera : MonoBehaviour
{
    // various initialization paramters for camera, can be modified in editor
    [SerializeField] private float _fieldOfView = 40f;
    [SerializeField] private float _nearClipPlane = 10f;
    [SerializeField] private float _farClipPlane = 50f;
    [SerializeField] private float _distance = 20f;
    // internal reference to camera component of this gameobject
    private Camera _camera;
    // (built-in function) first function called on object initialized/spawned
    void Awake()
    {
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
        if (controller_player.Instance) transform.position = controller_player.Instance.Position - Vector3.forward * _distance;
    }
}