using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberPadController : MonoBehaviour
{
    public static NumberPadController instance;

    string tempCode;
    [SerializeField] GameObject numPadObj;
    [SerializeField] Button[] numberButtons;
    int numberVal;
    public Button currentButton;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void OnEnable()
    {
        numberVal = 0;
        currentButton = numberButtons[numberVal];
        currentButton.transform.Find("Highlight").gameObject.SetActive(true);
    }


    private void Update()
    {
        if (numPadObj.activeSelf && InputController.instance.inputMaster.Player.Move.triggered)
        {
            //Refresh all button highlights
            foreach (Button button in numberButtons)
            {
                button.transform.Find("Highlight").gameObject.SetActive(false);
            }

            //Read inputs and update current button value
            Vector2 inputVal = InputController.instance.inputMaster.Player.Move.ReadValue<Vector2>();
            if (inputVal.x > 0) { numberVal += 1; }
            else if (inputVal.x < 0) { numberVal -= 1; }
            else if (inputVal.y > 0) { numberVal -= 3; }
            else if (inputVal.y < 0) { numberVal += 3; }

            //Lock currentButton values to always stay within the button array
            if (numberVal < 0) { numberVal = 0; }
            if (numberVal > numberButtons.Length - 1) { numberVal = numberButtons.Length - 1; }

            //Set current button to the associated numberVal and toggle the highlight
            currentButton = numberButtons[numberVal];
            currentButton.transform.Find("Highlight").gameObject.SetActive(true);
        }
    }


    public void ToggleNumPad(bool numPadState)
    {
        numPadObj.SetActive(numPadState);
    }

    public void ButtonClick(int number)
    {
        tempCode += number;
    }

    public string GetCurrentCode()
    {
        return tempCode;
    }

    public void ClearCurrentCode()
    {
        tempCode = "";
    }
}
