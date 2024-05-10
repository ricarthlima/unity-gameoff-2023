using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBeatController : MonoBehaviour
{

    private float timeStarted;
    [SerializeField] private float timePassed;

    [Header("Visual")]
    [SerializeField] VisualBeatIndicatorController currentBeatIndicator;

    [Header("Controllers")]
    GameController gameController;


    int beatStep = 0;
    bool canHitTheBeat = false;
    float sizeMultiplier = 1;

    bool hasClicked = false;

    float innerBPM;

    private void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        innerBPM = gameController.bpm / gameController.beatsToPortal;
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
        if (timePassed > timeDie())
        {
            Destroy(gameObject);
        }

        if (gameController.devIsAutoGeneratingPlatforms && !devHasGenerated)
        {
            if (timePassed > bpmTime())
            {
                OnTouched(currentBeatIndicator.transform.position, false);
                devHasGenerated = true;
            }
        }

        switch (beatStep)
        {
            case 0:
                {
                    if (timePassed > timeEarly())
                    {
                        //currentBeatIndicator.ShowPortal();
                        beatStep++;
                    }
                    return;
                }
            case 1:
                {
                    if (timePassed > timeOK())
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
                    if (timePassed > bpmTime())
                    {
                        gameController.TeleportPlayer(transform.position);
                        beatStep++;
                    }
                    return;
                }
                // case 3:
                //     {
                //         if (timePassed > timeLate())
                //         {
                //             currentBeatIndicator.MakeRed();
                //             canHitTheBeat = false;
                //         }
                //         return;
                //     }
        }



    }

    public void OnTouched(Vector2 touchPosition, bool needToConvert = true)
    {
        if (!hasClicked)
        {
            hasClicked = true;

            Vector3 clickOnWorld = touchPosition;

            if (needToConvert)
            {
                clickOnWorld = Camera.main.ScreenToWorldPoint(touchPosition);
            }

            if (timePassed < timeOK())
            {
                //Instantiate(textFailPrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
            }
            else if (timePassed < timeNice())
            {
                //Instantiate(textOKPrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
            }
            else if (timePassed < timePerfect())
            {
                //Instantiate(textNicePrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
                sizeMultiplier = 1;
            }
            else if (timePassed < timeDie())
            {
                //Instantiate(textPerfectPrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
                sizeMultiplier = 1.30f;
            }
            // else if (timePassed > timeLate())
            // {
            //     //Instantiate(textFailPrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);                
            // }

            GeneratePlatform(touchPosition, needToConvert);
        }

    }


    void GeneratePlatform(Vector2 touchPosition, bool needToConvert = true)
    {
        // if (canHitTheBeat)
        // {
        //     currentBeatIndicator.ShowPortal();
        if (!gameController.hasMissedClick)
        {
            Vector3 clickOnWorld = touchPosition;
            if (needToConvert)
            {
                clickOnWorld = Camera.main.ScreenToWorldPoint(touchPosition);
            }

            GameObject platform = Instantiate(gameController.listNextPlatforms[0], new Vector3(clickOnWorld.x, clickOnWorld.y, 0), Quaternion.identity);
            platform.transform.localScale = platform.transform.localScale * sizeMultiplier;
            currentBeatIndicator.gameObject.SetActive(false);
            gameController.audioController.PlaySFXClap();
            gameController.HasMatchedClick();
            GetComponent<SelfDestroyController>().isStoped = false;
            GetComponent<SelfDestroyController>().timeToDestroy = 1;
        }


        // }
        // else
        // {
        //     gameController.audioController.PlaySFXError();
        //     currentBeatIndicator.MakeRed();
        //     gameController.HasMissedClick();
        // }
    }

    float bpmTime()
    {
        return 60f / innerBPM;
    }

    float timeEarly()
    {
        return bpmTime() * 0.45f;
    }

    float timeOK()
    {
        return bpmTime() * 0.9f;
    }

    float timeNice()
    {
        return bpmTime() * 0.75f;
    }

    float timePerfect()
    {
        return bpmTime() * 0.95f;
    }

    // float timeLate()
    // {
    //     return bpmTime() * 1.33f;
    // }

    float timeDie()
    {
        return bpmTime() * 1.10f;
    }
}
