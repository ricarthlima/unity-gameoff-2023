using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [SerializeField] GameController gameController;
    private float speed = 1;

    public Transform target;
    [SerializeField] private float damping;
    [SerializeField] private Vector3 offset;

    private Vector3 velocity = Vector3.zero;

    private float innerDamping;
    private Vector3 innerOffset = Vector3.zero;

    bool isFalling;
    bool isRising;
    bool isFollowing;

    float followSpeed = 8;

    private void Start()
    {
        innerOffset = offset;
        innerDamping = damping;
    }

    void FixedUpdate()
    {
        if (!isFalling)
        {
            if (isRising)
            {
                Vector3 movePosistion = new Vector3(transform.position.x, 1000000) + innerOffset;
                Vector3 damp = Vector3.SmoothDamp(transform.position, movePosistion, ref velocity, innerDamping, speed);
                transform.position = new Vector3(transform.position.x, damp.y, transform.position.z);
            }
            else if (isFollowing)
            {
                Vector3 movePosistion = target.position + innerOffset;
                Vector3 damp = Vector3.SmoothDamp(transform.position, movePosistion, ref velocity, innerDamping, followSpeed);
                transform.position = new Vector3(transform.position.x, damp.y, transform.position.z);
            }

        }
        else
        {
            Vector3 newPosition = target.position;
            newPosition.x = transform.position.x;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
    }

    public void SetFalling(bool setFalling)
    {
        isFalling = setFalling;
    }

    public void StartRising(float newSpeed)
    {
        isRising = true;
        isFollowing = false;
        speed = newSpeed;
    }

    public void StartFollowing()
    {
        isFollowing = true;
        isRising = false;
    }

    public void StopCamera()
    {
        isFalling = false;
        isRising = false;
    }
}
