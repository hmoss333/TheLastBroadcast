using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class SpawnOnDeath : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToSpawn;
    Health objHealth;

    private void Start()
    {
        objHealth = GetComponent<Health>();
    }

    private void Update()
    {
        if (objHealth.currentHealth <= 0)
        {
            float randVal = Random.value;
            if (randVal % 2 > 0.65f)
            {
                //TODO modify this to prefer either battey or health depending on which resource the player has less of
                int objVal = Random.Range(0, objectsToSpawn.Length);
                GameObject spawnObj = Instantiate(objectsToSpawn[objVal], transform.position, Quaternion.identity);
            }

            this.enabled = false;
        }
    }
}
