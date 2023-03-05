// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// displays the input pointer's press and drag positions for visualizing expected player movement
public class feedback_touch : MonoBehaviour
{
    // reference to different parts of the ui display
    // - showed when braking
    public RectTransform _iconPress = null;
    // - rotates based on the direction of the dragged pointer
    public RectTransform _iconDragAnchor = null;
    // - enabled when dash is ready
    public RectTransform _iconDrag = null;
    // - shows the strength of the dash if executed
    public RectTransform _iconDragAmount = null;
    // reference to singleton instance of this class
    public static feedback_touch Instance;
    // (built-in function) first function called on object initialized/spawned
    void Awake()
    {
        // initialize singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        // bind the brake EVENT of input manager class to the press FUNCTION of this class
        manager_input.Instance.OnStartBrake += TouchPress;
        // bind the dash EVENT of input manager class to the release FUNCTION of this class
        manager_input.Instance.OnStartDash += TouchRelease;
        // by default, disable the objects that show the input
        _iconPress.gameObject.SetActive(false);
        _iconDrag.gameObject.SetActive(false);
    	_iconDragAmount.gameObject.SetActive(false);
    }
    // executed when brake input detected by input manager
    private void TouchPress()
    {
        // update displayed input pointer positions to detected input
        // show the brake icon
        _iconPress.gameObject.SetActive(true);
        // move the brake icon to the current pointer position
        _iconPress.position = manager_input.Instance.PositionPress;
        // scale the brake and dash amount icons to brake input sensitivity
        _iconPress.sizeDelta = Vector2.one * controller_player.Instance.SensitivityBrake * 2f;
        _iconDrag.sizeDelta = Vector2.one * controller_player.Instance.SensitivityBrake * .3f;
    	// move the drag direction icon based on the brake input sensitivity
    	_iconDrag.localPosition = Vector2.right * controller_player.Instance.SensitivityBrake * .96f;
    }
    // executed when dash input detected by input manager
    private void TouchRelease()
    {
        // hide all UI that shows the input
        _iconPress.gameObject.SetActive(false);
        _iconDrag.gameObject.SetActive(false);
    	_iconDragAmount.gameObject.SetActive(false);
    }
    // called by the player class to provide the current status of input ? weird data routing
    public void SetDrag(bool isDrag, float angle, float magnitude)
    {
        // show/hide the drag direction and amount icons
    	_iconDrag.gameObject.SetActive(isDrag);
    	_iconDragAmount.gameObject.SetActive(isDrag);
    	// rotate the drag direction icon based on the recieved pointer angle
    	_iconDragAnchor.eulerAngles = Vector3.forward * angle;
    	// scale the drag amount icon based on the recieved drag amount
    	_iconDragAmount.localScale = Vector3.one * magnitude;
    }
}
