using UnityEngine;
// * for devog only *
public class dummy_uiSpin : MonoBehaviour
{
    public float speed = 15f;
    private RectTransform knob;
    void Awake()
    {
        knob = transform.GetChild(0) as RectTransform;
    }
    void Update()
    {
        knob.localEulerAngles += Vector3.forward * speed * Time.deltaTime;
    }
}