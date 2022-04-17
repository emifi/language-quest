using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    float sensitivity = 100;
    GameObject player;
    MouseLook mouseLook;
    ResolutionWrapper[] resolutions = {new ResolutionWrapper(1920, 1080, true), 
                                        new ResolutionWrapper(2560, 1440, true)};
    int currentResolution = 0;
    int unsavedResolution = 0;
    bool unsavedFullscreen = true;
    GameObject resolutionElement;
    GameObject fullscreenElement;
    GameObject sensitivityElement;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("First Person Player");
        if (player) {
            mouseLook = player.GetComponentInChildren<MouseLook>();
        }
        resolutionElement = GameObject.Find("Resolution");
        fullscreenElement = GameObject.Find("Fullscreen");
        sensitivityElement = GameObject.Find("Sensitivity");
        transform.gameObject.GetComponent<Canvas>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onOpen() {
        // Display sensitivity value
        sensitivityElement.GetComponentInChildren<Slider>().value = mouseLook.mouseSensitivity;

        // Display resolution options
        resolutionElement.GetComponentInChildren<TMP_Dropdown>().ClearOptions();
        List<string> options = new List<string>();
        foreach (ResolutionWrapper res in resolutions) {
            options.Add(res.toString());
        }
        resolutionElement.GetComponentInChildren<TMP_Dropdown>().AddOptions(options);
        resolutionElement.GetComponentInChildren<TMP_Dropdown>().value = currentResolution;

        // Display fullscreen value
        if (resolutions[currentResolution].fullscreen) {
            fullscreenElement.GetComponentInChildren<Toggle>().isOn = true;
        } else {
            fullscreenElement.GetComponentInChildren<Toggle>().isOn = false;
        }
    }

    public void onSave() {
        mouseLook.mouseSensitivity = sensitivity;
        currentResolution = unsavedResolution;
        resolutions[currentResolution].fullscreen = unsavedFullscreen;
        ResolutionWrapper savedReolution = resolutions[currentResolution];
        Screen.SetResolution(savedReolution.x, savedReolution.y, savedReolution.fullscreen);
        MouseLook.dictClose();
        PlayerMovement.dictClose();
        transform.gameObject.GetComponent<Canvas>().enabled = false;
        Debug.Log("Saved!");
    }

    public void onCancel() {
        transform.gameObject.GetComponent<Canvas>().enabled = false;
        MouseLook.dictClose();
        PlayerMovement.dictClose();
        Debug.Log("Cancelled!");
    }

    public void updateSensitivity(float value) {
        Debug.Log($"Sensitivity changed from {sensitivity} to {value}");
        sensitivity = value;

    }

    public void updateResolution(int newRes) {
        unsavedResolution = newRes;
        Debug.Log($"Resolution updated to {resolutions[currentResolution].toString()}");
    }

    public void updateFullscreen(bool value) {
        unsavedFullscreen = value;
    }
}

public class ResolutionWrapper {
    public int x;
    public int y;
    public bool fullscreen;

    public ResolutionWrapper(int x, int y, bool fullscreen) {
        this.x = x;
        this.y = y;
        this.fullscreen = fullscreen;
    }

    public void toggleFullscreen() {
        this.fullscreen = !this.fullscreen;
    }

    public string toString() {
        return $"{this.x}x{this.y}";
    }
}
