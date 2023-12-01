using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float damping;
    [SerializeField] private Vector3 offset;

    private bool isFalling;

    private Vector3 velocity = Vector3.zero;

    void FixedUpdate()
    {
        if (!isFalling)
        {
            Vector3 movePosistion = target.position + offset;
            Vector3 damp = Vector3.SmoothDamp(transform.position, movePosistion, ref velocity, damping);
            transform.position = new Vector3(transform.position.x, damp.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z);
        }
        
    }

    public void SetFalling(bool setFalling)
    {
        isFalling = setFalling;
    }
}
