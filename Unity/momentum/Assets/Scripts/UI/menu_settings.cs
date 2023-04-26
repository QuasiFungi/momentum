using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// placeholder for testing various player, environment and camera settings to see what feels best
public class menu_settings : MonoBehaviour
{
    // external reference to each specific setting, the text value displays (if any), and the default values
    public Slider control_mass = null;
    public Text value_mass = null;
    private float mass = 1;
    public Slider control_speed = null;
    public Text value_speed = null;
    private float speed = 10;
    public Slider control_terminal = null;
    public Text value_terminal = null;
    private float terminal = 30;
    public Slider control_brake = null;
    public Text value_brake = null;
    private float brake = 1f;
    public Slider control_size = null;
    public Text value_size = null;
    private float size = 1;
    public Slider control_trail = null;
    public Text value_trail = null;
    private float trail = 1;
    public Slider control_damage = null;
    public Text value_damage = null;
    private float damage = .1f;
    public Slider control_stall = null;
    public Text value_stall = null;
    private float stall = .5f;
    public Slider control_boost = null;
    public Text value_boost = null;
    private float boost = .3f;
    public Slider control_steer = null;
    public Text value_steer = null;
    private float steer = 1f;
    public Slider control_sizeBrake = null;
    public Text value_sizeBrake = null;
    private float sizeBrake = 50f;
    public Slider control_sizeDash = null;
    public Text value_sizeDash = null;
    private float sizeDash = 200f;
    public Slider control_speedBoost = null;
    public Text value_speedBoost = null;
    private float speedBoost = 10f;
    //
    public Slider control_drag = null;
    public Text value_drag = null;
    private float drag = 1;
    public Slider control_gravX = null;
    public Text value_gravX = null;
    private float gravX = 0;
    public Slider control_gravY = null;
    public Text value_gravY = null;
    private float gravY = -10;
    public Slider control_gravZ = null;
    public Text value_gravZ = null;
    private float gravZ = 0;
    //
    public Dropdown control_type = null;
    private int type = 0;
    public Slider control_posX = null;
    public Text value_posX = null;
    private float posX = 0;
    public Slider control_posY = null;
    public Text value_posY = null;
    private float posY = 0;
    public Slider control_posZ = null;
    public Text value_posZ = null;
    private float posZ = -20;
    public Dropdown control_fps = null;
    private int fps = 1;
    void Awake()
    {
        // assiging a function to be called each time the value for a particular setting is changed
        control_mass.onValueChanged.AddListener(delegate{ChangedSlider(control_mass, value_mass);});
        control_speed.onValueChanged.AddListener(delegate{ChangedSlider(control_speed, value_speed);});
        control_terminal.onValueChanged.AddListener(delegate{ChangedSlider(control_terminal, value_terminal);});
        control_brake.onValueChanged.AddListener(delegate{ChangedSlider(control_brake, value_brake, "F1", .1f);});
        control_size.onValueChanged.AddListener(delegate{ChangedSlider(control_size, value_size);});
        control_trail.onValueChanged.AddListener(delegate{ChangedSlider(control_trail, value_trail);});
        control_damage.onValueChanged.AddListener(delegate{ChangedSlider(control_damage, value_damage, "F1", .1f);});
        control_stall.onValueChanged.AddListener(delegate{ChangedSlider(control_stall, value_stall, "F1", .1f);});
        control_boost.onValueChanged.AddListener(delegate{ChangedSlider(control_boost, value_boost, "F1", .1f);});
        control_steer.onValueChanged.AddListener(delegate{ChangedSlider(control_steer, value_steer, "F1", .1f);});
        control_sizeBrake.onValueChanged.AddListener(delegate{ChangedSlider(control_sizeBrake, value_sizeBrake);});
        control_sizeDash.onValueChanged.AddListener(delegate{ChangedSlider(control_sizeDash, value_sizeDash);});
        control_speedBoost.onValueChanged.AddListener(delegate{ChangedSlider(control_speedBoost, value_speedBoost);});
        //
        control_drag.onValueChanged.AddListener(delegate{ChangedSlider(control_drag, value_drag);});
        control_gravX.onValueChanged.AddListener(delegate{ChangedSlider(control_gravX, value_gravX);});
        control_gravY.onValueChanged.AddListener(delegate{ChangedSlider(control_gravY, value_gravY);});
        control_gravZ.onValueChanged.AddListener(delegate{ChangedSlider(control_gravZ, value_gravZ);});
        //
        control_posX.onValueChanged.AddListener(delegate{ChangedSlider(control_posX, value_posX);});
        control_posY.onValueChanged.AddListener(delegate{ChangedSlider(control_posY, value_posY);});
        control_posZ.onValueChanged.AddListener(delegate{ChangedSlider(control_posZ, value_posZ);});
        // make sure all settings values start off as their defaults
        ParametersReset();
    }
    // handles what to do when the value of a slider changes
    public void ChangedSlider(Slider control, Text value, string unit = "F0", float scale = 1f)
    {
        // update the displayed value for this settings option on the screen
        value.text = (control.value * scale).ToString(unit);
    }
    // built-in function called each time this gameObject is shown
    void OnEnable()
    {
        // pause the game when this menu is open
        Time.timeScale = 0f;
    }
    // built-in function called each time this gameObject is hidden
    void OnDisable()
    {
        // unpause the game when this menu is closed
        Time.timeScale = 1f;
        // push the modified settings values onto each object that uses them
        ParametersApply();
    }
    // used by the param button to show/hide the settings menu
    public void ToggleActive()
    {
        // if this menu is visible, hide it, and vice versa
    	gameObject.SetActive(!gameObject.activeSelf);
    }
    // called by the reset button
    public void ParametersReset()
    {
        // reset all settings values to the default ones and update the displays (if any)
        //player
        //--mass
        control_mass.value = mass;
        value_mass.text = mass.ToString();
        //--speed
        control_speed.value = speed;
        value_speed.text = speed.ToString();
        //--terminal
        control_terminal.value = terminal;
        value_terminal.text = terminal.ToString();
        //--brake
        control_brake.value = brake * 10f;
        value_brake.text = brake.ToString("F1");
        //--size
        control_size.value = size;
        value_size.text = size.ToString();
        //--trail
        control_trail.value = trail;
        value_trail.text = trail.ToString();
        //--damage
        control_damage.value = damage * 10f;
        value_damage.text = damage.ToString("F1");
        //--stall
        control_stall.value = stall * 10f;
        value_stall.text = stall.ToString("F1");
        //--boost
        control_boost.value = boost * 10f;
        value_boost.text = boost.ToString("F1");
        //--steer
        control_steer.value = steer * 10f;
        value_steer.text = steer.ToString("F1");
        //--sizeBrake
        control_sizeBrake.value = sizeBrake;
        value_sizeBrake.text = sizeBrake.ToString();
        //--sizeDash
        control_sizeDash.value = sizeDash;
        value_sizeDash.text = sizeDash.ToString();
        //--speedBoost
        control_speedBoost.value = speedBoost;
        value_speedBoost.text = speedBoost.ToString();
        //environment
        //--drag
        control_drag.value = drag;
        value_drag.text = drag.ToString();
        //--gravX
        control_gravX.value = gravX;
        value_gravX.text = gravX.ToString();
        //--gravY
        control_gravY.value = gravY;
        value_gravY.text = gravY.ToString();
        //--gravZ
        control_gravZ.value = gravZ;
        value_gravZ.text = gravZ.ToString();
        //camera
        //--type
        control_type.value = type;
        //--posX
        control_posX.value = posX;
        value_posX.text = posX.ToString();
        //--posY
        control_posY.value = posY;
        value_posY.text = posY.ToString();
        //--posZ
        control_posZ.value = posZ;
        value_posZ.text = posZ.ToString();
        //--fps
        control_fps.value = fps;
    }
    // push all modified settings values onto the objects that use them
    private void ParametersApply()
    {
        //player
        if (controller_player.Instance)
        {
            //--mass
            controller_player.Instance.Mass = control_mass.value;
            //--speed
            controller_player.Instance.SpeedDash = control_speed.value;
            //--terminal
            controller_player.Instance.SpeedTerminal = control_terminal.value;
            //--brake
            controller_player.Instance.DragBrake = control_brake.value * .1f;
            //--size
            controller_player.Instance.Size = control_size.value;
            //--trail
            controller_player.Instance.Trail = control_trail.value;
            //--damage
            controller_player.Instance.Damage = control_damage.value * .1f;
            //--stall
            controller_player.Instance.Stall = control_stall.value * .1f;
            //--boost
            controller_player.Instance.Boost = control_boost.value * .1f;
            //--steer
            controller_player.Instance.Steer = control_steer.value * .1f;
            //--sizeBrake
            controller_player.Instance.SensitivityBrake = control_sizeBrake.value;
            //--sizeDash
            controller_player.Instance.SensitivityDash = control_sizeDash.value;
            //--speedBoost
            controller_player.Instance.SpeedBoost = control_speedBoost.value;
            //environment
            //--drag
            controller_player.Instance.Drag = control_drag.value;
        }
        //--gravX
        //--gravY
        //--gravZ
        Physics.gravity = new Vector3(control_gravX.value, control_gravY.value, control_gravZ.value);
        //camera
        //--type
        game_camera.Instance._isFollow = control_type.value == 0;
        //--posX
        game_camera.Instance._distanceX = control_posX.value;
        //--posY
        game_camera.Instance._distanceY = control_posY.value;
        //--posZ
        game_camera.Instance.DistanceZ = control_posZ.value;
        //--fps
        Application.targetFrameRate = control_fps.value == 0 ? 30 : 60;
    }
    // called by the load button
    public void ParametersLoad()
    {
        // use the settings configuration that was last saved to memory, use defaults if there is no saved data, and update the displayed values
        //player
        //--mass
        control_mass.value = PlayerPrefs.GetFloat("mass", mass);
        value_mass.text = control_mass.value.ToString();
        //--speed
        control_speed.value = PlayerPrefs.GetFloat("speed", speed);
        value_speed.text = control_speed.value.ToString();
        //--terminal
        control_terminal.value = PlayerPrefs.GetFloat("terminal", terminal);
        value_terminal.text = control_terminal.value.ToString();
        //--brake
        control_brake.value = PlayerPrefs.GetFloat("brake", brake * 10f);
        value_brake.text = (control_brake.value * .1f).ToString("F1");
        //--size
        control_size.value = PlayerPrefs.GetFloat("size", size);
        value_size.text = control_size.value.ToString();
        //--trail
        control_trail.value = PlayerPrefs.GetFloat("trail", trail);
        value_trail.text = control_trail.value.ToString();
        //--damage
        control_damage.value = PlayerPrefs.GetFloat("damage", damage * 10f);
        value_damage.text = (control_damage.value * .1f).ToString("F1");
        //--stall
        control_stall.value = PlayerPrefs.GetFloat("stall", stall * 10f);
        value_stall.text = (control_stall.value * .1f).ToString("F1");
        //--boost
        control_boost.value = PlayerPrefs.GetFloat("boost", boost * 10f);
        value_boost.text = (control_boost.value * .1f).ToString("F1");
        //--steer
        control_steer.value = PlayerPrefs.GetFloat("steer", steer * 10f);
        value_steer.text = (control_steer.value * .1f).ToString("F1");
        //--sizeBrake
        control_sizeBrake.value = PlayerPrefs.GetFloat("sizeBrake", sizeBrake);
        value_sizeBrake.text = control_sizeBrake.value.ToString();
        //--sizeDash
        control_sizeDash.value = PlayerPrefs.GetFloat("sizeDash", sizeDash);
        value_sizeDash.text = control_sizeDash.value.ToString();
        //--speedBoost
        control_speedBoost.value = PlayerPrefs.GetFloat("speedBoost", speedBoost);
        value_speedBoost.text = control_speedBoost.value.ToString();
        //environment
        //--drag
        control_drag.value = PlayerPrefs.GetFloat("drag", drag);
        value_drag.text = control_drag.value.ToString();
        //--gravX
        control_gravX.value = PlayerPrefs.GetFloat("gravX", gravX);
        value_gravX.text = control_gravX.value.ToString();
        //--gravY
        control_gravY.value = PlayerPrefs.GetFloat("gravY", gravY);
        value_gravY.text = control_gravY.value.ToString();
        //--gravZ
        control_gravZ.value = PlayerPrefs.GetFloat("gravZ", gravZ);
        value_gravZ.text = control_gravZ.value.ToString();
        //camera
        //--type
        control_type.value = PlayerPrefs.GetInt("type", type);
        //--posX
        control_posX.value = PlayerPrefs.GetFloat("posX", posX);
        value_posX.text = control_posX.value.ToString();
        //--posY
        control_posY.value = PlayerPrefs.GetFloat("posY", posY);
        value_posY.text = control_posY.value.ToString();
        //--posZ
        control_posZ.value = PlayerPrefs.GetFloat("posZ", posZ);
        value_posZ.text = control_posZ.value.ToString();
        //--fps
        control_fps.value = PlayerPrefs.GetInt("fps", fps);
    }
    // called by the save button
    public void ParametersSave()
    {
        // push the current settings configuration to memory
        //player
        //--mass
        PlayerPrefs.SetFloat("mass", control_mass.value);
        //--speed
        PlayerPrefs.SetFloat("speed", control_speed.value);
        //--terminal
        PlayerPrefs.SetFloat("terminal", control_terminal.value);
        //--brake
        PlayerPrefs.SetFloat("brake", control_brake.value);
        //--size
        PlayerPrefs.SetFloat("size", control_size.value);
        //--trail
        PlayerPrefs.SetFloat("trail", control_trail.value);
        //--damage
        PlayerPrefs.SetFloat("damage", control_damage.value);
        //--stall
        PlayerPrefs.SetFloat("stall", control_stall.value);
        //--boost
        PlayerPrefs.SetFloat("boost", control_boost.value);
        //--steer
        PlayerPrefs.SetFloat("steer", control_steer.value);
        //--sizeBrake
        PlayerPrefs.SetFloat("sizeBrake", control_sizeBrake.value);
        //--sizeDash
        PlayerPrefs.SetFloat("sizeDash", control_sizeDash.value);
        //--speedBoost
        PlayerPrefs.SetFloat("speedBoost", control_speedBoost.value);
        //environment
        //--drag
        PlayerPrefs.SetFloat("drag", control_drag.value);
        //--gravX
        PlayerPrefs.SetFloat("gravX", control_gravX.value);
        //--gravY
        PlayerPrefs.SetFloat("gravY", control_gravY.value);
        //--gravZ
        PlayerPrefs.SetFloat("gravZ", control_gravZ.value);
        //camera
        //--type
        PlayerPrefs.SetInt("type", control_type.value);
        //--posX
        PlayerPrefs.SetFloat("posX", control_posX.value);
        //--posY
        PlayerPrefs.SetFloat("posY", control_posY.value);
        //--posZ
        PlayerPrefs.SetFloat("posZ", control_posZ.value);
        //--fps
        PlayerPrefs.SetInt("fps", control_fps.value);
    }
    // called by the delete button
    public void ParametersDelete()
    {
        PlayerPrefs.DeleteAll();
    }
}
