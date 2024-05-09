using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum TowerLevel { dungeon, stairway, throne };

public class GameController : MonoBehaviour
{
    #region "Atributes"
    [Header("Levels Data")]
    [SerializeField] LevelData levelData;

    [Header("Controllers")]
    [SerializeField] PlayerPrefsController prefs;
    [SerializeField] CanvasController canvasController;

    public float bpm;
    [SerializeField] private int beatsUntilMoveCamera;
    public float maxSpawnX;
    [SerializeField] private float minSpawnY;
    [SerializeField] private float maxSpawnY;
    public TowerLevel level = TowerLevel.dungeon;
    public int beatCount = 0;

    [SerializeField] private float timeAwaintingDungeonFloor;

    [Header("Scene Objects")]
    public SmoothCameraFollow cameraFollow;
    public PlayerController player;
    [SerializeField] private GameObject guidePlataform;

    [Header("Background Objects")]
    [SerializeField] private GameObject backgroundLoopPrefab;
    private GameObject backgroundLoop;
    private BackgroundLoopController backgroundLoopController;
    readonly float backgroundLoopVerticalDistance = 12.4f;

    [Header("UI")]
    [SerializeField] Canvas renderCanvas;
    [SerializeField] TextMeshProUGUI textHeightTraveled;
    [SerializeField] TextMeshProUGUI textBPM;
    [SerializeField] private Image imageNextPlatform;
    [SerializeField] private Image imageSecondPlatform;
    [SerializeField] private GameObject textPause;

    [Header("Audio")]
    public AudioController audioController;

    [Header("Prefabs")]
    [SerializeField] GameObject portalPrefab;
    [SerializeField] GameObject[] listPlatformsPrefab;


    // Controllers
    public Vector2 portalPosition = Vector2.zero;
    public List<GameObject> listNextPlatforms = new List<GameObject>();

    GameObject currentPortal;

    // Infos
    float maxHeightTraveled = 0;
    float heightTraveled = 0;
    float countTimePlaying = 0;

    // Spawn da Plafatorma
    float timerBeat = 0;

    public bool hasMissedClick = false;

    Vector2 lastBackgroundPosition = new Vector2(0, 14.77f);

    // New game controllers
    public bool isGamePaused = true;
    bool isGameInCutscene = true; // Começos de fase são cutscene, cutscenes são cutscenes, queda é cutscene

    // Cutscenes
    bool needToShowFirstCutsceneDungeon = true;
    bool needToShowStairwayCutscene = false;

    bool isPlayerFalling = false;
    bool hasStartedToFall = false;

    [Header("DevTools")]
    [SerializeField] private TextMeshProUGUI textDevAutoGeneratePlatform;
    [SerializeField] private TextMeshProUGUI textDevGameTimeScale;
    public bool devIsAutoGeneratingPlatforms = false;
    float devTimeScale = 1;

    #endregion

    #region "Life Cycles"
    void Start()
    {
        //Application.targetFrameRate = 60;
        portalPosition = player.transform.position;

        audioController.PlayMenuBGM();

        // Platforms
        listNextPlatforms.Add(listPlatformsPrefab[0]);
        listNextPlatforms.Add(listPlatformsPrefab[0]);

        UpdateImagesPlatform();

        backgroundLoop = Instantiate(backgroundLoopPrefab, new Vector3(-20, 0, 0), Quaternion.identity);
        backgroundLoopController = backgroundLoop.GetComponent<BackgroundLoopController>();

        maxHeightTraveled = prefs.RecHigh;

        maxSpawnX = levelData.GetInitialHorizontalRange(level);
        bpm = levelData.GetInitialBPM(level);
    }

    private void Update()
    {
        VerifyMouseClick();
        CycleGuideFollowsMouse();
        CycleEscButton();
        CycleTestFalling();
        UpdateUI();

        timerBeat += Time.deltaTime;
        if (!isGamePaused)
        {
            countTimePlaying += Time.deltaTime;
        }

        //TODO: DEV TOOLS
        if (Input.GetKeyDown(KeyCode.F1))
        {
            devIsAutoGeneratingPlatforms = !devIsAutoGeneratingPlatforms;
            textDevAutoGeneratePlatform.color = devIsAutoGeneratingPlatforms == true ? Color.green : Color.red;
        }

        bool changeScale = false;
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (devTimeScale > 1)
            {
                devTimeScale -= 1f;
            }
            else
            {
                devTimeScale -= 0.25f;
            }

            devTimeScale %= 6;
            devTimeScale = Mathf.Max(devTimeScale, 0);

            changeScale = true;
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            devTimeScale += 1f;
            devTimeScale %= 6;
            devTimeScale = Mathf.Floor(devTimeScale);
            changeScale = true;
        }

        if (changeScale)
        {
            Time.timeScale = devTimeScale;
            audioController.ChangeBGMSpeed(devTimeScale);
            textDevGameTimeScale.text = "Velocidade do jogo: x" + devTimeScale.ToString("F2");
        }
    }

    private void FixedUpdate()
    {
        if (!isGamePaused)
        {
            CycleUniqueSceneEvents();
            if (!isGameInCutscene)
            {
                if (!isPlayerFalling)
                {
                    CycleGeneratePortalsByBeat();
                }
                else
                {
                    IsFalling();
                }
            }
            else
            {
                if (needToShowFirstCutsceneDungeon)
                {
                    StartCoroutine(WaitCutsceneFirstDungeon());
                    needToShowFirstCutsceneDungeon = false;
                }

                if (needToShowStairwayCutscene)
                {
                    StartCoroutine(WaitCutsceneStairway());
                    needToShowStairwayCutscene = false;
                }
            }

        }
    }

    bool hasEnteredInPortal = false;
    bool hasExitedPortal = false;
    void CycleGeneratePortalsByBeat()
    {
        if (timerBeat > 0.3f && !hasExitedPortal)
        {
            player.AnimationExitPortal(false);
            hasExitedPortal = true;
        }

        if (timerBeat >= (60f / bpm) - 0.3f && !hasEnteredInPortal)
        {
            player.AnimationEnterPortal();
            hasEnteredInPortal = true;
        }

        if (timerBeat >= (60f / bpm))
        {
            player.AnimationEnterPortal(false);
            player.transform.position = portalPosition;
            player.AnimationExitPortal();
            hasEnteredInPortal = false;
            hasExitedPortal = false;

            if (player.transform.position.y > lastBackgroundPosition.y - backgroundLoopVerticalDistance)
            {
                lastBackgroundPosition = new Vector2(lastBackgroundPosition.x, lastBackgroundPosition.y + backgroundLoopVerticalDistance);
                GameObject newBackground = Instantiate(backgroundLoop, lastBackgroundPosition, Quaternion.identity);
                newBackground.transform.parent = GameObject.Find("Background").transform;
            }

            GeneratePortal();

            timerBeat = 0;

            canvasController.MoveProgressMage(beatCount / 105f);
            return;
        }


    }

    IEnumerator WaitCutsceneFirstDungeon()
    {
        maxSpawnX = levelData.GetInitialHorizontalRange(level);
        beatCount = 0;

        CleanPortalsAndPlatforms();

        player.WalkToCenter(timeAwaintingDungeonFloor);
        audioController.PlayDungeon();

        cameraFollow.SetFastFollow();

        yield return new WaitForSeconds(timeAwaintingDungeonFloor);

        cameraFollow.SetSlowFollow();

        portalPosition = player.transform.position;
        isGameInCutscene = false;
    }

    IEnumerator WaitCutsceneStairway()
    {
        maxSpawnX = 0;
        beatCount = 0;

        CleanPortalsAndPlatforms();

        player.WalkToCenter(8.5f);
        audioController.PlayStairway();

        yield return new WaitForSeconds(8.5f);

        portalPosition = player.transform.position;
        isGameInCutscene = false;
    }

    void CycleTestFalling()
    {
        if (player.rb.velocity.y < -7)
        {
            isPlayerFalling = true;
        }
    }

    void CycleUniqueSceneEvents()
    {
        switch (level)
        {
            case TowerLevel.dungeon:
                {
                    if (beatCount == 101)
                    {
                        backgroundLoopController.SetStairwayStart();
                        return;
                    }
                    if (beatCount == 104)
                    {
                        backgroundLoopController.SetStairwayLoop();
                    }
                    if (beatCount == 110)
                    {
                        isGameInCutscene = true;
                        CleanPortalsAndPlatforms();
                    }
                    break;
                }
            case TowerLevel.stairway:
                {
                    break;
                }
            case TowerLevel.throne:
                {
                    break;
                }
        }
    }

    void CycleGuideFollowsMouse()
    {
        if (!isGamePaused)
        {
            guidePlataform.SetActive(true);
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 1;
            guidePlataform.transform.position = mousePosition;
            Cursor.visible = false;
        }
        else
        {
            guidePlataform.SetActive(false);
            Cursor.visible = true;
        }
    }

    void CleanPortalsAndPlatforms()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        foreach (GameObject platform in platforms)
        {
            Destroy(platform);
        }

        GameObject[] portals = GameObject.FindGameObjectsWithTag("Portal");
        foreach (GameObject portal in portals)
        {
            Destroy(portal);
        }
    }

    #endregion

    #region "Game Logic"

    void StartScene(TowerLevel scene)
    {
        isGameInCutscene = true;

        if (scene == TowerLevel.dungeon)
        {
            needToShowFirstCutsceneDungeon = true;
            bpm = audioController.bpmDungeon;
            return;
        }

        if (scene == TowerLevel.stairway)
        {
            needToShowStairwayCutscene = true;
            bpm = audioController.bpmStairway;
            return;
        }

        if (scene == TowerLevel.throne)
        {
            bpm = audioController.bpmThrone;
            return;
        }
    }

    void GeneratePortal()
    {
        if (beatCount % beatsUntilMoveCamera == 0)
        {
            cameraFollow.SetFastFollow();
        }
        else
        {
            cameraFollow.SetSlowFollow();
        }

        beatCount += 1;
        if (beatCount == 1)
        {
            portalPosition = Vector2.zero;
        }
        else
        {
            float y = player.gameObject.transform.position.y + Random.Range(minSpawnY, maxSpawnY); ; //  + (backgroundLoopVerticalDistance/4) ;
            float x = Random.Range(-1 * maxSpawnX, maxSpawnX);
            portalPosition = new Vector2(x, y);
        }


        currentPortal = Instantiate(portalPrefab, portalPosition, Quaternion.identity);
        currentPortal.GetComponent<PlatformBeatController>().bpm = bpm;
        currentPortal.transform.Find("VisualBeatIndicator").GetComponent<VisualBeatIndicatorController>().bpm = bpm;

    }

    void IsFalling()
    {

        player.ResetToIdle();
        if (!hasStartedToFall)
        {
            hasStartedToFall = true;
            cameraFollow.SetFalling(true);
            audioController.PlayEffectFalling();
            player.ResetToIdle();

            CleanPortalsAndPlatforms();
        }

        GameObject platform = GameObject.FindGameObjectWithTag("StartPlatform");
        float diff = Mathf.Abs(platform.transform.position.y - player.transform.position.y);
        if (diff < 25 && player.rb.velocity.y < -25)
        {
            player.rb.drag += 25 * Time.deltaTime;
            player.rb.drag = Mathf.Min(10, player.rb.drag);
        }

    }


    void StabilizePlayerAndCamera()
    {
        if (player.gameObject.transform.rotation.z != 0)
        {
            player.transform.localEulerAngles = Vector3.zero;
        }
        player.rb.drag = 0.15f;
        cameraFollow.SetFalling(false);
    }

    private bool hasTouchedGround = false;

    public void TouchedGround(TowerLevel paramLevel)
    {
        print("Tocou o chão em " + paramLevel);
        level = paramLevel;

        hasStartedToFall = false;
        if (!isGamePaused && !hasTouchedGround)
        {
            player.ResetToIdle();
            StartScene(level);
            StabilizePlayerAndCamera();
            beatCount = 0;
            hasTouchedGround = true;
            hasMissedClick = false;
            isPlayerFalling = false;
        }
    }

    public void ExitedGround()
    {
        hasTouchedGround = false;
    }

    #endregion

    #region "UI"

    void UpdateUI()
    {
        heightTraveled = (player.transform.position.y + 4.029932f) * 1.8f;
        if (heightTraveled > maxHeightTraveled)
        {
            maxHeightTraveled = heightTraveled;
            prefs.RecHigh = maxHeightTraveled;
        }

        float fps = 1f / Time.deltaTime;
        textBPM.text = "BPM: " + bpm.ToString();
        textHeightTraveled.text =
            "Contagem de Beats: " + beatCount +
            "\nAltura Máxima: " + maxHeightTraveled.ToString("F2") + "m" +
            "\nAltura: " + heightTraveled.ToString("F2") + "m" +
            "\nTempo jogando: " + countTimePlaying.ToString("F0") +
            "\nFPS: " + fps.ToString("F2");
    }

    void UpdateImagesPlatform()
    {
        imageNextPlatform.sprite = listNextPlatforms[0].GetComponent<SpriteRenderer>().sprite;
        imageSecondPlatform.sprite = listNextPlatforms[1].GetComponent<SpriteRenderer>().sprite;
        guidePlataform.GetComponent<SpriteRenderer>().sprite = imageNextPlatform.sprite;
        guidePlataform.transform.localScale = listNextPlatforms[0].transform.localScale;
    }
    #endregion

    #region "Platform"

    public void HasMatchedClick()
    {
        listNextPlatforms[0] = listNextPlatforms[1];
        listNextPlatforms[1] = listPlatformsPrefab[Random.Range(0, listPlatformsPrefab.Length)];

        UpdateImagesPlatform();
    }

    public void HasMissedClick()
    {
        hasMissedClick = true;
    }

    #endregion

    #region "Controls"
    void CycleEscButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }


    void VerifyMouseClick()
    {
        if (!isGamePaused)
        {
            bool isClicked = false;
            Vector2 touchPosition = Vector2.negativeInfinity;

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.X))
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
                bool needToBeat = true;

                RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(touchPosition), Vector2.zero);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.CompareTag("Portal"))
                        {
                            hit.collider.gameObject.GetComponent<PlatformBeatController>().OnTouched(touchPosition);
                            needToBeat = false;
                        }
                    }
                }
                if (needToBeat)
                {
                    audioController.PlaySFXBeat();
                }
            }
        }

    }
    #endregion

    #region "Game"
    public void StartGame()
    {
        PauseGame();
    }

    public void PauseGame()
    {
        if (!isGamePaused)
        {
            isGamePaused = true;
            canvasController.ShowScene(InternalScenes.main);
            Time.timeScale = 0;
            audioController.PauseEverything();
            textPause.SetActive(true);
        }
        else
        {
            isGamePaused = false;
            canvasController.ShowScene(InternalScenes.hud);
            Time.timeScale = 1;
            audioController.UnPauseEverything();
            textPause.SetActive(false);
        }

    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QUIT");
    }
    #endregion
}
