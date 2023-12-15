using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Controllers")]
    public float bpm;
    [SerializeField] private float maxSpawnX;    
    [SerializeField] float waitSecondsToRestart;

    [Header("SceneObjects")]
    public SmoothCameraFollow cameraFollow;
    public PlayerController player;

    [Header("UI")]
    [SerializeField] GameObject panelMainMenu;
    [SerializeField] GameObject panelGameHUD;
    [SerializeField] TextMeshProUGUI textHeightTraveled;
    [SerializeField] TextMeshProUGUI textBPM;
    [SerializeField] private Image imageNextPlatform;
    [SerializeField] private Image imageSecondPlatform;

    [Header("BGM and SFX")]
    [SerializeField] private AudioSource audioBGM;
    [SerializeField] private AudioSource audioRewindSFX;
    [SerializeField] private GameObject sfxPrefabError;

    [Header("Prefabs")]
    [SerializeField] GameObject portalPrefab;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] private GameObject beatIndicatorPrefab;
    [SerializeField] GameObject[] listPlatformsPrefab;

    // Controllers
    float timePassed = 0;
    Vector2 portalPosition = Vector2.zero;
    bool isStartGame = false;
    bool isGameRunning = false;
    List<GameObject> listNextPlatforms = new List<GameObject>();
    bool isFirstPlatform = true;

    GameObject currentPortal;
    VisualBeatIndicatorController currentBeatIndicator;

    // Infos
    float maxHeightTraveled = 0;
    float heightTraveled = 0;

    // Esperar para recomeçar
    bool isWaitingTimeToRestart = false;
    float countTimeToRestart = 0;

    // Spawn da Plafatorma
    bool canSpawnPlatform = false;

    float timerBeat = 0;

    bool hasMissedClick = false;

    bool canHitTheBeat = false;

    int beatStep = 0;

    #region "Life Cycles"
    void Start()
    {
        portalPosition = player.transform.position;

        // Open MainMenu 
        panelMainMenu.SetActive(true);
        panelGameHUD.SetActive(false);

        // Platforms
        listNextPlatforms.Add(listPlatformsPrefab[0]);
        listNextPlatforms.Add(listPlatformsPrefab[0]);

        UpdateImagesPlatform();
    }

    private void Update()
    {
        if (isStartGame)
        {
            bool isWaiting = CycleToRestartGame();
            UpdateUI();

            if (isGameRunning && !isWaiting)
            {                
                VerifyMouseClick();

                bool isFalling = CycleTestFalling();
                if (!isFalling)
                {
                    timerBeat += Time.deltaTime;


                    switch (beatStep)
                    {
                        // 1. Um beat antes 
                        case 0:
                            {
                                // Gera o portal e o indicador
                                GeneratePortal();

                                // Liberar o Clique
                                canSpawnPlatform = true;
                                beatStep++;
                                return;
                            }
                        case 1:
                            {
                                if (timerBeat > ((60f / bpm) * 0.55f) && !hasMissedClick)
                                {
                                    currentBeatIndicator.ShowPortal();
                                    beatStep++;
                                }
                                return;
                            }
                        case 2:
                            {
                                if (timerBeat > ((60f / bpm) * 0.65f) && !hasMissedClick)
                                {
                                    // Libera clique correto
                                    canHitTheBeat = true;

                                    // Indicador fica amaerelo
                                    if (currentBeatIndicator != null)
                                    {
                                        currentBeatIndicator.MakeYellow();
                                    }
                                    beatStep++;
                                }
                                return;
                            }
                        case 3:
                            {
                                if (timerBeat >= (60f / bpm) && !hasMissedClick)
                                {
                                    // Teleporta o player
                                    player.gameObject.transform.position = portalPosition;

                                    RestartBeat();
                                }

                                return;
                            }
                    }                    
                }
            }
        }
    }

    bool CycleToRestartGame()
    {
        if (isWaitingTimeToRestart)
        {
            countTimeToRestart += Time.deltaTime;
            if (countTimeToRestart > waitSecondsToRestart)
            {
                Restart();
                RestartBeat();
                isWaitingTimeToRestart = false;
                countTimeToRestart = 0;
            }
            return true;
        }
        return false;        
    }

    bool CycleTestFalling()
    {
        if (player.rb.velocity.y < -5)
        {
            IsFalling();
            return true;
        }
        return false;
    }
    #endregion

    void GeneratePortal()
    {
        float y = player.gameObject.transform.position.y + Random.Range(1.5f, 5f);
        float x = Random.Range(-1*maxSpawnX, maxSpawnX);

        portalPosition = new Vector2(x, y);

        currentPortal = Instantiate(portalPrefab, portalPosition, Quaternion.identity);
        currentBeatIndicator = currentPortal.GetComponent<PortalController>().visualBeatIndicator;
        currentBeatIndicator.bpm = bpm;
    }

    void DestroyPortal()
    {
        if (currentPortal != null)
        {
            currentPortal.GetComponent<SelfDestroyController>().timeToDestroy = 0.25f;
            currentPortal.GetComponent<SelfDestroyController>().isStoped = false;
        }        
    }

    void RestartBeat()
    {
        // Proibe o clique
        canSpawnPlatform = false;

        // Reiniciar beat
        timerBeat = 0;
        beatStep = 0;
        canHitTheBeat = false;
        hasMissedClick = false;

        // Destruir portal
        DestroyPortal();
    }

    void IsFalling()
    {
        isGameRunning = false;
        canHitTheBeat = false;
        cameraFollow.SetFalling(true);
        audioBGM.Stop();
        audioRewindSFX.Play();

        GameObject[] listPortalsRemaining = GameObject.FindGameObjectsWithTag("Portal");

        foreach (GameObject portal in listPortalsRemaining)
        {
            Destroy(portal);
        }
        GameObject[] listPlatformRemaining = GameObject.FindGameObjectsWithTag("Portal");
        foreach (GameObject platform in listPlatformRemaining)
        {
            Destroy(platform);
        }
    }
    
    void Restart()
    {
        timePassed = 0;
        portalPosition = player.transform.position;
        isGameRunning = true;
        isFirstPlatform = true;
    }

    public void TouchedTheGround()
    {
        audioRewindSFX.Stop();
        if (player.gameObject.transform.rotation.z != 0)
        {
            Vector3 position = player.transform.position;
            player.transform.localEulerAngles = Vector3.zero;
        }
        cameraFollow.SetFalling(false);
        isWaitingTimeToRestart = true;
    }

    public void StartGame()
    {
        panelMainMenu.SetActive(false);
        panelGameHUD.SetActive(true);
        isGameRunning = true;
        isStartGame = true;
        Time.timeScale = 1;
        Restart();
    }

    public void PauseGame()
    {
        panelMainMenu.SetActive(true);
        panelGameHUD.SetActive(false);
        Time.timeScale = 0;
    }

    #region "UI"

    public void UpdateBPM(float newBPM)
    {
        bpm = newBPM;
    }

    void UpdateUI()
    {
        heightTraveled = (player.transform.position.y + 4.029932f) * 1.8f;
        if (heightTraveled > maxHeightTraveled)
        {
            maxHeightTraveled = heightTraveled;
        }

        textBPM.text = "BPM: " + bpm.ToString();
        textHeightTraveled.text = "Max Height: " + maxHeightTraveled.ToString("F2") + "m\nHeight: " + heightTraveled.ToString("F2")+"m";
    }

    void UpdateImagesPlatform()
    {
        imageNextPlatform.sprite = listNextPlatforms[0].GetComponent<SpriteRenderer>().sprite;
        imageSecondPlatform.sprite = listNextPlatforms[1].GetComponent<SpriteRenderer>().sprite;
    }
    #endregion

    #region "Platform"
    void GeneratePlatform(Vector2 touchPosition)
    {
        if (isGameRunning && canSpawnPlatform)
        {
            if (canHitTheBeat)
            {
                if (isFirstPlatform)
                {
                    isFirstPlatform = false;
                    audioBGM.Play();
                }

                Vector3 clickOnWorld = Camera.main.ScreenToWorldPoint(touchPosition);
                Instantiate(listNextPlatforms[0], new Vector3(clickOnWorld.x, clickOnWorld.y, 0), Quaternion.identity);

                listNextPlatforms[0] = listNextPlatforms[1];
                listNextPlatforms[1] = listPlatformsPrefab[Random.Range(0, listPlatformsPrefab.Length)];

                UpdateImagesPlatform();

                canSpawnPlatform = false;
            }
            else
            {
                Instantiate(sfxPrefabError);
                currentBeatIndicator.MakeRed();
                hasMissedClick = true;
            }
            
        }
        
    }
    #endregion

    #region "Verify Clicks and Touchs"
    void VerifyMouseClick()
    {
        if (isGameRunning)
        {
            bool isClicked = false;
            Vector2 touchPosition = Vector2.negativeInfinity;

            if (Input.GetMouseButtonDown(0))
            {
                touchPosition = Input.mousePosition;
                isClicked = true;
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    touchPosition = touch.position;
                    isClicked = true;
                }
            }

            if (isClicked)
            {
                GeneratePlatform(touchPosition);

                RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(touchPosition), Vector2.zero);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider != null)
                    {
                        //if (hit.collider.CompareTag("Area"))
                        //{
                            
                        //}
                    }
                }
            }
        }

    }
    #endregion
}
