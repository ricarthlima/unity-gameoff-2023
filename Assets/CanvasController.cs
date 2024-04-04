using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InternalScenes {main, settings, hud};

public class CanvasController : MonoBehaviour
{   
    [Header("Controllers")]
    [SerializeField] private PlayerPrefsController prefs;

    [Header("Main Menu")]
    [SerializeField] private GameObject panelMainMenu;

    [Header("Settings")]
    [SerializeField] private GameObject panelSettings;
    [SerializeField] private Slider sliderBGM;
    [SerializeField] private Slider sliderSFX;

    [Header("Game HUD")]
    [SerializeField] private GameObject panelGameHUD;

    void Start()
    {
        ShowScene(InternalScenes.main);
    }

    public void ShowSettingsScene(){
        ShowScene(InternalScenes.settings);
    }

    public void SettingsSaveButtonClicked(){
        prefs.SoundBGM = sliderBGM.value;
        prefs.SoundSFX = sliderSFX.value;
        ShowScene(InternalScenes.main);
    }

    public void ShowScene(InternalScenes hud){
        switch (hud){
            case InternalScenes.main: {
                panelMainMenu.SetActive(true);
                panelSettings.SetActive(false);
                panelGameHUD.SetActive(false);
                break;
            }
            case InternalScenes.settings: {
                panelMainMenu.SetActive(false);
                panelSettings.SetActive(true);
                panelGameHUD.SetActive(false);

                
                sliderBGM.value = prefs.SoundBGM;
                sliderSFX.value = prefs.SoundSFX;
                break;
            }
            case InternalScenes.hud: {
                panelMainMenu.SetActive(false);
                panelSettings.SetActive(false);
                panelGameHUD.SetActive(true);
                break;
            }
        }
    }
}
