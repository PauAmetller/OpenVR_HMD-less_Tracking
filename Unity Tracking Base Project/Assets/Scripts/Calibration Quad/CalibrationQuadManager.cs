using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CalibrationQuadManager : MonoBehaviour
{
    private List<GameObject> spherePoints = new List<GameObject>();
    private GameObject quadObject;

    [SerializeField] private Material material;
    //[SerializeField] private Material ghostMaterial;
    [SerializeField] private GameObject parentObject;

    public void CreateSphere(Vector3 position)
    {

        if (spherePoints.Count == 4)
        {
            return;
        }

        // Create an empty GameObject as the parent
        GameObject sphereParent = new GameObject("SphereParent" + spherePoints.Count);

        // Create a new sphere primitive
        GameObject newSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        // Set its position and scale
        newSphere.transform.position = position;
        newSphere.transform.localScale = new Vector3(2f, 2f, 2f);
        newSphere.GetComponent<Renderer>().material = material;

        spherePoints.Add(newSphere); // Add to list

        newSphere.transform.SetParent(sphereParent.transform);

        if (parentObject != null)
        {
            sphereParent.transform.SetParent(parentObject.transform);
        }

        if (spherePoints.Count == 4)
            CreateQuadFromPoints();
    }


    public void CreateQuadFromPoints()
    {
        if (spherePoints.Count != 4)
        {
            Debug.LogError("You must assign exactly four sphere points!");
            return;
        }


        GameObject sphereParent = new GameObject("QuadParent");

        // Create a new GameObject to hold the mesh
        quadObject = new GameObject("Quad");

        // Add a MeshFilter and MeshRenderer components
        MeshFilter meshFilter = quadObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = quadObject.AddComponent<MeshRenderer>();

        // Set the mesh to the MeshFilter
        meshFilter.mesh = createQuadMesh();

        // Apply the material to the mesh renderer
        meshRenderer.material = material;
        Color newColor = meshRenderer.material.color;
        newColor.a *= 2;
        meshRenderer.material.color = newColor;

        quadObject.transform.SetParent(sphereParent.transform);

        if (parentObject != null)
        {
            sphereParent.transform.SetParent(parentObject.transform);
        }
    }

    private Mesh createQuadMesh()
    {

        // Get the positions of the four corner points from the main spheres
        Vector3 p1 = spherePoints[0].transform.position;
        Vector3 p2 = spherePoints[1].transform.position;
        Vector3 p3 = spherePoints[2].transform.position;
        Vector3 p4 = spherePoints[3].transform.position;

        // Create the mesh
        Mesh mesh = new Mesh();

        // Define the vertices (corners of the quadrilateral)
        Vector3[] vertices = new Vector3[4] { p1, p2, p3, p4 };

        int[] triangles = new int[6]
        {
            0, 1, 2, // First triangle
            2, 3, 0  // Second triangle
        };

        Vector2[] uvs = new Vector2[4]
        {
            new Vector2(0, 0),  // UV for p1
            new Vector2(1, 0),  // UV for p2
            new Vector2(1, 1),  // UV for p3
            new Vector2(0, 1)   // UV for p4
        };

        // Assign the vertices, triangles, and UVs to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        // Recalculate normals for proper lighting/shading
        mesh.RecalculateNormals();

        // Calculate the dot product between the normal and the upward direction
        float dotProduct = Vector3.Dot(mesh.normals[0], Vector3.up);

        // If the normal is pointing in the opposite direction of (0, 1, 0), reverse it
        if (dotProduct < 0)
        {
            // Reverse the normal for all vertices
            for (int i = 0; i < mesh.normals.Length; i++)
            {
                mesh.normals[i] = -mesh.normals[i];
            }
        }

        return mesh;
    }

    public void LerpToCalibratedPosition(Calibration calibrationData, Vector3 virtualWorldSpace)
    {
        float duration = 2.0f;
        foreach (var sphere in spherePoints)
        {
            StartCoroutine(SphereLerpToCalibratedPosition(sphere, calibrationData, virtualWorldSpace, duration));
        }
        StartCoroutine(UpdateMeshForDuration(duration));
    }

    private IEnumerator SphereLerpToCalibratedPosition(GameObject sphere, Calibration calibrationData, Vector3 virtualWorldSpace, float duration)
    {
        Vector3 startPos = sphere.transform.position;
        Vector3 calibratedPos = CalibrationUtils.CalibrateRawPos(startPos, true, calibrationData, virtualWorldSpace);
        Vector3 targetPos = new Vector3(calibratedPos.x * 10 / 9, calibratedPos.y, calibratedPos.z * 10 / 9);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            sphere.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position is set correctly
        sphere.transform.position = targetPos;
    }

    private IEnumerator UpdateMeshForDuration(float duration)
    {
        float elapsedTime = 0f;
        MeshFilter meshFilter = quadObject.GetComponent<MeshFilter>();

        while (elapsedTime < duration)
        {
            meshFilter.mesh = createQuadMesh();
            elapsedTime += Time.deltaTime;
            yield return null;

        }

        yield return new WaitForSeconds(duration);

        EliminateObjects();
    }

    public void EliminateObjects()
    {
        foreach (var sphere in spherePoints)
        {
            if (sphere != null)
            {
                Destroy(sphere);
                if (sphere.transform.parent != null)
                {
                    Destroy(sphere.transform.parent.gameObject);
                }
            }
        }
        spherePoints.Clear(); 

        if (quadObject != null)
        {
            Destroy(quadObject);
            if (quadObject.transform.parent != null)
            {
                Destroy(quadObject.transform.parent.gameObject);
            }
            quadObject = null;
        }
    }

}
