using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    float timeToCenter;

    public GameObject sprite;

    [Header("Portal")]
    [SerializeField] private GameObject portalSFX;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        CycleWalkToCenter();

        if (rb.velocity.y < -50)
        {
            rb.velocity = new Vector2(rb.velocity.x, -50);
        }
    }

    public void AnimationEnterPortal()
    {
        animator.SetBool("enterPortal", true);
        portalSFX.SetActive(true);
    }

    public void AnimationEnterPortalExit()
    {
        animator.SetBool("enterPortal", false);
    }

    public void AnimationExitPortal()
    {
        animator.SetBool("exitPortal", true);
        portalSFX.SetActive(false);
    }

    public void AnimationExitPortalExit()
    {
        animator.SetBool("exitPortal", false);
    }

    public void WalkToCenter(float timer)
    {
        isWalkingToCenter = true;
        startWalkCenterTime = Time.time;
        startPositionCenter = transform.position;
        timeToCenter = timer;
        //TODO: Iniciar anima��o de andar
    }

    public void ResetToIdle()
    {
        sprite.transform.localScale = Vector3.one;
        sprite.GetComponent<SpriteRenderer>().color = Color.white;
        animator.SetBool("enterPortal", false);
        animator.SetBool("exitPortal", false);
        animator.Play("Idle");
    }
    void CycleWalkToCenter()
    {
        if (isWalkingToCenter)
        {
            float timeSinceStarted = Time.time - startWalkCenterTime;
            float percentageComplete = Mathf.Clamp(timeSinceStarted / timeToCenter, 0f, 1f);

            //print("START: " + startWalkCenterTime + " | TIME: " + Time.time + " | SINCE: " + timeSinceStarted + " | %: " + percentageComplete);
            if (transform.position.x != 0)
            {
                transform.position = Vector3.Lerp(startPositionCenter, new Vector3(0, transform.position.y, transform.position.z), percentageComplete);
            }
            else
            {
                isWalkingToCenter = false;
                //TODO: Parar anima��o de andar
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other != null)
        {
            if (other.gameObject.CompareTag("StartPlatform"))
            {
                gameController.TouchedGround(TowerLevel.dungeon);
            }

            if (other.gameObject.CompareTag("StairwayPlatform"))
            {
                gameController.TouchedGround(TowerLevel.stairway);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("StartPlatform") || collision.gameObject.CompareTag("StairwayPlatform"))
            {
                gameController.ExitedGround();
            }
        }
    }
}
