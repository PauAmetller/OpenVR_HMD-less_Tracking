using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CalibrationQuadManager : MonoBehaviour
{
    private List<GameObject> spherePoints = new List<GameObject>();
    private GameObject quadObject;
    private GameObject ghostQuadObject;

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
        //GameObject newGhostSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        // Set its position and scale
        newSphere.transform.position = position;
        newSphere.transform.localScale = new Vector3(2f, 2f, 2f);
        newSphere.GetComponent<Renderer>().material = material;

        //newGhostSphere.transform.position = position;
        //newGhostSphere.transform.localScale = new Vector3(2f, 2f, 2f);
        //Renderer ghostMeshRenderer = newGhostSphere.GetComponent<Renderer>(); ;
        //ghostMeshRenderer.material = ghostMaterial;


        //ghostMeshRenderer.material.SetFloat("_Mode", 3); // 3 corresponds to Transparent mode
        //ghostMeshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        //ghostMeshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        //ghostMeshRenderer.material.SetInt("_ZWrite", 0);
        //ghostMeshRenderer.material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        //ghostMeshRenderer.material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);

        //ghostMeshRenderer.material.DisableKeyword("_ALPHATEST_ON");
        //ghostMeshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
        //ghostMeshRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        //ghostMeshRenderer.material.renderQueue = 3100;



        spherePoints.Add(newSphere); // Add to list

        newSphere.transform.SetParent(sphereParent.transform);
        //newGhostSphere.transform.SetParent(sphereParent.transform);

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

        // Get the positions of the four corner points from the main spheres
        Vector3 p1 = spherePoints[0].transform.position;
        Vector3 p2 = spherePoints[1].transform.position;
        Vector3 p3 = spherePoints[2].transform.position;
        Vector3 p4 = spherePoints[3].transform.position;

        GameObject sphereParent = new GameObject("QuadParent");

        // Create a new GameObject to hold the mesh
        quadObject = new GameObject("Quad");
        ghostQuadObject = new GameObject("GhostQuad");

        // Add a MeshFilter and MeshRenderer components
        MeshFilter meshFilter = quadObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = quadObject.AddComponent<MeshRenderer>();
        MeshFilter ghostMeshFilter = ghostQuadObject.AddComponent<MeshFilter>();
        MeshRenderer ghostMeshRenderer = ghostQuadObject.AddComponent<MeshRenderer>();

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

        // Set the mesh to the MeshFilter
        meshFilter.mesh = mesh;
        ghostMeshFilter.mesh = mesh;

        // Apply the material to the mesh renderer
        meshRenderer.material = material;
        //ghostMeshRenderer.material = ghostMaterial;

        //ghostMeshRenderer.material.SetInt("_ZWrite", 0); // Disable depth writing
        //ghostMeshRenderer.material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Greater); // Render behind
        //ghostMeshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        //ghostMeshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        quadObject.transform.SetParent(sphereParent.transform);
        //ghostQuadObject.transform.SetParent(sphereParent.transform);

        if (parentObject != null)
        {
            sphereParent.transform.SetParent(parentObject.transform);
        }
    }
}
