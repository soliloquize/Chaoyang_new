using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC
{
    public int npcId;
    public float npcPosX;
    public float npcPosY;
    public int npcCurState;
}

public class MapManager : MonoBehaviour {

    public List<NPC> npcList;

    void Awake()
    {

    }

	void Update () {
		
	}
}
