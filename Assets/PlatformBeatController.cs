using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBeatController : MonoBehaviour
{
    [Header("Timers")]
    public float timeEarly;
    public float timeOK;
    public float timeNice;
    public float timePerfect;
    public float timeLate;

    [SerializeField] private float timeCount;

    void Update()
    {
        timeCount += Time.deltaTime;
        if (timeCount > timeLate)
        {
            Destroy(gameObject);
        }
    }

    public void OnTouched()
    {
        if (timeCount < timeEarly)
        {
            // Falha
        }
        else if (timeCount < timeOK)
        {
            // Spawn OK
        }else if (timeCount < timeNice)
        {
            // Spawn Nice
        }else if (timeCount < timePerfect)
        {
           // Spawn Perfect
        }else if (timeCount < timeLate)
        {
           // Falha
        }
    }
}
