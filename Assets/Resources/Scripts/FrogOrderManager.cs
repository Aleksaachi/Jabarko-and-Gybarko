using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FrogOrderManager : MonoBehaviour
{
    public static FrogOrderManager instance;
    public static List<Frog> followers = new List<Frog>();
    public float selectRadius;
    public int workerFrogCarryCapacity;
    public Vector3 tongueStart;
    Vector3 tongueEnd;
    public float tongueTime;
    float tongueTimer;
    public LineRenderer tongue;
    bool tongueOut;
    bool tongueReturn;
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
        if (Input.GetMouseButtonDown(0) && !RaycastChecker.Check())
        {

            CheckForSelectables();
            Tongue();
        }

        if (tongueOut)
        {
            if (tongueReturn) tongueTimer -= Time.deltaTime;
            else tongueTimer += Time.deltaTime;

            tongueTimer = Mathf.Clamp(tongueTimer, 0, tongueTime);

            Vector3 endPoint = Vector3.Lerp(tongueStart + transform.position, tongueEnd, tongueTimer / tongueTime);
            tongue.SetPosition(0, endPoint);
            tongue.SetPosition(1, tongueStart + transform.position);

            if (tongueTimer == tongueTime && !tongueReturn)
            {
                tongueReturn = true;
            }
            else if (tongueTimer == 0 && tongueReturn)
            {
                tongueOut = false;
                tongueTimer = 0;
            }
        }
    }
    void Tongue()
    {
        if (tongueOut) return;

        tongueOut = true;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        tongueEnd = mousePosition;
        tongueReturn = false;
    }

    void CheckForSelectables()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

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
