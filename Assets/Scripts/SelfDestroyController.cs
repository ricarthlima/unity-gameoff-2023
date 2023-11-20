using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyController : MonoBehaviour
{
    [SerializeField] float timeToDestroy;

    private bool isStoped = false;
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
