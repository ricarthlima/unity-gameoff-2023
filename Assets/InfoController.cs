using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoController : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [SerializeField] PlayerPrefsController prefs;

    public float maxHeightTraveled = 0;
    public float heightTraveled = 0;
    public float countTimePlaying = 0;

    Vector2 playerStartPosition;

    private void Awake()
    {
        playerStartPosition = gameController.player.transform.position;
    }

    void Start()
    {
        maxHeightTraveled = prefs.RecHigh;
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
        }
    }
}
