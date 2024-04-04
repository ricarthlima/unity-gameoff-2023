using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] PlayerPrefsController prefs;

    private void Start() {
        sourceBGM.volume = prefs.SoundBGM;
        sourceEffectFalling.volume = prefs.SoundSFX;    
    }

    private void Update() {
        sourceBGM.volume = prefs.SoundBGM;
        sourceEffectFalling.volume = prefs.SoundSFX;  
    }

    #region  "Music"
    
    [Header("Source Background Music")]
    public AudioSource sourceBGM;

    [Header("Dungeon Settings")]
    [SerializeField] private AudioClip bgmDungeon;
    public float bpmDungeon;

    [Header("Stairway Settings")]
    [SerializeField] private AudioClip bgmStairway;
    public float bpmStairway;

    [Header("Throne Settings")]
    [SerializeField] private AudioClip bgmThrone;
    public float bpmThrone;

    public float PlayDungeon(bool fromTheBegining = true){
        sourceEffectFalling.Stop();

        if (fromTheBegining){
            sourceBGM.time = 0;
        }
        sourceBGM.clip = bgmDungeon;
        sourceBGM.Play();

        return bpmDungeon;
    }

    public float PlayStairway(bool fromTheBegining = true){
        sourceEffectFalling.Stop();
        
        if (fromTheBegining){
            sourceBGM.time = 0;
        }
        sourceBGM.clip = bgmStairway;
        sourceBGM.Play();

        return bpmStairway;
    }

    public float PlayThrone(bool fromTheBegining = true){
        sourceEffectFalling.Stop();
        
        if (fromTheBegining){
            sourceBGM.time = 0;
        }
        sourceBGM.clip = bgmThrone;
        sourceBGM.Play();

        return bpmThrone;
    }

    public void ChangeBGMSpeed(float speed){
        sourceBGM.pitch = speed;
    }

    #endregion

    #region  "SFX"    
    [Header("Source Falling")]
    [SerializeField] private AudioSource sourceEffectFalling;

    public void PlayEffectFalling(){
        sourceBGM.Stop();
        sourceEffectFalling.time = 0;
        sourceEffectFalling.Play();
    }

    #endregion

    #region  "Global"
    public void PauseEverything(){
        sourceBGM.Pause();
        sourceEffectFalling.Pause();
    }

    public void UnPauseEverything(){
        sourceBGM.UnPause();
        sourceEffectFalling.UnPause();
    }
    #endregion
}
