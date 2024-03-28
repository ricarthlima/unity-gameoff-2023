using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    #region "Atributes"
    [Header("Controllers")]
    public float bpm;
    private float maxSpawnX = 0;    
    public int level = 0;

    [Header("Scene Objects")]
    public SmoothCameraFollow cameraFollow;
    public PlayerController player;
    [SerializeField] private GameObject guidePlataform;

    [Header("Background Objects")]
    [SerializeField] private GameObject bgDungeonLoop; 
    readonly float verticalLoopDistanceDungeon = 25;
    [SerializeField] private GameObject bgStairwayStart;
    [SerializeField] private GameObject bgStairwayLoop;
    readonly float verticalLoopDistanceStairway = 12.92f;
    float platformStairwayPosition;

    GameObject currentLoopBackground;
    float currentVerticalDistanceLoopBackground;

    

    [Header("UI")]
    [SerializeField] Canvas renderCanvas;
    [SerializeField] GameObject panelMainMenu;
    [SerializeField] GameObject panelGameHUD;
    [SerializeField] TextMeshProUGUI textHeightTraveled;
    [SerializeField] TextMeshProUGUI textBPM;
    [SerializeField] private Image imageNextPlatform;
    [SerializeField] private Image imageSecondPlatform;
    [SerializeField] private GameObject textPause;

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
    float countTimePlaying = 0;

    // Esperar para recome�ar
    public bool isWaitingTimeToRestart = false;
    float countTimeToRestart = 0;
    float waitSecondsToRestart = 11;

    // Spawn da Plafatorma
    bool canSpawnPlatform = false;

    float timerBeat = 0;

    public bool hasMissedClick = false;

    Vector2 lastBackgroundPosition = new Vector2(0, 20f);

    bool isStartToPlayMusic = false;

    [SerializeField] private GameObject leftCover;
    [SerializeField] private GameObject rightCover;

    bool isCutScene = false;

    
    #endregion

    #region "Life Cycles"
    void Start()
    {
        //Application.targetFrameRate = 60;
        portalPosition = player.transform.position;

        // Open MainMenu 
        panelMainMenu.SetActive(true);
        panelGameHUD.SetActive(false);

        // Platforms
        listNextPlatforms.Add(listPlatformsPrefab[0]);
        listNextPlatforms.Add(listPlatformsPrefab[0]);

        UpdateImagesPlatform();
        CallScene();

        waitSecondsToRestart -= 60f / bpm;

        currentLoopBackground = bgDungeonLoop;
        currentVerticalDistanceLoopBackground = verticalLoopDistanceDungeon;
    }

    private void Update()
    {
        VerifyMouseClick();
        CycleControlCover();
        CycleGuideFollowsMouse();
        CycleEscButton();
        timerBeat += Time.deltaTime;
        UpdateUI();

        //TODO: DEV TOOLS
        if (Input.GetKeyDown(KeyCode.Alpha2)){
            audioBGM.time = audioBGM.clip.length - 11;
            maxSpawnX = 5;
        }
    }

    private void FixedUpdate()
    {
        CycleVerifyNextScene();
        CycleUniqueSceneEvents();
        if (isStartGame && !isCutScene)
        {
            bool isWaiting = CycleToRestartGame();
            countTimePlaying += Time.deltaTime;
            if (isGameRunning && !isWaiting)
            {                
                bool isFalling = CycleTestFalling();
                if (!isFalling)
                {
                    //timerBeat += Time.deltaTime;      
                    
                    if (timerBeat >= (60f / bpm))
                    {
                        player.gameObject.transform.position = portalPosition;
                        if (player.transform.position.y > lastBackgroundPosition.y - currentVerticalDistanceLoopBackground)
                        {
                            lastBackgroundPosition = new Vector2(lastBackgroundPosition.x, lastBackgroundPosition.y + currentVerticalDistanceLoopBackground);
                            GameObject newBackground = Instantiate(currentLoopBackground, lastBackgroundPosition, Quaternion.identity);
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
            if (!isStartToPlayMusic)
            {
                audioBGM.Play();
                player.WalkToCenter();
                isStartToPlayMusic = true;
            }
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
    
    bool isTransitionTime = false;
    void CycleVerifyNextScene()
    {
        if (audioBGM.time >= listBGMS[level].length - 10)
        {
            if (level < listBGMS.Count - 1 && !isTransitionTime)
            {
                level++;
                isTransitionTime = true;
                CallScene();
            }
        }        
    }

    void CallScene()
    {        
        if (level == 0)
        {
            currentLoopBackground = bgDungeonLoop;
            currentVerticalDistanceLoopBackground = verticalLoopDistanceDungeon;
        }

        if (level == 1)
        {         
            currentVerticalDistanceLoopBackground = verticalLoopDistanceStairway;

            GameObject startStairway = Instantiate(bgStairwayStart, lastBackgroundPosition, Quaternion.identity);
            platformStairwayPosition = startStairway.transform.position.y - 5;
            startStairway.transform.parent = GameObject.Find("Background").transform;

            lastBackgroundPosition = new Vector2(lastBackgroundPosition.x, lastBackgroundPosition.y + currentVerticalDistanceLoopBackground);
            GameObject loopStairway = Instantiate(bgStairwayLoop, lastBackgroundPosition, Quaternion.identity);
            loopStairway.transform.parent = GameObject.Find("Background").transform;

            currentLoopBackground = loopStairway;
        }

        if (level == 2)
        {

        }
    }
    
    bool hasCallStairwayCutScene = false;
    void CycleUniqueSceneEvents()
    {
        switch (level)
        {
            case 0:
                {
                    break;
                }
            case 1:
                {
                    if (player.transform.position.y > platformStairwayPosition){
                        if (!hasCallStairwayCutScene){
                            isCutScene = true;
                        
                            GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
                            foreach (GameObject platform in platforms){
                                Destroy(platform);
                            }

                            GameObject[] portals = GameObject.FindGameObjectsWithTag("Portal");
                            foreach (GameObject portal in portals){
                                Destroy(portal);
                            }
                            TouchedStairwayPlatform();
                            hasCallStairwayCutScene = true;
                        }
                        
                    }
                    break;
                }
            case 2:
                {
                    break;
                }
        }
    }

    void CycleControlCover()
    {
        if (!isWaitingTimeToRestart)
        {
            leftCover.transform.position = Vector3.MoveTowards(leftCover.transform.position, new Vector3(-6f - maxSpawnX, leftCover.transform.position.x), Time.deltaTime*5);
            rightCover.transform.position = Vector3.MoveTowards(rightCover.transform.position, new Vector3(6f + maxSpawnX, rightCover.transform.position.x), Time.deltaTime*5);
        }
    }
    void CycleGuideFollowsMouse()
    {
        if (isGameRunning)
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

    #endregion

    #region "Game Logic"
    void GeneratePortal()
    {
        maxSpawnX += 0.05f;
        maxSpawnX = Mathf.Min(maxSpawnX, 5);
        float y = player.gameObject.transform.position.y + 3f;
        float x = Random.Range(-1 * maxSpawnX, maxSpawnX);

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
        audioBGM.time = 0;
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
        maxSpawnX = 0;
        portalPosition = player.transform.position;
        isGameRunning = true;
        isFirstPlatform = true;
        isStartToPlayMusic = false;
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

    public void TouchedStairwayPlatform(){
        audioRewindSFX.Stop();
        if (player.gameObject.transform.rotation.z != 0)
        {
            Vector3 position = player.transform.position;
            player.transform.localEulerAngles = Vector3.zero;
        }
        cameraFollow.SetFalling(false);
        CutSceneStairway();
    }

    void CutSceneStairway(){        
        audioBGM.Stop();
        isTransitionTime = false;
        isCutScene = false;
        audioBGM.time = 0;
        audioBGM.clip = listBGMS[1];
        bpm = listBPMS[1];
        audioBGM.Play();
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

        float fps = 1f / Time.deltaTime;
        textBPM.text = "BPM: " + bpm.ToString();
        textHeightTraveled.text = "Altura M�xima: " + maxHeightTraveled.ToString("F2") + "m\nAltura: " + heightTraveled.ToString("F2") + "m\nTempo jogando: " +  countTimePlaying.ToString("F0")  +"\nFPS: " + fps.ToString("F2"); 
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
        if (isFirstPlatform)
        {
            isFirstPlatform = false;
        }

        listNextPlatforms[0] = listNextPlatforms[1];
        listNextPlatforms[1] = listPlatformsPrefab[Random.Range(0, listPlatformsPrefab.Length)];

        UpdateImagesPlatform();

        canSpawnPlatform = false;
    }

    public void HasMissedClick()
    {
        hasMissedClick = true;
        player.gameObject.transform.position = portalPosition;
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

    #region "Game"
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
        if (isGameRunning)
        {
            isGameRunning = false;
            panelMainMenu.SetActive(true);
            panelGameHUD.SetActive(false);
            Time.timeScale = 0;            
            audioBGM.Pause();
            textPause.SetActive(true);
        }
        else
        {
            isGameRunning = true;
            panelMainMenu.SetActive(false);
            panelGameHUD.SetActive(true);
            Time.timeScale = 1;
            audioBGM.UnPause();
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
