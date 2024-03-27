using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameController gameController;
    public Rigidbody2D rb;
    public bool isTouchingGround;
    public bool isWalkingToCenter;

    Animator animator;

    float startWalkCenterTime;
    Vector3 startPositionCenter;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        CycleWalkToCenter();
    }

    public void EnterPortalAnimation()
    {
        animator.SetTrigger("enterPortal");
        StartCoroutine(ExitPortal());
    }

    public void WalkToCenter()
    {
        isWalkingToCenter = true;
        startWalkCenterTime = Time.time;
        startPositionCenter = transform.position;
        //TODO: Iniciar animação de andar
    }

    void CycleWalkToCenter()
    {
        if (isWalkingToCenter)
        {
            float timeSinceStarted = Time.time - startWalkCenterTime;
            float percentageComplete = Mathf.Clamp(timeSinceStarted / 9, 0f, 1f);

            print("START: " + startWalkCenterTime + " | TIME: " + Time.time + " | SINCE: " + timeSinceStarted + " | %: " + percentageComplete);
            if (gameController.isWaitingTimeToRestart  && transform.position.x != 0)
            {                
                transform.position = Vector3.Lerp(startPositionCenter, new Vector3(0, transform.position.y, transform.position.z), percentageComplete);
            }
            else
            {
                isWalkingToCenter = false;
                //TODO: Parar animação de andar
            }
        }
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
