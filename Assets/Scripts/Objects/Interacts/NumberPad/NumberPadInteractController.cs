using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberPadInteractController : InteractObject
{
    [SerializeField] string code;
    [SerializeField] string inputCode;
    [SerializeField] bool unlocked;

    public override void Interact()
    {
        if (!unlocked)
        {
            if (!interacting)
            {
                interacting = true;
                NumberPadController.instance.ToggleNumPad(interacting);
            }
            else
            {
                NumberPadController.instance.currentButton.onClick.Invoke();
                inputCode = NumberPadController.instance.GetCurrentCode();

                if (inputCode.Length >= 4)
                {
                    if (inputCode == code)
                    {
                        unlocked = true;
                        hasActivated = true;
                        interacting = false;
                        NumberPadController.instance.ToggleNumPad(interacting);
                        if (CameraController.instance.GetTriggerState())
                            CameraController.instance.SetRotation(true);

                        m_OnTrigger.Invoke();

                        //play correct audio clip
                    }
                    else
                    {
                        inputCode = "";
                        NumberPadController.instance.ClearCurrentCode();
                        //play incorrect audio clip
                    }
                }
            }
        }
    }
}
