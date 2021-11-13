using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public DoorController[] doors;
    public enum Special { start, generator, antenna, exit, none };
    public Special special;
}
