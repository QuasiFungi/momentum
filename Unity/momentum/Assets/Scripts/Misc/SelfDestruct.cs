// standard import for all objects that perform behaviours based on Unity's object lifecycle
using UnityEngine;
// used to delete objects after a delay or when triggered by some event
public class SelfDestruct : MonoBehaviour
{
    // delay till object is deleted, can be modifed in editor
    [SerializeField] private float _timer = -1f;
    // (built-in function) executed every frame
    void Update()
    {
        // ignore if object deletion set to event based only
        if (_timer == -1f) return;
        // tick deletion timer
        _timer -= Time.deltaTime;
        // delete if timer fully ticked
        if (_timer <= 0) Destroy(gameObject);
    }
    // used to delete this object from another script
    public void Trigger()
    {
        // initiate deletion immediately
        _timer = 0f;
    }
}