// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// defines the behaviour of a trampoline
public class base_trampoline : MonoBehaviour
{
    // the force with which to repel any physics based object that touches the trampoline, can be modified in editor
    [SerializeField] private float _forceRebound = 10f;
    // (built-in function) executed when this object collides with another collider in the scene
    void OnCollisionEnter(Collision other)
    {
        // ignore collisions with interactive objects
        if (other.gameObject.layer == game_variables.Instance.LayerInteractive) return;
        // try and get a reference to the rigidbody component attached to the collided object
        Rigidbody rb = other.transform.GetComponent<Rigidbody>();
        // ignore if the object has no rigidbody component attached
        if (!rb) return;
        // rebound the colliding object away from the trampoline
        rb.AddForce(Vector3.Reflect((other.contacts[0].point - rb.position).normalized, other.contacts[0].normal).normalized * _forceRebound, ForceMode.Impulse);
    }
}