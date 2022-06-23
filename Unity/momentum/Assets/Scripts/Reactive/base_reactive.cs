// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// defines the behaviour for the display panels that show whether a signal object is active/inactive
public class base_reactive : MonoBehaviour
{
    // reference to the signal object whose state this display will show, to be assigned in editor
    [SerializeField] private base_signal _signal = null;
    // texture coordinates for when to display an active or inactive signal state
    [SerializeField] protected Vector2 _offsetOff = Vector2.zero;
    [SerializeField] protected Vector2 _offsetOn = Vector2.zero;
    // reference to the component that handles object materials
    private Renderer _renderer;
    // (built-in function) first function called on object initialized/spawned
    void Awake()
    {
        // get the reference to this object's component that handles materials
        _renderer = GetComponent<Renderer>();
    }
    // (built-in function) executed when gameobject initialized/enabled
    void OnEnable()
    {
        // if no signal source is assigned, issue a warning to the console
        if (_signal == null) Debug.LogWarning(gameObject.name + ": No object assigned for signal.", transform);
        // otherwise, subscribe the event function to the incoming signal
        else _signal.OnSignal += SignalEvent;
    }
    // (built-in function) executed when gameobject disabled
    void OnDisable()
    {
        // unsubscribe the event function from the incoming signal, if it exists
        if (_signal != null) _signal.OnSignal -= SignalEvent;
    }
    // function that responds to the signal event
    void SignalEvent(bool state)
    {
        // apply the appropriate texture offset
        _renderer.material.mainTextureOffset = state ? _offsetOn : _offsetOff;
    }
    // (built-in function) used to render things in editor view for debugging purposes, when this object is selected
    void OnDrawGizmosSelected()
    {
        // ignore if no signal source is assigned
        if (_signal == null) return;
        // display link to signal source
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position + Vector3.forward * 2.5f, _signal.transform.position);
    }
}