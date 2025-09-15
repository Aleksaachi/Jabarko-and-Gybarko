using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrass : Plant
{
    public Sprite grown;
    public Sprite stem;
    SpriteRenderer spriteRenderer;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public override void Grow()
    {
        harvestable = true;
        spriteRenderer.sprite = grown;

        freezeGrowth = true;
    }

    public override void Harvest()
    {
        base.Harvest();
        
        harvestable = false;
        spriteRenderer.sprite = stem;
        
        freezeGrowth = false;
    }
}
