using UnityEngine;
using UnityEngine.SceneManagement;
// placeholder for a level clear screen, right now this only has a level restart button
public class menu_clear : MonoBehaviour
{
    public void Trigger_LevelRestart()
    {
        // load scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}