using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInteract : InteractObject
{
    [SerializeField] Vector3 defaultPos, activatedPos;
    [SerializeField] float moveSpeed;
    [SerializeField] string triggerText;
    Coroutine moveObjRout;


    private void Start()
    {
        defaultPos = transform.localPosition;
        //inputCount = 0;

        if (hasActivated)
            transform.localPosition = activatedPos;
        else
            transform.localPosition = defaultPos;
    }

    public override void Interact()
    {
        if (!hasActivated)
            base.Interact();
    }

    public override void StartInteract()
    {
        if (triggerText != string.Empty)
        {
            UIController.instance.SetDialogueText(triggerText, false);
            UIController.instance.ToggleDialogueUI(true);
        }
    }

    public override void EndInteract()
    {
        UIController.instance.ToggleDialogueUI(false);
        MoveObj();
    }

    public void MoveObj()
    {
        if (moveObjRout == null)
            moveObjRout = StartCoroutine(MoveObjRoutine());
    }

    IEnumerator MoveObjRoutine()
    {
        SetHasActivated();
        //PlayerController.instance.animator.SetTrigger("isMovingObj");

        while (transform.localPosition != activatedPos)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, activatedPos, moveSpeed * Time.deltaTime);

            float dist = Vector3.Distance(transform.localPosition, activatedPos);
            if (dist <= 0.25f)
            {
                print(dist);
                break;
            }

            yield return null;
        }

        moveObjRout = null;
    }
}
