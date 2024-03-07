using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class InputController : MonoBehaviour
{
    public static InputController instance;

    public InputMaster inputMaster { get; private set; }
    PlayerInput input;
    public string currentInputDevice { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        inputMaster = new InputMaster();
        inputMaster.Enable();

        input = FindObjectOfType<PlayerInput>();
    }

    private void Update()
    {
        UpdateCurrentInputDevice(input.currentControlScheme);
    }

    public void UpdateCurrentInputDevice(string deviceName)
    {
        if (currentInputDevice != deviceName)
        {
            print($"Current Input Device: {deviceName}");
            currentInputDevice = deviceName;
        }
    }
}
