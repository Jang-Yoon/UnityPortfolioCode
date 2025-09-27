using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Vector3 moveDirection;

    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    public Vector3 MoveDirection { set { moveDirection = value; } }

    void Update()
    {
        transform.position += moveDirection * Time.deltaTime * moveSpeed;
    }
}
