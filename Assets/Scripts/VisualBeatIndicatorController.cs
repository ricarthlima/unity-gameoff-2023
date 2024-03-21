using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualBeatIndicatorController : MonoBehaviour
{
    public float bpm;

    [SerializeField] private GameObject beat;
    [SerializeField] private GameObject portalSprite;

    private float timePassed;
    public bool isRunning = true;
    
    void Update()
    {
        if (isRunning) {
            timePassed += Time.deltaTime;
            beat.transform.localScale = Vector3.Lerp(beat.transform.localScale, Vector3.one, timePassed / ((60f / bpm) * 30));            
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
