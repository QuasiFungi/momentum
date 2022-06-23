// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// defines the behaviour of a powered door, inherit from mechanism to allow responding to signals
public class mechanism_doorPowered : base_mechanism
{
    // reference to animator component that handles object animation, to be assigned in editor
    [SerializeField] private Animator _anim = null;
    // (built-in function) executed every frame
    void Update()
    {
        // if true - trigger open animation if not already open, if false - trigger close animation if not already closed
        if (_state != _anim.GetBool("IsOpen")) _anim.SetBool("IsOpen", _state);
    }
}