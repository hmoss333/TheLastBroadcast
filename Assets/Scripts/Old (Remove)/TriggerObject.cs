using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : InteractObject
{
    [SerializeField] bool interacted, hide;
    [SerializeField] List<GameObject> objectsToTrigger;
    [SerializeField] string triggerText;
    [SerializeField] float delayTime;


    [SerializeField] GameObject buttonObj;
    [SerializeField] Material buttonMat;

    // Start is called before the first frame update
    void Start()
    {
        if (buttonObj)
            buttonMat = buttonObj.GetComponent<MeshRenderer>().material;

        if (hide)
            gameObject.SetActive(hasActivated);
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
            hasActivated = true;
            StartCoroutine(TriggerEvent());
        }
    }

    IEnumerator TriggerEvent()
    {
        if (triggerText != "")
        {
            UIController.instance.DialogueUI(triggerText);//, 3f);
        }

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

            yield return new WaitForSeconds(delayTime);
        }

        CameraController.instance.SetTarget(PlayerController.instance.gameObject);
        PlayerController.instance.state = PlayerController.States.idle;
        UIController.instance.FadeUI(0f);

        if (hide)
            gameObject.SetActive(false);
    }
}
