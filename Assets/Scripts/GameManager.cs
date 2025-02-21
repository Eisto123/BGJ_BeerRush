using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int score;
    private int highestScore;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this);

    }
    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    public void GameOver()
    {
        if (score > highestScore)
        {
            highestScore = score;
            // Save the highest score
            // animation on celebration
        }
    }

    public void GetPoint(int point)
    {
        score = point;
    }

}
