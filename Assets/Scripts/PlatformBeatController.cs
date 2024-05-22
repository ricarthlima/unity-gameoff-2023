using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBeatController : MonoBehaviour
{
    private float timeStarted;
    [SerializeField] private float timePassed;

    [Header("Visual")]
    [SerializeField] private GameObject indicatorGroup;
    [SerializeField] private GameObject fixedBeat;
    [SerializeField] private GameObject beat;
    [SerializeField] private GameObject portalSprite;
    [SerializeField] private float initialBeatScale;
    [SerializeField] private float finalBeatScale;


    [Header("Controllers")]
    GameController gameController;



    int beatSteps = 0;
    bool canHitTheBeat = false;
    float sizeMultiplier = 1;

    bool hasClicked = false;

    float innerBPM;
    float innerSpeed;

    private void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        innerBPM = gameController.bpm / gameController.beatsToPortal;
        innerSpeed = gameController.beatsToBeat;
    }

    private void Start()
    {
        timeStarted = Time.time;
    }

    bool devHasGenerated = false;
    private void Update()
    {
        timePassed = Time.time - timeStarted;

        UpdateBeatIndicator();
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
                OnTouched(transform.position, false);
                devHasGenerated = true;
            }
        }

        switch (beatSteps)
        {
            case 0:
                {
                    if (timePassed > timeShowPortal())
                    {
                        //currentBeatIndicator.ShowPortal();
                        beatSteps++;
                    }
                    return;
                }
            case 1:
                {
                    if (timePassed > timeAllowClick())
                    {
                        // Libera clique correto
                        canHitTheBeat = true;

                        // Indicador fica amarelo
                        MakeYellow();
                        beatSteps++;
                    }
                    return;
                }
            case 2:
                {
                    if (timePassed > timeTeleportPlayer())
                    {
                        gameController.TeleportPlayer(transform.position);
                        beatSteps++;
                    }
                    return;
                }
            case 3:
                {
                    if (timePassed > timeDenyClick())
                    {
                        beatSteps++;
                        canHitTheBeat = false;
                        MakeRed();
                    }
                    return;
                }
        }
    }

    public void OnTouched(Vector2 touchPosition, bool needToConvert = true)
    {

        if (!hasClicked)
        {
            hasClicked = true;
            if (canHitTheBeat)
            {

                GeneratePlatform(touchPosition, needToConvert);
            }
            else
            {
                gameController.HasMissedClick();
            }
        }

    }

    void GeneratePlatform(Vector2 touchPosition, bool needToConvert = true)
    {
        if (!gameController.hasMissedClick)
        {
            Vector3 clickOnWorld = touchPosition;
            if (needToConvert)
            {
                clickOnWorld = Camera.main.ScreenToWorldPoint(touchPosition);
            }

            GameObject platform = Instantiate(gameController.listNextPlatforms[0], new Vector3(clickOnWorld.x, clickOnWorld.y, 0), Quaternion.identity);
            platform.transform.localScale = platform.transform.localScale * sizeMultiplier;
            indicatorGroup.SetActive(false);
            gameController.audioController.PlaySFXBeat();
            gameController.HasMatchedClick();
            GetComponent<SelfDestroyController>().isStoped = false;
            GetComponent<SelfDestroyController>().timeToDestroy = 1;
        }
    }

    #region "Timers"

    float bpmTime()
    {
        return (60f / innerBPM) * innerSpeed;
    }

    float timeShowPortal()
    {
        return 0;
    }

    float timeAllowClick()
    {
        return bpmTime() * 0.925f;
    }

    float timeTeleportPlayer()
    {
        return bpmTime() * 1.05f;
    }

    float timeDenyClick()
    {
        return bpmTime() * 1.25f;
    }

    float timeDie()
    {
        return bpmTime() * 1.50f;
    }

    #endregion

    #region "Indicator"
    void UpdateBeatIndicator()
    {
        float newScale = Mathf.Lerp(initialBeatScale, finalBeatScale, timePassed / bpmTime());// (initialBeatScale + 1) - (timePassed / (60f / innerBPM) * initialBeatScale);
        beat.transform.localScale = new Vector3(newScale, newScale, newScale);

        float newOpacity = (50f + (timePassed / (60f / innerBPM) * 200f)) / 250f;
        Color oldColor = beat.GetComponent<SpriteRenderer>().color;
        beat.GetComponent<SpriteRenderer>().color = new Color(oldColor.r, oldColor.g, oldColor.b, newOpacity);
    }

    public void ShowPortal()
    {
        portalSprite.SetActive(true);
    }

    public void MakeYellow()
    {
        fixedBeat.GetComponent<SpriteRenderer>().color = Color.yellow;
        beat.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    public void MakeRed()
    {
        fixedBeat.GetComponent<SpriteRenderer>().color = Color.red;
        beat.GetComponent<SpriteRenderer>().color = Color.red;
    }

    #endregion

    #region  "Old code"
    // float timeEarly()
    // {
    //     return bpmTime() * 0.45f;
    // }

    // float timeOK()
    // {
    //     return bpmTime() * 0.9f;
    // }

    // float timeNice()
    // {
    //     return bpmTime() * 0.75f;
    // }

    // float timePerfect()
    // {
    //     return bpmTime() * 0.95f;
    // }

    // float timeLate()
    // {
    //     return bpmTime() * 1.33f;
    // }

    //  Vector3 clickOnWorld = touchPosition;

    //         if (needToConvert)
    //         {
    //             clickOnWorld = Camera.main.ScreenToWorldPoint(touchPosition);
    //         }

    //         if (timePassed < timeOK())
    //         {
    //             //Instantiate(textFailPrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
    //         }
    //         else if (timePassed < timeNice())
    //         {
    //             //Instantiate(textOKPrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
    //         }
    //         else if (timePassed < timePerfect())
    //         {
    //             //Instantiate(textNicePrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
    //             sizeMultiplier = 1;
    //         }
    //         else if (timePassed < timeDie())
    //         {
    //             //Instantiate(textPerfectPrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);
    //             sizeMultiplier = 1.30f;
    //         }
    //         // else if (timePassed > timeLate())
    //         // {
    //         //     //Instantiate(textFailPrefab, new Vector3(clickOnWorld.x + 1f, clickOnWorld.y, 0), Quaternion.identity);                
    //         // }
    #endregion

}
