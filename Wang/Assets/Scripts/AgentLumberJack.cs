﻿using UnityEngine;
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

    public float m_MovSpeed = 2f;
    public float m_ChopSpeed = 0.25f;

    private uint m_InventorySize = 10;
    private uint m_CurrentNWood = 0;
    private uint m_CurrentPine = 0;

    GameObject m_MyRDP;

    GameObject m_CurrentTile;

    bool m_ShouldSearch = true;
    bool m_IsChopping = false;

    enum CurrentState
    {
        MOVINGTORDP,
        MOVINGTOTILE,
        SEARCHINGFORTILE,
        CHOPPINGWOOD
    }

    CurrentState m_MyState;

    public enum Choice
    {
        NWOOD,
        PINE
    }

    void Start()
    {
        m_Wang         = GameObject.FindGameObjectWithTag("WangObject");
        m_WangObject   = m_Wang.GetComponent<Wang>();
        m_Seeker       = GetComponent<Seeker>();
        m_Controller   = GetComponent<CharacterController>();
        m_MyLerp       = GetComponent<AILerp>();
        m_MyState      = CurrentState.SEARCHINGFORTILE;
        m_MovSpeed     = Random.Range(0.5f, 2.5f);
        m_ChopSpeed    = Random.Range(0.1f, 0.5f);
    }

    void Update()
    {
        m_MyLerp.speed = m_MovSpeed;
        if(m_ShouldSearch && m_MyState == CurrentState.SEARCHINGFORTILE)
            SearchForTrees();
        
        if(m_Seeker.IsDone() && m_MyState == CurrentState.MOVINGTOTILE)
        {
            if (m_MyLerp.targetReached && !m_IsChopping)
            {
                // Begin chopping wood?
                
                StartCoroutine(IChop(m_CurrentTile, Choice.NWOOD));
                m_IsChopping = true;
            }
        }
        else if(m_Seeker.IsDone() && m_MyLerp.targetReached && m_MyState == CurrentState.MOVINGTORDP)
        {
            DepositResources();
            m_ShouldSearch = true;
            m_MyState = CurrentState.SEARCHINGFORTILE;
        }
    }

    // NWood Priorities: Cube11 -> Cube12&15 -> Cube3&9 -> Cube16 -> Cube1 -> Cube8&14 -> Cube2&5 -> Cube6
    // Pine  Priorities: Cube1 -> Cube3&9 -> Cube11 ->Cube2&5 -> Cube12&15

    void SearchForTrees()
    {
        bool _foundTile = false;
        while(!_foundTile)
        {
            int[] _matsToFind = new int[] { 11, 12, 15 };
            GameObject[] _found = m_WangObject.FindCollection(transform.position, _matsToFind, 15);

            if(_found != null)
            {
                for(int i = 0; i < _found.Length; i++)
                {
                    _found[i].SetActive(true);
                    if (!_found[i].GetComponent<TileResources>().m_NWoodDepleted)
                    {
                        m_MyState = CurrentState.MOVINGTOTILE;

                        m_Seeker.StartPath(transform.position, _found[i].transform.position, OnPathComplete);

                        _foundTile = true;
                        m_CurrentTile = _found[i];
                        m_ShouldSearch = false;
                        break;
                    }
                    else
                    {
                        _found[i].SetActive(false);
                        continue;
                    }
                }
                break;
            }
        }

    }

    void DepositResources()
    {
        if(m_CurrentNWood > 0)
        {
            m_MyRDP.GetComponent<RDPManager>().m_WoodAmount += m_CurrentNWood;
            m_CurrentNWood = 0;
        }
        else if(m_CurrentPine > 0)
        {

        }
    }

    IEnumerator IChop(GameObject _currentTile, Choice _type)
    {
        if (_type == Choice.NWOOD)
        {
            m_MyState = CurrentState.CHOPPINGWOOD;
            while (m_CurrentNWood < m_InventorySize || !_currentTile.GetComponent<TileResources>().m_NWoodDepleted)
            {

                m_CurrentNWood++;
                _currentTile.GetComponent<TileResources>().m_NWood--;
                yield return new WaitForSeconds(m_ChopSpeed);
                if (m_CurrentNWood == m_InventorySize)
                    break;
            }
        }
        else if (_type == Choice.PINE)
        {
            m_MyState = CurrentState.CHOPPINGWOOD;
            while (m_CurrentPine <= m_InventorySize || !_currentTile.GetComponent<TileResources>().m_PineDepleted)
            {
                m_CurrentPine++;
                _currentTile.GetComponent<TileResources>().m_Pine--;
                yield return new WaitForSeconds(m_ChopSpeed * 3f);
            }
        }
        else yield break;
        
        if(m_CurrentNWood == m_InventorySize || _currentTile.GetComponent<TileResources>().m_NWoodDepleted)
        {
            _currentTile.SetActive(false);
            SetRDP();
            ReturnToRDP();
            m_IsChopping = false;
            yield return null;
        }
    }

    void SetRDP()
    {
        if(m_MyRDP == null)
        {
            m_MyRDP = m_WangObject.FindRDP(transform.position);
            m_MyRDP.GetComponent<RDPManager>().m_LumberjackAmount++;
        }
    }

    void ReturnToRDP()
    {
        if(m_MyRDP != null)
        {
            m_MyLerp.enabled = true;
            m_Seeker.StartPath(transform.position, m_MyRDP.transform.position, Deposit);

        }
    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            m_Path = p;
        }
    }

    void Deposit(Path p)
    {
        if(!p.error)
        {
            m_Path = p;
            m_MyState = CurrentState.MOVINGTORDP;
        }
    }
}
