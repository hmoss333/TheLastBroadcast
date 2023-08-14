using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(InteractObject))]
public class CollectablePickupEffectController : MonoBehaviour
{
    InteractObject baseInteractObj;
    [SerializeField] GameObject effectPrefab;


    // Start is called before the first frame update
    void Start()
    {
        baseInteractObj = GetComponent<InteractObject>();
        ToggleEffect();
    }

    // Update is called once per frame
    void Update()
    {
        ToggleEffect();
    }

    void ToggleEffect()
    {
        effectPrefab.SetActive(baseInteractObj.active && !baseInteractObj.hasActivated ? true : false);
    }
}
