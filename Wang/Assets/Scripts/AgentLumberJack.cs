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
    private uint m_CurrentInvent = 0;

    Vector3 m_MyRDP;

    GameObject m_CurrentTile;

    bool m_ShouldSearch = true;
    bool m_IsChopping = false;

    public enum Choice
    {
        NWOOD,
        PINE
    }

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
        if(m_Seeker.IsDone())
        {
            if (m_MyLerp.targetReached && !m_IsChopping)
            {
                // Begin chopping wood?

                
                StartCoroutine(IChop(m_CurrentTile, Choice.NWOOD));
                m_IsChopping = true;
            }
        }


        if(m_Seeker.)
        {

        }

        //Debug.Log("MyLerp Enabled: " + m_MyLerp.enabled);
    }

    // NWood Priorities: Cube11 -> Cube12&15 -> Cube3&9 -> Cube16 -> Cube1 -> Cube8&14 -> Cube2&5 -> Cube6
    // Pine  Priorities: Cube1 -> Cube3&9 -> Cube11 ->Cube2&5 -> Cube12&15

    void SearchForTrees()
    {
        bool _foundTile = false;
        GameObject _go = null;
        while(!_foundTile)
        {
            int[] _matsToFind = new int[] { 11 };
            GameObject[] _found = m_WangObject.FindCollection(transform.position, _matsToFind, 3);
            // Does tile have resources left?
            // _found.GetComponent<TileResources>().isEmpty?
            
            if(_found != null)
            {
                for(int i = 0; i < _found.Length; i++)
                {
                    if(!_found[i].GetComponent<TileResources>().m_Depleted)
                    {
                        m_Seeker.StartPath(transform.position, _found[i].transform.position, OnPathComplete);
                        
                        _foundTile = true;
                        m_CurrentTile = _found[i];
                        m_ShouldSearch = false;
                        break;
                    }
                }
                break;
            }
        }

    }
    
    
    IEnumerator IChop(GameObject _currentTile, Choice _type)
    {
        if(_type == Choice.NWOOD)
        {
            while(m_CurrentInvent < m_InventorySize || !_currentTile.GetComponent<TileResources>().m_Depleted)
            {
                m_CurrentInvent++;
                _currentTile.GetComponent<TileResources>().m_NWood--;
                Debug.Log(m_CurrentInvent);
                yield return new WaitForSeconds(1);
                if(m_CurrentInvent == m_InventorySize)
                    break;
            }
        }
        else if(_type == Choice.PINE)
        {
            while (m_CurrentInvent <= m_InventorySize || !_currentTile.GetComponent<TileResources>().m_Depleted)
            {
                m_CurrentInvent++;
                _currentTile.GetComponent<TileResources>().m_Pine--;
                yield return new WaitForSeconds(2);
            }
        }

        if(m_CurrentInvent == m_InventorySize || _currentTile.GetComponent<TileResources>().m_Depleted)
        {
            SetRDP();
            ReturnToRDP();
            m_IsChopping = false;
            yield return null;
        }
    }

    void SetRDP()
    {
        if(m_MyRDP == Vector3.zero)
        {
            m_MyRDP = m_WangObject.FindRDP(transform.position).transform.position;
        }
    }

    void ReturnToRDP()
    {
        if(m_MyRDP != Vector3.zero)
        {
            m_MyLerp.enabled = true;
            m_Seeker.StartPath(transform.position, m_MyRDP, Deposit);

        }
    }

    void OnPathComplete(Path p)
    {
        Debug.Log("Error?: " + p.error);
        if(!p.error)
        {
            m_Path = p;
        }
        else if(p.error)
        {
            Debug.Log(p.errorLog);
        }
    }

    void Deposit(Path p)
    {
        if(!p.error)
        {
            m_Path = p;
            //m_ShouldSearch = true;
        }
    }
}
