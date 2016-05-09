using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RDPManager : MonoBehaviour {

    /*
        Used to keep track of the RDP this script is attached to.
    */

    public List<GameObject> m_Miners, m_Lumberjacks;
    
    public uint m_PineAmount = 0, m_WoodAmount = 0, m_StoneAmount = 0, m_IronAmount = 0;
}
