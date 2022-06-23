// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// allows access to Unity's Input System
using UnityEngine.InputSystem;
// makes it so this script is executed before all other user created scripts (unless specified otherwise similarly)
[DefaultExecutionOrder(-1)]
// main Singleton that takes data from Unity's input system and 
public class manager_input : MonoBehaviour
{
    // regions are good for organization since they can be minimzed and make it easier to read lengthy code
    // standard declarations for delegate/event pairs, optionally pass any parameters with delegate, like StartBrake(Vector2 position)
    #region Events
    // delegate/event pair for brake input
    public delegate void StartBrake();
    public event StartBrake OnStartBrake;
    // delegate/event pair for dash input
    public delegate void StartDash();
    public event StartDash OnStartDash;
    #endregion
    // allow access to the input manager from any script
    public static manager_input Instance;
    // local reference to the input controller script auto-generated by Unity's input system
    private controller_input _input;
    // local reference to the currently active camera being used to render to the device screen
    private Camera _camera;
    // local caches of input values for when an input is detected
    private Vector3 _positionPress;
    private Vector3 _positionDrag;
    // set to true when an input is held down, and false otherwise
    private bool _isTouch;
    // (built-in function) first function called on object initialized/spawned
    void Awake()
    {
        // initialize a Singleton to ensure only one input manager can exist at a time, discarding any duplicates
        if (Instance) Destroy(gameObject);
        else Instance = this;
        // create a new instance of the input system's auto-generated script that handles user input
        _input = new controller_input();
        // get reference to currently active camera in scene
        _camera = Camera.main;
        // bind the brake start EVENT of the input controller class to the brake FUNCTION of this class
        _input.Player.Brake.started += Brake;
        // bind the brake cancelled EVENT of the input controller class to the dash FUNCTION of this class
        _input.Player.Brake.canceled += Dash;
        // initialize input caches
        _positionPress = Vector3.zero;
        _positionDrag = Vector3.zero;
        // no input detected by default
        _isTouch = false;
    }
    // (built-in function) executed when gameobject initialized/enabled
    private void OnEnable()
    {
        // start detecting user inputs
        _input.Enable();
    }
    // (built-in function) executed when gameobject disabled
    private void OnDisable()
    {
        // stop detecting user inputs
        _input.Disable();
    }
    // (built-in function) executed every frame
    void Update()
    {
        // input is currently held down
        if (_isTouch)
        {
            // record current pointer position
            _positionDrag = _input.Player.Dash.ReadValue<Vector2>();
            // camera distance correction, prevents weird glitches later on when converting from screen position to world position
            _positionDrag.z = _camera.nearClipPlane;
        }
    }
    // function that handles brake event
    private void Brake(InputAction.CallbackContext context)
    {
        // record newly detected input values
        _positionPress = _input.Player.Dash.ReadValue<Vector2>();
        _positionDrag = _input.Player.Dash.ReadValue<Vector2>();
        // camera distance correction
        _positionPress.z = _camera.nearClipPlane;
        _positionDrag.z = _camera.nearClipPlane;
        // start recording the input drag position
        _isTouch = true;
        // trigger the brake event if there are any subscribers to it
        if (OnStartBrake != null) OnStartBrake();
    }
    // function that handles dash event
    private void Dash(InputAction.CallbackContext context)
    {
        // stop recording the input drag position
        _isTouch = false;
        // trigger the dash event if there are any subscribers to it
        if (OnStartDash != null) OnStartDash();
    }
    // regions are good for organization since they can be minimzed and make it easier to read lengthy code
    #region Properties
    // return the pointer position when a new input was detected
    public Vector2 PositionPress
    {
        get { return _positionPress; }
    }
    // return the pointer position when input detection ended
    public Vector2 PositionDrag
    {
        get { return _positionDrag; }
    }
    // return the pointer position when a new input was detected, converted to world coordinates
    public Vector3 PositionPressWorld
    {
        get { return _camera.ScreenToWorldPoint(_positionPress); }
    }
    // return the pointer position when input detection ended, converted to world coordinates
    public Vector3 PositionDragWorld
    {
        get { return _camera.ScreenToWorldPoint(_positionDrag); }
    }
    #endregion
}