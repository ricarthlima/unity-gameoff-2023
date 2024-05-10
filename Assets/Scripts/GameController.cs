using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TowerLevel { dungeon, stairway, throne };

public class GameController : MonoBehaviour
{
    #region "Atributes"
    [Header("Level Data")]
    public LevelData levelData;

    [Header("Controllers")]
    [SerializeField] CanvasController canvasController;

    public float bpm;
    [SerializeField] private int portalsUntilMoveCamera;
    public float maxSpawnX;
    [SerializeField] private float minSpawnY;
    [SerializeField] private float maxSpawnY;
    public TowerLevel level = TowerLevel.dungeon;
    public int beatCount = 0;

    [SerializeField] private float timeAwaintingDungeonFloor;

    [Header("Scene Objects")]
    public SmoothCameraFollow cameraFollow;
    public PlayerController player;
    public GameObject guidePlataform;

    [Header("Background Objects")]
    [SerializeField] private GameObject backgroundLoopPrefab;
    private GameObject backgroundLoop;
    private BackgroundLoopController backgroundLoopController;
    readonly float backgroundLoopVerticalDistance = 12.4f;

    [Header("Audio")]
    public AudioController audioController;

    [Header("Prefabs")]
    [SerializeField] GameObject portalPrefab;
    [SerializeField] GameObject[] listPlatformsPrefab;


    // Controllers
    //public Vector2 portalPosition = Vector2.zero;
    public List<GameObject> listNextPlatforms = new List<GameObject>();

    GameObject currentPortal;

    // Spawn da Plafatorma
    //float timerBeat = 0;
    float timeRestarted = 0;

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

    List<int> listUniqueEventsHappend = new List<int>();
    Vector2 lastPortalPosition = Vector2.zero;

    public float beatsToPortal;
    public int countPortals = 0;

    bool canGeneratePortals = false;

    float averageHumanReactionTime = 0.2f;

    #endregion

    #region "Life Cycles"
    float GetTimerBeat()
    {
        return Time.time - timeRestarted;
    }

    void RestartTimerBeat()
    {
        timeRestarted = Time.time;
    }
    void Start()
    {
        //Application.targetFrameRate = 60;
        //listPortalPositions.Add(player.transform.position);

        audioController.PlayMenuBGM();

        // Platforms
        listNextPlatforms.Add(listPlatformsPrefab[0]);
        listNextPlatforms.Add(listPlatformsPrefab[0]);

        backgroundLoop = Instantiate(backgroundLoopPrefab, new Vector3(-20, 0, 0), Quaternion.identity);
        backgroundLoopController = backgroundLoop.GetComponent<BackgroundLoopController>();

        maxSpawnX = levelData.GetInitialHorizontalRange(level);
        bpm = levelData.BPM(level);
    }

    private void Update()
    {
        VerifyMouseClick();
        CycleGuideFollowsMouse();
        CycleEscButton();
        CycleTestFalling();


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

            if (GetTimerBeat() > (60f / bpm) && !isPlayerFalling)
            {
                beatCount += 1;
                RestartTimerBeat();
            }

            if (!isGameInCutscene)
            {
                if (!isPlayerFalling)
                {
                    CycleUpdateBackground();
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

    void CycleUpdateBackground()
    {
        if (player.transform.position.y > lastBackgroundPosition.y - backgroundLoopVerticalDistance)
        {
            lastBackgroundPosition = new Vector2(lastBackgroundPosition.x, lastBackgroundPosition.y + backgroundLoopVerticalDistance);
            GameObject newBackground = Instantiate(backgroundLoop, lastBackgroundPosition, Quaternion.identity);
            newBackground.transform.parent = GameObject.Find("Background").transform;
        }
    }
    void CycleGeneratePortalsByBeat()
    {


        if (beatCount % beatsToPortal == 0 && canGeneratePortals)
        {
            GeneratePortal();
        }

        if (GetTimerBeat() > (60f / bpm))
        {
            beatCount += 1;
            RestartTimerBeat();
        }
    }

    IEnumerator WaitCutsceneFirstDungeon()
    {
        maxSpawnX = levelData.GetInitialHorizontalRange(level);
        canGeneratePortals = false;

        CleanPortalsAndPlatforms();

        player.WalkToCenter(timeAwaintingDungeonFloor - averageHumanReactionTime);
        audioController.PlayDungeon();

        cameraFollow.SetFastFollow();

        yield return new WaitForSeconds(timeAwaintingDungeonFloor - averageHumanReactionTime);

        canGeneratePortals = true;

        hasMissedClick = false;

        cameraFollow.SetSlowFollow();

        isGameInCutscene = false;
    }

    IEnumerator WaitCutsceneStairway()
    {
        maxSpawnX = 0;
        beatCount = 0;

        CleanPortalsAndPlatforms();

        player.WalkToCenter(8.5f);
        audioController.PlayStairway();

        canGeneratePortals = false;

        yield return new WaitForSeconds(8.5f);


        hasMissedClick = false;

        canGeneratePortals = true;

        isGameInCutscene = false;
    }

    void CycleTestFalling()
    {
        if (player.rb.velocity.y < -7)
        {
            isPlayerFalling = true;
        }
    }

    void CheckUniqueEvent(TowerLevel towerLevel, int beat, int id, System.Action action)
    {
        if (level == towerLevel && beatCount == beat && !listUniqueEventsHappend.Contains(id))
        {
            listUniqueEventsHappend.Add(id);
            action();
        }
    }

    void CycleUniqueSceneEvents()
    {
        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 68 - Mathf.FloorToInt(beatsToPortal),
            id: 1,
            action: () =>
                {
                    maxSpawnX = 4f;
                    canvasController.ShowRedWarning();
                    canGeneratePortals = false;
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 68,
            id: 2,
            action: () =>
                {
                    beatsToPortal = 3;
                    canGeneratePortals = true;
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 130 - Mathf.FloorToInt(beatsToPortal),
            id: 3,
            action: () =>
                {
                    maxSpawnX = 3.5f;
                    canvasController.ShowRedWarning();
                    canGeneratePortals = false;
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 130,
            id: 4,
            action: () =>
                {
                    beatsToPortal = 2;
                    canGeneratePortals = true;
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 190 - Mathf.FloorToInt(beatsToPortal),
            id: 5,
            action: () =>
                {
                    maxSpawnX = 2;
                    canvasController.ShowRedWarning();
                    canGeneratePortals = false;
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 190,
            id: 6,
            action: () =>
                {
                    beatsToPortal = 1;
                    canGeneratePortals = true;
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 220 - Mathf.FloorToInt(beatsToPortal),
            id: 7,
            action: () =>
                {
                    maxSpawnX = levelData.GetInitialHorizontalRange(level);
                    canvasController.ShowWhiteWarning();
                    canGeneratePortals = false;
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 220,
            id: 8,
            action: () =>
                {
                    beatsToPortal = 4;
                    canGeneratePortals = true;
                }
        );
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
        bpm = levelData.BPM(scene);

        if (scene == TowerLevel.dungeon)
        {
            needToShowFirstCutsceneDungeon = true;

            return;
        }

        if (scene == TowerLevel.stairway)
        {
            needToShowStairwayCutscene = true;
            return;
        }

        if (scene == TowerLevel.throne)
        {
            return;
        }
    }
    List<int> listBeatsWithPortals = new List<int>();
    void GeneratePortal()
    {
        if (!listBeatsWithPortals.Contains(beatCount))
        {
            listBeatsWithPortals.Add(beatCount);

            if (countPortals % portalsUntilMoveCamera == 0)
            {
                cameraFollow.SetFastFollow();
            }
            else
            {
                cameraFollow.SetSlowFollow();
            }

            countPortals++;

            Vector2 newPortalPosition = Vector2.zero;

            if (countPortals > 1)
            {
                float y = lastPortalPosition.y + Random.Range(minSpawnY, maxSpawnY); ; //  + (backgroundLoopVerticalDistance/4) ;
                float x = Random.Range(-1 * maxSpawnX, maxSpawnX);
                newPortalPosition = new Vector2(x, y);
            }


            currentPortal = Instantiate(portalPrefab, newPortalPosition, Quaternion.identity);
            lastPortalPosition = newPortalPosition;
        }

    }

    void IsFalling()
    {
        beatCount = 0;
        maxSpawnX = levelData.GetInitialHorizontalRange(level);

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
        listUniqueEventsHappend.Clear();

        print("Tocou o chão em " + paramLevel);
        level = paramLevel;

        hasStartedToFall = false;
        if (!isGamePaused && !hasTouchedGround)
        {
            player.ResetToIdle();
            StartScene(level);
            StabilizePlayerAndCamera();
            beatCount = 0;
            beatsToPortal = levelData.GetBeatsToPortal(level);
            countPortals = 0;
            listBeatsWithPortals.Clear();
            lastPortalPosition = Vector2.zero;
            hasTouchedGround = true;
            hasMissedClick = false;
            isPlayerFalling = false;
        }
    }

    public void ExitedGround()
    {
        hasTouchedGround = false;
    }

    public float progress;
    public void TeleportPlayer(Vector2 position)
    {
        player.transform.position = position;

        progress = countPortals / levelData.GetPortalsToEnd(level);

        canvasController.MoveProgressMage(progress);
    }

    #endregion



    #region "Platform"

    public void HasMatchedClick()
    {
        listNextPlatforms[0] = listNextPlatforms[1];
        listNextPlatforms[1] = listPlatformsPrefab[Random.Range(0, listPlatformsPrefab.Length)];

        canvasController.UpdateImagesPlatform();
    }

    public void HasMissedClick()
    {
        hasMissedClick = true;
        audioController.PlaySFXError();
        canvasController.ShowMistakeGuide();
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
                    HasMissedClick();
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
            canvasController.textPause.SetActive(true);
        }
        else
        {
            isGamePaused = false;
            canvasController.ShowScene(InternalScenes.hud);
            Time.timeScale = 1;
            audioController.UnPauseEverything();
            canvasController.textPause.SetActive(false);
        }

    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QUIT");
    }
    #endregion
}
