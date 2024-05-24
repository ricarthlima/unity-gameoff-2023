using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailFXController : MonoBehaviour
{
    TrailRenderer trailRenderer;
    GameController gameController;
    Vector2 target;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, 10 * Time.deltaTime * gameController.BPSR());
    }

    public void Emitting(Vector2 position)
    {
        trailRenderer.emitting = true;
        trailRenderer.time = gameController.BPS();
        target = position;
    }

    public void EmittingStop()
    {
        trailRenderer.emitting = false;
    }
}
