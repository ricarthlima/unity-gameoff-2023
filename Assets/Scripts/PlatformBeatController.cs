using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBeatController : MonoBehaviour
{
    [Header("Timers")]
    public float bpm;

    [SerializeField] private float timerBeat;

    [Header("BeatTexts")]
    [SerializeField] GameObject textFailPrefab;
    [SerializeField] GameObject textOKPrefab;
    [SerializeField] GameObject textNicePrefab;
    [SerializeField] GameObject textPerfectPrefab;

    [Header("Visual")]
    [SerializeField] VisualBeatIndicatorController currentBeatIndicator;

    [Header("Controllers")]
    GameController gameController;

    [Header("SFX")]
    [SerializeField] private GameObject sfxError;

    int beatStep = 0;
    bool canHitTheBeat = false;
    float sizeMultiplier = 0.75f;

    private void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void Update()
    {
        timerBeat += Time.deltaTime;
        switch (beatStep)
        {
            case 0:
                {
                    if (timerBeat > ((60f / bpm) * 0.45f) && !gameController.hasMissedClick)
                    {
                        currentBeatIndicator.ShowPortal();
                        beatStep++;
                    }
                    return;
                }
            case 1:
                {
                    if (timerBeat > timeEarly() && !gameController.hasMissedClick)
                    {
                        // Libera clique correto
                        canHitTheBeat = true;

                        // Indicador fica amaerelo
                        currentBeatIndicator.MakeYellow();
                        beatStep++;
                    }
                    return;
                }
            case 2:
                {
                    if (timerBeat > bpmTime())
                    {
                        currentBeatIndicator.MakeRed();
                        canHitTheBeat = false;
                    }
                    return;
                }
        }
        if (timerBeat > timeLate())
        {
            Destroy(gameObject);
        }
    }

    public void OnTouched(Vector2 touchPosition)
    {
        Vector3 clickOnWorld = Camera.main.ScreenToWorldPoint(touchPosition);

        if (timerBeat < timeEarly())
        {
            Instantiate(textFailPrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
        }
        else if (timerBeat < timeOK())
        {
            Instantiate(textOKPrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
            sizeMultiplier = 0.70f;
        }
        else if (timerBeat < timeNice())
        {
            Instantiate(textNicePrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
            sizeMultiplier = 1;
        }
        else if (timerBeat < timePerfect())
        {
            Instantiate(textPerfectPrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
            sizeMultiplier = 1.30f;
        }
        else if (timerBeat > bpmTime())
        {
            Instantiate(textFailPrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
            
        }

        GeneratePlatform(touchPosition);
    }


    void GeneratePlatform(Vector2 touchPosition)
    {
        if (canHitTheBeat)
        {
            Vector3 clickOnWorld = Camera.main.ScreenToWorldPoint(touchPosition);
            GameObject platform = Instantiate(gameController.listNextPlatforms[0], new Vector3(clickOnWorld.x, clickOnWorld.y, 0), Quaternion.identity);
            platform.transform.localScale = platform.transform.localScale * sizeMultiplier;
            currentBeatIndicator.gameObject.SetActive(false);
            gameController.HasMatchedClick();
        }
        else
        {
            Instantiate(sfxError);
            currentBeatIndicator.MakeRed();
            gameController.HasMissedClick();
        }
    }

    float bpmTime()
    {
        return 60f / bpm;
    }

    float timeEarly()
    {
        return (bpmTime() * 0.65f);
    }

    float timeOK()
    {
        return (bpmTime() * 0.75f);
    }

    float timeNice()
    {
        return (bpmTime() * 0.85f);
    }

    float timePerfect()
    {
        return (bpmTime() * 0.95f);
    }

    float timeLate()
    {
        return (bpmTime() * 1.25f);
    }
}
