using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBeatController : MonoBehaviour
{
    [Header("Timers")]
    public float bpm;

    private float timeStarted;
    [SerializeField] private float timePassed;

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
    [SerializeField] private GameObject sfxClap;
    [SerializeField] private GameObject sfxBeat;
    [SerializeField] private GameObject sfxPerfect;

    int beatStep = 0;
    bool canHitTheBeat = false;
    float sizeMultiplier = 1;

    bool hasClicked = false;

    private void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    private void Start()
    {
        timeStarted = Time.time;
    }

    bool devHasGenerated = false;
    private void Update()
    {
        timePassed = Time.time - timeStarted;        
    }

    void FixedUpdate()
    {       
        if (gameController.devIsAutoGeneratingPlatforms && !devHasGenerated){
            if (timePassed > timePerfect()){
                OnTouched(transform.position - new Vector3(0, 1.25f, 0), false);
                devHasGenerated = true;
            }
        }

        switch (beatStep)
        {
            case 0:
                {
                    if (timePassed > timeEarly() && !gameController.hasMissedClick)
                    {
                        currentBeatIndicator.ShowPortal();
                        beatStep++;
                    }
                    return;
                }
            case 1:
                {
                    if (timePassed > timeOK() && !gameController.hasMissedClick)
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
                    if (timePassed > timeLate())
                    {
                        currentBeatIndicator.MakeRed();
                        canHitTheBeat = false;
                    }
                    return;
                }
        }
        if (timePassed > timeDie())
        {
            Destroy(gameObject);
        }

        
    }

    public void OnTouched(Vector2 touchPosition, bool needToConvert = true)
    {
        if (!hasClicked){
            hasClicked = true;
            
            Vector3 clickOnWorld = touchPosition;

            if (needToConvert){
                clickOnWorld = Camera.main.ScreenToWorldPoint(touchPosition);
            }        

            if (timePassed < timeOK())
            {
                Instantiate(textFailPrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
            }
            else if (timePassed < timeNice())
            {
                Instantiate(textOKPrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
            }
            else if (timePassed < timePerfect())
            {
                Instantiate(textNicePrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
                sizeMultiplier = 1;
            }
            else if (timePassed < timeLate())
            {
                Instantiate(textPerfectPrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
                sizeMultiplier = 1.30f;
            }
            else if (timePassed > timeLate())
            {
                Instantiate(textFailPrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
                
            }

            GeneratePlatform(touchPosition, needToConvert);
        }
        
    }


    void GeneratePlatform(Vector2 touchPosition, bool needToConvert = true)
    {
        if (canHitTheBeat)
        {
            Vector3 clickOnWorld = touchPosition;
            if (needToConvert){
                clickOnWorld = Camera.main.ScreenToWorldPoint(touchPosition);
            }

            GameObject platform = Instantiate(gameController.listNextPlatforms[0], new Vector3(clickOnWorld.x, clickOnWorld.y, 0), Quaternion.identity);
            platform.transform.localScale = platform.transform.localScale * sizeMultiplier;
            currentBeatIndicator.gameObject.SetActive(false);
            if (timePassed >= timePerfect())
            {
                //TODO
                //Instantiate(sfxPerfect);                
            }
            //Instantiate(sfxClap);
            gameController.HasMatchedClick();
            GetComponent<SelfDestroyController>().isStoped = false;
            GetComponent<SelfDestroyController>().timeToDestroy = 1;
            
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
        return (bpmTime() * 0.45f);
    }

    float timeOK()
    {
        return (bpmTime() * 0.65f);
    }

    float timeNice()
    {
        return (bpmTime() * 0.75f);
    }

    float timePerfect()
    {
        return (bpmTime() * 0.95f);
    }

    float timeLate()
    {
        return (bpmTime() * 1.1f);
    }

    float timeDie()
    {
        return (bpmTime() * 1.25f);
    }
}
