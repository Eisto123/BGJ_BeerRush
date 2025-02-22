using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Unity.Mathematics;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int score;
    private int highestScore;

    public GameObject scoreUI;
    public GameObject yourScore;
    public Image fadeMask;
    public float fadeSpeed = 3f;
    public UImanager Endscores;
    public GameObject gameoverPanel;

    public Image beerImage;
    public Sprite beerBreak;
    public float fallSpeed;
    public float fallEndY = 0;

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

    }
    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    private bool ifBGMFade = false;
    public void GameOver()
    {   
        if(!ifBGMFade)
        {
            //Audio
            AudioManager.Instance.FadeOutBGM();
            AudioManager.Instance.DisableAudioSource("Environment", "Cowboy");
            ifBGMFade = true;
        }

        if (score > highestScore)
        {
            highestScore = score;
        }
        StartCoroutine(EndProcess());

    }
    private IEnumerator EndProcess()
    {
        scoreUI.SetActive(false);
        yourScore.SetActive(false);
        StartCoroutine(FadeOutLayermasK());
        
        yield return new WaitForSeconds(fadeSpeed);
        StartCoroutine(FallDownBeer());
        yield return new WaitForSeconds(fallSpeed+1f);
        gameoverPanel.SetActive(true);
        Endscores.UpdateScore(score);

        
    }
    private IEnumerator FadeOutLayermasK(){
        float startAlpha = fadeMask.color.a;
        float rate = 1.0f / fadeSpeed;
        float progress = 0.0f;

        while(progress < 1.0f)
        {
            Color tempColor = fadeMask.color;
            tempColor.a = Mathf.Lerp(startAlpha, 0.85f, progress);
            fadeMask.color = tempColor;
            progress += rate * Time.deltaTime;
            yield return null;
        }
        Color finalColor = fadeMask.color;
        finalColor.a = 0.85f;
        fadeMask.color = finalColor;
    }

    private bool ifFadeSFX = false;

    private IEnumerator FallDownBeer()
    {
        //Audio
        /*if (!ifFadeSFX)
        {
            AudioManager.Instance.FadeOutSFX();
            ifFadeSFX = true;
        }*/
        //AudioManager.Instance.PlayEnviroment(6);
        //StartCoroutine(AudioManager.Instance.PlayEnviroment(6, 0.83f));
        
        
        StartCoroutine(AudioManager.Instance.PlayBGM(1, 4f));

        float start = beerImage.rectTransform.anchoredPosition.y;
        float rate = 1.0f / fallSpeed;
        float progress = 0.0f;

        while(progress < 1f&&beerImage.rectTransform.anchoredPosition.y > fallEndY)
        {
            Vector2 anchorPos = beerImage.rectTransform.anchoredPosition;
            anchorPos.y = Mathf.Lerp(start, fallEndY-30, progress);
            beerImage.rectTransform.anchoredPosition = anchorPos;
            progress += rate * Time.deltaTime;
            yield return null;
        }
        if(beerImage.rectTransform.anchoredPosition.y <= fallEndY)
        {
            
            beerImage.rectTransform.anchoredPosition = new Vector2(beerImage.rectTransform.anchoredPosition.x, fallEndY);
            beerImage.sprite = beerBreak;
            float startAlpha = beerImage.color.a;
            if(!AudioManager.Instance.SFX.isPlaying){
                AudioManager.Instance.PlaySFX(4);
            }
            float colorRate = 1.0f / fallSpeed;
            progress = 0.0f;

            while(progress < 1.0f)
            {
                Color tempColor = beerImage.color;
                tempColor.a = Mathf.Lerp(startAlpha, 0f, progress);
                beerImage.color = tempColor;
                progress += colorRate * Time.deltaTime;
                yield return null;
            }
            Color finalColor = beerImage.color;
            finalColor.a = 0f;
            beerImage.color = finalColor;
        }
        
    }

    public void GetPoint(int point)
    {
        //Audio, getpoint
        AudioManager.Instance.PlaySFX(3);

        score += point;
        if(yourScore!=null){
            yourScore.SetActive(true);
        }
        
        scoreUI.GetComponent<UImanager>().UpdateScore(score);
        
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

}
