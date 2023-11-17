using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyController : MonoBehaviour
{
    GameController gameController;

    [SerializeField] float timeToDestroy;

    float timePassed = 0;
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.isCycleGameRunning)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= timeToDestroy)
            {
                Destroy(gameObject);
            }
        }        
    }
}
