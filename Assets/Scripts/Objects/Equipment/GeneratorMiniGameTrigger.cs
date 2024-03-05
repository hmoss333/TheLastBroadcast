using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorMiniGameTrigger : MonoBehaviour
{
    [SerializeField] GeneratorController generator;
    bool inTrigger, hasHit;

    private void Update()
    {
        if (inTrigger && !hasHit && InputController.instance.inputMaster.Player.Interact.triggered)
        {
            hasHit = true;
            generator.Hit();
        }
        else if (InputController.instance.inputMaster.Player.Interact.triggered)
        {
            generator.Miss();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "MiniGameCol")
        {
            inTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "MiniGameCol")
        {          
            inTrigger = false;
            hasHit = false;
        }
    }
}
