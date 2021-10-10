using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : InteractObject
{
    [SerializeField] bool interacted;
    [SerializeField] List<GameObject> objectsToTrigger;
    [SerializeField] float delayTime;

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
        if (!interacted)
        {
            base.Interact();

            interacted = true;

            StartCoroutine(TriggerEvent());
        }
    }

    IEnumerator TriggerEvent()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < objectsToTrigger.Count; i++)
        {
            CameraController.instance.SetTarget(objectsToTrigger[i]);

            objectsToTrigger[i].SetActive(!objectsToTrigger[i].activeSelf);

            yield return new WaitForSeconds(delayTime);
        }

        //yield return new WaitForSeconds(delayTime);

        CameraController.instance.SetTarget(PlayerController.instance.gameObject);
        PlayerController.instance.interacting = false;
    }
}
