using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Wang : MonoBehaviour
{
    public List<GameObject> m_Mats;
    public List<GameObject> m_Prefabs;

    private static Vector2 m_NorthPos, m_EastPos, m_SouthPos, m_WestPos;

    private static Color m_Red, m_Green, m_Blue, m_Yellow;

    private Color m_PrevColor;

    private bool m_FoundFirst = false;
    private int m_FoundCount = 0;
    private int m_LineCount = 0;

    #region ColorDirections
    private int[] BlueWest   = { 1,  3,  5,  7,  9, 11, 13, 15 };
    private int[] BlueEast   = { 1,  2,  3,  4,  9, 10, 11, 12 };
    private int[] YellowWest = { 2,  4,  6,  8, 10, 12, 14, 16 };
    private int[] YellowEast = { 5,  6,  7,  8, 13, 14, 15, 16 };
    private int[] RedNorth   = { 1,  2,  3,  4,  5,  6,  7,  8 };
    private int[] RedSouth   = { 1,  2,  5,  6,  9, 10, 13, 14 };
    private int[] GreenNorth = { 9, 10, 11, 12, 13, 14, 15, 16 };
    private int[] GreenSouth = { 3,  4,  7,  8, 11, 12, 15, 16 };

    private int[] BlueGreen   = { 3, 7, 11, 15 }; // west & south
    private int[] BlueRed     = { 1, 5,  9, 13 };
    private int[] YellowGreen = { 4, 8, 12, 16 };
    private int[] YellowRed   = { 2, 6, 10, 14 };

    #endregion

    private Color[] m_CachedNorthTile;

    public int m_Length = 16;

    Color GetNorth(GameObject prefab)
    {
        Texture2D prefabTex = (Texture2D)prefab.GetComponent<Renderer>().sharedMaterial.mainTexture;
        Color northColor = prefabTex.GetPixel((int)m_NorthPos.x, (int)m_NorthPos.y);
        return northColor;
    }

    Color GetEast(GameObject prefab)
    {
        Texture2D prefabTex = (Texture2D)prefab.GetComponent<Renderer>().sharedMaterial.mainTexture;
        Color eastColor = prefabTex.GetPixel((int)m_EastPos.x, (int)m_EastPos.y);
        return eastColor;
    }

    Color GetWest(GameObject prefab)
    {
        Texture2D prefabTex = (Texture2D)prefab.GetComponent<Renderer>().sharedMaterial.mainTexture;
        Color WestColor = prefabTex.GetPixel((int)m_WestPos.x, (int)m_WestPos.y);
        return WestColor;
    }

    Color GetSouth(GameObject prefab)
    {
        Texture2D prefabTex = (Texture2D)prefab.GetComponent<Renderer>().sharedMaterial.mainTexture;
        Color SouthColor = prefabTex.GetPixel((int)m_SouthPos.x, (int)m_SouthPos.y);
        return SouthColor;
    }

    void Start()
    {
        #region Pos&Color
        m_NorthPos = new Vector2(5, 8);
        m_EastPos  = new Vector2(8, 5);
        m_SouthPos = new Vector2(5, 3);
        m_WestPos  = new Vector2(3, 5);

        m_Red    = new Color(1, 0, 0, 1);
        m_Green  = new Color(0, 1, 0, 1);
        m_Blue   = new Color(0, 0, 1, 1);
        m_Yellow = new Color(1, 0.9490196f, 0, 1);
        #endregion

        m_CachedNorthTile = new Color[m_Length];
        GenerateLine();
    }

    void GenerateLine()
    {
        MakeBlock(0, 0);
        for (int i = 1; i < m_Length; i++)
        {
            // Check if a new object after the first has been created
            if (!m_FoundFirst)
            {
                // Check if init east color is blue
                if (m_PrevColor == m_Blue && !m_FoundFirst)
                {
                    // Blue west
                    // get random int (0-7)
                    int selected = Random.Range(0, 7);
                    // Choose from pool of tiles with a blue west from random int
                    int pos = (BlueWest[selected] - 1);
                    if(pos != -1)
                        MakeBlock(pos, i);
                    
                    m_FoundFirst = true;
                }
                //Check if init east color is yellow
                else if (m_PrevColor == m_Yellow && !m_FoundFirst)
                {
                    //Yellow West
                    int selected = Random.Range(0, 7);
                    int pos = (YellowWest[selected] - 1);
                    if (pos != -1)
                        MakeBlock(pos, i);
                    m_FoundFirst = true;
                }
                continue;
            }

            // Check West of current vs west of prev
            if (m_PrevColor == m_Blue && m_FoundFirst)
            {
                int selected = Random.Range(0, 7);
                int pos = (BlueWest[selected] - 1);
                if (pos != -1)
                    MakeBlock(pos, i);
                continue;
            }
            else if (m_PrevColor == m_Yellow && m_FoundFirst)
            {
                //m_FoundCount++;
                int selected = Random.Range(0, 7);
                int pos = (YellowWest[selected] - 1);
                if (pos != -1)
                    MakeBlock(pos, i);
                continue;
            }
        }
        m_LineCount = 1;
        for (int i = 1; i < m_Length; i++)
            NewLine();
    }

    void NewLine()
    {
        m_FoundFirst = false;
        m_FoundCount = 0;
        for (int i = 0; i < m_Length; i++)
        {
            if (!m_FoundFirst)
            {
                if (m_CachedNorthTile[i] == m_Red)
                {
                    int selected = Random.Range(0, 7);
                    int pos = (RedSouth[selected] - 1);
                    if (pos != -1)
                        MakeBlock(pos, i);
                    m_FoundFirst = true;
                }
                else if (m_CachedNorthTile[i] == m_Green)
                {
                    int selected = Random.Range(0, 7);
                    int pos = (GreenSouth[selected] - 1);
                    if (pos != -1)
                        MakeBlock(pos, i);
                    m_FoundFirst = true;
                }
                continue;
            }
            if (m_PrevColor == m_Blue && m_FoundFirst)
            {
                if (m_CachedNorthTile[i] == m_Red)
                {
                    int selected = Random.Range(0, 3);
                    int pos = (BlueRed[selected] - 1);
                    if (pos != -1)
                        MakeBlock(pos, i);
                    continue;
                }
                else if (m_CachedNorthTile[i] == m_Green)
                {
                    int selected = Random.Range(0, 3);
                    int pos = (BlueGreen[selected] - 1);
                    if (pos != -1)
                        MakeBlock(pos, i);
                    continue;
                }
            }
            else if (m_PrevColor == m_Yellow && m_FoundFirst)
            {
                if (m_CachedNorthTile[i] == m_Red)
                {
                    int selected = Random.Range(0, 3);
                    int pos = (YellowRed[selected] - 1);
                    if (pos != -1)
                        MakeBlock(pos, i);
                    continue;
                }
                else if (m_CachedNorthTile[i] == m_Green)
                {
                    int selected = Random.Range(0, 4);
                    int pos = (YellowGreen[selected] - 1);
                    if (pos != -1)
                        MakeBlock(pos, i);
                    continue;
                }
            }
        }
        m_LineCount++;
        if(m_LineCount == m_Length)
            Combine();
        
    }

    void MakeBlock(int _position, int _cachePos)
    {
        m_FoundCount++;
        GameObject go = (GameObject)Instantiate(m_Prefabs[_position], new Vector3(m_Prefabs[_position].transform.position.x + m_FoundCount,
                                        m_Prefabs[_position].transform.position.y,
                                        m_Prefabs[_position].transform.position.z - m_LineCount),
                                        m_Prefabs[_position].transform.rotation);
        go.transform.localScale = new Vector3(-1, 1, 1);

        go.transform.SetParent(m_Mats[_position].transform);

        m_PrevColor = GetEast(go);
        m_CachedNorthTile[_cachePos] = GetNorth(go);
    }

    void Combine()
    {
        for (int i = 0; i < m_Mats.Capacity; i++)
        {
            Vector3 position = m_Mats[i].transform.position;
            m_Mats[i].transform.position = Vector3.zero;

            MeshFilter[] _meshFilters = m_Mats[i].GetComponentsInChildren<MeshFilter>();
            CombineInstance[] _combine = new CombineInstance[_meshFilters.Length];
            for(int j = 0; j < _meshFilters.Length; j++)
            {
                _combine[j].mesh = _meshFilters[j].sharedMesh;
                _combine[j].transform = _meshFilters[j].transform.localToWorldMatrix;
                _meshFilters[j].gameObject.SetActive(false);
            }

            while(_combine.Length > 2000)
            {
                GameObject SplitInstance = (GameObject)Instantiate(m_Mats[i]);
                SplitInstance.transform.SetParent(m_Mats[i].transform);
                CombineInstance[] _temp = new CombineInstance[_combine.Length - 2000];
                CombineInstance[] _splitCombine = new CombineInstance[2000];
                System.Array.ConstrainedCopy(_combine, 0, _splitCombine, 0, 2000);
                System.Array.ConstrainedCopy(_combine, 2000, _temp, 0, _temp.Length);
                _combine = _temp;

                SplitInstance.GetComponent<MeshFilter>().mesh = new Mesh();
                SplitInstance.GetComponent<MeshFilter>().mesh.CombineMeshes(_splitCombine);
                SplitInstance.SetActive(true);
            }

            m_Mats[i].GetComponent<MeshFilter>().mesh = new Mesh();
            m_Mats[i].GetComponent<MeshFilter>().mesh.CombineMeshes(_combine);

            m_Mats[i].SetActive(true);

            m_Mats[i].transform.position = position;
        }
    }
}
