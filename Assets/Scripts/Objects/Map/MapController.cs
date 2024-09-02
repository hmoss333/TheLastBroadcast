using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : InteractObject
{
    [SerializeField] List<ObjectiveContainer> objectives;
    [SerializeField] private int progress;


    private void Update()
    {
        if (progress > objectives.Count)
            progress = objectives.Count;

        //Hide all objectives
        for (int i = 0; i < objectives.Count; i++)
        {
            objectives[i].HideObjectives();
        }

        //Toggle all MabObjectives to only display the current progress level
        for (int j = 0; j < objectives[progress].objectiveObjs.Count; j++)
        {
            objectives[progress].objectiveObjs[j].gameObject.SetActive(true);
        }
    }

    public void IncrementProgress()
    {
        progress++;
    }
}


[Serializable]
public class ObjectiveContainer
{
    public List<MapObjective> objectiveObjs; //Container for multiple MapObjective instances
    public bool completed;

    public void HideObjectives()
    {
        foreach (MapObjective obj in objectiveObjs)
        {
            obj.gameObject.SetActive(false);
        }
    }
}
