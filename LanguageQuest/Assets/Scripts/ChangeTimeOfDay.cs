using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTimeOfDay : MonoBehaviour
{
    public Material[] skyboxes;
    public GameObject[] lights;

    /*
     * 0. morning
     * 1. afternoon
     * 2. evening
     * 3. night
    */

    void ClearLights() {
        foreach (GameObject o in lights) {
            o.SetActive(false);
        }
    }

    static public void SetTimeMorning() {
        ClearLights();
        lights[0].SetActive(true);
        RenderSettings.skybox = skyboxes[0];
    }

    static public void SetTimeAfternoon() {
        ClearLights();
        lights[1].SetActive(true);
        RenderSettings.skybox = skyboxes[1];
    }

    static public void SetTimeEvening() {
        ClearLights();
        lights[2].SetActive(true);
        RenderSettings.skybox = skyboxes[2];
    }

    static public void SetTimeNight() {
        ClearLights();
        lights[3].SetActive(true);
        RenderSettings.skybox = skyboxes[3];
    }

    static public void GetRandomTime() {
        ClearLights();
        int x = Random.Range(0, skyboxes.Length - 1);
        RenderSettings.skybox = skyboxes[x];
        lights[x].SetActive(true);
    }
}
