using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZombieController : CharacterController
{
    [NaughtyAttributes.HorizontalLine]
    [Header("Boss State Variables")]
    [SerializeField] private float seeDist;
    private float countDownTime = 15f;
    [SerializeField] private int bossHitNum = 0;
    [SerializeField] private int towerNum = 0;
    [SerializeField] private int bossStage = 1;
    bool settingUp;
    public enum BossState { idle, setup, aggro, hurt, dead }
    [SerializeField] BossState bossState;
    Health health;
    Collider bossCollider;

    [NaughtyAttributes.HorizontalLine]
    [Header("Boss Prefab Variables")]
    [SerializeField] GameObject shieldObject;
    [SerializeField] BossRadioTower[] radioTowers;
    [SerializeField] ZombieController zombiePrefab;
    [SerializeField] GameObject handLeft, handRight;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] DoorController[] roomDoors;
    SaveObject saveObj;


    private void Start()
    {
        health = GetComponent<Health>();
        bossCollider = GetComponent<Collider>();
        saveObj = GetComponent<SaveObject>();
        if (saveObj.hasActivated) { shieldObject.SetActive(true); }

        base.Start();
    }

    private void Update()
    {
        bossCollider.enabled = bossState != BossState.dead ? !shieldObject.activeSelf : false;
        if (hurt)
        {
            SetState(BossState.hurt);
        }
        if (dead || saveObj.hasActivated)
        {
            dead = true;          
            bossState = BossState.dead;
        }

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
                    //TODO
                    //Fire projectiles at player on set intervals
                    //If player takes too long to deal damage, reset the scenario
                    if (!shieldObject.activeSelf)
                    {
                        countDownTime -= Time.deltaTime;
                        if (countDownTime <= 0f)
                        {
                            countDownTime = 15f;
                            SetState(BossState.setup);
                        }
                    }
                    break;
                case BossState.hurt:
                    if (!isPlaying("Hurt"))
                    {
                        animator.SetTrigger("isHurt");
                        bossHitNum++;                       
                        if (bossHitNum == 3)
                        {
                            bossHitNum = 0;
                            bossStage++;
                            SetState(BossState.setup);
                        }
                        else
                        {
                            SetState(BossState.aggro);
                        }
                    }
                    break;
                case BossState.dead:
                    if (!saveObj.hasActivated)
                    {
                        saveObj.SetHasActivated();
                        StartCoroutine(Death());
                    }
                    break;
                default:
                    break;
            }
        }

        base.Update();
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

        CameraController.instance.SetTarget(this.gameObject);

        yield return new WaitForSeconds(1f);

        shieldObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        PlayerController.instance.SetState(PlayerController.States.idle);
        CameraController.instance.SetTarget(PlayerController.instance.gameObject);
    }

    IEnumerator Setup()
    {
        PlayerController.instance.SetState(PlayerController.States.listening);

        foreach (BossRadioTower tower in radioTowers)
        {
            tower.DeActivate();
        }
        CameraController.instance.SetTarget(this.gameObject);

        yield return new WaitForSeconds(1f);

        shieldObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < bossStage; i++)
        {
            int randVal = Random.Range(0, radioTowers.Length);
            if (!radioTowers[randVal].GetActiveState())
            {
                radioTowers[randVal].Activate();
                CameraController.instance.SetTarget(radioTowers[randVal].gameObject);

                yield return new WaitForSeconds(1f);
            }
            else { i--; }
        }

        List<int> randPos = new List<int>();
        for (int i = 0; i < bossStage - 1; i++)
        {
            int randVal = Random.Range(0, radioTowers.Length);
            if (!randPos.Contains(randVal))
            {
                ZombieController zombie = Instantiate(zombiePrefab, spawnPoints[randVal].position, Quaternion.identity);
                CameraController.instance.SetTarget(zombie.gameObject);
                randPos.Add(randVal);

                yield return new WaitForSeconds(1f);
            }
            else { i--; }
        }

        CameraController.instance.SetTarget(PlayerController.instance.gameObject);

        yield return new WaitForSeconds(0.35f);

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
        CameraController.instance.SetTarget(this.gameObject);

        yield return new WaitForSeconds(1.5f);

        shieldObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        foreach (BossRadioTower tower in radioTowers)
        {
            tower.DeActivate();
        }

        for (int i = 0; i < roomDoors.Length; i++)
        {
            CameraController.instance.SetTarget(roomDoors[i].focusPoint);
            yield return new WaitForSeconds(1f);
            roomDoors[i].Activate();
            yield return new WaitForSeconds(1f);
        }

        PlayerController.instance.SetState(PlayerController.States.idle);
        CameraController.instance.SetTarget(PlayerController.instance.gameObject);
    }
}
