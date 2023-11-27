using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TVScreenMatController : MonoBehaviour
{
    [SerializeField] Material baseMat, imageFlashMat;
    [SerializeField] float imageFlashTime;
    [SerializeField] float countdownTimer;
    float countdownTemp;
    Renderer tvRend;
    Coroutine tvFlashRoutine;


    private void Start()
    {
        tvRend = GetComponent<Renderer>();
        countdownTemp = countdownTimer;
    }

    private void Update()
    {
        countdownTimer -= Time.deltaTime;
        if (countdownTimer <= 0)
        {
            countdownTimer = countdownTemp;
            FlashMat();
        }
    }

    void SwapMat(Material matToRemove, Material matToAdd, Renderer renderer)
    {
        Material[] tempMats = renderer.materials;
        List<Material> returnMats = new List<Material>();

        for (int i = 0; i < tempMats.Length; i++)
        {
            if (!tempMats[i].name.Contains(matToRemove.name))
            {
                returnMats.Add(tempMats[i]);
            }
        }

        returnMats.Add(matToAdd);
        renderer.materials = returnMats.ToArray();
    }

    void FlashMat()
    {
        if (tvFlashRoutine == null)
            tvFlashRoutine = StartCoroutine(FlashMatRoutine());
    }

    IEnumerator FlashMatRoutine()
    {
        SwapMat(baseMat, imageFlashMat, tvRend);

        yield return new WaitForSeconds(imageFlashTime);

        SwapMat(imageFlashMat, baseMat, tvRend);

        tvFlashRoutine = null;
    }
}
