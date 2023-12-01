using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualBeatIndicatorController : MonoBehaviour
{
    public float bpm;
    public bool isOnRightTime;
    public bool isPassedTime;

    [SerializeField] private GameObject beat;

    private float timePassed;
    public bool isRunning = true;
    
    void Update()
    {
        if (isRunning) {
            timePassed += Time.deltaTime;
            beat.transform.localScale = Vector3.Lerp(beat.transform.localScale, Vector3.one, timePassed / ((60f / bpm) * 30));            
        }
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
