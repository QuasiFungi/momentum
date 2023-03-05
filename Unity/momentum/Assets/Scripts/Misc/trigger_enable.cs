using UnityEngine;
// placeholder for a level end trigger, right now this only shows a dummy level clear/restart screen
public class trigger_enable : MonoBehaviour
{
    public GameObject target;
    // built-in function called when an object enter this objects hitbox
    void OnTriggerEnter(Collider other)
    {
        // enable the target object if the player touched this trigger
        if (other.gameObject.layer == game_variables.Instance.LayerPlayer) target.SetActive(true);
    }
}
