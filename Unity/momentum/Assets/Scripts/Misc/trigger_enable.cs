using UnityEngine;
// placeholder for a level end trigger, right now this only shows a dummy level clear/restart screen
public class trigger_enable : MonoBehaviour
{
    public GameObject target;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == game_variables.Instance.LayerPlayer) target.SetActive(true);
    }
}