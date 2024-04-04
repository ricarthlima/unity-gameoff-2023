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
        sourceMenu.volume = prefs.SoundBGM;
        sourceBGM.volume = prefs.SoundBGM;
        sourceEffectFalling.volume = prefs.SoundSFX;  
    }

    #region "Menu"
    [Header("Main Menu")]
    [SerializeField] private AudioSource sourceMenu;

    public void PlayMenuBGM(){
        sourceBGM.Pause();
        sourceEffectFalling.Pause();
        
        sourceMenu.time = 0;
        sourceMenu.Play();
    }

    #endregion

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
        sourceMenu.Stop();

        if (fromTheBegining){
            sourceBGM.time = 0;
        }
        sourceBGM.clip = bgmDungeon;
        sourceBGM.Play();

        return bpmDungeon;
    }

    public float PlayStairway(bool fromTheBegining = true){
        sourceEffectFalling.Stop();
        sourceMenu.Stop();

        if (fromTheBegining){
            sourceBGM.time = 0;
        }
        sourceBGM.clip = bgmStairway;
        sourceBGM.Play();

        return bpmStairway;
    }

    public float PlayThrone(bool fromTheBegining = true){
        sourceEffectFalling.Stop();
        sourceMenu.Stop();

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
    [Header("Falling")]
    [SerializeField] private AudioSource sourceEffectFalling;

    public void PlayEffectFalling(){
        sourceMenu.Stop();
        sourceBGM.Stop();
        sourceEffectFalling.time = 0;
        sourceEffectFalling.Play();
    }

    [Header("Beat SFXs")]
    [SerializeField] private GameObject sfxError;
    [SerializeField] private GameObject sfxClap;
    [SerializeField] private GameObject sfxBeat;
    [SerializeField] private GameObject sfxPerfect;

    void EquilizeSFX(GameObject gameObject){
        gameObject.GetComponent<AudioSource>().volume = prefs.SoundSFX;
    }

    public void PlaySFXPerfect(){
        EquilizeSFX(sfxPerfect);
        Instantiate(sfxPerfect);
    }

    public void PlaySFXError(){
        EquilizeSFX(sfxError);
        Instantiate(sfxError);
    }


    #endregion

    #region  "Global"
    public void PauseEverything(){
        sourceMenu.Play();

        sourceBGM.Pause();
        sourceEffectFalling.Pause();
    }

    public void UnPauseEverything(){
        sourceMenu.Stop();

        sourceBGM.UnPause();
        sourceEffectFalling.UnPause();
    }
    #endregion
}
