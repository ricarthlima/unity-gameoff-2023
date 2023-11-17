using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GetComponent<SelfDestroyController>().StopTime();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GetComponent<SelfDestroyController>().ResumeTime();
            }
        }
    }
}
