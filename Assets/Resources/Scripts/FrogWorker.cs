using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogWorker : Frog
{
    public Building workingIn;

    public override void Select()
    {
        base.Select();

        if (workingIn != null)
        {
            workingIn.workers.Remove(this);
            workingIn = null;
        }
    }
    public override FrogWorker GetWorker()
    {
        return this;
    }
}
