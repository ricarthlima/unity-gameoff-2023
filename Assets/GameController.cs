using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Controllers")]
    public int bpm;
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

    [Header("Prefabs")]
    [SerializeField] GameObject portalPrefab;
    [SerializeField] GameObject[] listPlatformsPrefab;

    // Controllers
    float timePassed = 0;
    Vector2 portalPosition = Vector2.zero;
    bool isStartedGame = false;
    [HideInInspector] public bool isCycleGameRunning = false;
    List<GameObject> listNextPlatforms = new List<GameObject>();

    // Game Phases
    enum PhaseBPM { portal, player, gameOver };
    PhaseBPM phaseBPM = PhaseBPM.portal;

    // Infos
    float maxHeightTraveled = 0;
    float heightTraveled = 0;

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

    // Update is called once per frame
    void Update()
    {
        if (isStartedGame && isCycleGameRunning)
        {
            VerifyMouseClick();
            timePassed += Time.deltaTime;
            if (timePassed > (60f/(float) bpm))
            {
                timePassed = 0;

                if (phaseBPM == PhaseBPM.gameOver)
                {
                    if (player.gameObject.transform.position.y < portalPosition.y - 1)
                    {                        
                        IsFalling();
                    }
                    else
                    {
                        phaseBPM = PhaseBPM.portal;
                    }
                    return;
                }

                if (phaseBPM == PhaseBPM.player)
                {
                    player.gameObject.transform.position = portalPosition;
                    phaseBPM = PhaseBPM.gameOver;
                    return;
                }

                if (phaseBPM == PhaseBPM.portal)
                {
                    GeneratePortal();
                    phaseBPM = PhaseBPM.player;
                    
                    return;
                }

            }

            heightTraveled = (player.transform.position.y + 4.029932f) * 1.8f;
            if (heightTraveled > maxHeightTraveled)
            {
                maxHeightTraveled = heightTraveled;
            }
            bpm = 60 + (int) (player.transform.position.y);
            UpdateUI();
        }        
    }

    void GeneratePortal()
    {
        
        float y = player.gameObject.transform.position.y + Random.Range(3,6);
        float x = Random.Range(-1*maxSpawnX, maxSpawnX);

        portalPosition = new Vector2(x, y);

        Instantiate(portalPrefab, portalPosition, Quaternion.identity);
        
    }

    void IsFalling()
    {
        isCycleGameRunning = false;
        cameraFollow.SetFalling(true);
    }

    public void Restart()
    {
        portalPosition = player.transform.position;
        timePassed = 0;
        isCycleGameRunning = true;
        cameraFollow.SetFalling(false);
    }

    public void StartGame()
    {
        panelMainMenu.SetActive(false);
        panelGameHUD.SetActive(true);
        isStartedGame = true;
        Restart();
    }

    public void PauseGame()
    {
        panelMainMenu.SetActive(true);
        panelGameHUD.SetActive(false);
        isCycleGameRunning = false;
    }

    #region "UI"
    void UpdateUI()
    {
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
        Vector3 clickOnWorld = Camera.main.ScreenToWorldPoint(touchPosition);
        Instantiate(listNextPlatforms[0], new Vector3(clickOnWorld.x, clickOnWorld.y, 0), Quaternion.identity);

        listNextPlatforms[0] = listNextPlatforms[1];
        listNextPlatforms[1] = listPlatformsPrefab[Random.Range(0, listPlatformsPrefab.Length)];

        UpdateImagesPlatform();
    }
    #endregion

    #region "Verify Clicks and Touchs"
    void VerifyMouseClick()
    {
        if (isStartedGame && isCycleGameRunning)
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
