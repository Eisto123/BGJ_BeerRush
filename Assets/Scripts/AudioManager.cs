using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioClip> BGMClips;
    public List<AudioClip> PlayerSFX;
    public List<AudioClip> enviromentSFX;

    public AudioSource BGM;
    public AudioSource SFX;
    public AudioSource Enviroment;

    public static AudioManager Instance;

    private void Awake()
    {
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void playBGM(int i){
        if(BGM.clip == BGMClips[i]){
            return;
        }
        BGM.clip = BGMClips[i];
        BGM.Play();
    }

    public void PlaySFX(int i){
        SFX.clip = PlayerSFX[i];
        SFX.Play();
    }

    public void PlayEnviroment(int i){
        Enviroment.clip = enviromentSFX[i];
        Enviroment.Play();
    }

    public void FadeOutBGM(){
        StartCoroutine(FadeOut(BGM,3f));
    }

    public void FadeOutSFX(){
        StartCoroutine(FadeOut(SFX,1f));
    }
    public void FadeOutEnviroment(){
        StartCoroutine(FadeOut(Enviroment,1f));
    }

    private IEnumerator FadeOut(AudioSource audioSource, float fadeTime){
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.Stop ();
        audioSource.volume = startVolume;
    }

}
