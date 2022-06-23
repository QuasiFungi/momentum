// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// built in c# tool, allows defining lists of all types
using System.Collections.Generic;
// defines the behaviour of a trolley vehicle
public class base_trolley : MonoBehaviour
{
    // reference to all attched wheels of this trolley, to be assigned in editor
    [SerializeField] private List<WheelInfo> _wheels = new List<WheelInfo>();
    // (built-in function) first function called on object initialized/spawned
    void Awake()
    {
        // set the rotation of all wheels so they move along the X and Y axes
        foreach (WheelInfo wheel in _wheels) wheel.collider.steerAngle = 90f;
    }
    // (built-in function) executed when Unity updates all physics objects in the scene
    void FixedUpdate()
    {
        // local variables for reuse
        Vector3 position;
        Quaternion rotation;
        // iterate all assigned wheels of this trolley
        foreach (WheelInfo wheel in _wheels)
        {
            // apply local position/rotation to each wheel's visuals, since they are separate from the colliders that perform the physics movement
            // - get the position/rotation of the wheel physics collider
            wheel.collider.GetWorldPose(out position, out rotation);
            // - apply the position to the wheel mesh
            wheel.mesh.position = position;
            // - apply the rotation to the wheel mesh, correcting for only X Y movement
            wheel.mesh.rotation = rotation * Quaternion.Euler(0f, 90f, 0f);
        }
    }
}
// ..?
[System.Serializable]
public class WheelInfo {
    public WheelCollider collider;
    public Transform mesh;
}