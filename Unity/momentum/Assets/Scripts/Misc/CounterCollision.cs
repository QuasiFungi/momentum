using UnityEngine;
using UnityEngine.UI;
public class CounterCollision : MonoBehaviour
{
    public Text _collisionCurrent = null;
    public Text _collisionTotal = null;
    void Update()
    {
        _collisionCurrent.text = "COLLISIONS ~ " + controller_player.Instance.CounterCollision.ToString();
        if (Time.timeScale == 0f) _collisionTotal.text = _collisionCurrent.text;
    }
}
