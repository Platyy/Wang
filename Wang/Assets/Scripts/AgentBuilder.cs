using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.Util;
using KdTree;
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
    
    Seeker m_Seeker;

    Path m_Path;

    CharacterController m_Controller;
    
    Wang m_WangObject;

    AILerp m_MyLerp;

    public float m_Speed = 1f;

    Transform m_RDPTile;

    Vector3 m_RDP;

    GameObject m_MyRDP;

    bool m_Found = false;
    bool m_RDPPlaced = false;

    enum CurrentState
    {
        MOVINGTORDP,
        MOVINGTOTILE,
        SEARCHINGFORTILE,
        CONSTRUCTINGBUILDING,
        WAITINGFORRESOURCES
    }

    CurrentState m_MyState;

    void Start()
    {
        var _Wang    = GameObject.FindGameObjectWithTag("WangObject");
        m_WangObject = _Wang.GetComponent<Wang>();
        m_Seeker     = GetComponent<Seeker>();
        m_Controller = GetComponent<CharacterController>();
        m_MyLerp     = GetComponent<AILerp>();
        m_MyState    = CurrentState.SEARCHINGFORTILE;
    }

    void Update()
    {
        if (m_MyState == CurrentState.SEARCHINGFORTILE)
            FindRDPTile();
        if (m_Seeker.IsDone() && m_MyState == CurrentState.MOVINGTORDP)
            PlaceRDP();

        if(m_MyState == CurrentState.WAITINGFORRESOURCES)
        {
            // Choose a building
            // Wait for resources
        }
    }

    void PlaceRDP()
    {
        if (m_MyLerp.targetReached)
        {
            if (!m_RDPPlaced && m_MyRDP == null)
            {
                m_WangObject.CreateRDP(m_RDPTile);
                m_MyRDP = m_WangObject.FindRDP(m_RDPTile.position);
                m_RDPPlaced = true;
                m_MyState = CurrentState.WAITINGFORRESOURCES;
            }
            m_MyLerp.enabled = false;
        }
    }

    void FindRDPTile()
    {
        while (!m_Found)
        {
            int[] _matsToFind = new int[] { 4, 7, 10, 13 };
            GameObject _found = m_WangObject.FindNearest(transform.position, _matsToFind);
            if (_found != null)
            {
                m_RDP = _found.transform.position;
                m_Seeker.StartPath(transform.position, _found.transform.position, OnPathComplete);
                m_MyState = CurrentState.MOVINGTORDP;
                m_Found = true;
                m_RDPTile = _found.transform;
                break;
            }
        }
    }

    void OnPathComplete(Path p)
    {
        Debug.Log("Error?: " + p.error);
        if(!p.error)
        {
            m_Path = p;
        }
    }

    void ReturnToRDP()
    {
        m_Seeker.StartPath(transform.position, m_RDP, OnPathComplete);
    }
    
    void Choose()
    {
        var _rdpMan = m_MyRDP.GetComponent<RDPManager>();
        int _minerAmount = _rdpMan.m_Miners.Count;
        int _lumberAmount = _rdpMan.m_Lumberjacks.Count;
        if(_minerAmount > _lumberAmount)
        { // More Miners
            int _slowMovers = 0, _regMovers = 0, _fastMovers = 0, _slowMiners = 0, _regMiners = 0, _fastMiners = 0;
            for(int i = 0; i < _rdpMan.m_Miners.Count; i++)
            {
                var _miner = _rdpMan.m_Miners[i].GetComponent<AgentMiner>();
                if (_miner.m_MovSpeed > 2.0f)
                    _fastMovers++;
                else if (_miner.m_MovSpeed > 0.75f)
                    _regMovers++;
                else
                    _slowMovers++;

                if (_miner.m_MineSpeed > 0.4f)
                    _fastMiners++;
                else if (_miner.m_MineSpeed > 0.2f)
                    _regMiners++;
                else
                    _slowMiners++;
            }
            if(_slowMovers > _slowMiners)
            {
                // Build Move Upgrade
            }
            else
            {
                // Build Mine Upgrade
            }
        }
        else
        {
            int _slowMovers = 0, _regMovers = 0, _fastMovers = 0, _slowChoppers = 0, _regChoppers = 0, _fastChoppers = 0;
            for (int i = 0; i < _rdpMan.m_Lumberjacks.Count; i++)
            {
                var _lumber = _rdpMan.m_Lumberjacks[i].GetComponent<AgentLumberJack>();
                if (_lumber.m_MovSpeed > 2.0f)
                    _fastMovers++;
                else if (_lumber.m_MovSpeed > 0.75f)
                    _regMovers++;
                else
                    _slowMovers++;

                if (_lumber.m_ChopSpeed > 0.4f)
                    _fastChoppers++;
                else if (_lumber.m_ChopSpeed > 0.2f)
                    _regChoppers++;
                else
                    _slowChoppers++;
            }
            if (_slowMovers > _slowChoppers)
            {
                // Build Move Upgrade
            }
            else
            {
                // Build Chop Upgrade
            }
        }
    }
}