using UnityEngine;
using UnityEngine.UI;
public class menu_dead : MonoBehaviour
{
    public float _delay = 1f;
    public float _speed = 1f;
    private float _opacity = 0f;
    private CanvasGroup _menu = null;
    void Awake()
    {
        _menu = GetComponent<CanvasGroup>();
        _menu.alpha = 0f;
        _menu.blocksRaycasts = false;
    }
    void Update()
    {
        if (controller_player.Instance) return;
        //
        _menu.alpha = _opacity;
        if (!_menu.blocksRaycasts) _menu.blocksRaycasts = true;
        //
        if (_delay > 0f) _delay -= Time.deltaTime;
        else if (_opacity < 1f) _opacity += Time.deltaTime * _speed;
        else if (_opacity > 1f) _opacity = 1f;
    }
}
