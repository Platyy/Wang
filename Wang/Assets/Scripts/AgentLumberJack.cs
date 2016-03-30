using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.Util;
public class AgentLumberJack : MonoBehaviour {

    Seeker m_Seeker;
    CharacterController m_Controller;
    
    Wang m_WangObject;
    GameObject m_Wang;
    AILerp m_MyLerp;
    
    Path m_Path;

    public float m_Speed = 1f;

    private uint m_InventorySize = 10;

    Vector3 m_MyRDP;

    bool m_ShouldSearch = true;

    void Start()
    {
        m_Wang = GameObject.FindGameObjectWithTag("WangObject");
        m_WangObject = m_Wang.GetComponent<Wang>();
        m_Seeker     = GetComponent<Seeker>();
        m_Controller = GetComponent<CharacterController>();
        m_MyLerp     = GetComponent<AILerp>();
    }

    void Update()
    {
        if(m_ShouldSearch)
        {
            SearchForTrees();
        }
    }

    // Prioritise Cube11 -> Cube1 -> Cube3&9 -> Cube2&5 -> Cube16 -> Cube8&16

    void SearchForTrees()
    {
        bool _foundTile = false;
        while(!_foundTile)
        {
            int[] _matsToFind = new int[] { 11 };
            GameObject _found = m_WangObject.FindNearest(transform.position, _matsToFind);
            // Does tile have resources left?
            if(_found != null)
            {
                m_Seeker.StartPath(transform.position, _found.transform.position, OnPathComplete);
                m_ShouldSearch = false;
                _foundTile = true;
                break;
            }
        }
        if(m_MyLerp.targetReached)
        {
            // Begin chopping wood?
            m_MyLerp.enabled = false;
        }
    }

    void ChopWood()
    {
        // Chop until either tile is depleted or invent is full. If either is true, deposit at RDP then search again.
    }

    void OnPathComplete(Path p)
    {
        Debug.Log("Error?: " + p.error);
        if(!p.error)
        {
            m_Path = p;
        }
    }
}
