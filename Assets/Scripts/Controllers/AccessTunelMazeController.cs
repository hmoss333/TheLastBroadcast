using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessTunelMazeController : MonoBehaviour
{
    public static AccessTunelMazeController instance;

    [SerializeField] int correctCount;
    [SerializeField] int goalCount;
    [SerializeField] List<AccessTunelMazeExitPoint> exitTriggers;
    [SerializeField] Transform exitInitPoint;
    Coroutine initRoutine;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        correctCount = 0;
        RefreshMaze();
    }

    private void RefreshMaze()
    {
        float randVal = Random.Range((int)0, (int)exitTriggers.Count - 1);
        for (int i = 0; i < exitTriggers.Count; i++)
        {
            exitTriggers[i].Initialize();
            if (i != randVal)
            {
                exitTriggers[i].SetFalse();
            }
        }
    }

    public void Correct(Transform exitPoint)
    {
        correctCount++;
        StartInitRoutine(correctCount < goalCount ? exitPoint : exitInitPoint);
    }

    public void Incorrect(Transform exitPoint)
    {
        correctCount = 0;
        StartInitRoutine(exitPoint);
    }

    void StartInitRoutine(Transform initTransform)
    {
        if (initRoutine == null)
            initRoutine = StartCoroutine(InitRoutine(initTransform));
    }

    IEnumerator InitRoutine(Transform initPoint)
    {
        FadeController.instance.StartFade(1.0f, 1.0f);
        while (FadeController.instance.isFading) { yield return null; }

        PlayerController.instance.SetState(PlayerController.States.listening);

        PlayerController.instance.transform.position = initPoint.position;
        PlayerController.instance.SetLastDir(initPoint.forward);

        RefreshMaze();

        FadeController.instance.StartFade(0.0f, 1.0f);
        while (FadeController.instance.isFading) { yield return null; }

        PlayerController.instance.SetState(PlayerController.States.idle);

        initRoutine = null;
        print("End routine");
    }
}

