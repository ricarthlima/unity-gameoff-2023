using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum InternalScenes { main, settings, hud };

public class CanvasController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private PlayerPrefsController prefs;
    [SerializeField] private GameController gameController;

    [SerializeField] private InfoController infoController;

    [Header("Main Menu")]
    [SerializeField] private GameObject panelMainMenu;
    [SerializeField] private TextMeshProUGUI textVersion;

    [Header("Settings")]
    [SerializeField] private GameObject panelSettings;
    [SerializeField] private Slider sliderBGM;
    [SerializeField] private Slider sliderSFX;

    [Header("Game HUD")]
    [SerializeField] private GameObject panelGameHUD;
    [SerializeField] private Image panelWarning;

    List<Resolution> resolutions = new List<Resolution>();
    public TMP_Dropdown dropdownResolution;

    [SerializeField] private Toggle toggleFullscreen;

    [SerializeField] private RectTransform progressMageRect;
    [SerializeField] TextMeshProUGUI textInfos;
    [SerializeField] private Image imageNextPlatform;
    [SerializeField] private Image imageSecondPlatform;
    public GameObject textPause;

    [SerializeField] private Sprite spriteMistake;

    bool isFullscreen;
    int selectedRes;

    float warningElapsedTime;
    float warningDuration;

    void Start()
    {
        textVersion.text = "Versão de Desenvolvimento - " + Application.version;

        ShowScene(InternalScenes.main);

        toggleFullscreen.isOn = Screen.fullScreen;

        foreach (Resolution res in Screen.resolutions)
        {
            if (res.width / res.height == 16 / 9)
            {
                resolutions.Add(res);
            }
        }

        dropdownResolution.ClearOptions();

        int currentResIndex = resolutions.Count - 1;
        List<string> ress = new List<string>();
        for (int i = 0; i < resolutions.Count; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            ress.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width
            && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        dropdownResolution.AddOptions(ress);
        dropdownResolution.value = currentResIndex;
        dropdownResolution.RefreshShownValue();

        UpdateImagesPlatform();
    }

    private void Update()
    {
        UpdateUI();

        if (panelWarning.color != Color.clear)
        {
            warningElapsedTime += Time.deltaTime;
            panelWarning.color = Color.Lerp(panelWarning.color, Color.clear, warningElapsedTime / warningDuration);
        }


    }

    public void ShowSettingsScene()
    {
        ShowScene(InternalScenes.settings);
    }

    public void SettingsSaveButtonClicked()
    {
        prefs.SoundBGM = sliderBGM.value;
        prefs.SoundSFX = sliderSFX.value;

        Screen.fullScreen = isFullscreen;
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

        Screen.SetResolution(resolutions[selectedRes].width, resolutions[selectedRes].height, isFullscreen);

        ShowScene(InternalScenes.main);
    }

    public void ShowScene(InternalScenes hud)
    {
        switch (hud)
        {
            case InternalScenes.main:
                {
                    panelMainMenu.SetActive(true);
                    panelSettings.SetActive(false);
                    panelGameHUD.SetActive(false);
                    break;
                }
            case InternalScenes.settings:
                {
                    panelMainMenu.SetActive(false);
                    panelSettings.SetActive(true);
                    panelGameHUD.SetActive(false);


                    sliderBGM.value = prefs.SoundBGM;
                    sliderSFX.value = prefs.SoundSFX;
                    break;
                }
            case InternalScenes.hud:
                {
                    panelMainMenu.SetActive(false);
                    panelSettings.SetActive(false);
                    panelGameHUD.SetActive(true);
                    break;
                }
        }
    }



    public void SetFullscreen(bool value)
    {
        isFullscreen = value;
    }

    public void SetResolution(int index)
    {
        selectedRes = index;
    }

    public void MoveProgressMage(float progress)
    {
        Vector3 newPos = progressMageRect.anchoredPosition;
        newPos.y = -225 + (progress * 450);
        progressMageRect.anchoredPosition = newPos;
    }

    public void UpdateUI()
    {
        float fps = 1f / Time.deltaTime;

        textInfos.text = "FPS: " + fps.ToString("F2") + "\n";
        textInfos.text += "\n";
        textInfos.text += "BPM: " + gameController.bpm.ToString() + "\n";
        textInfos.text += "Beats: " + gameController.beatCount + "\n";
        textInfos.text += "Beats to Portal: " + gameController.beatsToPortal + "\n";
        textInfos.text += "\n";
        textInfos.text += "Height: " + infoController.heightTraveled.ToString("F2") + "m" + "\n";
        textInfos.text += "Max Height: " + infoController.maxHeightTraveled.ToString("F2") + "m" + "\n";
        textInfos.text += "\n";
        textInfos.text += "%: " + gameController.progress + "\n";
        textInfos.text += "Portals Count: " + gameController.countPortals + "\n";
        textInfos.text += "Total Portals: " + gameController.levelData.GetPortalsToEnd(gameController.level) + "\n";
        //textInfos.text += "Time Playing: " + infoController.countTimePlaying.ToString("F2") + "m" + "\n";
    }

    public void UpdateImagesPlatform()
    {
        imageNextPlatform.sprite = gameController.listNextPlatforms[0].GetComponent<SpriteRenderer>().sprite;
        imageSecondPlatform.sprite = gameController.listNextPlatforms[1].GetComponent<SpriteRenderer>().sprite;
        gameController.guidePlataform.GetComponent<SpriteRenderer>().sprite = imageNextPlatform.sprite;
        gameController.guidePlataform.transform.localScale = gameController.listNextPlatforms[0].transform.localScale;
    }

    public void ShowMistakeGuide()
    {
        gameController.guidePlataform.GetComponent<SpriteRenderer>().sprite = spriteMistake;
        StartCoroutine(HideMistakeGuide());
    }

    IEnumerator HideMistakeGuide()
    {
        yield return new WaitForSeconds(1);
        UpdateImagesPlatform();
    }

    public void ShowRedWarning()
    {
        warningElapsedTime = 0;
        warningDuration = 25;
        panelWarning.color = new Color(0.5f, 0, 0, 0.33f);
    }

    public void ShowWhiteWarning()
    {
        warningElapsedTime = 0;
        warningDuration = 25;
        panelWarning.color = new Color(0.5f, 0.5f, 0.5f, 0.33f);
    }
}
