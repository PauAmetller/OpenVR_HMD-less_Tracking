using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsatingCircle : MonoBehaviour
{
    public float maxRadius = 2f;
    public float minRadius = 0.3f; // Minimum radius during pulsation
    public float pulsationSpeed = 0.5f;
    public int resolution = 100; // Number of points on the circle
    public Vector3 circleCenter = Vector3.zero; // Center of the circle
    public float lineThickness = 0.1f; // Thickness of the line
    public Material lineMaterial; // Reference to the material
    public float startDelay = 0.5f;

    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private List<float> elapsedTimes = new List<float>();
    private List<bool> visibilities = new List<bool>();

    public bool active = false;
    void Start()
    {
        CreateCircle("Circle1"); 
        CreateCircle("Circle2", startDelay); 
        CreateCircle("Circle3", startDelay * 2);
        CreateCircle("Circle4", startDelay * 3);
        CreateCircle("Circle5", startDelay * 4);
    }

    void CreateCircle(string name, float initialDelay = 0f)
    {
        GameObject circle = new GameObject(name);

        circle.transform.parent = transform;

        LineRenderer lineRenderer = circle.AddComponent<LineRenderer>();
        lineRenderer.positionCount = resolution + 1; // +1 to close the loop
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = lineThickness;
        lineRenderer.endWidth = lineThickness;

        if (lineMaterial != null)
        {
            lineRenderer.material = lineMaterial;
        }


        lineRenderers.Add(lineRenderer);
        elapsedTimes.Add(-initialDelay);
        visibilities.Add(true);

    }

    void Update()
    {
        if (active)
        {
            for (int i = 0; i < lineRenderers.Count; i++)
            {
                elapsedTimes[i] += Time.deltaTime;

                if (elapsedTimes[i] >= 0.0f)
                {
                    bool isVisible = visibilities[i];
                    LineRenderer lineRenderer = lineRenderers[i];

                    UpdateLineRendered(elapsedTimes[i], ref lineRenderer, ref isVisible);

                    visibilities[i] = isVisible;
                    lineRenderers[i] = lineRenderer;
                }
            }
        }
        else
        {
            for (int i = 0; i < lineRenderers.Count; i++)
            {
                lineRenderers[i].enabled = false;
            }
        }
    }

    void UpdateLineRendered(float elapsedTime, ref LineRenderer lineRenderer, ref bool isVisible)
    {

        float cycleDuration = Mathf.PI * pulsationSpeed;

        // Use modulo to keep elapsedTime within the cycle duration
        elapsedTime = elapsedTime % cycleDuration;

        // Calculate current radius based on sine wave
        float radiusScale = Mathf.Sin(pulsationSpeed * elapsedTime); // Simplified sine wave

        // Ensure radiusScale is always positive
        radiusScale = Mathf.Abs(radiusScale);

        float currentRadius = Mathf.Lerp(maxRadius, minRadius - 1f, radiusScale);

        // Visibility check (more aggressive)
        if (currentRadius <= minRadius)
        {
            isVisible = false;
        }
        else if (currentRadius >= minRadius - 0.001f)
        {
            isVisible = true;
        }

        // Update the circle's position and visibility
        if (isVisible)
        {
            UpdateCircle(currentRadius, ref lineRenderer);
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    void UpdateCircle(float currentRadius, ref LineRenderer lineRenderer)
    {
        Vector3[] points = new Vector3[resolution + 1];
        for (int i = 0; i <= resolution; i++)
        {
            float angle = i * Mathf.PI * 2f / resolution;
            points[i] = circleCenter + new Vector3(Mathf.Cos(angle) * currentRadius, 0.4f, Mathf.Sin(angle) * currentRadius);
        }

        lineRenderer.SetPositions(points);
    }
}
