// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// displays the input pointer's press and drag positions for visualizing expected player movement
public class feedback_touch : MonoBehaviour
{
    // reference to the object that draws the input pointer path on screen
    [SerializeField] private LineRenderer _touchDirection = null;
    // (built-in function) first function called on object initialized/spawned
    void Awake()
    {
        // bind the brake EVENT of input manager class to the press FUNCTION of this class
        manager_input.Instance.OnStartBrake += TouchPress;
        // bind the dash EVENT of input manager class to the release FUNCTION of this class
        manager_input.Instance.OnStartDash += TouchRelease;
        // by default, disable the object that shows the input
        _touchDirection.gameObject.SetActive(false);
    }
    // executed when brake input detected by input manager
    private void TouchPress()
    {
        // update displayed input pointer positions to detected input
        _touchDirection.SetPosition(0, manager_input.Instance.PositionPressWorld);
        _touchDirection.SetPosition(1, manager_input.Instance.PositionDragWorld);
        // 
        _touchDirection.gameObject.SetActive(true);
    }
    // (built-in function) executed every frame
    void Update()
    {
        // ignore if no touch input
        if (!_touchDirection.gameObject.activeSelf) return;
        // update displayed input pointer positions to detected input
        _touchDirection.SetPosition(0, manager_input.Instance.PositionPressWorld);
        _touchDirection.SetPosition(1, manager_input.Instance.PositionDragWorld);
    }
    // executed when dash input detected by input manager
    private void TouchRelease()
    {
        // disable the object that shows the input
        _touchDirection.gameObject.SetActive(false);
    }
}