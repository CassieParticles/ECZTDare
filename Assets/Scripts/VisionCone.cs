using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VisionCone : MonoBehaviour
{
    [SerializeField, Range(1,40)]
    private int sectorCount = 10;

    [SerializeField]
    private float radius = 30;
    [SerializeField]
    private float distance = 4;

    private MeshFilter visionConeMeshFilter;
    private PolygonCollider2D visionConeCollider;

    private Mesh visionConeMesh;

    private void Start()
    {
        //Get the required components
        visionConeMeshFilter = GetComponent<MeshFilter>();
        visionConeCollider = GetComponent<PolygonCollider2D>();

        visionConeMesh = visionConeMeshFilter.mesh;
        visionConeMesh.MarkDynamic();

        GenerateConeMesh();
    }

    private void GenerateConeMesh()
    {
        visionConeMesh.Clear();

        //Create arrays for mesh
        Vector3[] newVertices =  new Vector3[2 + sectorCount];
        Vector2[] newUVs = new Vector2[2 + sectorCount];
        int[] newTriangles= new int[3 * sectorCount];

        //Create array for colldider
        Vector2[] colliderVertices = new Vector2[2 + sectorCount];
        
        //Create vertices
        newVertices[0] = new Vector3(0,0,0);
        newUVs[0] = new Vector2(0, 0);
        colliderVertices[0] = new Vector2(0, 0);
        for (int i = 0; i < sectorCount; i++)
        {
            //Get the angle of the line out (in radians)
            float angle = i * (radius / sectorCount);
            angle -= radius / 2;
            angle *= Mathf.Deg2Rad;

            //Get the vertex position
            Vector3 vertex = new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);
            newVertices[i+1] = vertex;
            newUVs[i + 1] = new Vector2(1, 0);
            colliderVertices[i + 1] = vertex;


            //Get indices for that triangle
            newTriangles[i * 3 + 0] = 0;
            newTriangles[i * 3 + 1] = i+2;
            newTriangles[i * 3 + 2] = i+1;
        }
        //Calculate final vertex 
        float finalAngle = (radius / 2) * Mathf.Deg2Rad;   //SectorCount cancels out
        Vector3 finalVertex = new Vector3(Mathf.Cos(finalAngle) * distance, Mathf.Sin(finalAngle) * distance);

        newVertices[sectorCount + 1] = finalVertex;
        colliderVertices[sectorCount + 1] = finalVertex;
        newUVs[sectorCount + 1] = new Vector2(1, 0);

        //Update mesh
        visionConeMesh.vertices = newVertices;
        visionConeMesh.triangles = newTriangles;
        visionConeMesh.uv = newUVs;
        visionConeMesh.RecalculateBounds();

        visionConeCollider.points = colliderVertices;
    }

    private void Update()
    {
        GenerateConeMesh();
    }
}
