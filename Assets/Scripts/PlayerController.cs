using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameController gameController;
    public Rigidbody2D rb;
    public bool isTouchingGround;

    private void Start()
    {
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision!= null & collision.CompareTag("StartPlatform"))
        {
            gameController.TouchedTheGround();
            isTouchingGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("StartPlatform"))
            {
                isTouchingGround = false;
            }
        }
    }
}
