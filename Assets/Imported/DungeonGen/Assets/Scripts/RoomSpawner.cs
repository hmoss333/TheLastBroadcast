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
	public bool spawned = false;
	private GameObject spawnedRoom;


    private void Awake()
    {
		templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
	}

    void Start(){
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
						? templates.bottomRooms[Random.Range(1, templates.bottomRooms.Length)]
						: templates.bottomRooms[0];
					break;
				case 2:
					spawnedRoom = templates.dungeonDepth > 0
						? templates.topRooms[Random.Range(1, templates.topRooms.Length)]
						: templates.topRooms[0];
					break;
				case 3:
					spawnedRoom = templates.dungeonDepth > 0
						? templates.leftRooms[Random.Range(1, templates.leftRooms.Length)]
						: templates.leftRooms[0];
					break;
				case 4:
					spawnedRoom = templates.dungeonDepth > 0
						? templates.rightRooms[Random.Range(1, templates.rightRooms.Length)]
						: templates.rightRooms[0];
					break;
				default:
					spawnedRoom = null;
					Debug.Log("OpeningDirection not found: " + openingDirection);
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
			if (other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
			{
				Instantiate(templates.intersectRoom, transform.position, templates.intersectRoom.transform.rotation);
			}
    //        else if (transform.parent.parent.gameObject.tag == "Intersect Room")
    //        {
				////templates.rooms.Remove(transform.parent.parent.gameObject);
				////Destroy(transform.parent.parent.gameObject);
				//Destroy(gameObject);
    //        }
            spawned = true;
		}
	}
}
