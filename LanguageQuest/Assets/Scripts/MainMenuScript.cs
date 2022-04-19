using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    Fade fade;
    bool playing = false;


    void Start() {
        fade = GameObject.Find("Fade").GetComponent<Fade>();
    }

    public void PlayGame()
    {
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

    IEnumerator Wait(float t) {
        fade.FadeOut(Color.black, 0.0f, t);
        yield return new WaitForSeconds(t);
        SceneManager.LoadScene(1);
    }
}
