using UnityEngine;
using UnityEngine.UI;
// * for devog only *
public class dummy_uiCycle : MonoBehaviour
{
    private Image button = null;
    public Sprite[] sprites = new Sprite[2];
    public float timer = 1f;
    public float delay = 0f;
    private int counter = 0;
    // text color invert
    public Text label = null;
    public Color[] colors = new Color[2];
    void Awake()
    {
        button = GetComponent<Image>();
    }
    void Update()
    {
        // tock
        if (timer < 0f)
        {
            // reset
            timer += delay;
            // apply
            button.sprite = sprites[counter];
            if (label) label.color = colors[counter];
            // cycle
            counter++;
            if (counter == 2) counter = 0;
        }
        // tick
        else timer -= Time.deltaTime;
    }
}