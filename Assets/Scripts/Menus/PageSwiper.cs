using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageSwiper : MonoBehaviour
{
    [Header("Sliding Values")]
    [SerializeField] private float easing = 0.5f;
    int currentChild;
    private Vector3 panelLocation;
    bool changingPanel;


    void Start()
    {
        currentChild = 0;
        changingPanel = false;

        panelLocation = transform.position;
    }

    private void Update()
    {
        if (PauseMenuController.instance.isPaused && !changingPanel)
        {
            if (PlayerController.instance.inputMaster.Player.RadioSpecial.triggered && currentChild < transform.childCount - 1)
            {
                currentChild++;
                panelLocation -= new Vector3(Screen.width, 0, 0);
                StartCoroutine(SmoothMove(transform.position, panelLocation, easing));
            }

            if (PlayerController.instance.inputMaster.Player.Radio.triggered && currentChild > 0)
            {
                currentChild--;
                panelLocation += new Vector3(Screen.width, 0, 0);
                StartCoroutine(SmoothMove(transform.position, panelLocation, easing));
            }
        }
    }

    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
    {
        changingPanel = true;
        float t = 0.0f;
        while (t <= 1.0f)
        {
            t += Time.fixedDeltaTime / seconds;
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        changingPanel = false;
    }
}
