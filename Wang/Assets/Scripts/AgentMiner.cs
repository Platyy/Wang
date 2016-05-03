using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.Util;
public class AgentMiner : MonoBehaviour {

    Seeker m_Seeker;
    CharacterController m_Controller;

    Wang m_WangObject;
    GameObject m_Wang;
    AILerp m_MyLerp;

    Path m_Path;

    public float m_MovSpeed = 2f;
    public float m_MineSpeed = 0.25f;
    public float m_ChanceForRare = 0.25f;

    private uint m_InventorySize = 10;
    private uint m_CurrentStone = 0;
    private uint m_CurrentIron = 0;

    GameObject m_MyRDP;

    GameObject m_CurrentTile;

    bool m_ShouldSearch = true;
    bool m_IsMining = false;

    enum CurrentState
    {
        MOVINGTORDP,
        MOVINGTOTILE,
        SEARCHINGFORTILE,
        MININGRESOURCES
    }

    CurrentState m_MyState;

    public enum Choice
    {
        STONE,
        IRON
    }

    public Choice m_MyChoice;

    void Start()
    {
        m_Wang          = GameObject.FindGameObjectWithTag("WangObject");
        m_WangObject    = m_Wang.GetComponent<Wang>();
        m_Seeker        = GetComponent<Seeker>();
        m_Controller    = GetComponent<CharacterController>();
        m_MyLerp        = GetComponent<AILerp>();
        m_MyState       = CurrentState.SEARCHINGFORTILE;
        m_MovSpeed      = Random.Range(0.5f, 2.5f);
        m_MineSpeed     = Random.Range(0.1f, 0.5f);
        m_ChanceForRare = Random.Range(0.01f, 0.99f);

        if (m_ChanceForRare > 0.25f)
            m_MyChoice = Choice.STONE;
        else
            m_MyChoice = Choice.IRON;
    }

    void Update()
    {
        m_MyLerp.speed = m_MovSpeed;
        if (m_ShouldSearch && m_MyState == CurrentState.SEARCHINGFORTILE)
            SearchForRes();

        if (m_Seeker.IsDone() && m_MyState == CurrentState.MOVINGTOTILE)
        {
            if (m_MyLerp.targetReached && !m_IsMining)
            {
                StartCoroutine(IMine(m_CurrentTile, m_MyChoice));
                m_IsMining = true;
            }
        }
        else if (m_Seeker.IsDone() && m_MyLerp.targetReached && m_MyState == CurrentState.MOVINGTORDP)
        {
            DepositResources();
            m_ShouldSearch = true;
            m_MyState = CurrentState.SEARCHINGFORTILE;
        }
    }

    // Stone Priorities: Cube6 -> Cube8&14 -> Cube2&5, -> Cube16
    // Iron  Priorities: Cube6 -> Cube2&5 -> Cube1

    void SearchForRes()
    {
        bool _foundTile = false;
        while (!_foundTile)
        {
            int[] _matsToFind = new int[] { };
            if (m_MyChoice == Choice.STONE)
                _matsToFind = new int[] { 6, 8, 14 };
            else if (m_MyChoice == Choice.IRON)
                _matsToFind = new int[] { 6, 2, 5 };

            GameObject[] _found = m_WangObject.FindCollection(transform.position, _matsToFind, 15);

            if (_found != null)
            {
                for (int i = 0; i < _found.Length; i++)
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
        if (m_CurrentStone > 0)
        {
            m_MyRDP.GetComponent<RDPManager>().m_StoneAmount += m_CurrentStone;
            m_CurrentStone = 0;
        }
        else if (m_CurrentIron > 0)
        {
            m_MyRDP.GetComponent<RDPManager>().m_IronAmount += m_CurrentIron;
            m_CurrentIron = 0;
        }
    }

    IEnumerator IMine(GameObject _currentTile, Choice _type)
    {
        var _tileRes = _currentTile.GetComponent<TileResources>();
        if (_type == Choice.STONE)
        {
            m_MyState = CurrentState.MININGRESOURCES;
            while (m_CurrentStone < m_InventorySize || !_tileRes.m_StoneDepleted)
            {

                m_CurrentStone++;
                _currentTile.GetComponent<TileResources>().m_Stone--;
                yield return new WaitForSeconds(m_MineSpeed);
                if (m_CurrentStone == m_InventorySize)
                    break;
            }
        }
        else if (_type == Choice.IRON)
        {
            m_MyState = CurrentState.MININGRESOURCES;
            while (m_CurrentIron <= m_InventorySize || !_tileRes.m_IronDepleted)
            {
                m_CurrentIron++;
                _tileRes.m_Iron--;
                yield return new WaitForSeconds(m_MineSpeed * 3f);
                if (m_CurrentIron == m_InventorySize)
                    break;
            }
        }
        else yield break;

        if (m_CurrentStone == m_InventorySize || m_CurrentIron == m_InventorySize || _tileRes.m_StoneDepleted || _tileRes.m_IronDepleted)
        {
            _currentTile.SetActive(false);
            SetRDP();
            ReturnToRDP();
            m_IsMining = false;
            yield return null;
        }
    }

    bool SetRDP()
    {
        if (m_MyRDP == null)
        {
            m_MyRDP = m_WangObject.FindRDP(transform.position);
            if (m_MyRDP != null)
                m_MyRDP.GetComponent<RDPManager>().m_Miners.Add(gameObject);
            return true;
        }
        return false;
    }

    void ReturnToRDP()
    {
        if (m_MyRDP != null)
        {
            m_MyLerp.enabled = true;
            m_Seeker.StartPath(transform.position, m_MyRDP.transform.position, Deposit);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            m_Path = p;
        }
    }

    void Deposit(Path p)
    {
        if (!p.error)
        {
            m_Path = p;
            m_MyState = CurrentState.MOVINGTORDP;
        }
    }
}
