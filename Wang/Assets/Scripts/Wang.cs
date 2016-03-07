using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wang : MonoBehaviour {

    public List<Texture2D> m_Tiles;
    public List<GameObject> m_Prefabs;

    private Vector2 m_NorthPos, m_EastPos, m_SouthPos, m_WestPos;

    private Color m_PrevColor;

    private bool m_FoundFirst = false;
    private int m_FoundCount = 0;


    private int _Length = 16;

	void Start () {
        m_NorthPos = new Vector2(5, 8);
        m_EastPos  = new Vector2(8, 5);
        m_SouthPos = new Vector2(5, 3);
        m_WestPos  = new Vector2(3, 5);

        Instantiate(m_Prefabs[0]);

        Texture2D _initialColor = (Texture2D)m_Prefabs[0].GetComponent<Renderer>().sharedMaterial.mainTexture;
        m_PrevColor = _initialColor.GetPixel((int)m_EastPos.x, (int)m_EastPos.y);

        for (int i = 1; i < _Length; i++)
        {
            // Get current prefab texture
            Texture2D tex = (Texture2D)m_Prefabs[i].GetComponent<Renderer>().sharedMaterial.mainTexture;

            // Check against initial colour
            if (!m_FoundFirst)
            {
                Color first = tex.GetPixel((int)m_WestPos.x, (int)m_WestPos.y);
                // Prev(init east) vs current in loop(west)
                if (m_PrevColor == first && !m_FoundFirst)
                {
                    m_FoundCount++;
                    Instantiate(m_Prefabs[i], new Vector3(m_Prefabs[i].transform.position.x - m_FoundCount, m_Prefabs[i].transform.position.y, m_Prefabs[i].transform.position.z), m_Prefabs[i].transform.rotation);
                    m_PrevColor = tex.GetPixel((int)m_EastPos.x, (int)m_EastPos.y);
                    m_FoundFirst = true;
                }
                continue;
            }
            
            // Current East
            Color current = tex.GetPixel((int)m_WestPos.x, (int)m_WestPos.y);
            
            // Check West of current vs west of prev
            if(m_PrevColor == current && m_FoundFirst)
            {
                m_FoundCount++;
                Instantiate(m_Prefabs[i], new Vector3(m_Prefabs[i].transform.position.x - m_FoundCount, m_Prefabs[i].transform.position.y, m_Prefabs[i].transform.position.z), m_Prefabs[i].transform.rotation);
                m_PrevColor = tex.GetPixel((int)m_EastPos.x, (int)m_EastPos.y);
            }
        }
    }
}
