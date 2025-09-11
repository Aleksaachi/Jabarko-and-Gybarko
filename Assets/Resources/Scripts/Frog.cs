using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Selectable
{
    public float walkSpeed;
    bool walking = false;
    float walkTime;
    float walkTimeToEnd;
    Vector3 startPoint;
    Vector3 endPoint;
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
        if (walking) walking = false;
        FrogOrderManager.AddFollower(this);
    }

    public virtual FrogWorker GetWorker()
    {
        return null;
    }
}
