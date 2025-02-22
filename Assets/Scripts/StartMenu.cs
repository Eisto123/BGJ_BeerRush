using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public string mainLevelName;

    void Start()
    {
        AudioManager.Instance.EnableAudioSource();
        StartCoroutine(AudioManager.Instance.PlayBGM(0,0.01f,true));   
    }

    public void PlayClick()
    {
        Debug.Log("load");
        SceneManager.LoadScene(mainLevelName);
    }

    public void ClickSFX()
    {
        AudioManager.Instance.PlayEnviroment(5);
    }
}
