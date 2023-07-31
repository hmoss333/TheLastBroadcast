using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAnimController : MonoBehaviour
{
    [SerializeField] Collider col;

    // Start is called before the first frame update
    void Start()
    {
        col.isTrigger = true;
    }

    public void ToggleMeleeCollider()
    {
        col.enabled = !col.enabled;
    }
}
