using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class BossZombieController : SaveObject
{
    [Header("Boss State Variables")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] private int bossStage = 1;
    [SerializeField] private int maxHealth;
    private int towerNum = 0;
    bool isDead;
    public enum BossState { idle, setup, aggro, hurt, dead, waiting }
    [SerializeField] BossState bossState;
    Health health;

    [Header("Boss Object References")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] Animator avatarBody;
    [SerializeField] Animator tulpaBody;
    bool attackLeft;
    [SerializeField] GameObject handLeft, handRight;
    [SerializeField] float attackDelay = 2f;
    [SerializeField] Transform camTarget, deathCamTarget;
    [SerializeField] BossRadioTower[] radioTowers;
    [SerializeField] CamTriggerZone[] camTriggers;
    [SerializeField] ParticleSystem electricParticles;

    [Header("Boss UI References")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] GameObject healthBarObj;
    [SerializeField] Image healthBar;

    [Header("Boss Audio References")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] AudioClip stage1;
    [SerializeField] AudioClip stage2;
    [SerializeField] AudioClip stage3;

    Coroutine setupRoutine;
    Coroutine flickerRoutine;
    Coroutine healthRoutine;

    [Header("Death Event Triggers")]
    [FormerlySerializedAs("onTrigger")]
    public UnityEvent m_DeathOnTrigger = new UnityEvent();


    private void OnEnable()
    {
        if (hasActivated)
        {
            avatarBody.SetTrigger("isDead");
        }
    }

    private void Start()
    {
        health = GetComponent<Health>();
        maxHealth = health.currentHealth;
    }

    private void Update()
    {
        if (bossState != BossState.dead && bossState != BossState.idle) { PlayerController.instance.SeePlayer(); }
        tulpaBody.gameObject.SetActive(bossState != BossState.idle && !hasActivated);
        healthBarObj.SetActive(active && !hasActivated);


        if (active && !hasActivated)
        {
            switch (bossState)
            {
                case BossState.idle:
                    //float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.instance.transform.position)
                    Flicker(1.25f);
                    SetState(BossState.setup);
                    break;
                case BossState.setup:
                    if (setupRoutine == null)
                    {
                        setupRoutine = StartCoroutine(Setup());
                    }
                    break;
                case BossState.aggro:
                    //Attacking phase
                    if (!handLeft.activeSelf && !handRight.activeSelf)
                    {
                        attackLeft = !attackLeft;

                        if (attackLeft)
                        {
                            handLeft.GetComponent<BossHandController>().SetTarget(PlayerController.instance.transform);
                            handLeft.SetActive(true);
                            if (!isPlaying(tulpaBody, "Attack_Left"))
                                tulpaBody.SetTrigger("attack_L");
                        }
                        else
                        {
                            handRight.GetComponent<BossHandController>().SetTarget(PlayerController.instance.transform);
                            handRight.SetActive(true);
                            if (!isPlaying(tulpaBody, "Attack_Right"))
                                tulpaBody.SetTrigger("attack_R");
                        }
                    }

                    if (bossStage > 1)
                    {
                        attackDelay -= Time.deltaTime;
                        if (attackDelay < 0)
                        {
                            attackDelay = bossStage == 2 ? 2.5f : 1.75f;

                            if (attackLeft)
                            {
                                handRight.GetComponent<BossHandController>().SetTarget(PlayerController.instance.transform);
                                handRight.SetActive(true);
                            }
                            else
                            {
                                handLeft.GetComponent<BossHandController>().SetTarget(PlayerController.instance.transform);
                                handLeft.SetActive(true);
                            }
                        }
                    }
                    break;
                case BossState.hurt:
                    foreach (CamTriggerZone zone in camTriggers)
                    {
                        zone.gameObject.SetActive(false);
                    }

                    CameraController.instance.SetTarget(camTarget);
                    CamEffectController.instance.ForceEffect();

                    avatarBody.SetTrigger("isHurt");
                    tulpaBody.SetTrigger("isHurt");
                    bossStage++;

                    //Add audio layer
                    switch (bossStage)
                    {
                        case 2:
                            AudioController.instance.AddLayer(stage1);
                            break;
                        case 3:
                            AudioController.instance.AddLayer(stage2);
                            break;
                        //case 4:
                        //    AudioController.instance.AddLayer(stage3);
                        //    break;
                        default:
                            print($"Boss stage: {bossStage}");
                            break;
                    }
                    SetState(BossState.setup);
                    break;
                case BossState.waiting:
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
        print($"Boss State: {stateToSet}");
        bossState = stateToSet;
    }

    public int GetBossStage()
    {
        return bossStage;
    }

    public void SetDead()
    {
        bossState = BossState.dead;
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

        UpdateHealth();

        //Check current health to determine state
        if (health.currentHealth <= 0)
        {
            foreach (CamTriggerZone zone in camTriggers)
            {
                zone.gameObject.SetActive(false);
            }

            CameraController.instance.SetRotation(false);
            CameraController.instance.SetTarget(deathCamTarget);
            SetState(BossState.waiting);//BossState.dead);

            m_DeathOnTrigger.Invoke();
        }
        else
        {
            CameraController.instance.SetRotation(false);
            CameraController.instance.SetTarget(camTarget);

            SetState(BossState.hurt);
            electricParticles.Play();
            CamEffectController.instance.ForceEffect();
            Flicker(1.25f);

            yield return new WaitForSeconds(1.75f);
            CamEffectController.instance.SetEffectState(false);

            yield return new WaitForSeconds(2.25f);
           
            PlayerController.instance.SetState(PlayerController.States.idle);
        }
    }

    void UpdateHealth()
    {
        if (healthRoutine == null)
            healthRoutine = StartCoroutine(UpdateHealthRoutine());
    }

    IEnumerator UpdateHealthRoutine()
    {
        float targetFillAmount = health.currentHealth / (float)maxHealth;
        while (healthBar.fillAmount > targetFillAmount)
        {
            healthBar.fillAmount -= Time.deltaTime * 0.25f;
            yield return null;
        }

        healthRoutine = null;
    }

    public void Flicker(float waitTime)
    {
        if (flickerRoutine == null)
            flickerRoutine = StartCoroutine(FlickerRoutine(waitTime));
    }

    IEnumerator FlickerRoutine(float waitTime)
    {
        SkinnedMeshRenderer[] tulpaMeshes = tulpaBody.GetComponentsInChildren<SkinnedMeshRenderer>();
        float endTime = Time.time + waitTime;
        while (Time.time < endTime)
        {
            foreach (SkinnedMeshRenderer mesh in tulpaMeshes)
            {
                mesh.enabled = false;
            }
            yield return new WaitForSeconds(Random.Range(0.0f, 0.1f));
            foreach (SkinnedMeshRenderer mesh in tulpaMeshes)
            {
                mesh.enabled = true;
            }
            yield return new WaitForSeconds(Random.Range(0.0f, 0.1f));
        }

        foreach (SkinnedMeshRenderer mesh in tulpaMeshes)
        {
            mesh.enabled = true;
        }

        flickerRoutine = null;
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
                CameraController.instance.SetTarget(radioTowers[randVal].focusPoint);

                yield return new WaitForSeconds(1.75f);
            }
            else { i--; }
        }

        CameraController.instance.SetTarget(PlayerController.instance.lookTransform);
        PlayerController.instance.SetState(PlayerController.States.idle);

        yield return new WaitForSeconds(1.5f);

        foreach (CamTriggerZone zone in camTriggers)
        {
            zone.gameObject.SetActive(true);
        }

        SetState(BossState.aggro);
        setupRoutine = null;
    }

    IEnumerator Death()
    {
        handLeft.SetActive(false);
        handRight.SetActive(false);
        PlayerController.instance.SetState(PlayerController.States.listening);
        //CameraController.instance.SetRotation(true);
        CameraController.instance.SetTarget(PlayerController.instance.lookTransform);

        avatarBody.SetTrigger("isDead");
        tulpaBody.SetTrigger("isDead");

        Flicker(8f);
        yield return new WaitForSeconds(8f);

        foreach (BossRadioTower tower in radioTowers)
        {
            tower.DeActivate();
        }

        m_OnTrigger.Invoke();
        SetHasActivated();

        PlayerController.instance.SetState(PlayerController.States.idle);
        CameraController.instance.SetTarget(PlayerController.instance.lookTransform);
        if (CameraController.instance.GetTriggerState())
            CameraController.instance.SetRotation(true);
    }

    public bool isPlaying(Animator animator, string stateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            return true;
        else
            return false;
    }
}
