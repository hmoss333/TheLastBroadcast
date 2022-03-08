using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntennaController : InteractObject
{
    public static AntennaController instance;

    public float value;

    [SerializeField] string targetScenario;
    ScenarioObjective currentScenario;
    ObjectiveObj currentObjective;
    [SerializeField] float targetValue;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentScenario = SaveDataController.instance.GetScenario(targetScenario);
        currentObjective = currentScenario.GetObjective("antenna");
    }

    private void Update()
    {
        if (interacting)
        {
            float xInput = Input.GetAxis("Horizontal");
            value += (float)(xInput * Time.deltaTime);
            //value = (float)System.Math.Round(value, 2);

            if ((value >= targetValue - 0.025f && value <= targetValue + 0.025f)
                && targetValue != 0.0f
                && !currentObjective.hasSet)
            {
                currentScenario.UpdateObjective(currentObjective.name);
            }
        }
    }

    public override void Interact()
    {
        if (active)
        {
            targetValue = currentObjective.value;
            base.Interact();
            //TODO add logic here for configuring transmitter object
        }
    }
}
