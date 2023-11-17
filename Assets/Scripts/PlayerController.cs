using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameController gameController;
    public Rigidbody2D rb;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("StartPlatform"))
            {
                gameController.Restart();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("StartPlatform"))
            {
                gameController.PlayerLeftPlatform();
            }
        }
    }
}
