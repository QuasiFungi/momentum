// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// required when accessing UI elements in code
using UnityEngine.UI;
// package that allows access to generic lists datatype
using System.Collections.Generic;
// displays damage inflicted on objects as numbers on screen
public class feedback_health : MonoBehaviour
{
    // reference to singleton instance of this class
    public static feedback_health Instance;
    // template object that shows damage amount as text on screen
    public GameObject _display;
    // how long the numbers should stay on screen
    public float _durationPopup = 3f;
    // we'll be using instances of this class for each damage number being displayed
    protected class Message
    {
        // reference to the object that was damaged
        public Transform Source;
        // reference to the specific object thats rendering the damage amount text on screen
        public Transform Display;
        // records position of the source at the time it registers itself being damaged
        public Vector3 Position;
        //
        private int _cells;
        //
        private float _spacing;
        //
        private float _fill;
        //
        private float _fillMax;
        //
        private Cell[] Cells;
        //
        public CanvasGroup Color;
        //
        protected class Cell
        {
            //
            private RectTransform _cell;
            //
            private Image _bg;
            //
            private Image _fill;
            //
            public Cell(Transform cell, float rotation, float fill, float health)
            {
                //
                _cell = cell.GetComponent<RectTransform>();
                //
                _bg = cell.GetComponent<Image>();
                //
                _fill = cell.GetChild(0).GetComponent<Image>();
                //
                _cell.eulerAngles = Vector3.back * rotation;
                //
                Fill(fill, health);
            }
            private void Fill(float fill, float health)
            {
                //
                _bg.fillAmount = fill;
                //
                _fill.fillAmount = health;
            }
            public void SetFill(float fill)
            {
                _fill.fillAmount = fill;
            }
        }
        // keeps track of how long this damage number has been on screen
        public float Timer;
        // initialize damage for a new object
        public Message(Transform source, int cells, float spacing, float timer, float[] health)
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
            //
            Color = Display.GetComponent<CanvasGroup>();
            //
            _cells = cells;
            //
            _spacing = spacing;
            //
            _fill = 360f / cells - spacing;
            //
            _fillMax = _fill / 360f;
            //
            Cells = new Cell[cells];
            //
            Transform cell = Display.GetChild(0);
            //
            for (int i = cells - 1; i > -1; i--)
            {
                //print(i + " : " + health[i] + "" + health[i] * _fillMax);
                //
                if (i > 0) Cells[i] = new Cell(Instantiate(cell, Display, false).transform, CalculateRotation(i), _fillMax, health[i] * _fillMax);
                //
                else Cells[i] = new Cell(cell, CalculateRotation(i), _fillMax, health[i] * _fillMax);
            }
            // initialize the display timer
            Timer = timer;
            // record the damage amount
            //Damage = value;
        }
        //
        private float CalculateRotation(int id)
        {
            //
            if (id > 0) return CalculateRotation(id - 1) + _fill + _spacing;
            //
            else return _spacing / 2f;
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
                // reduce the colors opacity
                Color.alpha -= Time.deltaTime;
        }
        //
        public void SetFill(float[] health)
        {
            //
            Color.alpha = 1f;
            //
            for (int i = _cells - 1; i > -1; i--) Cells[i].SetFill(health[i] * _fillMax);
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
    public void Register(Transform source, int cells, float[] health, float spacing = 10f)
    {
        // check all existing damage displays
        foreach (Message message in _messages)
            // if this object is already registered
            if (message.Source == source)
            {
                // update this objects position
                message.Position = source.position;
                // reset the display timer
                message.Timer = _durationPopup;
                //
                message.SetFill(health);
                // exit out of the function
                return;
            }
        // create and record a new damage display
        _messages.Add(new Message(source, cells, spacing, _durationPopup, health));
    }
}
