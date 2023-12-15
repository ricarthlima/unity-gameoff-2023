using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameController gameController;
    public Rigidbody2D rb;
    public bool isTouchingGround;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void EnterPortalAnimation()
    {
        animator.SetTrigger("enterPortal");
        StartCoroutine(ExitPortal());
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

    IEnumerator ExitPortal()
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetTrigger("exitPortal");
    }
}
