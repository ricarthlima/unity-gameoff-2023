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
    public int level = 0;

    [Header("SceneObjects")]
    public SmoothCameraFollow cameraFollow;
    public PlayerController player;
    [SerializeField] private GameObject loopBackgroundDungeon;

    [Header("UI")]
    [SerializeField] Canvas renderCanvas;
    [SerializeField] GameObject panelMainMenu;
    [SerializeField] GameObject panelGameHUD;
    [SerializeField] TextMeshProUGUI textHeightTraveled;
    [SerializeField] TextMeshProUGUI textBPM;
    [SerializeField] private Image imageNextPlatform;
    [SerializeField] private Image imageSecondPlatform;

    [Header("BGM and SFX")]
    [SerializeField] private AudioSource audioBGM;
    [SerializeField] private AudioSource audioRewindSFX;
    [SerializeField] private List<AudioClip> listBGMS;
    [SerializeField] private List<int> listBPMS;

    [Header("Prefabs")]
    [SerializeField] GameObject portalPrefab;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject[] listPlatformsPrefab;


    // Controllers
    float timePassed = 0;
    Vector2 portalPosition = Vector2.zero;
    bool isStartGame = false;
    bool isGameRunning = false;
    public List<GameObject> listNextPlatforms = new List<GameObject>();
    bool isFirstPlatform = true;

    GameObject currentPortal;
    

    // Infos
    float maxHeightTraveled = 0;
    float heightTraveled = 0;

    // Esperar para recomeçar
    bool isWaitingTimeToRestart = false;
    float countTimeToRestart = 0;

    // Spawn da Plafatorma
    bool canSpawnPlatform = false;

    float timerBeat = 0;

    public bool hasMissedClick = false;

    Vector2 lastBackgroundPosition = new Vector2(0, 20f);

    // Levels
    
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
        CallScene();
    }

    private void Update()
    {
        CycleVerifyNextScene();
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
                    
                    if (timerBeat >= (60f / bpm))
                    {
                        player.gameObject.transform.position = portalPosition;
                        if (player.transform.position.y > lastBackgroundPosition.y - 25)
                        {
                            lastBackgroundPosition = new Vector2(lastBackgroundPosition.x, lastBackgroundPosition.y + 25);
                            GameObject newBackground = Instantiate(loopBackgroundDungeon, lastBackgroundPosition, Quaternion.identity);
                            newBackground.transform.parent = GameObject.Find("Background").transform;
                        }
                        GeneratePortal();

                        timerBeat = 0;
                        return;
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
    
    void CycleVerifyNextScene()
    {
        if (audioBGM.time >= (listBGMS[level].length - 2))
        {
            if (level < listBGMS.Count - 1)
            {
                level++;
                audioBGM.Stop();
                CallScene();
            }
        }
    }

    void CallScene()
    {
        audioBGM.clip = listBGMS[level];
        bpm = listBPMS[level];

        if (level == 0)
        {
            
        }

        if (level == 1)
        {

        }

        if (level == 2)
        {

        }
    }

    #endregion

    #region "Game Logic"
    void GeneratePortal()
    {
        float y = player.gameObject.transform.position.y + Random.Range(1.5f, 5f);
        float x = Random.Range(-1*maxSpawnX, maxSpawnX);

        portalPosition = new Vector2(x, y);

        currentPortal = Instantiate(portalPrefab, portalPosition, Quaternion.identity);
        currentPortal.GetComponent<PlatformBeatController>().bpm = bpm;
        currentPortal.transform.Find("VisualBeatIndicator").GetComponent<VisualBeatIndicatorController>().bpm = bpm;
    }

    void DestroyPortal()
    {
        if (currentPortal != null)
        {
            currentPortal.GetComponent<SelfDestroyController>().timeToDestroy = 0f;
            currentPortal.GetComponent<SelfDestroyController>().isStoped = false;
        }        
    }

    void RestartBeat()
    {
        // Proibe o clique
        canSpawnPlatform = false;

        // Reiniciar beat
        timerBeat = 0;
        hasMissedClick = false;

        // Destruir portal
        DestroyPortal();
    }

    void IsFalling()
    {
        isGameRunning = false;
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

    #endregion

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

    public void HasMatchedClick()
    {
        if (isFirstPlatform)
        {
            isFirstPlatform = false;
            audioBGM.Play();
        }

        listNextPlatforms[0] = listNextPlatforms[1];
        listNextPlatforms[1] = listPlatformsPrefab[Random.Range(0, listPlatformsPrefab.Length)];

        UpdateImagesPlatform();

        canSpawnPlatform = false;
    }

    public void HasMissedClick()
    {
        hasMissedClick = true;

        // Para o caso do player errar a primeira plataforma                
        if (player.isTouchingGround)
        {
            player.gameObject.transform.position = portalPosition;
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
                RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(touchPosition), Vector2.zero);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.CompareTag("Portal"))
                        {
                            hit.collider.gameObject.GetComponent<PlatformBeatController>().OnTouched(touchPosition);
                        }
                    }
                }
            }
        }

    }
    #endregion
}
