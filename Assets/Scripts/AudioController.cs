using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] PlayerPrefsController prefs;

    private void Start()
    {
        sourceBGM.volume = prefs.SoundBGM;
        sourceEffectFalling.volume = prefs.SoundSFX;
    }

    private void Update()
    {
        sourceMenu.volume = prefs.SoundBGM;
        sourceBGM.volume = prefs.SoundBGM;
        sourceMetronome.volume = prefs.SoundMTR;
        sourceEffectFalling.volume = prefs.SoundSFX;
    }

    #region "Menu"
    [Header("Main Menu")]
    [SerializeField] private AudioSource sourceMenu;

    public void PlayMenuBGM()
    {
        sourceBGM.Pause();
        sourceEffectFalling.Pause();

        sourceMenu.time = 0;
        sourceMenu.Play();
    }

    #endregion

    #region  "Music"

    [Header("Source Background Music")]
    public AudioSource sourceMetronome;
    [SerializeField] private AudioClip metronome120;

    [Header("Source Background Music")]
    public AudioSource sourceBGM;

    [Header("Dungeon Settings")]
    [SerializeField] private AudioClip bgmDungeon;

    [Header("Stairway Settings")]
    [SerializeField] private AudioClip bgmStairway;

    [Header("Throne Settings")]
    [SerializeField] private AudioClip bgmThrone;

    public void PlayByLevel(TowerLevel level, bool fromTheBegining = true)
    {
        switch (level)
        {
            case TowerLevel.dungeon: { PlayDungeon(fromTheBegining); break; }
            case TowerLevel.stairway: { PlayStairway(fromTheBegining); break; }
            case TowerLevel.throne: { PlayThrone(fromTheBegining); break; }
        }
    }
    public void PlayDungeon(bool fromTheBegining = true)
    {
        sourceEffectFalling.Stop();
        sourceMenu.Stop();

        if (fromTheBegining)
        {
            sourceBGM.time = 0;
            sourceMetronome.time = 0;
        }
        sourceBGM.clip = bgmDungeon;
        sourceBGM.Play();

        sourceMetronome.clip = metronome120;
        sourceMetronome.Play();
    }

    public void PlayStairway(bool fromTheBegining = true)
    {
        sourceEffectFalling.Stop();
        sourceMenu.Stop();

        if (fromTheBegining)
        {
            sourceBGM.time = 0;
        }
        sourceBGM.clip = bgmStairway;
        sourceBGM.Play();
    }

    public void PlayThrone(bool fromTheBegining = true)
    {
        sourceEffectFalling.Stop();
        sourceMenu.Stop();

        if (fromTheBegining)
        {
            sourceBGM.time = 0;
        }
        sourceBGM.clip = bgmThrone;
        sourceBGM.Play();
    }

    public void ChangeBGMSpeed(float speed)
    {
        sourceBGM.pitch = speed;
        sourceMetronome.pitch = speed;
    }

    #endregion

    #region  "SFX"    
    [Header("Falling")]
    [SerializeField] private AudioSource sourceEffectFalling;

    public void PlayEffectFalling()
    {
        sourceMenu.Stop();
        sourceBGM.Stop();
        sourceMetronome.Stop();
        sourceEffectFalling.time = 0;
        sourceEffectFalling.Play();
    }

    [Header("Beat SFXs")]
    [SerializeField] private GameObject sfxError;
    [SerializeField] private GameObject sfxClap;
    [SerializeField] private GameObject sfxBeat;
    [SerializeField] private GameObject sfxPerfect;

    void EquilizeSFX(GameObject gameObject, float modifier = 1)
    {
        gameObject.GetComponent<AudioSource>().volume = prefs.SoundSFX * modifier;
    }

    public void PlaySFXPerfect()
    {
        EquilizeSFX(Instantiate(sfxPerfect), 0.75f);
    }

    public void PlaySFXError()
    {
        EquilizeSFX(Instantiate(sfxError));
    }

    public void PlaySFXBeat()
    {
        EquilizeSFX(Instantiate(sfxBeat), 0.75f);
        ;
    }

    public void PlaySFXClap()
    {
        EquilizeSFX(Instantiate(sfxClap), 0.1f);
    }
    #endregion

    #region  "Global"
    public void PauseEverything()
    {
        sourceMenu.Play();

        sourceBGM.Pause();
        sourceMetronome.Pause();

        sourceEffectFalling.Pause();
    }

    public void UnPauseEverything()
    {
        sourceMenu.Stop();

        sourceBGM.UnPause();
        sourceMetronome.UnPause();

        sourceEffectFalling.UnPause();
    }
    #endregion
}
