using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class NumberPadInteractController : InteractObject
{
    [NaughtyAttributes.HorizontalLine]

    [Header("Code Variables")]
    [SerializeField] private string code;
    private string inputCode = "";
    [SerializeField] private bool unlocked;

    [NaughtyAttributes.HorizontalLine]

    [Header("Audio Elements")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip selectClip, correctClip, incorrectClip;

    Coroutine checkCodeRoutine;


    private void Update()
    {
        if (interacting && !unlocked)
        {
            if (InputController.instance.inputMaster.Player.Run.triggered)
            {
                interacting = false;
                NumberPadController.instance.ToggleNumPad(interacting);
                PlayerController.instance.SetState(PlayerController.States.idle);
                if (CameraController.instance.GetTriggerState())
                    CameraController.instance.SetRotation(true);
            }
        }
    }

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
                //Play select clip
                PlayClip(selectClip);
                if (inputCode.Length < 4) { NumberPadController.instance.currentButton.onClick.Invoke(); } //Limit inputs to only 4 numbers
                inputCode = NumberPadController.instance.GetCurrentCode(); //Get latest code value from Number Pad UI
                if (inputCode.Length >= 4 && checkCodeRoutine == null) { checkCodeRoutine = StartCoroutine(CheckCode()); } //Once 4 numbers have been input, perform a check and clear inputCode
            }
        }
    }

    private void PlayClip(AudioClip clip)
    {
        try
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
        catch
        {
            Debug.Log("Missing audioSource or audioClip");
        }
    }

    IEnumerator CheckCode()
    {
        yield return new WaitForSeconds(0.75f);

        //Code is correct
        if (inputCode == code)
        {
            unlocked = true;
            hasActivated = true;
            interacting = false;
            NumberPadController.instance.ToggleNumPad(interacting);
            PlayerController.instance.SetState(PlayerController.States.idle);
            if (CameraController.instance.GetTriggerState())
                CameraController.instance.SetRotation(true);

            m_OnTrigger.Invoke();
            PlayClip(correctClip);
        }
        //Code is incorrect
        else
        {
            inputCode = "";
            PlayClip(incorrectClip);
        }

        NumberPadController.instance.ClearCurrentCode();
        checkCodeRoutine = null;
    }
}
