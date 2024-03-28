using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float damping;
    [SerializeField] private Vector3 offset;

    private Vector3 velocity = Vector3.zero;

    private float innerDamping;
    private Vector3 innerOffset = Vector3.zero;

    bool isFalling;

    private void Start()
    {
        innerOffset = offset;
        innerDamping = damping;
    }

    void FixedUpdate()
    {
        if (!isFalling) { 
        Vector3 movePosistion = target.position + innerOffset;
        Vector3 damp = Vector3.SmoothDamp(transform.position, movePosistion, ref velocity, innerDamping);
        transform.position = new Vector3(transform.position.x, damp.y, transform.position.z);
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
        //if (setFalling)
        //{
        //    innerOffset = Vector3.zero;
        //    innerDamping = 0.2f;
        //}
        //else
        //{
        //    innerOffset = offset;
        //    innerDamping = damping;
        //}
    }
}
