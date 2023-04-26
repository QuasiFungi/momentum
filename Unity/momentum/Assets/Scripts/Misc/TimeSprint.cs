// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// built-in Unity package for using UI features in code like Text, Buttons etc
using UnityEngine.UI;
//
using UnityEngine.SceneManagement;
// dummy class that measures time taken between two triggers being activated, for testing player speed
public class TimeSprint : MonoBehaviour
{
    public Text _timerCurrent = null;
    public Text _timerLast = null;
    public Text _timerBest = null;
    public GameObject _menuClear = null;
    private float _timer = 0f;
    private bool _isUpdated = false;
    private string _labelSave = "";
    void Awake()
    {
        _labelSave = "timeBest" + SceneManager.GetActiveScene().buildIndex;
    }
    // is disabled by default, enabled by the timer start trigger (the yellow one in LevelSprint)
    void OnEnable()
    {
        _timer = 0f;
        _isUpdated = false;
    }
    void Update()
    {
        if (_isUpdated || !controller_player.Instance) return;
        // update the timer on screen
        if (Time.timeScale > 0f)
        {
            _timer += Time.deltaTime;
            _timerCurrent.text = "TIME ~ " + _timer.ToString("F2");
        }
        // menu clear not visible
        else if (!_menuClear.activeSelf) return;
        // save best time to memory
        else if (!PlayerPrefs.HasKey(_labelSave)) PlayerPrefs.SetFloat(_labelSave, _timer);
        // update various text on the level clear screen
        else
        {
            _isUpdated = true;
            PlayerPrefs.SetFloat("timeLast", _timer);
            if (PlayerPrefs.GetFloat(_labelSave, float.MaxValue) > _timer) PlayerPrefs.SetFloat(_labelSave, _timer);
            _timerLast.text = "LAST TIME ~ " + _timer.ToString("F2");
            _timerBest.text = "BEST TIME ~ " + PlayerPrefs.GetFloat(_labelSave, 0f).ToString("F2");
        }
    }
    // * temporary * reset best time
    public void Clear_TimeBest()
    {
        PlayerPrefs.SetFloat(_labelSave, float.MaxValue);
        _timerBest.text = "BEST TIME ~ 0.00";
    }
}
