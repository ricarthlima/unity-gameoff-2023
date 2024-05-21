using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualBeatIndicatorController : MonoBehaviour
{
    GameController gameController;

    [SerializeField] private GameObject fixedBeat;
    [SerializeField] private GameObject beat;
    [SerializeField] private GameObject portalSprite;

    private float timeStart;
    private float timePassed;
    private readonly float initialBeatScale = 3f;
    private readonly float finalBeatScale = 1f;

    private float innerBPM;
    private void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        innerBPM = gameController.bpm / gameController.beatsToPortal;
    }
    private void Start()
    {
        timeStart = Time.time;
    }

    void Update()
    {
        timePassed = Time.time - timeStart;
        float newScale = Mathf.Lerp(initialBeatScale, finalBeatScale, timePassed / (60f / innerBPM));// (initialBeatScale + 1) - (timePassed / (60f / innerBPM) * initialBeatScale);
        beat.transform.localScale = new Vector3(newScale, newScale, newScale);

        float newOpacity = (50f + (timePassed / (60f / innerBPM) * 200f)) / 250f;
        Color oldColor = beat.GetComponent<SpriteRenderer>().color;
        beat.GetComponent<SpriteRenderer>().color = new Color(oldColor.r, oldColor.g, oldColor.b, newOpacity);
    }

    public void ShowPortal()
    {
        portalSprite.SetActive(true);
    }


    public void MakeYellow()
    {
        fixedBeat.GetComponent<SpriteRenderer>().color = Color.yellow;
        beat.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    public void MakeRed()
    {
        fixedBeat.GetComponent<SpriteRenderer>().color = Color.red;
        beat.GetComponent<SpriteRenderer>().color = Color.red;
    }
}
