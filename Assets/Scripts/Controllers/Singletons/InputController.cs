using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController instance;
    public InputMaster inputMaster { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);


        inputMaster = new InputMaster();
        inputMaster.Enable();
    }
}
