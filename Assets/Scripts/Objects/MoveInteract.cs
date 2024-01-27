using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MoveInteract : InteractObject
{
    [NaughtyAttributes.HorizontalLine]

    [SerializeField] Vector3[] positions;
    [SerializeField] float moveSpeed;
    [SerializeField] string triggerText;
    Coroutine moveObjRout;
    AudioSource audioSource;
    [SerializeField] AudioClip audioClip;



    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
    }

    public override void InitializeObject()
    {
        base.InitializeObject();

        if (hasActivated)
            transform.localPosition = positions[positions.Length - 1];
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

    public void MoveObj(int index)
    {
        if (moveObjRout == null)
            moveObjRout = StartCoroutine(MoveObjIndexRoutine(index));
    }

    IEnumerator MoveObjRoutine()
    {
        SetHasActivated();
        m_OnTrigger.Invoke();
        audioSource.Play();

        for (int i = 0; i < positions.Length; i++)
        {
            while(true)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, positions[i], moveSpeed * Time.deltaTime);

                float dist = Vector3.Distance(transform.localPosition, positions[i]);
                if (dist <= 0.25)
                    break;

                yield return null;
            }
        }

        moveObjRout = null;
    }

    IEnumerator MoveObjIndexRoutine(int index)
    {
        SetHasActivated();
        m_OnTrigger.Invoke();
        audioSource.Play();

        while (true)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, positions[index], moveSpeed * Time.deltaTime);

            float dist = Vector3.Distance(transform.localPosition, positions[index]);
            if (dist <= 0.25)
                break;

            yield return null;
        }

        moveObjRout = null;
    }
}
