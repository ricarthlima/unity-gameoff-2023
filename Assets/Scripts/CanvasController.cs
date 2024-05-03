using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    List<Resolution> resolutions = new List<Resolution>();
    public TMP_Dropdown dropdownResolution;

    [SerializeField] private Toggle toggleFullscreen;

    bool isFullscreen;
    int selectedRes;

    void Start()
    {
        ShowScene(InternalScenes.main);

        toggleFullscreen.isOn = Screen.fullScreen;

        foreach (Resolution res in Screen.resolutions){
            if (res.width/res.height == 16/9){
                resolutions.Add(res);
            }
        }

        dropdownResolution.ClearOptions();
        
        int currentResIndex = resolutions.Count - 1;
        List<string> ress = new List<string>();
        for (int i = 0; i < resolutions.Count; i++){
            string option = resolutions[i].width + " x " + resolutions[i].height;
            ress.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width 
            && resolutions[i].height == Screen.currentResolution.height){
                currentResIndex = i;
            }
        }

        dropdownResolution.AddOptions(ress);
        dropdownResolution.value = currentResIndex;
        dropdownResolution.RefreshShownValue();
    }

    public void ShowSettingsScene(){
        ShowScene(InternalScenes.settings);
    }

    public void SettingsSaveButtonClicked(){
        prefs.SoundBGM = sliderBGM.value;
        prefs.SoundSFX = sliderSFX.value;

        Screen.fullScreen = isFullscreen;
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        
        Screen.SetResolution(resolutions[selectedRes].width, resolutions[selectedRes].height, isFullscreen);

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

    

    public void SetFullscreen(bool value){
        isFullscreen = value;
    }

    public void SetResolution(int index){
        selectedRes = index;
    }

}
