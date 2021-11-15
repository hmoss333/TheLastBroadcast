using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour {

	public int openingDirection;
	// 1 --> need bottom door
	// 2 --> need top door
	// 3 --> need left door
	// 4 --> need right door


	private RoomTemplates templates;
	private int rand;
	public bool spawned = false;

	//public float waitTime = 4f;
	public GameObject spawnedRoom;


    private void Awake()
    {
		templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
	}

    void Start(){
		//Destroy(gameObject, waitTime);
		//templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
		Invoke("Spawn", 0.1f);
	}


	public void Spawn()
	{
		if (spawned == false)
		{
			switch (openingDirection)
			{
				case 1:
					spawnedRoom = templates.dungeonDepth > 0
						? templates.bottomRooms[Random.Range(0, templates.bottomRooms.Length)]
						: templates.bottomRooms[0];
					break;
				case 2:
					spawnedRoom = templates.dungeonDepth > 0
						? templates.topRooms[Random.Range(0, templates.topRooms.Length)]
						: templates.topRooms[0];
					break;
				case 3:
					spawnedRoom = templates.dungeonDepth > 0
						? templates.leftRooms[Random.Range(0, templates.leftRooms.Length)]
						: templates.leftRooms[0];
					break;
				case 4:
					spawnedRoom = templates.dungeonDepth > 0
						? templates.rightRooms[Random.Range(0, templates.rightRooms.Length)]
						: templates.rightRooms[0];
					break;
			}

			Instantiate(spawnedRoom, transform.position, spawnedRoom.transform.rotation);
			spawned = true;
		}
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpawnPoint"))
        {
            ///TODO add logic here on how to handle two room exits overlapping
            ///Should only handle cases where an opening occurs at the edge of a generated map and ignore generation for centralized rooms
            spawned = true;
        }
    }
}
