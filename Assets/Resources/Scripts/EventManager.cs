using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;
    public static Action newBuildingPlaced;

    void Awake()
    {
        instance = this;
    }
}
