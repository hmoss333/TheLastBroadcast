using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI.Extensions;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] Health health;
    UILineRenderer lineRenderer;
    [SerializeField] int points;
    [SerializeField] float amplitude = 1;
    [SerializeField] float frequency = 1;
    [SerializeField] Vector2 xLimits = new Vector2(0, 1);
    [SerializeField] float movementSpeed = 1;
    List<Vector2> pointList = new List<Vector2>();

    private void Start()
    {
        lineRenderer = GetComponent<UILineRenderer>();
    }

    void AddNewPoint(float x, float y)
    {
        var point = new Vector2(x, y);//) { x, y };
        pointList.Add(point);
    }

    private void Draw()
    {
        float xStart = xLimits.x;//0;
        float Tau = 2 * Mathf.PI;
        float xFinish = xLimits.y;//Tau;
        //amplitude = health.CurrentHealth() * 5f;
        frequency = (health.CurrentHealth() / 5f) + 1f;

        pointList.Clear();
        for (int currentPoint = 0; currentPoint < points; currentPoint++)
        {
            float progress = (float)currentPoint / (points - 1);
            float x = Mathf.Lerp(xStart, xFinish, progress);
            float y = amplitude * Mathf.Sin((Tau * frequency * x) + Time.timeSinceLevelLoad * movementSpeed);
            AddNewPoint(x, y);
        }

        lineRenderer.Points = pointList.ToArray();
    }

    private void Update()
    {
        Draw();
    }
}
