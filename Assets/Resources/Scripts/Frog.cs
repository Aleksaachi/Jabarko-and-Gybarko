using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Selectable
{
    public void WalkToPoint(Vector3 point)
    {
        transform.position = point;
    }
    public void WalkToKing()
    {
        WalkToPoint(FrogOrderManager.instance.transform.position);
    }

    public override void Select()
    {
        FrogOrderManager.AddFollower(this);
    }
}
