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


    public GameObject m_ChopSpeedBuilding, m_MineSpeedBuilding, m_ChopMoveBuilding, m_MineMoveBuilding;

    Seeker m_Seeker;

    Path m_Path;

    CharacterController m_Controller;
    
    Wang m_WangObject;

    AILerp m_MyLerp;

    public float m_Speed = 1f;

    Transform m_RDPTile;

    Vector3 m_RDP, m_BuildingTile;

    GameObject m_MyRDP, m_CurrentBuilding;

    bool m_Found = false;
    bool m_RDPPlaced = false;
    bool m_Moving = false;

    enum CurrentState
    {
        MOVINGTORDP,
        MOVINGTOTILE,
        RETURNINGTORDP,
        SEARCHINGFORRDP,
        FINDINGTILE,
        MAKINGDECISION,
        WAITING,
        FINISHED,
    }

    CurrentState m_MyState;

    void Start()
    {
        var _Wang        = GameObject.FindGameObjectWithTag("WangObject");
        m_WangObject     = _Wang.GetComponent<Wang>();
        m_Seeker         = GetComponent<Seeker>();
        m_Controller     = GetComponent<CharacterController>();
        m_MyLerp         = GetComponent<AILerp>();
        m_MyState        = CurrentState.SEARCHINGFORRDP;
        m_MyLerp.enabled = false;
    }

    void Update()
    {
        if (m_MyState == CurrentState.SEARCHINGFORRDP)
            FindRDPTile();
        if (m_MyLerp.targetReached && m_MyState == CurrentState.MOVINGTORDP)
            PlaceRDP();
        if(m_MyState == CurrentState.MOVINGTOTILE && m_MyLerp.targetReached && m_Moving)
        {
            m_Moving = false;
            m_MyLerp.enabled = false;
            Build(m_CurrentBuilding);
        }

        if(m_MyState == CurrentState.MOVINGTOTILE && !m_MyLerp.targetReached && m_Seeker.IsDone())
            m_MyState = CurrentState.MOVINGTOTILE;

        if(m_MyState == CurrentState.WAITING && m_MyState != CurrentState.MOVINGTOTILE)
            StartCoroutine(Choose(1));

        if (m_Seeker.IsDone())
        {
            if (!m_MyLerp.targetReached && m_MyState == CurrentState.RETURNINGTORDP)
                m_MyState = CurrentState.RETURNINGTORDP;
            else if (m_MyLerp.targetReached && m_MyState == CurrentState.RETURNINGTORDP)
                m_MyState = CurrentState.WAITING;
        }
        Debug.Log(m_MyState);
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
                m_MyLerp.StopAllCoroutines();
                m_MyLerp.enabled = false;
                m_MyState = CurrentState.WAITING;
            }
        }
    }

    void FindRDPTile()
    {
        while (!m_Found)
        {
            int[] _matsToFind = new int[] { 4, 7, 10, 13 };
            GameObject _found = m_WangObject.FindNearest(transform.position, _matsToFind);
            if (_found != null && !_found.GetComponent<TileResources>().m_HasBuilding)
            {
                m_RDP = _found.transform.position;
                m_MyLerp.enabled = true;
                m_Seeker.StartPath(transform.position, _found.transform.position, OnPathComplete);
                m_MyState = CurrentState.MOVINGTORDP;

                _found.GetComponent<TileResources>().m_HasBuilding = true;
                m_RDPTile = _found.transform;
                m_Found = true;
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
        m_MyLerp.enabled = true;
        m_Seeker.StartPath(transform.position, m_RDP, OnPathComplete);
        m_MyState = CurrentState.RETURNINGTORDP;
    }
    
    void FindBuildTile()
    {
        if (m_Moving || m_MyState == CurrentState.MOVINGTOTILE)
            return;
        
        bool _found = false;
        m_MyState = CurrentState.FINDINGTILE;

        int[] _matsToFind = new int[] { 4, 7, 10, 13 };
        GameObject[] _go = m_WangObject.FindCollection(transform.position, _matsToFind, 10);
        for(int i = 0; i < _go.Length; i++)
        {
            if (_go[i] != null && !_go[i].GetComponent<TileResources>().m_HasBuilding)
            {
                m_MyLerp.enabled = true;
                m_Seeker.StartPath(transform.position, _go[i].transform.position, OnPathComplete);
                m_Moving = true;
                _found = true;
                _go[i].GetComponent<TileResources>().m_HasBuilding = true;
                m_BuildingTile = _go[i].transform.position;
                m_MyState = CurrentState.MOVINGTOTILE;
                break;
            }
        }
        if(!_found)
            m_MyState = CurrentState.FINISHED;
    }

    void Build(GameObject _selectedBuilding)
    {
        if (_selectedBuilding != null && m_Seeker.IsDone() && m_MyLerp.targetReached && !m_Moving)
        {
            m_MyLerp.StopAllCoroutines();
            m_MyLerp.enabled = false;
            Instantiate(_selectedBuilding, new Vector3(m_BuildingTile.x, m_BuildingTile.y + 0.5f, m_BuildingTile.z), Quaternion.identity);
            //m_CurrentBuilding = null;
            Debug.Log("Built Thing");
            ReturnToRDP();
        }
    }

    IEnumerator Choose(int _seconds)
    {
        if (m_MyState == CurrentState.MOVINGTOTILE || m_Moving || m_MyState == CurrentState.RETURNINGTORDP)
            yield break;

        //Debug.Log("CHOOSING");
        yield return new WaitForSeconds(_seconds);
        var _rdpMan = m_MyRDP.GetComponent<RDPManager>();
        int _minerAmount = _rdpMan.m_Miners.Count;
        int _lumberAmount = _rdpMan.m_Lumberjacks.Count;
        if (_minerAmount > _lumberAmount)
        { // More Miners
            int _slowMovers = 0, _regMovers = 0, _fastMovers = 0, _slowMiners = 0, _regMiners = 0, _fastMiners = 0;
            for (int i = 0; i < _rdpMan.m_Miners.Count; i++)
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
            if (_slowMovers > _slowMiners)
            {
                // MoveMineBuild
                yield return new WaitForSeconds(_seconds);

                m_CurrentBuilding = m_MineMoveBuilding;
                FindBuildTile();
                yield break;
            }
            else if (_slowMovers < _slowMiners)
            {
                // Build MineSpeed Upgrade
                yield return new WaitForSeconds(_seconds);

                m_CurrentBuilding = m_MineSpeedBuilding;
                FindBuildTile();
                yield break;

            }
            else yield return null;
            yield return null;
        }
        else if (_minerAmount < _lumberAmount)
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
                yield return new WaitForSeconds(_seconds);

                m_CurrentBuilding = m_ChopMoveBuilding;
                FindBuildTile();
                yield break;


            }
            else if (_slowMovers < _slowChoppers)
            {
                // Build Chop Upgrade
                yield return new WaitForSeconds(_seconds);

                m_CurrentBuilding = m_ChopSpeedBuilding;
                FindBuildTile();
                yield break;

            }
            else yield return null;
            yield return null;

        }
        else yield return null;
        yield break;

    }
}