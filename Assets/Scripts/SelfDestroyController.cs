using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyController : MonoBehaviour
{
    public float timeToDestroy;
    public bool isStoped = true;

    private float timePassed = 0;

    void Update()
    {
        if (!isStoped)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= timeToDestroy)
            {
                Destroy(gameObject);
            }
        }
    }

    public void StopTime()
    {
        isStoped = true;
    }

    public void ResumeTime()
    {
        isStoped = false;
    }
}
