using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationController : MonoBehaviour
{
    public static NavigationController instance;

    public List<RoomController> rooms;
    RoomController[] roomOptions;
    public RoomController startRoom;
    public RoomController exitRoom;
    public RoomController currentRoom;
    [SerializeField] int mapDepth;


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
        rooms = new List<RoomController>();

        GenerateRooms();
    }

    void GenerateRooms()
    {
        RoomController newRoom = new RoomController();
        rooms.Add(startRoom);

        for (int i = 0; i < mapDepth; i++)
        {
            if (i == 0)
            {
                newRoom = GetRoom(startRoom.doors[0]);
                //Instantiate room object
                rooms.Add(newRoom);
                //link room doors
            }
            else
            {
                newRoom = GetRoom(rooms[i + 1].doors[1]);
                //link room doors
            }
        }

        //Once mapDepth has been reached, instantiate exit room
        rooms.Add(exitRoom);
    }

    public RoomController GetRoom(DoorController room)
    {
        RoomController returnRoom = new RoomController();

        //switch (room.direction)
        //{
        //    case DoorController.Direction.left:
        //        break;
        //    case DoorController.Direction.right:
        //        break;
        //    case DoorController.Direction.top:
        //        break;
        //    case DoorController.Direction.bottom:
        //        break;
        //    default:
        //        break;
        //}

        return returnRoom;
    }
}
