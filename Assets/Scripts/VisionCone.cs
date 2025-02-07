using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    MeshFilter visionConeMeshFilter;
    PolygonCollider2D visionConeCollider;

    Mesh visionConeMesh;

    private void Start()
    {
        //Get the required components
        visionConeMeshFilter = GetComponent<MeshFilter>();
        PolygonCollider2D visionConeCollider = GetComponent<PolygonCollider2D>();

        visionConeMesh = new Mesh();
        visionConeMesh.MarkDynamic();

        visionConeMeshFilter.mesh = visionConeMesh;
    }
}
