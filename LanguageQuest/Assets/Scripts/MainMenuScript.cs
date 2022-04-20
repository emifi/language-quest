using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    Fade fade;
    bool playing = false;
    string playerName;


    void Start() {
        fade = GameObject.Find("Fade").GetComponent<Fade>();
    }

    public void PlayGame()
    {
        if (playerName != null) {
            DataStructs.playerName = playerName;
        } else {
            DataStructs.playerName = "Player";
        }
        Debug.Log(DataStructs.playerName);
        if (!playing) {
            playing = true;
            StartCoroutine(Wait(3.0f));
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }

    public void updateName(string name) {
        playerName = name;
    }

    IEnumerator Wait(float t) {
        fade.FadeOut(Color.black, 0.0f, t);
        yield return new WaitForSeconds(t);
        SceneManager.LoadScene(1);
    }
}
