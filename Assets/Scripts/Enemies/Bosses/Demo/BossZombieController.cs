using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static UnityEngine.GraphicsBuffer;

public class BossZombieController : MonoBehaviour
{
    [NaughtyAttributes.HorizontalLine]
    [Header("Boss State Variables")]
    [SerializeField] private float seeDist;
    private float countDownTime = 15f;
    private int towerNum = 0;
    [SerializeField] private int bossStage = 1;
    bool settingUp, isDead;
    public enum BossState { idle, setup, aggro, hurt, dead }
    [SerializeField] BossState bossState;
    Health health;

    [NaughtyAttributes.HorizontalLine]
    [Header("Boss Prefab Variables")]
    [SerializeField] Animator avatarBody;
    [SerializeField] Animator tulpaBody;
    [SerializeField] Transform camTarget;
    [SerializeField] BossRadioTower[] radioTowers;
    [SerializeField] GameObject handLeft, handRight;
    SaveObject saveObj;


    [Header("Event Triggers")]
    [FormerlySerializedAs("onTrigger")]
    public UnityEvent m_OnTrigger = new UnityEvent();


    private void Start()
    {
        health = GetComponent<Health>();
        saveObj = GetComponent<SaveObject>();

        //base.Start();
    }

    private void Update()
    {
        if (bossState != BossState.dead && bossState != BossState.idle) { PlayerController.instance.SeePlayer(); }
        tulpaBody.gameObject.SetActive(bossState != BossState.idle && !saveObj.hasActivated);


        if (saveObj.active && !saveObj.hasActivated)
        {
            switch (bossState)
            {
                case BossState.idle:
                    float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
                    if (distanceToPlayer <= seeDist)
                    {
                        SetState(BossState.setup);
                    }
                    break;
                case BossState.setup:
                    if (!settingUp)
                    {
                        settingUp = true;
                        StartCoroutine(Setup());
                    }
                    break;
                case BossState.aggro:
                    //Attacking phase
                    break;
                case BossState.hurt:
                    CameraController.instance.SetTarget(camTarget);

                    avatarBody.SetTrigger("isHurt");
                    tulpaBody.SetTrigger("isHurt");
                    bossStage++;

                    SetState(BossState.setup);
                    break;
                case BossState.dead:
                    if (!isDead)
                    {
                        isDead = true;
                        StartCoroutine(Death());
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void SetState(BossState stateToSet)
    {
        bossState = stateToSet;
    }

    public void SetTower()
    {
        towerNum++;
        if (towerNum >= bossStage)
        {
            towerNum = 0;
            handLeft.SetActive(false);
            handRight.SetActive(false);

            StartCoroutine(SetTowerStun());
        }
    }

    IEnumerator SetTowerStun()
    {
        PlayerController.instance.SetState(PlayerController.States.listening);
        yield return new WaitForSeconds(0.5f);

        CameraController.instance.SetTarget(camTarget);
        health.Hurt(1, true);

        if (health.currentHealth <= 0)
        {
            SetState(BossState.dead);
        }
        else
        {
            SetState(BossState.hurt);

            yield return new WaitForSeconds(3.5f);

            PlayerController.instance.SetState(PlayerController.States.idle);
            //CameraController.instance.SetTarget(PlayerController.instance.lookTransform);
        }
    }

    IEnumerator Setup()
    {
        PlayerController.instance.SetState(PlayerController.States.listening);

        foreach (BossRadioTower tower in radioTowers)
        {
            tower.DeActivate();
        }
        CameraController.instance.SetTarget(camTarget);

        yield return new WaitForSeconds(3f);

        for (int i = 0; i < bossStage; i++)
        {
            int randVal = Random.Range(0, radioTowers.Length);
            if (!radioTowers[randVal].GetActiveState())
            {
                radioTowers[randVal].Activate();
                CameraController.instance.SetTarget(radioTowers[randVal].transform);

                yield return new WaitForSeconds(1.75f);
            }
            else { i--; }
        }

        CameraController.instance.SetTarget(PlayerController.instance.lookTransform);

        yield return new WaitForSeconds(0.5f);

        handLeft.SetActive(true);
        SetState(BossState.aggro);
        settingUp = false;

        PlayerController.instance.SetState(PlayerController.States.idle);
    }

    IEnumerator Death()
    {
        handLeft.SetActive(false);
        handRight.SetActive(false);
        PlayerController.instance.SetState(PlayerController.States.listening);
        CameraController.instance.SetTarget(camTarget);

        avatarBody.SetTrigger("isDead");
        tulpaBody.SetTrigger("isDead");

        yield return new WaitForSeconds(10f);

        foreach (BossRadioTower tower in radioTowers)
        {
            tower.DeActivate();
        }

        m_OnTrigger.Invoke();
        saveObj.SetHasActivated();

        PlayerController.instance.SetState(PlayerController.States.idle);
        CameraController.instance.SetTarget(PlayerController.instance.lookTransform);
    }
}
