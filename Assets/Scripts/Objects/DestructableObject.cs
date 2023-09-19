using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DestructableObject : SaveObject
{
    private Health health;
    Coroutine shakeRoutine;

    [Header("Shake Pos Values")]
    private Vector3 startPos;
    private float timer;
    private Vector3 randomPos;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float time = 0.2f;
    [Range(0f, 2f)]
    public float distance = 0.1f;
    [Range(0f, 0.1f)]
    public float delayBetweenShakes = 0f;


    private void Awake()
    {
        startPos = transform.position;
    }

    private void Start()
    {
        health = GetComponent<Health>();

        gameObject.SetActive(!hasActivated);
    }

    private void OnValidate()
    {
        if (delayBetweenShakes > time)
            delayBetweenShakes = time;
    }

    private void Update()
    {
        if (health.currentHealth <= 0 && !hasActivated)
        {
            SetHasActivated();
            gameObject.SetActive(false);
        }
    }

    public void Shake()
    {
        if (shakeRoutine == null)
            shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        timer = 0f;

        while (timer < time)
        {
            timer += Time.deltaTime;

            randomPos = startPos + (Random.insideUnitSphere * distance);

            transform.position = randomPos;

            if (delayBetweenShakes > 0f)
            {
                yield return new WaitForSeconds(delayBetweenShakes);
            }
            else
            {
                yield return null;
            }
        }

        transform.position = startPos;
        shakeRoutine = null;
    }
}
