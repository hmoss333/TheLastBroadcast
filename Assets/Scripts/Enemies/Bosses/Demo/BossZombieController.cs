using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class BossZombieController : SaveObject
{
    [Header("Boss State Variables")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] private int bossStage = 1;
    private float countDownTime = 15f;
    private int towerNum = 0;
    bool settingUp, isDead;
    public enum BossState { idle, setup, aggro, hurt, dead }
    [SerializeField] BossState bossState;
    Health health;

    [Header("Boss Object References")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] Animator avatarBody;
    [SerializeField] Animator tulpaBody;
    bool attackLeft;
    [SerializeField] GameObject handLeft, handRight;
    [SerializeField] Transform camTarget;
    [SerializeField] BossRadioTower[] radioTowers;

    [Header("Boss UI References")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] GameObject healthBarObj;
    [SerializeField] Image healthBar;


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
        healthBarObj.SetActive(active && !hasActivated);
        //healthBar.fillAmount = health.currentHealth / 4f;


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

        if (bossState != BossState.aggro)
        {
            handLeft.SetActive(false);
            handRight.SetActive(false);
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

        CameraController.instance.SetRotation(false);
        CameraController.instance.SetTarget(camTarget);

        float targetFillAmount = health.currentHealth / 4f;
        while (healthBar.fillAmount > targetFillAmount)
        {
            healthBar.fillAmount -= Time.deltaTime * 0.25f;
            yield return null;
        }


        //Check current health to determine state
        if (health.currentHealth <= 0)
        {
            SetState(BossState.dead);
        }
        else
        {
            SetState(BossState.hurt);

            CamEffectController.instance.ForceEffect();
            yield return new WaitForSeconds(1.75f);
            CamEffectController.instance.SetEffectState(false);

            yield return new WaitForSeconds(2.25f);
           
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
        CameraController.instance.SetRotation(false);
        CameraController.instance.SetTarget(camTarget);

        yield return new WaitForSeconds(3.0f);

        for (int i = 0; i < bossStage; i++)
        {
            int randVal = Random.Range(0, radioTowers.Length);
            if (!radioTowers[randVal].GetActiveState())
            {
                radioTowers[randVal].Activate();
                CameraController.instance.SetTarget(radioTowers[randVal].focusPoint);//.transform);

                yield return new WaitForSeconds(1.75f);
            }
            else { i--; }
        }


        if (CameraController.instance.GetTriggerState())
        {
            CameraController.instance.SetRotation(true);
            CameraController.instance.LoadLastTarget();
        }
        else
        {
            CameraController.instance.SetTarget(PlayerController.instance.lookTransform);
        }

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
