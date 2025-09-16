using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogKingMovement : MonoBehaviour
{
    public float movespeed;
    float realSpeed;
    float horizontalAxis;
    float verticalAxis;

    void Update()
    {
        GetDirection();
        Move();
    }

    void GetDirection()
    {
        horizontalAxis = Input.GetAxis("Horizontal");
        verticalAxis = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontalAxis) == 1 && verticalAxis != 0) Input.GetAxisRaw("Horizontal");
        else if (Mathf.Abs(verticalAxis) == 1 && horizontalAxis != 0) Input.GetAxisRaw("Vertical");

        if (horizontalAxis != 0 && verticalAxis != 0) realSpeed = movespeed / Mathf.Sqrt(2);
        else realSpeed = movespeed;
    }

    void Move()
    {
        Vector3 moveVector = new Vector3(horizontalAxis, verticalAxis, 0) * realSpeed * Time.deltaTime;
        transform.position = transform.position + moveVector;
    }
}
