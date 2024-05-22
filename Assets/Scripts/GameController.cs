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
    [SerializeField] InfoController infoController;

    public float bpm;
    [SerializeField] private int portalsUntilMoveCamera;
    public float maxSpawnX;
    [SerializeField] private float minSpawnY;
    [SerializeField] public float maxSpawnY;
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

    // Spawn da Plataforma
    //float timerBeat = 0;
    float timeRestarted = 0;

    public bool hasMissedClick = false;

    Vector2 lastBackgroundPosition = new Vector2(0, 14.77f);

    // New game controllers
    public bool isGamePaused = true;
    //bool isGameInCutscene = true; // Começos de fase são cut scene, cut scenes são cut scenes, queda é cut scene

    // Cut Scenes
    bool needToShowFirstCutsceneDungeon = true;
    bool needToShowStairwayCutscene = false;

    bool isPlayerFalling = false;
    bool hasStartedToFall = false;

    [Header("DevTools")]
    [SerializeField] private TextMeshProUGUI textDevAutoGeneratePlatform;
    [SerializeField] private TextMeshProUGUI textDevGameTimeScale;
    public bool devIsAutoGeneratingPlatforms = false;
    float devTimeScale = 1;

    List<float> listUniqueEventsHappend = new List<float>();
    Vector2 lastPortalPosition = Vector2.zero;

    public float beatsToPortal;
    public int countPortals = 0;
    public int beatsToBeat;

    public bool canGeneratePortals = false;

    float averageHumanReactionTime = 0;

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
                innerBeatCount++;
                beatCount++;
                RestartTimerBeat();
            }

            // if (!isGameInCutscene)
            // {
            if (!isPlayerFalling)
            {
                CycleUpdateBackground();
                CycleGeneratePortalsByBeat();
            }
            else
            {
                IsFalling();
            }
            // }
            // else
            // {
            //     if (needToShowFirstCutsceneDungeon)
            //     {
            //         StartCoroutine(WaitCutsceneFirstDungeon());
            //         needToShowFirstCutsceneDungeon = false;
            //     }

            //     if (needToShowStairwayCutscene)
            //     {
            //         StartCoroutine(WaitCutsceneStairway());
            //         needToShowStairwayCutscene = false;
            //     }
            // }

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
    public int innerBeatCount = 0;

    void CycleGeneratePortalsByBeat()
    {
        if (innerBeatCount % beatsToPortal == 0 && canGeneratePortals)
        {
            GeneratePortal();
        }

        if (GetTimerBeat() > (60f / bpm))
        {
            innerBeatCount++;
            beatCount++;
            RestartTimerBeat();
        }
    }

    #region "Old Religion"
    //IEnumerator WaitCutsceneFirstDungeon()
    // {
    //     maxSpawnX = levelData.GetInitialHorizontalRange(level);
    //     canGeneratePortals = false;

    //     CleanPortalsAndPlatforms();

    //     player.WalkToCenter(timeAwaintingDungeonFloor - averageHumanReactionTime);
    //     audioController.PlayDungeon();

    //     cameraFollow.SetFastFollow();

    //     yield return new WaitForSeconds(timeAwaintingDungeonFloor - averageHumanReactionTime);

    //     canGeneratePortals = true;

    //     hasMissedClick = false;

    //     cameraFollow.SetSlowFollow();

    //     isGameInCutscene = false;
    // }

    // IEnumerator WaitCutsceneStairway()
    // {
    //     maxSpawnX = 0;
    //     beatCount = 0;

    //     CleanPortalsAndPlatforms();

    //     player.WalkToCenter(8.5f);
    //     audioController.PlayStairway();

    //     canGeneratePortals = false;

    //     yield return new WaitForSeconds(8.5f);


    //     hasMissedClick = false;

    //     canGeneratePortals = true;

    //     isGameInCutscene = false;
    // }
    #endregion

    void CycleTestFalling()
    {
        if (player.rb.velocity.y < -7)
        {
            isPlayerFalling = true;
        }
    }

    void CheckUniqueEvent(TowerLevel towerLevel, int beat, float id, System.Action action)
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
            beat: 14,
            id: 0,
            action: () =>
                {
                    maxSpawnX = 3f;

                    canGeneratePortals = true;
                    innerBeatCount = 0;

                    beatsToPortal = 2;
                    beatsToBeat = 4;
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 22,
            id: 0.1f,
            action: () =>
                {
                    cameraFollow.StartRising(newSpeed: 1.78f);
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 58,
            id: 1,
            action: () =>
                {
                    maxSpawnX = 5f;
                    canvasController.ShowRedWarning();
                    canGeneratePortals = false;

                    beatsToPortal = 1;
                    beatsToBeat = 4;
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 66,
            id: 2,
            action: () =>
                {
                    innerBeatCount = 0;
                    listBeatsWithPortals.Clear();
                    canGeneratePortals = true;
                    cameraFollow.StartFollowing();
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 70,
            id: 2,
            action: () =>
                {
                    cameraFollow.StartRising(3.64f);
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 94,
            id: 3,
            action: () =>
                {
                    maxSpawnX = 3.5f;
                    canvasController.ShowRedWarning();
                    canGeneratePortals = false;

                    beatsToPortal = 1;
                    beatsToBeat = 4;
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 98,
            id: 4,
            action: () =>
                {
                    innerBeatCount = 0;
                    listBeatsWithPortals.Clear();
                    canGeneratePortals = true;
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 128,
            id: 5,
            action: () =>
                {
                    maxSpawnX = 4f;
                    canvasController.ShowRedWarning();
                    canGeneratePortals = false;

                    beatsToPortal = 1;
                    beatsToBeat = 2;
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 132,
            id: 6,
            action: () =>
                {
                    innerBeatCount = 0;
                    listBeatsWithPortals.Clear();
                    canGeneratePortals = true;
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 180,
            id: 7,
            action: () =>
                {
                    maxSpawnX = 2f;
                    canvasController.ShowRedWarning();
                    canGeneratePortals = false;

                    beatsToPortal = 1;
                    beatsToBeat = 1;
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 190,
            id: 8,
            action: () =>
                {
                    innerBeatCount = 0;
                    listBeatsWithPortals.Clear();
                    canGeneratePortals = true;
                }
        );

        CheckUniqueEvent(
            towerLevel: TowerLevel.dungeon,
            beat: 228,
            id: 9,
            action: () =>
                {
                    maxSpawnX = levelData.GetInitialHorizontalRange(level);
                    canvasController.ShowWhiteWarning();
                    canGeneratePortals = false;
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
        bpm = levelData.BPM(scene);
        audioController.PlayByLevel(scene);
    }

    List<int> listBeatsWithPortals = new List<int>();
    void GeneratePortal()
    {
        if (!listBeatsWithPortals.Contains(innerBeatCount))
        {
            listBeatsWithPortals.Add(innerBeatCount);
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
        innerBeatCount = 0;
        beatCount = 0;
        maxSpawnX = levelData.GetInitialHorizontalRange(level);
        canGeneratePortals = false;

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
        level = paramLevel;

        hasStartedToFall = false;
        if (!isGamePaused && !hasTouchedGround)
        {
            player.ResetToIdle();
            player.WalkToCenter(5);
            StartScene(level);
            StabilizePlayerAndCamera();
            innerBeatCount = 0;
            beatCount = 0;
            beatsToPortal = levelData.GetBeatsToPortal(level);
            countPortals = 0;
            listBeatsWithPortals.Clear();
            lastPortalPosition = Vector2.zero;
            hasTouchedGround = true;
            hasMissedClick = false;
            isPlayerFalling = false;
            canvasController.MoveProgressMage(0, level: level);
            infoController.isFirstTimePlayingRecordSFX = true;
            cameraFollow.StartFollowing();
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

        progress = beatCount / levelData.GetPortalsToEnd(level);

        canvasController.MoveProgressMage(progress, level: level);
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
    bool isFirstTimeStarted = true;
    public void StartGame()
    {
        if (isFirstTimeStarted)
        {
            audioController.PlayByLevel(level);
            player.WalkToCenter(5);
            isFirstTimeStarted = false;
        }
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
