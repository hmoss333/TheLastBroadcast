using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVController : InteractObject
{
    [SerializeField] Material tvMat;
    [SerializeField] Shader defaultShader;
    [SerializeField] Shader staticShader;
    [SerializeField] GameObject tvLight;
    [SerializeField] GameObject focusPoint;
    [SerializeField] AudioSource staticSource;
    [SerializeField] float audioDist;


    private void Start()
    {
        //tvRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        Vector3 playerPos = PlayerController.instance.gameObject.transform.position; //Get current player position
        staticSource.volume = (audioDist - Vector3.Distance(transform.position, playerPos))/audioDist; //scale volume based on how close the player is to the TV
        if (Vector3.Distance(transform.position, playerPos) < audioDist)
            staticSource.mute = !activated; //if TV is activated, unmute audio when in range
        else
            staticSource.mute = true; //If TV is not active, keep audio muted

        tvMat.shader = activated ? staticShader : defaultShader;
        tvLight.SetActive(activated);
    }

    public override void Interact()
    {
        if (activated)
        {
            base.Interact();

            //CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
            //CameraController.instance.FocusTarget();
            //PlayerController.instance.ToggleAvatar();

            if (interacting)
                SaveGame();
        }
    }

    public override void Activate()
    {
        base.Activate();       
    }

    void SaveGame()
    {
        Debug.Log("Add save logic here");
    }
}
