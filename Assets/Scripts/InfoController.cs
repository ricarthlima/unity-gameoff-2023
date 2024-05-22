using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoController : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [SerializeField] CanvasController canvasController;
    [SerializeField] PlayerPrefsController prefs;

    public float maxHeightTraveled = 0;
    public float heightTraveled = 0;
    public float countTimePlaying = 0;

    public int maxPortalsDungeon = 0;

    Vector2 playerStartPosition;

    public bool isFirstTimePlayingRecordSFX = true;

    private void Awake()
    {
        playerStartPosition = gameController.player.transform.position;
    }

    void Start()
    {
        maxHeightTraveled = prefs.RecHigh;
        maxPortalsDungeon = prefs.RecPortalsDungeon;

        switch (gameController.level)
        {
            case TowerLevel.dungeon:
                {
                    canvasController.MoveProgressRecord(maxPortalsDungeon / gameController.levelData.GetPortalsToEnd(TowerLevel.dungeon), TowerLevel.dungeon);
                    break;
                }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameController.isGamePaused)
        {
            countTimePlaying += Time.deltaTime;
            heightTraveled = (gameController.player.transform.position.y - playerStartPosition.y) * 1.8f;

            if (heightTraveled > maxHeightTraveled)
            {
                maxHeightTraveled = heightTraveled;
                prefs.RecHigh = maxHeightTraveled;
            }

            switch (gameController.level)
            {
                case TowerLevel.dungeon:
                    {
                        if (gameController.countPortals > maxPortalsDungeon)
                        {
                            maxPortalsDungeon = gameController.beatCount;
                            prefs.RecPortalsDungeon = maxPortalsDungeon;
                            canvasController.MoveProgressRecord(maxPortalsDungeon / gameController.levelData.GetPortalsToEnd(TowerLevel.dungeon), TowerLevel.dungeon);

                            if (isFirstTimePlayingRecordSFX)
                            {
                                gameController.audioController.PlaySFXPerfect();
                                isFirstTimePlayingRecordSFX = false;
                            }
                        }
                        break;
                    }
            }
        }
    }
}
