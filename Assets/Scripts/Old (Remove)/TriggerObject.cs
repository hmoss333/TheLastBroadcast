using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This class has been depreciated in favor of the ButtonController
//Please refrain from using this script in the future unless re-worked into a trigger field
 
public class TriggerObject : InteractObject
{
    [SerializeField] bool interacted;
    [SerializeField] List<GameObject> objectsToTrigger;
    //[SerializeField] float delayTime;


    [SerializeField] GameObject buttonObj;
    [SerializeField] Material buttonMat;

    // Start is called before the first frame update
    void Start()
    {
        if (buttonObj)
            buttonMat = buttonObj.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonMat)
            buttonMat.color = interacted ? Color.green : Color.red;
    }

    public override void Interact()
    {
        if (!interacted)
        {
            base.Interact();

            interacted = true;
            //hasActivated = true;
            StartCoroutine(TriggerEvent());
        }
    }

    IEnumerator TriggerEvent()
    {
        //if (triggerText != "")
        //{
        //    UIController.instance.SetDialogueText(triggerText);
        //    UIController.instance.ToggleDialogueUI(true);
        //}

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < objectsToTrigger.Count; i++)
        {
            try
            {
                objectsToTrigger[i].GetComponent<InteractObject>().active = true;
            }
            catch
            {
                Debug.Log("Object " + objectsToTrigger[i] + " does not have an attached InteractObject script");
            }

            if (objectsToTrigger[i].transform.parent.gameObject.activeSelf)
            {
                CameraController.instance.SetTarget(objectsToTrigger[i]);
            }

            //yield return new WaitForSeconds(delayTime);
        }

        CameraController.instance.SetTarget(PlayerController.instance.gameObject);
        PlayerController.instance.state = PlayerController.States.idle;
        //UIController.instance.ToggleDialogueUI(false);
        SetHasActivated();
    }
}
