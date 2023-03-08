using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZombieController : CharacterController
{
    [SerializeField] GameObject handLeft, handRight;
    Health health;
    [SerializeField] int bossHealth, bossStage;
    enum BossState { idle, aggro, stun, hurt, dead }
    [SerializeField] BossState bossState;
    [SerializeField] GameObject shieldObject;
    [SerializeField] GameObject radioTowerPrefab;
    [SerializeField] ZombieController zombiePrefab;
    [SerializeField] Transform[] spawnPoints;


    private void Start()
    {
        health = GetComponent<Health>();
        bossHealth = health.CurrentHealth();
    }

    private void FixedUpdate()
    {
        //if (active && !hasActivated)
        //{
            switch (bossState)
            {
                case BossState.idle:
                    if (health.CurrentHealth() < bossHealth)
                    {
                        bossStage = 1;
                        SetState(BossState.aggro);
                        //TODO enable shield object around boss
                    }
                    break;
                case BossState.aggro:
                    //TODO Attack logic goes here
                    for (int i = 0; i < bossStage; i++)
                    {
                        SpawnTowers(spawnPoints[i]);

                        if (bossStage > 2)
                        {
                            SpawnZombies(spawnPoints[i]);
                        }
                    }

                    shieldObject.SetActive(true);
                    handLeft.SetActive(true);
                    break;
                case BossState.stun:
                    //TODO disable hands and shield
                    handLeft.SetActive(false);
                    handRight.SetActive(false);                   
                    break;
                case BossState.hurt:
                    //TODO reset room and increment boss stage
                    //Change back to aggro state
                    break;
                case BossState.dead:
                    //Play all relevant animations and disable hands/zombies
                    handLeft.SetActive(false);
                    handRight.SetActive(false);
                    break;
                default:
                    break;
            }
        //}
    }

    void SetState(BossState stateToSet)
    {
        bossState = stateToSet;
    }

    void SpawnTowers(Transform spawnPoint)
    {
        GameObject tempTower = Instantiate(radioTowerPrefab, spawnPoint.position, Quaternion.identity);
    }

    void SpawnZombies(Transform spawnPoint)
    {
        ZombieController tempZombie = Instantiate(zombiePrefab, spawnPoint.position, Quaternion.identity);
    }
}
