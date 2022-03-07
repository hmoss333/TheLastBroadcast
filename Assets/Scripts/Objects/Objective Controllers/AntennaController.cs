using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntennaController : InteractObject
{
    public static AntennaController instance;

    [SerializeField] string targetScenario;
    ScenarioObjective currentScenario;
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
    }

    public override void Interact()
    {
        if (active && !hasActivated)
        {
            //base.Interact();
            //TODO add logic here for configuring antenna object

            for (int i = 0; i < currentScenario.objectives.Count; i++)
            {
                if (currentScenario.objectives[i].name == "antenna")
                {
                    targetValue = currentScenario.objectives[i].value;
                    break;
                }
            }

            if (targetValue != 0.0f)
            {
                hasActivated = true;
                SceneInitController.instance.SaveInteractObjs();
                SaveDataController.instance.SaveFile();
            }
        }
    }
}
