using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeTimeOfDay : MonoBehaviour
{
    public enum TimeType {NotSet,Morning,Afternoon,Evening,Night,Random};
    static public Material[] skyboxes;
    static public GameObject[] lights;
    public TimeType defaultTime;

    void Start(){
            skyboxes = new Material[4];
            skyboxes[0] = Resources.Load<Material>("Sky_Anime_11_morning_a");
            skyboxes[1] = Resources.Load<Material>("Sky_Anime_03_Day_a");
            skyboxes[2] = Resources.Load<Material>("Sky_Anime_11_morning_a");
            skyboxes[3] = Resources.Load<Material>("Sky_LowPoly_02_Night_a");

            lights = new GameObject[4];
            lights[0] = GameObject.Find("Morning");
            lights[1] = GameObject.Find("Day");
            lights[2] = GameObject.Find("Sunset");
            lights[3] = GameObject.Find("Night");


            TimeType recTime = DataStructs.timeList[SceneManager.GetActiveScene().buildIndex-1];
            if(recTime==TimeType.NotSet){
                DataStructs.timeList[SceneManager.GetActiveScene().buildIndex-1] = defaultTime;
                recTime = defaultTime;
            }
            
            if(recTime==TimeType.Morning){
                SetTimeMorning();
            }else if(recTime==TimeType.Afternoon){
                SetTimeAfternoon();
            }else if(recTime==TimeType.Evening){
                SetTimeEvening();
            }else if(recTime==TimeType.Night){
                SetTimeNight();
            }
    }

    /*
     * 0. morning
     * 1. afternoon
     * 2. evening
     * 3. night
    */

    static void ClearLights() {
        foreach (GameObject o in lights) {
            o.SetActive(false);
        }
    }

    static public void SetTimeMorning() {
        ClearLights();
        DataStructs.timeList[SceneManager.GetActiveScene().buildIndex-1]=TimeType.Morning;
        lights[0].SetActive(true);
        RenderSettings.skybox = skyboxes[0];
    }

    static public void SetTimeAfternoon() {
        ClearLights();
        DataStructs.timeList[SceneManager.GetActiveScene().buildIndex-1]=TimeType.Afternoon;
        lights[1].SetActive(true);
        RenderSettings.skybox = skyboxes[1];
    }

    static public void SetTimeEvening() {
        ClearLights();
        DataStructs.timeList[SceneManager.GetActiveScene().buildIndex-1]=TimeType.Evening;
        lights[2].SetActive(true);
        RenderSettings.skybox = skyboxes[2];
    }

    static public void SetTimeNight() {
        ClearLights();
        DataStructs.timeList[SceneManager.GetActiveScene().buildIndex-1]=TimeType.Night;
        lights[3].SetActive(true);
        RenderSettings.skybox = skyboxes[3];
    }

    static public void SetTimeWithInt(int i){
        if(i==0){
            SetTimeMorning();
        }else if(i==1){
            SetTimeAfternoon();
        }else if(i==2){
            SetTimeEvening();
        }else if(i==3){
            SetTimeNight();
        }
    }

    static public void GetRandomTime() {
        ClearLights();
        DataStructs.timeList[SceneManager.GetActiveScene().buildIndex-1]=TimeType.Random;
        int x = Random.Range(0, skyboxes.Length - 1);
        RenderSettings.skybox = skyboxes[x];
        lights[x].SetActive(true);
    }
}
