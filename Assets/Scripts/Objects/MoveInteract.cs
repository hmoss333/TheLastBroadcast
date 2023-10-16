using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInteract : InteractObject
{
    [SerializeField] Vector3 defaultPos, activatedPos;
    [SerializeField] float moveSpeed;
    Coroutine moveObjRout;

    private void Start()
    {
        defaultPos = transform.localPosition;

        if (hasActivated)
            transform.localPosition = activatedPos;
        else
            transform.localPosition = defaultPos;
    }

    public override void EndInteract()
    {
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

        while (transform.localPosition != activatedPos)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, activatedPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        moveObjRout = null;
    }
}
