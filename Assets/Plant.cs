using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public bool harvestable;
    public int yield;
    public float growTime;
    protected float timer;
    protected bool freezeGrowth;
    public bool targetedForHarvest;
    void Update()
    {
        if (freezeGrowth) return;
        
        timer += Time.deltaTime;

        if (timer >= growTime)
        {
            Grow();
            timer = 0;
        }
    }
    public virtual void Harvest()
    {
        targetedForHarvest = false;
    }

    public virtual void Grow()
    {
        
    }
}
