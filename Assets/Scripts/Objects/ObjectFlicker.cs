using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectFlicker : MonoBehaviour
{
    [SerializeField] float duration;
    Renderer renderer;

    public UnityEvent m_OnTrigger = new UnityEvent();


    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    public void StartFlicker()
    {
        StartCoroutine(Blink(duration));
    }
 
    IEnumerator Blink(float waitTime)
    {
        float endTime = Time.time + waitTime;
        while (Time.time < endTime)
        {
            renderer.enabled = false;
            yield return new WaitForSeconds(Random.Range(0.0f, 0.1f));
            renderer.enabled = true;
            yield return new WaitForSeconds(Random.Range(0.0f, 0.1f));
        }

        renderer.enabled = false; //hide object
        m_OnTrigger.Invoke(); //call any event triggers once flicker has completed
    }
}
