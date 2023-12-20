using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//Any flicker object should be set as a child of the controller object so that it can be toggled without interrupting the Blink coroutine
public class ObjectFlicker : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] GameObject objToFlicker;

    public UnityEvent m_OnTrigger = new UnityEvent();


    public void StartFlicker()
    {
        StartCoroutine(Blink(duration));
    }
 
    IEnumerator Blink(float waitTime)
    {
        float endTime = Time.time + waitTime;
        while (Time.time < endTime)
        {
            objToFlicker.SetActive(false);
            yield return new WaitForSeconds(Random.Range(0.0f, 0.1f));
            objToFlicker.SetActive(true);
            yield return new WaitForSeconds(Random.Range(0.0f, 0.1f));
        }

        objToFlicker.SetActive(false);
        m_OnTrigger.Invoke(); //call any event triggers once flicker has completed
    }
}
