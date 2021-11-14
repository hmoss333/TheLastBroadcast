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

	public float waitTime = 4f;


    private void Awake()
    {
		templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
	}

    void Start(){
		Destroy(gameObject, waitTime);
		//templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
		Invoke("Spawn", 0.1f);
	}


	void Spawn(){
		if(spawned == false)
		{
			if (templates.dungeonDepth >= 0)
			{
				if (openingDirection == 1)
				{
					// Need to spawn a room with a BOTTOM door.
					rand = Random.Range(0, templates.bottomRooms.Length);
					Instantiate(templates.bottomRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation);
				}
				else if (openingDirection == 2)
				{
					// Need to spawn a room with a TOP door.
					rand = Random.Range(0, templates.topRooms.Length);
					Instantiate(templates.topRooms[rand], transform.position, templates.topRooms[rand].transform.rotation);
				}
				else if (openingDirection == 3)
				{
					// Need to spawn a room with a LEFT door.
					rand = Random.Range(0, templates.leftRooms.Length);
					Instantiate(templates.leftRooms[rand], transform.position, templates.leftRooms[rand].transform.rotation);
				}
				else if (openingDirection == 4)
				{
					// Need to spawn a room with a RIGHT door.
					rand = Random.Range(0, templates.rightRooms.Length);
					Instantiate(templates.rightRooms[rand], transform.position, templates.rightRooms[rand].transform.rotation);
				}
			}
			else
            {
				if (openingDirection == 1)
				{
					// Need to spawn a room with a BOTTOM door.
					Instantiate(templates.bottomRooms[0], transform.position, templates.bottomRooms[0].transform.rotation);
				}
				else if (openingDirection == 2)
				{
					// Need to spawn a room with a TOP door.
					Instantiate(templates.topRooms[0], transform.position, templates.topRooms[0].transform.rotation);
				}
				else if (openingDirection == 3)
				{
					// Need to spawn a room with a LEFT door.
					Instantiate(templates.leftRooms[0], transform.position, templates.leftRooms[0].transform.rotation);
				}
				else if (openingDirection == 4)
				{
					// Need to spawn a room with a RIGHT door.
					Instantiate(templates.rightRooms[0], transform.position, templates.rightRooms[0].transform.rotation);
				}
			}
			spawned = true;
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.CompareTag("SpawnPoint")){
            //if (other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
            //{
            //    Instantiate(templates.closedRoom, transform.position, templates.closedRoom.transform.rotation);
            //    Destroy(gameObject);
            //}
            spawned = true;
		}
	}
}
