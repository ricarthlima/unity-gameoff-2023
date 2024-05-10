using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoversController : MonoBehaviour
{
    [Header("Dependecies")]
    [SerializeField] GameController gameController;
    [SerializeField] GameObject player;

    [Header("Covers")]
    public GameObject leftCover;
    public GameObject rightCover;

    [Header("Inputs")]
    public float speed;

    readonly float initialValue = 5.25f;

    void Update()
    {
        if (!gameController.isGamePaused)
        {
            float maxSpawnX = gameController.maxSpawnX;
            float x = maxSpawnX + initialValue;

            Vector3 leftPostion = leftCover.transform.position;
            leftPostion.y = player.transform.position.y;
            leftCover.transform.position = leftPostion;

            Vector3 rightPostion = rightCover.transform.position;
            rightPostion.y = player.transform.position.y;
            rightCover.transform.position = rightPostion;

            leftCover.transform.position = Vector3.MoveTowards(
                leftPostion,
                new Vector3(x * -1, player.transform.position.y),
                Time.deltaTime * speed
            );

            rightCover.transform.position = Vector3.MoveTowards(
                rightPostion,
                new Vector3(x, player.transform.position.y),
                Time.deltaTime * speed
            );
        }
    }
}
