using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberPadInteractController : InteractObject
{
    [SerializeField] string code;
    [SerializeField] string inputCode;
    [SerializeField] bool unlocked;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip selectClip, correctClip, incorrectClip;

    Coroutine checkCodeRoutine;

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
        yield return new WaitForSeconds(1f);

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
