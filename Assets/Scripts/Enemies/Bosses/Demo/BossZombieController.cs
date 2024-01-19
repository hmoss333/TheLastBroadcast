using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static UnityEngine.GraphicsBuffer;

public class BossZombieController : SaveObject
{
    [NaughtyAttributes.HorizontalLine]
    [Header("Boss State Variables")]
    [SerializeField] private int bossStage = 1;
    private float countDownTime = 15f;
    private int towerNum = 0;
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
    bool attackLeft;
    [SerializeField] GameObject handLeft, handRight;


    private void Start()
    {
        health = GetComponent<Health>();
        if (hasActivated)
        {
            avatarBody.SetTrigger("isDead");
        }
    }

    private void Update()
    {
        if (bossState != BossState.dead && bossState != BossState.idle) { PlayerController.instance.SeePlayer(); }
        tulpaBody.gameObject.SetActive(bossState != BossState.idle && !hasActivated);


        if (active && !hasActivated)
        {
            switch (bossState)
            {
                case BossState.idle:
                    float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
                    SetState(BossState.setup);
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
                    if (!handLeft.activeSelf && !handRight.activeSelf)
                    {
                        attackLeft = !attackLeft;

                        if (attackLeft)
                        {
                            handLeft.SetActive(true);
                            if (!isPlaying(tulpaBody, "Attack_Left"))
                                tulpaBody.SetTrigger("attack_L");
                        }
                        else
                        {
                            handRight.SetActive(true);
                            if (!isPlaying(tulpaBody, "Attack_Right"))
                                tulpaBody.SetTrigger("attack_R");
                        }
                    }
                    break;
                case BossState.hurt:
                    CameraController.instance.SetTarget(camTarget);
                    CamEffectController.instance.ForceEffect();

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
            //handLeft.SetActive(false);
            //handRight.SetActive(false);
            health.Hurt(1, true);

            StartCoroutine(SetTowerStun());
        }
    }

    IEnumerator SetTowerStun()
    {
        PlayerController.instance.SetState(PlayerController.States.listening);
        yield return new WaitForSeconds(0.5f);

        handLeft.SetActive(false);
        handRight.SetActive(false);

        CameraController.instance.SetTarget(camTarget);

        if (health.currentHealth <= 0)
        {
            SetState(BossState.dead);
        }
        else
        {
            SetState(BossState.hurt);

            yield return new WaitForSeconds(4.5f);

            PlayerController.instance.SetState(PlayerController.States.idle);
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

        yield return new WaitForSeconds(0.75f);

        CamEffectController.instance.SetEffectState(false);

        yield return new WaitForSeconds(2.25f);

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

        yield return new WaitForSeconds(8f);

        foreach (BossRadioTower tower in radioTowers)
        {
            tower.DeActivate();
        }

        m_OnTrigger.Invoke();
        SetHasActivated();

        PlayerController.instance.SetState(PlayerController.States.idle);
        CameraController.instance.SetTarget(PlayerController.instance.lookTransform);
    }

    public bool isPlaying(Animator animator, string stateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            return true;
        else
            return false;
    }
}
