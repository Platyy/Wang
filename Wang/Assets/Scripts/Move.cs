using UnityEngine;
using System.Collections;

using Pathfinding;
public class Move : MonoBehaviour
{
    public Vector3 targetPosition;
    Seeker seeker;
    public void Start()
    {
        //Get a reference to the Seeker component we added earlier
        seeker = GetComponent<Seeker>();
        //Start a new path to the targetPosition, return the result to the OnPathComplete function
    }

    void Update()
    {
        seeker.StartPath(transform.position, targetPosition, OnPathComplete);

    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
    }
}