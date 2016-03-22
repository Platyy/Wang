using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.Util;
public class AgentBuilder : MonoBehaviour {


    /*
        RDP MAT = MAT4
        BUILDING MATS = MAT7, MAT10, MAT13
    */

    // Set tile = MAT4
    // Go through the child of MAT4
    // find the nearest
    // Pathfind to it
    // Build ResourceDropPoint
    // Decide what building to construct based on nearby agents (Lumberjacks > Miners)?
    // Wait for resources
    // Collect max carryable resources
    // Pathfind to nearest build tile
    // Begin construction
    // Pathfind back to RDP
    // Collect more resources
    // Continue until building is complete.
    // Once complete, move back to RDP
    // If there is resources available to create an unbuilt building in the area -> Use those resources
    // Otherwise, based on agents, choose an unbuilt building of their preference

    /*
        1. Created.
        2. FindNearest RDP space.
        3. Move to RDP space.
        4. Create RDP on RDP space.
        5. Check other nearby agents (Lumberjack/Miner)
        6. Choose building to construct.
        7. Wait until RDP has required resources.
        8. Take resources from RDP.
        9. Move to nearest building space.
        10. Begin construction of new building.
        11. Return to RDP to replenish resources.
        12. Return to building space to continue contstruction.
        13. Repeat 11-12 until building is finished.
        14. Return to RDP and repeat 5-10
    */

    /*
        Bounds:
        z = 0 -> -49
        x = 1 -> 50

    */

    public GameObject m_RDPObj;
    Seeker m_Seeker;

    Path m_Path;

    CharacterController m_Controller;

    AILerp m_MyLerp;

    float m_Radius;
    float m_NextWaypointDist = 3f;
    public float m_Speed = 100f;

    int m_CurrentWaypoint = 0;


    public GameObject m_Mat;

    Vector3 m_RDP;
    
    bool m_Found = false;
    bool m_RDPPlaced = false;

    List<int> m_Used;

    void Start()
    {
        m_Seeker = GetComponent<Seeker>();
        m_Controller = GetComponent<CharacterController>();
        m_MyLerp = GetComponent<AILerp>();
    }

    void Update()
    {
        while(!m_Found)
        {
            Vector3 _found = FindNearest();
            if (_found != Vector3.zero)
            {
                m_RDP = _found;
                m_Seeker.StartPath(transform.position, _found, OnPathComplete);
                m_Found = true;
                break;
            }

        }
        if(m_MyLerp.targetReached)
        {
            m_MyLerp.enabled = false;
            CreateRDP();
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            m_MyLerp.enabled = true;
            ReturnToRDP();
        }
    }


    void OnPathComplete(Path p)
    {
        Debug.Log("Error?: " + p.error);
        if(!p.error)
        {
            m_Path = p;
            m_CurrentWaypoint = 0;
        }
    }


    Vector3 FindNearest()
    {
        m_Radius += 1 * Time.deltaTime;

        Vector3 _obj = Vector3.zero;

        for (int i = 0; i < m_Mat.transform.childCount; i++)
        {
            if (m_Mat.transform.GetChild(i) != null)
            {
                m_Mat.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        Collider[] objInRange = Physics.OverlapSphere(gameObject.transform.position, m_Radius, 1 << 10);
        foreach(Collider col in objInRange)
        {
            if (col != null)
            {
                for (int i = 0; i < m_Mat.transform.childCount; i++)
                {
                    if (m_Mat.transform.GetChild(i) != null)
                        m_Mat.transform.GetChild(i).gameObject.SetActive(false);
                }
                m_Radius = 0;
                return col.transform.position;
            }
        }
        return _obj;
    }

    void ReturnToRDP()
    {
        m_Seeker.StartPath(transform.position, m_RDP, OnPathComplete);
    }

    void CreateRDP()
    {
        if (!m_RDPPlaced)
        {
            Instantiate(m_RDPObj, new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z), transform.rotation);
            m_RDPPlaced = true;
        }
    }
}