using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VisionCone : MonoBehaviour
{
    [SerializeField]
    private int sectorCount = 30;

    [SerializeField]
    private float radius = 30;
    [SerializeField]
    private float distance = 4;

    [SerializeField]
    private Color EyeColour = Color.white;
    [SerializeField]
    private Color EndColour = new Color(1,1,1,0.2f);

    private MeshFilter visionConeMeshFilter;
    private PolygonCollider2D visionConeCollider;

    private Mesh visionConeMesh;

    private void Start()
    {
        //Get the required components
        visionConeMeshFilter = GetComponent<MeshFilter>();
        PolygonCollider2D visionConeCollider = GetComponent<PolygonCollider2D>();

        visionConeMesh = new Mesh();
        visionConeMesh.MarkDynamic();

        visionConeMeshFilter.mesh = visionConeMesh;

        GenerateConeMesh();
    }

    private void GenerateConeMesh()
    {
        visionConeMesh.Clear();

        //Create arrays for mesh
        Vector3[] newVertices =  new Vector3[2 + sectorCount];
        int[] newTriangles= new int[3 * sectorCount];
        Color[] colours = new Color[2 + sectorCount];
        
        //Create vertices
        newVertices[0] = new Vector3(0,0,0);
        colours[0] = EyeColour;
        for (int i = 0; i < sectorCount; i++)
        {
            //Get the angle of the line out (in radians)
            float angle = i * (radius / sectorCount);
            angle -= radius / 2;
            angle *= Mathf.Deg2Rad;

            //Get the vertex position
            Vector3 vertex = new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);
            newVertices[i+1] = vertex;
            colours[i + 1] = EndColour;

            //Get indices for that triangle
            newTriangles[i * 3 + 0] = 0;
            newTriangles[i * 3 + 1] = i+2;
            newTriangles[i * 3 + 2] = i+1;
        }
        //Calculate final vertex 
        float finalAngle = (radius / 2) * Mathf.Deg2Rad;   //SectorCount cancels out
        Vector3 finalVertex = new Vector3(Mathf.Cos(finalAngle) * distance, Mathf.Sin(finalAngle) * distance);

        newVertices[sectorCount + 1] = finalVertex;
        colours[sectorCount+1] = EndColour;

        visionConeMesh.vertices = newVertices;
        visionConeMesh.triangles = newTriangles;
        visionConeMesh.colors = colours;
        visionConeMesh.RecalculateBounds();
    }

    private void Update()
    {
        GenerateConeMesh();
    }
}
