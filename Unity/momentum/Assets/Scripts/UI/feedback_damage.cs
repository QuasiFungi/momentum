// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// required when accessing UI elements in code
using UnityEngine.UI;
// package that allows access to generic lists datatype
using System.Collections.Generic;
// displays damage inflicted on objects as numbers on screen
public class feedback_damage : MonoBehaviour
{
    // reference to singleton instance of this class
    public static feedback_damage Instance;
    // template object that shows damage amount as text on screen
    public GameObject _display;
    // how long the numbers should stay on screen
    public float _durationPopup = 3f;
    // the color of the text elements showing damage amounts
    public Color _tint = Color.white;
    // we'll be using instances of this class for each damage number being displayed
    protected class Message
    {
        // reference to the object that was damaged
        public Transform Source;
        // reference to the specific object thats rendering the damage amount text on screen
        public Transform Display;
        // records position of the source at the time it registers itself being damaged
        public Vector3 Position;
        // the current registered damage amount for this object, repeated damage adds up
        public float Damage;
        // text component that is displaying the damage amount
        public Text Text;
        // keeps track of how long this damage number has been on screen
        public float Timer;
        // initialize damage for a new object
        public Message(Transform source, float value, float timer)
        {
            // store a reference to the calling object
            Source = source;
            // create a new copy of the object that will render the damage text
            Display = Instantiate(Instance._display, Instance.transform, false).transform;
            // record the world position of the calling object
            Position = source.position;
            // convert the objects world position to screen position
            Display.position = game_camera.Instance.WorldToScreenPoint(Position);
            // start showing the damage text on screen
            Display.gameObject.SetActive(true);
            // get a reference to the text component of the object showing the damage amount
            Text = Display.GetComponent<Text>();
            // show the rounded off damage amount
            Text.text = Mathf.Abs(value).ToString("F2");
            // set the color of the text
            Text.color = Instance._tint;
            // initialize the display timer
            Timer = timer;
            // record the damage amount
            Damage = value;
        }
        // called every frame to update the dsplayed text position and color
        public void Tick()
        {
            // update the damage text position
            Display.position = game_camera.Instance.WorldToScreenPoint(Position);
            // tick down the display timer
            Timer -= Time.deltaTime;
            // if timer nearly run out
            if (Timer < 1f)
            {
                // ? garbage collection
                // get the current text color
                Color tint = Text.color;
                // reduce the colors opacity
                tint.a -= Time.deltaTime;
                // apply the updated color to the text
                Text.color = tint;
            }
        }
    }
    // the currently active damage texts
    protected List<Message> _messages;
    // temporary storage for text displays to be discarded
    protected List<Message> _toRemove;
    // built-in function called once when object first initializes
    void Awake()
    {
        // initialize a singleton instance for this class
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        //// ? use a prefab
        //_display = Resources.Load("UI/Feedback/display_damageAmount") as GameObject;
        // initialize lists for both messages
        _messages = new List<Message>();
        _toRemove = new List<Message>();
    }
    // built-in function called every frame
    void Update()
    {
        // update all damage displays if any
        foreach (Message message in _messages)
        {
            // this display can still be displayed
            if (message.Timer > 0) message.Tick();
            // mark this display to discard
            else _toRemove.Add(message);
        }
        // process any displays to discard
        foreach (Message message in _toRemove)
        {
            // destroy this display
            message.Display.GetComponent<SelfDestruct>().Trigger();
            // discard the reference for this display
            _messages.Remove(message);
        }
        // empty the display discard list
        _toRemove.Clear();
    }
    // used by objects to register themselves when hurt ? prefix suffix
    public void Register(Transform source, float value, float timer = -1f)
    {
        // check all existing damage displays
        foreach (Message message in _messages)
            // if this object is already registered
            if (message.Source == source)
            {
                // update this objects position
                message.Position = source.position;
                // reset the display timer
                message.Timer = timer < 0f ? _durationPopup : timer;
                // increment the damage amount
                message.Damage += value;
                // show the rounded off damage amount
                message.Text.text = Mathf.Abs(message.Damage).ToString("F2");
                // exit out of the function
                return;
            }
        // create and record a new damage display
        _messages.Add(new Message(source, value, timer < 0f ? _durationPopup : timer));
    }
}
