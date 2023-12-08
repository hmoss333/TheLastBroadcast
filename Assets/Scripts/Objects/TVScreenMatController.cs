using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Renderer))]
public class TVScreenMatController : MonoBehaviour
{
    [SerializeField] Material baseMat, loadMat, imageFlashMat;
    [SerializeField] float imageFlashTime;
    [SerializeField] float countdownTimer;
    [SerializeField] bool inLoadingScreen;
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
        Material[] tempMats = tvRend.materials;
        List<Material> matList = tempMats.ToList<Material>();

        if (MainMenuController.instance.loadGameCanvas.gameObject.activeSelf && !inLoadingScreen)
        {
            inLoadingScreen = true;
            SwapMat(baseMat, loadMat, tvRend);
        }
        else if (!MainMenuController.instance.loadGameCanvas.gameObject.activeSelf && inLoadingScreen)
        {
            inLoadingScreen = false;
            SwapMat(loadMat, baseMat, tvRend);
        }

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
        SwapMat(inLoadingScreen ? loadMat : baseMat, imageFlashMat, tvRend);

        yield return new WaitForSeconds(imageFlashTime);

        SwapMat(imageFlashMat, inLoadingScreen ? loadMat : baseMat, tvRend);

        tvFlashRoutine = null;
    }
}
