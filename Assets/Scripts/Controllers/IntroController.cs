using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FadeController.instance.StartFade(0.0f, 1.5f);
    }
}
