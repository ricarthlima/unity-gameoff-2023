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

    [Header("Prefabs")]
    [SerializeField] GameObject portalPrefab;
    [SerializeField] GameObject[] listPlatformsPrefab;

    // Controllers
    float timePassed = 0;
    Vector2 portalPosition = Vector2.zero;
    bool isStartGame = false;
    bool isGameRunning = false;
    List<GameObject> listNextPlatforms = new List<GameObject>();
    bool isFirstPlatform = true;


    bool isCanSpawnPlatform = false;
    float cooldownPlatformPassed = 0;

    // Infos
    float maxHeightTraveled = 0;
    float heightTraveled = 0;

    // Esperar para recomeçar
    float countTimeToRestart = 0;
    readonly float waitTimeToRestart = 0.5f;
    bool isWaitingTimeToRestart = false;

    #region "Life Cycles"
    void Start()
    {
        portalPosition = player.transform.position;

        // Open MainMenu 
        panelMainMenu.SetActive(true);
        panelGameHUD.SetActive(false);

        // Platforms
        listNextPlatforms.Add(listPlatformsPrefab[0]);
        listNextPlatforms.Add(listPlatformsPrefab[1]);

        UpdateImagesPlatform();
    }

    private void Update()
    {
        if (isStartGame)
        {
            bool isWaiting = CycleToRestartGame();

            if (isGameRunning && !isWaiting)
            {
                UpdateUI();
                VerifyMouseClick();

                bool isFalling = CycleTestFalling();
                if (!isFalling)
                {
                    CycleBPM();
                    CycleCooldownPlatform();
                }
            }
        }
    }

    bool CycleToRestartGame()
    {
        if (isWaitingTimeToRestart)
        {
            countTimeToRestart += Time.deltaTime;
            if (countTimeToRestart > waitTimeToRestart)
            {
                Restart();
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

    void CycleBPM()
    {
        timePassed += Time.deltaTime;
        if (timePassed > (60f / (float) bpm))
        {
            timePassed = 0;
            player.gameObject.transform.position = portalPosition;
            GeneratePortal();
        }
    }
    
    void CycleCooldownPlatform()
    {
        cooldownPlatformPassed += Time.deltaTime;
        if (cooldownPlatformPassed > (60f / (float) bpm) - 0.2f)
        {
            isCanSpawnPlatform = true;
        }
    }
    #endregion

    void GeneratePortal()
    {
        float y = player.gameObject.transform.position.y + Random.Range(3,6);
        float x = Random.Range(-1*maxSpawnX, maxSpawnX);

        portalPosition = new Vector2(x, y);

        Instantiate(portalPrefab, portalPosition, Quaternion.identity);
    }

    void IsFalling()
    {
        isGameRunning = false;
        cameraFollow.SetFalling(true);
        audioBGM.Stop();
        audioRewindSFX.Play();
    }
    
    void Restart()
    {
        timePassed = 0;
        portalPosition = player.transform.position;
        isGameRunning = true;
        isFirstPlatform = true;
        cameraFollow.SetFalling(false);
    }

    public void TouchedTheGround()
    {
        audioRewindSFX.Stop();
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
        if (isGameRunning && isCanSpawnPlatform)
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

            isCanSpawnPlatform = false;
            cooldownPlatformPassed = 0;
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
