using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
// * for devlog only *
public class base_demo : MonoBehaviour
{
    // count number of times space bar pressed
    // trigger actions on counts reached
    // ? public variable count start value
    private int _actionCounter = 0;
    // 
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            _actionCounter++;
            // 
            if (_actionCounter < 10) DemoA(_actionCounter);
            else if (_actionCounter < 20) DemoB(_actionCounter - 10);
            else if (_actionCounter < 30) DemoC(_actionCounter - 20);
            else if (_actionCounter < 40) DemoD(_actionCounter - 30);
            else if (_actionCounter < 50) DemoE(_actionCounter - 40);
            else if (_actionCounter < 60) DemoF(_actionCounter - 50);
            else if (_actionCounter < 70) DemoG(_actionCounter - 60);
            else if (_actionCounter < 80) DemoH(_actionCounter - 70);
            else if (_actionCounter < 90) DemoI(_actionCounter - 80);
            else if (_actionCounter < 100) DemoJ(_actionCounter - 90);
            else if (_actionCounter < 110) DemoK(_actionCounter - 100);
            else if (_actionCounter < 120) DemoL(_actionCounter - 110);
            else if (_actionCounter < 130) DemoM(_actionCounter - 120);
            else if (_actionCounter < 140) DemoN(_actionCounter - 130);
            else if (_actionCounter < 150) DemoO(_actionCounter - 140);
        }
    }
    // 1 player spawn
    // 2 layout pop in
    // 3 player fall to ground
    // 4 ui pop in, buttons toggle alternating, joystick spin counter clockwise
    // 5 ui disappear
    // 6 center obstacle bar pop in
    // 7 camera track to player
    public Transform player = null;
    public TrailRenderer trail = null;
    public GameObject layout = null;
    public GameObject ui = null;
    public GameObject obstacle = null;
    public game_camera cameraMain = null;
    void DemoA(int counter)
    {
        switch(counter)
        {
            case 1:
                player.gameObject.SetActive(true);
                player.position = Vector3.zero;
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                trail.Clear();
                break;
            case 2:
                layout.SetActive(true);
                break;
            case 3:
                player.GetComponent<Rigidbody>().useGravity = true;
                break;
            case 4:
                ui.SetActive(true);
                break;
            case 5:
                ui.SetActive(false);
                break;
            case 6:
                obstacle.SetActive(true);
                break;
            case 7:
                cameraMain.enabled = true;
                // 
                _actionCounter += 3;
                break;
        }
    }
    // 1 player spawn
    // 2 layout material disable shadows probes
    // 3 light source disabled
    // 4 lighitng type gradient
    // 5 player/layout use URP materials
    // 6 skybox use URP material
    // 7 enable player collision particles, layout use tag "concrete"
    // 8 player trail effect enable
    // 9 bgm sfx unmute
    public GameObject lightMain = null;
    public GameObject layout0 = null;
    public List<Renderer> objects = new List<Renderer>();
    public List<Material> materials = new List<Material>();
    public Material gradient = null;
    public List<GameObject> walls = new List<GameObject>();
    // public GameObject trail = null;
    public AudioMixer master = null;
    void DemoB(int counter)
    {
        switch(counter)
        {
            case 1:
                layout.SetActive(false);
                obstacle.SetActive(false);
                // 
                player.position = Vector3.zero;
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                trail.Clear();
                layout0.SetActive(true);
                break;
            case 2:
                foreach (Renderer obj in objects)
                {
                    obj.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    obj.receiveShadows = false;
                }
                break;
            case 3:
                lightMain.SetActive(false);
                break;
            case 4:
                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
                // 
                RenderSettings.ambientSkyColor = Color.white;
                RenderSettings.ambientEquatorColor = Color.grey;
                RenderSettings.ambientGroundColor = Color.white;
                break;
            case 5:
                objects[0].material = materials[0];
                objects[1].material = materials[0];
                objects[2].material = materials[0];
                objects[3].material = materials[0];
                // 
                objects[4].material = materials[1];
                // 
                objects[5].material = materials[2];
                break;
            case 6:
                RenderSettings.skybox = gradient;
                break;
            case 7:
                player.GetComponent<controller_player>()._isDemo = false;
                // 
                foreach (GameObject wall in walls)
                    wall.tag = "Concrete";
                break;
            case 8:
                trail.gameObject.SetActive(true);
                break;
            case 9:
                master.SetFloat("BGM", 0f);
                master.SetFloat("SFX", 0f);
                // 
                _actionCounter += 1;
                break;
        }
    }
    // 1 player spawn, demo crate
    // 2 enable crate rows
    public GameObject layout1 = null;
    public GameObject crates = null;
    void DemoC(int counter)
    {
        switch(counter)
        {
            case 1:
                layout0.SetActive(false);
                // 
                layout1.SetActive(true);
                player.position = Vector3.zero;
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                trail.Clear();
                break;
            case 2:
                crates.SetActive(true);
                // 
                _actionCounter += 8;
                break;
        }
    }
    // 1 player spawn, demo window
    public GameObject layout2 = null;
    void DemoD(int counter)
    {
        switch(counter)
        {
            case 1:
                crates.SetActive(false);
                layout1.SetActive(false);
                // 
                layout2.SetActive(true);
                player.position = Vector3.zero;
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                trail.Clear();
                // 
                _actionCounter += 9;
                break;
        }
    }
    // 1 player spawn, demo barrel
    public GameObject layout3 = null;
    void DemoE(int counter)
    {
        switch(counter)
        {
            case 1:
                layout2.SetActive(false);
                // 
                layout3.SetActive(true);
                player.position = Vector3.zero;
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                trail.Clear();
                // 
                _actionCounter += 9;
                break;
        }
    }
    // 1 player spawn, demo button
    public GameObject layout4 = null;
    void DemoF(int counter)
    {
        switch(counter)
        {
            case 1:
                layout3.SetActive(false);
                // 
                layout4.SetActive(true);
                player.position = Vector3.zero;
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                trail.Clear();
                // 
                _actionCounter += 9;
                break;
        }
    }
    // 1 player spawn, demo windup
    public GameObject layout5 = null;
    void DemoG(int counter)
    {
        switch(counter)
        {
            case 1:
                layout4.SetActive(false);
                // 
                layout5.SetActive(true);
                player.position = Vector3.zero;
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                trail.Clear();
                // 
                _actionCounter += 9;
                break;
        }
    }
    // 1 player spawn, demo door
    // 2 crate stack pop in
    public GameObject layout6 = null;
    public GameObject crateStack = null;
    void DemoH(int counter)
    {
        switch(counter)
        {
            case 1:
                layout5.SetActive(false);
                // 
                layout6.SetActive(true);
                player.position = Vector3.zero;
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                trail.Clear();
                break;
            case 2:
                crateStack.SetActive(true);
                // 
                _actionCounter += 8;
                break;
        }
    }
    // 1 player spawn, demo dispenser
    // 
    public GameObject layout7 = null;
    void DemoI(int counter)
    {
        switch(counter)
        {
            case 1:
                layout6.SetActive(false);
                // 
                layout7.SetActive(true);
                player.position = Vector3.zero;
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                trail.Clear();
                // 
                _actionCounter += 9;
                break;
        }
    }
    // 1 player spawn, demo elevator
    // 2-5 spawn crate rows
    public GameObject layout8 = null;
    public GameObject crate = null;
    public List<Transform> spawns = new List<Transform>();
    void DemoJ(int counter)
    {
        if (counter == 1)
        {
            layout7.SetActive(false);
            // 
            layout8.SetActive(true);
            player.position = Vector3.zero;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            trail.Clear();
        }
        else if (counter < 4)
        {
            foreach (Transform spawn in spawns)
                Instantiate(crate, spawn.position, spawn.rotation, spawn);
        }
        else if (counter == 4)
        {
            foreach (Transform spawn in spawns)
                Instantiate(crate, spawn.position, spawn.rotation, spawn);
            // 
            _actionCounter += 6;
        }
    }
    // 1 player spawn, demo conveyorBelt
    // 2 spawn crate pair
    // 3 spawn barrel pair
    public GameObject layout9 = null;
    public GameObject cratePair = null;
    public GameObject barrelPair = null;
    void DemoK(int counter)
    {
        switch(counter)
        {
            case 1:
                layout8.SetActive(false);
                // 
                layout9.SetActive(true);
                player.position = Vector3.zero;
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                trail.Clear();
                break;
            case 2:
                cratePair.SetActive(true);
                break;
            case 3:
                barrelPair.SetActive(true);
                // 
                _actionCounter += 7;
                break;
        }
    }
    // 1 player spawn, demo fan
    // 2 spawn crates barrels
    public GameObject layout10 = null;
    public GameObject cratesBarrels = null;
    void DemoL(int counter)
    {
        switch(counter)
        {
            case 1:
                layout9.SetActive(false);
                // 
                layout10.SetActive(true);
                player.position = Vector3.zero;
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                trail.Clear();
                break;
            case 2:
                cratesBarrels.SetActive(true);
                // 
                _actionCounter += 8;
                break;
        }
    }
    // 1 player spawn, demo trolley
    public GameObject layout11 = null;
    void DemoM(int counter)
    {
        switch(counter)
        {
            case 1:
                layout10.SetActive(false);
                // 
                layout11.SetActive(true);
                player.position = Vector3.zero;
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                trail.Clear();
                // 
                _actionCounter += 9;
                break;
        }
    }
    // 1 player spawn, demo trampoline
    // 2-4 spawn crate pair
    // ? 5 trolley with trampoline
    public GameObject layout12 = null;
    public List<Transform> spawns0 = new List<Transform>();
    void DemoN(int counter)
    {
        if (counter == 1)
        {
            layout11.SetActive(false);
            // 
            layout12.SetActive(true);
            player.position = Vector3.zero;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            trail.Clear();
        }
        else if (counter < 4)
        {
            foreach (Transform spawn in spawns0)
                Instantiate(crate, spawn.position, spawn.rotation, spawn);
        }
        else if (counter == 4)
        {
            foreach (Transform spawn in spawns0)
                Instantiate(crate, spawn.position, spawn.rotation, spawn);
            // 
            _actionCounter += 6;
        }
    }
    void DemoO(int counter)
    {
        print("- End of Demo -");
    }
}