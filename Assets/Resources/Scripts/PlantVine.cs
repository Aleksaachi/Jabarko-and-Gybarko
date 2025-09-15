using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantVine : Plant
{
    int stage;
    public List<GameObject> segments;
    void Awake()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].SetActive(false);
        }
    }
    public override void Grow()
    {
        stage++;
        harvestable = true;
        segments[stage - 1].SetActive(true);

        if (stage == 3) freezeGrowth = true;
    }

    public override void Harvest()
    {
        base.Harvest();
        
        segments[stage - 1].SetActive(false);
        stage--;
        if (stage == 0) harvestable = false;
        
        freezeGrowth = false;
    }
}
