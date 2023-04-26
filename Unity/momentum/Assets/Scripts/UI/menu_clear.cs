using UnityEngine;
//
using UnityEngine.SceneManagement;
// placeholder for a level clear screen
public class menu_clear : MonoBehaviour
{
    // built-in function called when gameObject enabled
    void OnEnable()
    {
        // pause gameplay on show
        Time.timeScale = 0f;
    }
    // called by the level restart button
    public void Trigger_LevelRestart()
    {
        // reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // called by the level buttons
    public void Trigger_Level(int id)
    {
        // load the level with given ID in the build menu
        SceneManager.LoadScene(id);
    }
}
