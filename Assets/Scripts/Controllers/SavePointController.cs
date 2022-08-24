using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class SavePointController : InteractObject
{
    public int ID;
    public Transform initPoint;
    [SerializeField] GameObject focusPoint;
    //[SerializeField] TextMeshPro saveText;
    //[SerializeField] AudioSource staticSource;
    //[SerializeField] float audioDist;
    [SerializeField] Material tvMat;
    MeshRenderer renderer;


    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();

        if (active)
            AddMaterial();
    }

    //private void Update()
    //{
    //    //Vector3 playerPos = PlayerController.instance.gameObject.transform.position; //Get current player position
    //    //staticSource.volume = (audioDist - Vector3.Distance(transform.position, playerPos)) / audioDist; //scale volume based on how close the player is to the TV
    //    //staticSource.mute = Vector3.Distance(transform.position, playerPos) < audioDist ? false : true; //play or mute audio based on distance
    //}

    void AddMaterial()
    {
        var materials = renderer.sharedMaterials.ToList();
        foreach (Material mat in materials)
        {
            if (mat == tvMat)
                return;
        }

        materials.Add(tvMat);

        renderer.materials = materials.ToArray();
    }

    public override void Activate()
    {
        base.Activate();

        AddMaterial();
    }

    public override void Interact()
    {
        if (active)
        {
            base.Interact();

            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
            CameraController.instance.FocusTarget();


            if (interacting)
            {
                Debug.Log("Saving game");
                SaveDataController.instance.SetSavePoint(SceneManager.GetActiveScene().name, ID);
                SaveDataController.instance.SaveObjectData(SceneManager.GetActiveScene().name);
                SaveDataController.instance.SaveFile();
            }
        }
    }
}
