using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{

    #region  "Music"
    
    [Header("Source Background Music")]
    public AudioSource sourceBGM;

    [Header("Dungeon Settings")]
    [SerializeField] private AudioClip bgmDungeon;
    [SerializeField] private float bpmDungeon;

    [Header("Stairway Settings")]
    [SerializeField] private AudioClip bgmStairway;
    [SerializeField] private float bpmStairway;

    [Header("Throne Settings")]
    [SerializeField] private AudioClip bgmThrone;
    [SerializeField] private float bpmThrone;

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
