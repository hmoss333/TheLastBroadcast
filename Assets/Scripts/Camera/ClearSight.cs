using UnityEngine;
using System.Collections;

public class ClearSight : MonoBehaviour
{
    [SerializeField] LayerMask layerToFade;


    void Update()
    {
        RaycastHit[] hits;
        Transform target = PlayerController.instance.abilityState == PlayerController.AbilityStates.isRat
            ? RatController.instance.transform
            : PlayerController.instance.transform;
        Vector3 forwardDir = target.position - transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        hits = Physics.RaycastAll(transform.position, forwardDir, distanceToPlayer, layerToFade);
        Debug.DrawRay(transform.position, forwardDir);


        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.tag != "SavePoint") //Need a more elegant solution for avoiding the TV
            {
                Renderer R = hit.collider.GetComponent<Renderer>();

                if (R == null || CameraController.instance.GetFocusState())
                    continue; // no renderer attached? go to next hit
                              // TODO: maybe implement here a check for GOs that should not be affected like the player

                AutoTransparent AT = R.GetComponent<AutoTransparent>();
                if (AT == null) // if no script is attached, attach one
                {
                    print($"Adding autotransparent from {this.name}");
                    AT = R.gameObject.AddComponent<AutoTransparent>();
                }
                AT.BeTransparent(); // get called every frame to reset the falloff
            }
        }
    }
}
