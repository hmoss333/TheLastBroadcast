using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZombieController : CharacterController
{
    [NaughtyAttributes.HorizontalLine]
    [Header("Boss State Variables")]
    [SerializeField] private int bossHealth;
    [SerializeField] private int bossHitNum = 0;
    [SerializeField] private int towerNum = 0;
    [SerializeField] private int bossStage;
    bool settingUp;
    public enum BossState { idle, setup, aggro, stun, hurt, dead }
    [SerializeField] BossState bossState;
    Health health;
    Collider bossCollider;

    [NaughtyAttributes.HorizontalLine]
    [Header("Boss Prefab Variables")]
    [SerializeField] GameObject shieldObject;
    [SerializeField] BossRadioTower radioTowerPrefab;
    [SerializeField] ZombieController zombiePrefab;
    [SerializeField] GameObject handLeft, handRight;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] List<BossRadioTower> currentRadioTowers;
    SaveObject saveObj;


    private void Start()
    {
        health = GetComponent<Health>();
        bossCollider = GetComponent<Collider>();
        bossHealth = health.CurrentHealth();
        saveObj = GetComponent<SaveObject>();

        base.Start();
    }

    private void Update()
    {
        bossHealth = health.CurrentHealth();
        bossCollider.enabled = bossState != BossState.dead ? !shieldObject.activeSelf : false;

        if (bossState == BossState.aggro && hurt)
            bossState = BossState.hurt;
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
                    if (hurt)
                    {
                        bossStage = 1;
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
                    
                    break;
                case BossState.stun:
                    handLeft.SetActive(false);
                    handRight.SetActive(false);
                    SetState(BossState.setup);
                    break;
                case BossState.hurt:
                    if (hurt && !isPlaying("Hurt"))
                    {
                        bossHitNum++;
                        hurt = false;
                        if (bossHitNum > 3)
                        {
                            bossHitNum = 0;
                            bossStage++;
                            SetState(BossState.stun);
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
                        handLeft.SetActive(false);
                        handRight.SetActive(false);
                        foreach (BossRadioTower tower in currentRadioTowers)
                        {
                            Destroy(tower.gameObject);
                        }
                        currentRadioTowers.Clear();
                        saveObj.SetHasActivated();
                    }
                    break;
                default:
                    break;
            }
        }

        base.Update();
    }

    IEnumerator Setup()
    {
        foreach (BossRadioTower tower in currentRadioTowers)
        {
            Destroy(tower.gameObject);
        }
        currentRadioTowers.Clear();
        shieldObject.SetActive(true);
        CameraController.instance.SetTarget(shieldObject);

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < bossStage; i++)
        {
            BossRadioTower radioTower = Instantiate(radioTowerPrefab, spawnPoints[i].position, Quaternion.identity);
            currentRadioTowers.Add(radioTower);
            CameraController.instance.SetTarget(radioTower.gameObject);

            //if (bossStage > 2)
            //{
            //    ZombieController zombie = Instantiate(zombiePrefab, spawnPoints[i].position, Quaternion.identity);
            //}

            yield return new WaitForSeconds(1f);
        }

        CameraController.instance.SetTarget(PlayerController.instance.gameObject);

        yield return new WaitForSeconds(0.35f);

        handLeft.SetActive(true);
        SetState(BossState.aggro);
        settingUp = false;
    }

    public void SetState(BossState stateToSet)
    {
        bossState = stateToSet;
    }

    public void SetTower()
    {
        towerNum++;
        if (towerNum >= currentRadioTowers.Count)
        {
            towerNum = 0;
            shieldObject.SetActive(false);
        }
    }
}
