using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRoom : MonoBehaviour {

	private RoomTemplates templates;
	public string special;

	void Start(){
		templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
		templates.rooms.Add(this.gameObject);
		templates.dungeonDepth--;
	}

	void GenerateRoomPrefab()
    {

    }
}
