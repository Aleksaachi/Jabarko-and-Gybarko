using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogWorker : Frog
{
    [Header("Inventory settings")]
    public Inventory inv;
    public Building workingIn;
    protected override void Awake()
    {
        base.Awake();
        inv = GetComponent<Inventory>();
    }
    public override void Select()
    {
        base.Select();

        if (workingIn != null)
        {
            workingIn.RemoveWorker(this);
        }
    }
    public override FrogWorker GetWorker()
    {
        return this;
    }
                
}
