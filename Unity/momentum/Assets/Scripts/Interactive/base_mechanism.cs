// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// defines shared behaviour of all mechanism type objects like fan, powered door, etc
public class base_mechanism : MonoBehaviour
{
    // optional reference to a signal type object that can control this mechanism, can be assigned in editor
    [SerializeField] private base_signal _signal = null;
    // the current state of the mechanism, can be modified in editor
    [Tooltip("Default signal value (overridden by received signal)")] [SerializeField] protected bool _state = true;
    // for visualizing the mechanisms current state (true/false) in editor, can be modified in editor
    [SerializeField] protected float _radius = 1f;
    // (built-in function) executed when gameobject initialized/enabled
    void OnEnable()
    {
        // if no signal source assigned, throw a warning to the console
        if (_signal == null) Debug.LogWarning(gameObject.name + ": No object assigned for signal.", transform);
        // signal source assigned, bind the assigned signal's event to this mechanism's SignalEvent function
        else _signal.OnSignal += SignalEvent;
    }
    // (built-in function) executed when gameobject disabled
    void OnDisable()
    {
        // if signal source assigned, unbind the event handler function from the signal event
        if (_signal != null) _signal.OnSignal -= SignalEvent;
    }
    // respond to signal event
    private void SignalEvent(bool state)
    {
        // apply signal state
        _state = state;
    }
    // (built-in function) used to render things in editor view for debugging purposes, when this object is selected
    void OnDrawGizmosSelected()
    {
        // display current signal state
        Gizmos.color = _state ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
        // display link to signal source, if assigned
        if (_signal == null) return;
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, _signal.transform.position);
    }
}