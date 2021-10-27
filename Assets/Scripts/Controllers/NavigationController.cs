using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationController : MonoBehaviour
{
    public static NavigationController instance;

    public RoomController[] rooms;
    public RoomController activeRoom;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        rooms = FindObjectsOfType<RoomController>();

        UpdateRooms();
    }

    public void UpdateRooms()
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            rooms[i].gameObject.SetActive(rooms[i] == activeRoom ? true : false);
        }
    }

    public void SetActiveRoom(RoomController roomObj)
    {
        activeRoom = roomObj;
    }
}
