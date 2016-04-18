using UnityEngine;
using System.Collections;

public class TileResources : MonoBehaviour {

    /*
    Cube1: 60% Pine, 60% Iron, 140% NWood, 140% Stone
    Cube2 & Cube5: 60% Iron, 200% Stone, 110% NWood, 30% Pine
    Cube3 & Cube9: 60% Pine, 200% NWood, 110% Stone, 30% Iron
    Cube6: 80% NWood, 260% Stone 60% Iron
    ##########################################################Cube4,7,10,13: 170% Stone, 170% NWood 30% Pine, 30% Iron  ###BUILDING TILES###
    Cube8 & Cube14: 140% NWood, 230% Stone, 30% Iron
    Cube11: 60% Pine, 260% NWood, 80% Stone
    Cube12 & 15: 140% stone 230% nwood, 30% pine
    Cube16: 200% NWood, 200% Stone
    */

    // NWood Priorities: Cube11 -> Cube12&15 -> Cube3&9 -> Cube16 -> Cube1 -> Cube8&14 -> Cube2&5 -> Cube6
    // Pine  Priorities: Cube1 -> Cube3&9 -> Cube11 ->Cube2&5 -> Cube12&15

    public int m_Pine = 0, m_NWood = 0, m_Iron = 0, m_Stone = 0;

    public bool m_Depleted = false;

    void Start()
    {


    }

}
