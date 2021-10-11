using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : InteractObject
{
    [SerializeField] DoorController targetDoor;
    [SerializeField] Transform exitPoint;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        base.Interact();

        StartCoroutine(DoorTrigger());
    }

    IEnumerator DoorTrigger()
    {
        yield return new WaitForSeconds(0.5f);

        FadeController.instance.StartFade(1.0f, 1f);
        //load next room

        while (FadeController.instance.isFading)
            yield return null;

        if (!targetDoor.transform.parent.gameObject.activeSelf)
            targetDoor.transform.parent.gameObject.SetActive(true);

        PlayerController.instance.gameObject.transform.position = targetDoor.exitPoint.position;

        transform.parent.gameObject.SetActive(false);

        FadeController.instance.StartFade(0.0f, 1f);

        //while (FadeController.instance.isFading)
        //    yield return null;

        PlayerController.instance.interacting = false;
    }
}
