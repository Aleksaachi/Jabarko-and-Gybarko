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
        newFollower.idle = false;
    }

    public static void RemoveFollower(Frog followerToRemove)
    {
        followers.Remove(followerToRemove);
        followerToRemove.idlePoint = followerToRemove.transform.position;
        followerToRemove.idle = true;
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePosition, selectRadius, LayerMask.GetMask("Selectable"));
        for (int i = 0; i < hits.Length; i++)
        {
            hits[i].transform.GetComponent<Selectable>().Select();
        }
    }

    public static List<FrogWorker> AssignWorkers()
    {
        List<FrogWorker> followersAsWorkers = new List<FrogWorker>();

        for (int i = 0; i < followers.Count; i++)
        {
            FrogWorker worker = followers[i].GetWorker();
            if (worker != null)
            {
                followersAsWorkers.Add(worker);
                worker.idle = true;
            }
        }

        followers.Clear();
        return followersAsWorkers;
    }
}
