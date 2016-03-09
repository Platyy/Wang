using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wang : MonoBehaviour {
    
    public List<GameObject> m_Prefabs;

    private Vector2 m_NorthPos, m_EastPos, m_SouthPos, m_WestPos;

    private static Color m_Red, m_Green, m_Blue, m_Yellow;

    private Color m_PrevColor;

    private bool m_FoundFirst = false;
    private int m_FoundCount = 0;
    private int m_LineCount = 0;

    #region ColorDirections
    private int[] BlueWest   = { 1, 3,  5,  7,  9,  11, 13, 15 };
    private int[] BlueEast   = { 1, 2,  3,  4,  9,  10, 11, 12 };
    private int[] YellowWest = { 2, 4,  6,  8,  10, 12, 14, 16 };
    private int[] YellowEast = { 5, 6,  7,  8,  13, 14, 15, 16 };
    private int[] RedNorth   = { 1, 2,  3,  4,  5,  6,  7,   8 };
    private int[] RedSouth   = { 1, 2,  5,  6,  9,  10, 13, 14 };
    private int[] GreenNorth = { 9, 10, 11, 12, 13, 14, 15, 16 };
    private int[] GreenSouth = { 3, 4,  7,  8,  11, 12, 15, 16 };

    private int[] BlueGreen   = { 3, 7, 11, 15 };
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

    void Start () {
        #region Pos&Color
        m_NorthPos = new Vector2(5, 8);
        m_EastPos  = new Vector2(8, 5);
        m_SouthPos = new Vector2(5, 3);
        m_WestPos  = new Vector2(3, 5);
        
        m_Red = new Color(1, 0, 0, 1);
        m_Green = new Color(0, 1, 0, 1);
        m_Blue = new Color(0, 0, 1, 1);
        m_Yellow = new Color(1, 0.9490196f, 0, 1);
        #endregion

        m_CachedNorthTile = new Color[m_Length];
        GenerateLine();
    }
    
    void GenerateLine()
    {
        //Create the first object
        Instantiate(m_Prefabs[0]);
        //Get the first objects' east color
        m_PrevColor = GetEast(m_Prefabs[0]);
        m_CachedNorthTile[0] = GetNorth(m_Prefabs[0]);
        for (int i = 1; i < m_Length; i++)
        {
            // Check if a new object after the first has been created
            if (!m_FoundFirst)
            {
                // Check if init east color is blue
                if (m_PrevColor == m_Blue && !m_FoundFirst)
                {
                    // Blue west
                    m_FoundCount++;
                    // get random int (0-7)
                    int selected = Mathf.RoundToInt(Random.Range(0, 7));
                    // Choose from pool of tiles with a blue west from random int
                    int pos = (BlueWest[selected]) - 1;

                    GameObject go = (GameObject)Instantiate(m_Prefabs[pos], new Vector3(m_Prefabs[pos].transform.position.x + m_FoundCount,
                                                            m_Prefabs[pos].transform.position.y,
                                                            m_Prefabs[pos].transform.position.z),
                                                            m_Prefabs[pos].transform.rotation);
                    go.transform.localScale = new Vector3(-1, 1, 1);


                    m_PrevColor = GetEast(go);
                    m_CachedNorthTile[i] = GetNorth(go);
                    m_FoundFirst = true;
                }
                //Check if init east color is yellow
                else if (m_PrevColor == m_Yellow && !m_FoundFirst)
                {
                    //Yellow West
                    m_FoundCount++;
                    int selected = Mathf.RoundToInt(Random.Range(0, 7));
                    int pos = (YellowWest[selected]) - 1;
                    GameObject go = (GameObject)Instantiate(m_Prefabs[pos], new Vector3(m_Prefabs[pos].transform.position.x + m_FoundCount,
                                                            m_Prefabs[pos].transform.position.y,
                                                            m_Prefabs[pos].transform.position.z),
                                                            m_Prefabs[pos].transform.rotation);
                    go.transform.localScale = new Vector3(-1, 1, 1);
                    m_PrevColor = GetEast(go);
                    m_CachedNorthTile[i] = GetNorth(go);
                    m_FoundFirst = true;
                }
                continue;
            }

            // Check West of current vs west of prev
            if (m_PrevColor == m_Blue && m_FoundFirst)
            {
                m_FoundCount++;
                int selected = Mathf.RoundToInt(Random.Range(0, 7));
                int pos = (BlueWest[selected]) - 1;

                GameObject go = (GameObject)Instantiate(m_Prefabs[pos], new Vector3(m_Prefabs[pos].transform.position.x + m_FoundCount,
                                                        m_Prefabs[pos].transform.position.y,
                                                        m_Prefabs[pos].transform.position.z),
                                                        m_Prefabs[pos].transform.rotation);

                go.transform.localScale = new Vector3(-1, 1, 1);

                m_PrevColor = GetEast(go);
                m_CachedNorthTile[i] = GetNorth(go);
                continue;
            }
            else if (m_PrevColor == m_Yellow && m_FoundFirst)
            {
                m_FoundCount++;
                int selected = Mathf.RoundToInt(Random.Range(0, 7));
                int pos = (YellowWest[selected]) - 1;
                GameObject go = (GameObject)Instantiate(m_Prefabs[pos], new Vector3(m_Prefabs[pos].transform.position.x + m_FoundCount,
                                                        m_Prefabs[pos].transform.position.y,
                                                        m_Prefabs[pos].transform.position.z),
                                                        m_Prefabs[pos].transform.rotation);

                go.transform.localScale = new Vector3(-1, 1, 1);

                m_PrevColor = GetEast(go);
                m_CachedNorthTile[i] = GetNorth(go);
                continue;
            }
        }
        m_LineCount = 1;
        for(int i = 0; i < m_Length; i++)
            NewLine();
    }

    void NewLine()
    {
        m_FoundFirst = false;
        m_FoundCount = 0;
        for(int i = 0; i < m_Length; i++)
        {
            if(!m_FoundFirst)
            {
                if(m_CachedNorthTile[i] == m_Red)
                {
                    int selected = Mathf.RoundToInt(Random.Range(0, 7));
                    int pos = (RedSouth[selected]) - 1;

                    GameObject go = (GameObject)Instantiate(m_Prefabs[pos], new Vector3(m_Prefabs[pos].transform.position.x + m_FoundCount,
                                                           m_Prefabs[pos].transform.position.y,
                                                           m_Prefabs[pos].transform.position.z - m_LineCount),
                                                           m_Prefabs[pos].transform.rotation);
                    go.transform.localScale = new Vector3(-1, 1, 1);
                    m_FoundCount++;
                    m_CachedNorthTile[i] = GetNorth(go);
                    m_FoundFirst = true;
                }
                else if(m_CachedNorthTile[i] == m_Green)
                {
                    int selected = Mathf.RoundToInt(Random.Range(0, 7));
                    int pos = (GreenSouth[selected]) - 1;

                    GameObject go = (GameObject)Instantiate(m_Prefabs[pos], new Vector3(m_Prefabs[pos].transform.position.x + m_FoundCount,
                                                           m_Prefabs[pos].transform.position.y,
                                                           m_Prefabs[pos].transform.position.z - m_LineCount),
                                                           m_Prefabs[pos].transform.rotation);
                    go.transform.localScale = new Vector3(-1, 1, 1);
                    m_FoundCount++;
                    m_CachedNorthTile[i] = GetNorth(go);
                    m_FoundFirst = true;
                }
                continue;
            }
            if(m_PrevColor == m_Blue && m_FoundFirst)
            {
                if (m_CachedNorthTile[i] == m_Red)
                {
                    int selected = Mathf.RoundToInt(Random.Range(0, 3));
                    int pos = (BlueRed[selected]) - 1;

                    GameObject go = (GameObject)Instantiate(m_Prefabs[pos], new Vector3(m_Prefabs[pos].transform.position.x + m_FoundCount,
                                                            m_Prefabs[pos].transform.position.y,
                                                            m_Prefabs[pos].transform.position.z - m_LineCount),
                                                            m_Prefabs[pos].transform.rotation);

                    go.transform.localScale = new Vector3(-1, 1, 1);

                    m_PrevColor = GetEast(go);
                    m_CachedNorthTile[i] = GetNorth(go);
                    m_FoundCount++;
                    continue;
                }
                else if(m_CachedNorthTile[i] == m_Green)
                {
                    int selected = Mathf.RoundToInt(Random.Range(0, 3));
                    int pos = (BlueGreen[selected]) - 1;

                    GameObject go = (GameObject)Instantiate(m_Prefabs[pos], new Vector3(m_Prefabs[pos].transform.position.x + m_FoundCount,
                                        m_Prefabs[pos].transform.position.y,
                                        m_Prefabs[pos].transform.position.z - m_LineCount),
                                        m_Prefabs[pos].transform.rotation);

                    go.transform.localScale = new Vector3(-1, 1, 1);

                    m_PrevColor = GetEast(go);
                    m_CachedNorthTile[i] = GetNorth(go);
                    m_FoundCount++;
                    continue;
                }
            }
            else if(m_PrevColor == m_Yellow && m_FoundFirst)
            {
                if (m_CachedNorthTile[i] == m_Red)
                {
                    int selected = Mathf.RoundToInt(Random.Range(0, 3));
                    int pos = (YellowRed[selected]) - 1;

                    GameObject go = (GameObject)Instantiate(m_Prefabs[pos], new Vector3(m_Prefabs[pos].transform.position.x + m_FoundCount,
                                        m_Prefabs[pos].transform.position.y,
                                        m_Prefabs[pos].transform.position.z - m_LineCount),
                                        m_Prefabs[pos].transform.rotation);

                    go.transform.localScale = new Vector3(-1, 1, 1);

                    m_PrevColor = GetEast(go);
                    m_CachedNorthTile[i] = GetNorth(go);
                    m_FoundCount++;
                    continue;
                }
                else if (m_CachedNorthTile[i] == m_Green)
                {
                    int selected = Mathf.RoundToInt(Random.Range(0, 3));
                    int pos = (YellowGreen[selected]) - 1;

                    GameObject go = (GameObject)Instantiate(m_Prefabs[pos], new Vector3(m_Prefabs[pos].transform.position.x + m_FoundCount,
                                        m_Prefabs[pos].transform.position.y,
                                        m_Prefabs[pos].transform.position.z - m_LineCount),
                                        m_Prefabs[pos].transform.rotation);

                    go.transform.localScale = new Vector3(-1, 1, 1);

                    m_PrevColor = GetEast(go);
                    m_CachedNorthTile[i] = GetNorth(go);
                    m_FoundCount++;
                    continue;
                }
            }
        }
        m_LineCount++;
    }
}
