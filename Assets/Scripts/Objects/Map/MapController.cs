using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MapController : InteractObject
{
    [NaughtyAttributes.HorizontalLine]

    [SerializeField] private int progress;
    [SerializeField] List<ObjectiveContainer> objectives;


    private void Update()
    {
        if (progress >= objectives.Count)
            progress = objectives.Count - 1;

        //Hide all objectives
        for (int i = 0; i < objectives.Count; i++)
        {
            objectives[i].HideObjectives();
        }

        //Toggle all MapObjectives to only display the current progress level
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

    public void HideObjectives()
    {
        foreach (MapObjective obj in objectiveObjs)
        {
            obj.gameObject.SetActive(false);
        }
    }
}
