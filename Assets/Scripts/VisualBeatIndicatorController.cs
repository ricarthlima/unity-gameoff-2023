using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualBeatIndicatorController : MonoBehaviour
{
    public float bpm;

    [SerializeField] private GameObject beat;
    [SerializeField] private GameObject portalSprite;

    private float timeStart;
    private float timePassed;
    public bool isRunning = true;

    private void Start()
    {
        timeStart = Time.time;
    }

    void Update()
    {
        float value = 2;

        if (isRunning && value >= 1) {
            timePassed = Time.time - timeStart;
            value = 2 - timePassed / (60f / bpm);
            beat.transform.localScale = new Vector3(value, value, value); // Vector3.Lerp(beat.transform.localScale, Vector3.one, timePassed / (60f / bpm));            
        }
    }

    public void ShowPortal()
    {
        portalSprite.SetActive(true);
    }


    public void MakeYellow()
    {
        beat.GetComponent<SpriteRenderer>().color = Color.yellow;        
    }
    
    public void MakeRed()
    {
        beat.GetComponent <SpriteRenderer>().color = Color.red; 
    }
}
