using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClampUIElement : MonoBehaviour
{
    [SerializeField] Transform target;

    // Update is called once per frame
    void Update()
    {
        Vector3 uiPos = Camera.main.WorldToScreenPoint(this.transform.position);
        target.position = uiPos;
    }
}
