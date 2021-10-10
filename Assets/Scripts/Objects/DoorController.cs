using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : InteractObject
{
    [SerializeField] GameObject nextRoom;


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

        //Testing for now
        yield return new WaitForSeconds(2f);

        FadeController.instance.StartFade(0.0f, 1f);
    }
}
