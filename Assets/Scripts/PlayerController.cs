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

    public GameObject leftCover;
    public GameObject rightCover;

    Animator animator;

    float startWalkCenterTime;
    Vector3 startPositionCenter;

    float timeToCenter;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        CycleWalkToCenter();
        leftCover.transform.position = new Vector2(leftCover.transform.position.x, transform.position.y);
        rightCover.transform.position = new Vector2(rightCover.transform.position.x, transform.position.y);
    }

    public void EnterPortalAnimation()
    {
        animator.SetTrigger("enterPortal");
        StartCoroutine(ExitPortal());
    }

    public void WalkToCenter(float timer)
    {
        isWalkingToCenter = true;
        startWalkCenterTime = Time.time;
        startPositionCenter = transform.position;
        timeToCenter = timer;
        //TODO: Iniciar anima��o de andar
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
                leftCover.transform.position = Vector2.Lerp(new Vector2(-12f, transform.position.y), new Vector2(-6f, transform.position.y), percentageComplete);
                rightCover.transform.position = Vector2.Lerp(new Vector2(12f, transform.position.y), new Vector2(6f, transform.position.y), percentageComplete);
            }
            else
            {
                isWalkingToCenter = false;
                //TODO: Parar anima��o de andar
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other != null)
        {
            if (other.gameObject.CompareTag("StartPlatform")){
                gameController.TouchedGround(TowerLevel.dungeon);   
            }

            if (other.gameObject.CompareTag("StairwayPlatform")){
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

    IEnumerator ExitPortal()
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetTrigger("exitPortal");
    }
}
