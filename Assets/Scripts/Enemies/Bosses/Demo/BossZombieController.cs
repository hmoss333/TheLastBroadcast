using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZombieController : CharacterController
{
    [NaughtyAttributes.HorizontalLine]
    [Header("Boss State Variables")]
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
    [SerializeField] BossRadioTower[] radioTowers;
    [SerializeField] ZombieController zombiePrefab;
    [SerializeField] GameObject handLeft, handRight;
    [SerializeField] Transform[] spawnPoints;
    SaveObject saveObj;


    private void Start()
    {
        health = GetComponent<Health>();
        bossCollider = GetComponent<Collider>();
        saveObj = GetComponent<SaveObject>();

        base.Start();
    }

    private void Update()
    {
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
                        if (bossHitNum == 3)
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
                        foreach (BossRadioTower tower in radioTowers)
                        {
                            tower.DeActivate();
                        }
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
        PlayerController.instance.SetState(PlayerController.States.listening);

        foreach (BossRadioTower tower in radioTowers)
        {
            tower.DeActivate();
        }
        shieldObject.SetActive(true);
        CameraController.instance.SetTarget(shieldObject);

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
       
        for (int i = 0; i < bossStage - 1; i++)
        {
            int randVal = Random.Range(0, radioTowers.Length);
            ZombieController zombie = Instantiate(zombiePrefab, spawnPoints[randVal].position, Quaternion.identity);
            CameraController.instance.SetTarget(zombie.gameObject);

            yield return new WaitForSeconds(1f);
        }

        CameraController.instance.SetTarget(PlayerController.instance.gameObject);

        yield return new WaitForSeconds(0.35f);

        handLeft.SetActive(true);
        SetState(BossState.aggro);
        settingUp = false;

        PlayerController.instance.SetState(PlayerController.States.idle);
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
}
