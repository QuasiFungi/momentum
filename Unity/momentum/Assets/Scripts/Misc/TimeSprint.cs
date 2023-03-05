// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// built-in Unity package for using UI features in code like Text, Buttons etc
using UnityEngine.UI;
// dummy class that measures time taken between two triggers being activated, for testing player speed
public class TimeSprint : MonoBehaviour
{
    public Text _timerCurrent = null;
    public Text _timerLast = null;
    public Text _timerBest = null;
    private float _timer;
    private bool _isUpdated;
    // is disabled by default, enabled by the timer start trigger (the yellow one in LevelSprint)
    void OnEnable()
    {
        _timer = 0f;
        _isUpdated = false;
    }
    void Update()
    {
        if (_isUpdated) return;
        // update the timer on screen
        if (Time.timeScale > 0f)
        {
            _timer += Time.deltaTime;
            _timerCurrent.text = "TIME ~ " + _timer.ToString("F2");
        }
        // save best time to memory ? give reset option
        else if (!PlayerPrefs.HasKey("timeBest")) PlayerPrefs.SetFloat("timeBest", _timer);
        // update various text on the level clear screen
        else
        {
            _isUpdated = true;
            PlayerPrefs.SetFloat("timeLast", _timer);
            if (PlayerPrefs.GetFloat("timeBest", 0f) > _timer) PlayerPrefs.SetFloat("timeBest", _timer);
            _timerLast.text = "LAST TIME ~ " + _timer.ToString("F2");
            _timerBest.text = "BEST TIME ~ " + PlayerPrefs.GetFloat("timeBest", 0f).ToString("F2");
        }
    }
}
