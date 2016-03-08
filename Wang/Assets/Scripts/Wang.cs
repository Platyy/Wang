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

    #region ColorDirections
    private int[] BlueWest   = { 1, 3,  5,  7,  9,  11, 13, 15 };
    private int[] BlueEast   = { 1, 2,  3,  4,  9,  10, 11, 12 };
    private int[] YellowWest = { 2, 4,  6,  8,  10, 12, 14, 16 };
    private int[] YellowEast = { 5, 6,  7,  8,  13, 14, 15, 16 };
    private int[] RedNorth   = { 1, 2,  3,  4,  5,  6,  7,   8 };
    private int[] RedSouth   = { 1, 2,  5,  6,  9,  10, 13, 14 };
    private int[] GreenNorth = { 9, 10, 11, 12, 13, 14, 15, 16 };
    private int[] GreenSouth = { 3, 4,  7,  8,  11, 12, 15, 16 };
    #endregion

    private int _Length = 64;

	void Start () {
        #region Pos&Color
        m_NorthPos = new Vector2(5, 8);
        m_EastPos  = new Vector2(8, 5);
        m_SouthPos = new Vector2(5, 3);
        m_WestPos  = new Vector2(3, 5);
        
        m_Red = new Color(1, 0, 0, 1);
        m_Green = new Color(0, 1, 0, 1);
        m_Blue = new Color(0, 0, 1, 1);
        m_Yellow = new Color(1, 0.949f, 0, 1);
        #endregion

        //Create the first object
        Instantiate(m_Prefabs[0]);
        //Get the first objects' east color
        Texture2D _initialColor = (Texture2D)m_Prefabs[0].GetComponent<Renderer>().sharedMaterial.mainTexture;
        m_PrevColor = _initialColor.GetPixel((int)m_EastPos.x, (int)m_EastPos.y);

        for (int i = 1; i < _Length; i++)
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
                    int pos = BlueWest[selected];

                    Instantiate(m_Prefabs[pos], new Vector3(m_Prefabs[pos].transform.position.x + m_FoundCount, 
                                                            m_Prefabs[pos].transform.position.y, 
                                                            m_Prefabs[pos].transform.position.z),
                                                            m_Prefabs[0].transform.rotation);

                    Texture2D _prevTex = (Texture2D)m_Prefabs[pos].GetComponent<Renderer>().sharedMaterial.mainTexture;
                    m_PrevColor = _prevTex.GetPixel((int)m_EastPos.x, (int)m_EastPos.y);
                    m_FoundFirst = true;
                }
                //Check if init east color is yellow
                else if(m_PrevColor == m_Yellow && !m_FoundFirst)
                {
                    //Yellow West
                    m_FoundCount++;
                    int selected = Mathf.RoundToInt(Random.Range(0, 7));
                    int pos = YellowWest[selected];
                    Instantiate(m_Prefabs[pos], new Vector3(m_Prefabs[pos].transform.position.x + m_FoundCount,
                                                            m_Prefabs[pos].transform.position.y,
                                                            m_Prefabs[pos].transform.position.z),
                                                            m_Prefabs[0].transform.rotation);

                    Texture2D _prevTex = (Texture2D)m_Prefabs[pos].GetComponent<Renderer>().sharedMaterial.mainTexture;
                    m_PrevColor = _prevTex.GetPixel((int)m_EastPos.x, (int)m_EastPos.y);
                    m_FoundFirst = true;
                }
                continue;
            }

            // Check West of current vs west of prev
            if (m_PrevColor == m_Blue && m_FoundFirst)
            {
                m_FoundCount++;
                int selected = Mathf.RoundToInt(Random.Range(0, 7));
                int pos = BlueWest[selected];

                Instantiate(m_Prefabs[pos], new Vector3(m_Prefabs[pos].transform.position.x + m_FoundCount,
                                                        m_Prefabs[pos].transform.position.y,
                                                        m_Prefabs[pos].transform.position.z),
                                                        m_Prefabs[0].transform.rotation);

                Texture2D _prevTex = (Texture2D)m_Prefabs[pos].GetComponent<Renderer>().sharedMaterial.mainTexture;
                m_PrevColor = _prevTex.GetPixel((int)m_EastPos.x, (int)m_EastPos.y);
            }
            else if (m_PrevColor == m_Yellow && m_FoundFirst)
            {
                m_FoundCount++;
                int selected = Mathf.RoundToInt(Random.Range(0, 7));
                int pos = YellowWest[selected];
                Instantiate(m_Prefabs[pos], new Vector3(m_Prefabs[pos].transform.position.x + m_FoundCount,
                                                        m_Prefabs[pos].transform.position.y,
                                                        m_Prefabs[pos].transform.position.z),
                                                        m_Prefabs[0].transform.rotation);

                Texture2D _prevTex = (Texture2D)m_Prefabs[pos].GetComponent<Renderer>().sharedMaterial.mainTexture;
                m_PrevColor = _prevTex.GetPixel((int)m_EastPos.x, (int)m_EastPos.y);
            }
        }
    }
}
