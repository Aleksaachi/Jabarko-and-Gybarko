using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogOrderManager : MonoBehaviour
{
    public static FrogOrderManager instance;
    public static List<Frog> followers = new List<Frog>();
    public float selectRadius;
    void Awake()
    {
        instance = this;
    }

    public static void AddFollower(Frog newFollower)
    {
        if (followers.Contains(newFollower))
        {
            RemoveFollower(newFollower);
            return;
        }

        followers.Add(newFollower);
        newFollower.WalkToKing();
    }

    public static void RemoveFollower(Frog followerToRemove)
    {
        followers.Remove(followerToRemove);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckForSelectables();
        }
    }

    void CheckForSelectables()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.CircleCastAll(mousePosition, selectRadius, Vector2.zero, LayerMask.NameToLayer("Selectable"));
        for (int i = 0; i < hits.Length; i++)
        {
            hits[i].transform.GetComponent<Selectable>().Select();
        }
    }
}
