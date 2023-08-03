using UnityEngine;
using System.Collections;

public class ClearSight : MonoBehaviour
{
    [SerializeField] LayerMask layerToFade;


    void Update()
    {
        RaycastHit[] hits;
        Vector3 forwardDir = PlayerController.instance.transform.position - transform.position; //CameraController.instance.GetTarget().position - transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.instance.transform.position); //CameraController.instance.GetTarget().position);
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
                    AT = R.gameObject.AddComponent<AutoTransparent>();
                }
                AT.BeTransparent(); // get called every frame to reset the falloff
            }
        }
    }
}
