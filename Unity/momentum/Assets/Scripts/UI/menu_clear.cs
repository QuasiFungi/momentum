using UnityEngine;
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
    // called by the level 0 button
    public void Trigger_Level0()
    {
        // load the level marked 0 in the build menu
        SceneManager.LoadScene(0);
    }
    // called by the level 1 button
    public void Trigger_Level1()
    {
        // load the level marked 1 in the build menu
        SceneManager.LoadScene(1);
    }
    // called by the level 2 button
    public void Trigger_Level2()
    {
        // load the level marked 2 in the build menu
        SceneManager.LoadScene(2);
    }
}
