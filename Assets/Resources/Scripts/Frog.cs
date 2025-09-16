using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Selectable
{
    public float walkSpeed;
    public bool walking = false;
    float walkTime;
    float walkTimeToEnd;
    Vector3 startPoint;
    Vector3 endPoint;
    public Vector3 idlePoint;
    public float wanderRange;
    public float wanderTime;
    float wanderTimer;
    public bool idle = true;
    SquashAndStretch squashAndStretch;
    protected virtual void Awake()
    {
        idlePoint = transform.position;
        wanderTimer = Random.Range(-0.5f * wanderTime, 0.5f * wanderTime);
        squashAndStretch = GetComponent<SquashAndStretch>();
    }
    void Update()
    {
        if (walking)
        {
            walkTime += Time.deltaTime;

            Vector3 newPosition = Vector3.Lerp(startPoint, endPoint, walkTime / walkTimeToEnd);
            transform.position = newPosition;

            if (walkTime >= walkTimeToEnd)
            {
                walking = false;
            }
        }

        if (idle)
        {
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= wanderTime)
            {
                if (!walking)
                {
                    Vector3 newPosition = new Vector3(idlePoint.x + Random.Range(-wanderRange, wanderRange), idlePoint.y + Random.Range(-wanderRange, wanderRange), 0);
                    WalkToPoint(newPosition);
                    wanderTimer = Random.Range(-0.5f * wanderTime, 0.5f * wanderTime);
                }
            }
        }
    }
    public void WalkToPoint(Vector3 point)
    {
        startPoint = transform.position;
        endPoint = point;

        float distance = Vector3.Distance(startPoint, endPoint);
        walkTimeToEnd = distance / walkSpeed;
        walkTime = 0;

        walking = true;
    }
    public void WalkToKing()
    {
        WalkToPoint(FrogOrderManager.instance.transform.position);
    }

    public override void Select()
    {
        squashAndStretch.Play();
        if (walking) walking = false;
        FrogOrderManager.AddFollower(this);
    }

    public virtual FrogWorker GetWorker()
    {
        return null;
    }
}
