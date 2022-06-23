// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// defines shared behaviour of all signal type objects like button, switch, etc
public class base_signal : MonoBehaviour
{
    // if enabled, send false when activated and true otherwise
    [Tooltip("Invert signal values")] [SerializeField] protected bool _isInvert = false;
    // the current state of the signal, can be modified in editor
    [Tooltip("Default signal value")] [SerializeField] private bool _state = false;
    // for visualizing the mechanisms current state (true/false) in editor, can be modified in editor
    [SerializeField] protected float _radius = 1f;
    // regions are good for organization since they can be minimzed and make it easier to read lengthy code
    #region Events
    // delegate/event pair for signal activation
    public delegate void SignalAction(bool state);
    public event SignalAction OnSignal;
    #endregion
    // used internally to set the correct signal state and then fire the signal event for any listening/subscribed mechanisms
    protected void SetSignal(bool state)
    {
        // safety, ignore if no change in current state and new value detected
        if ((!_isInvert && _state == state) || (_isInvert && _state != state)) return;
        // update current state to new value, but invert the value if the inversion flag is assigned
        _state = _isInvert ? !state : state;
        // fire the event for all registered subscribers, if any exist
        if (OnSignal != null) OnSignal(_state);
    }
    // (built-in function) first function called on object initialized/spawned
    protected virtual void Awake()
    {
        // inversion plus condition, this ensures that an event always fires at Awake, while also setting the state's value to the assigned default
        // - value correction for invert flag
        _state = !_state;
        // - reset to assigned default value
        SetSignal(_isInvert ? _state : !_state);
    }
    // (built-in function) used to render things in editor view for debugging purposes, when this object is selected
    void OnDrawGizmosSelected()
    {
        // Display the default signal state
        Gizmos.color = _state ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
    // regions are good for organization since they can be minimzed and make it easier to read lengthy code
    #region Properties
    // signal's current state
    protected bool State
    {
        // allow access to child classes
        get { return _state; }
    }
    #endregion
}