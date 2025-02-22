using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public string mainLevelName;

    public void PlayClick()
    {
        Debug.Log("load");
        SceneManager.LoadScene(mainLevelName);
    }
}
