using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : InteractObject
{
    [SerializeField] bool interacted;
    [SerializeField] List<GameObject> objectsToTrigger;
    [SerializeField] float delayTime;


    [SerializeField] GameObject buttonObj;
    [SerializeField] Material buttonMat;

    // Start is called before the first frame update
    void Start()
    {
        buttonMat = buttonObj.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        buttonMat.color = interacted ? Color.green : Color.red;
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
            try
            {
                Debug.Log("Put trigger activation here");
                objectsToTrigger[i].GetComponent<InteractObject>().activated = true; //testing for now
            }
            catch
            {
                Debug.Log("Object " + objectsToTrigger[i] + " does not have an attached InteractObject script");
            }

            if (objectsToTrigger[i].transform.parent.gameObject.activeSelf)
            {
                CameraController.instance.SetTarget(objectsToTrigger[i]);

                yield return new WaitForSeconds(delayTime);
            }
        }

        //yield return new WaitForSeconds(delayTime);

        CameraController.instance.SetTarget(PlayerController.instance.gameObject);
        PlayerController.instance.interacting = false;
    }
}
