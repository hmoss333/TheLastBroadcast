using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class MapObjective : MonoBehaviour
{
    public bool completed { get; private set; }
    [SerializeField] Color defaultColor = Color.red, completedColor = Color.black;
    SpriteRenderer objSprite;

    [SerializeField] SaveObject objective; //used to track a specific object's interact state to determine if it has been completed


    private void Start()
    {
        objSprite = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        objSprite.color = completed ? completedColor : defaultColor;

        completed = objective.hasActivated;
    }
}