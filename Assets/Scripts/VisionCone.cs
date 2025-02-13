using UnityEngine;

public class VisionCone : MonoBehaviour
{
    //Fields for controlling vision cone
    [SerializeField, Range(1,100)]
    private int sectorCount = 50;

    //Accessible in editor to tweak, but also directly modifiable 
    [SerializeField]
    public float angle = 30;
    [SerializeField]
    public float distance = 15;

    private LayerMask rayMask;

    private MeshFilter visionConeMeshFilter;
    private PolygonCollider2D visionConeCollider;

    private Mesh visionConeMesh;

    private CameraBehavior CameraEnemy;

    private float GetDistance(float angle)
    {
        //Add offset for object rotation
        float angleOffset = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;

        //Cast a ray in the direction of the vertex
        Vector2 direction = new Vector2(Mathf.Cos(angle + angleOffset), Mathf.Sin(angle + angleOffset));
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, direction, distance,rayMask);
        if(rayHit)
        {
            return rayHit.distance;
        }
        return distance;
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
            float lineAngle = i * (this.angle / sectorCount);
            lineAngle -= this.angle / 2;
            lineAngle *= Mathf.Deg2Rad;

            //Get the vertex position
            float pointDistance = GetDistance(lineAngle);
            Vector3 vertex = new Vector3(Mathf.Cos(lineAngle) * pointDistance, Mathf.Sin(lineAngle) * pointDistance);
            newVertices[i+1] = vertex;
            newUVs[i + 1] = new Vector2(1, 0);
            colliderVertices[i + 1] = vertex;


            //Get indices for that triangle
            newTriangles[i * 3 + 0] = 0;
            newTriangles[i * 3 + 1] = i+2;
            newTriangles[i * 3 + 2] = i+1;
        }
        //Calculate final vertex 
        float finalAngle = (angle / 2) * Mathf.Deg2Rad;   //SectorCount cancels out
        float finalPointDistance = GetDistance(finalAngle);
        Vector3 finalVertex = new Vector3(Mathf.Cos(finalAngle) * finalPointDistance, Mathf.Sin(finalAngle) * finalPointDistance);

        newVertices[sectorCount + 1] = finalVertex;
        colliderVertices[sectorCount + 1] = finalVertex;
        newUVs[sectorCount + 1] = new Vector2(1, 0);

        //Update mesh
        visionConeMesh.vertices = newVertices;
        visionConeMesh.triangles = newTriangles;
        visionConeMesh.uv = newUVs;
        visionConeMesh.RecalculateBounds();

        //Update collider
        visionConeCollider.points = colliderVertices;
    }

    private void Awake()
    {
        //Get the required components
        visionConeMeshFilter = GetComponent<MeshFilter>();
        visionConeCollider = GetComponent<PolygonCollider2D>();

        visionConeMesh = visionConeMeshFilter.mesh;
        visionConeMesh.MarkDynamic();

        rayMask = 0b0110011; //Ignore player and "ignoreCast" layers

        CameraEnemy = transform.parent.GetComponent<CameraBehavior>();

        GenerateConeMesh();
        
    }

    private void Update()
    {
        GenerateConeMesh();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Will be handled through inheritance in full game
            CameraEnemy.SeePlayer(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Will be handled through inheritance in full game
            CameraEnemy.LosePlayer();
        }
    }

    private void OnDrawGizmosSelected()
    {
        float angleRad = (angle) * Mathf.Deg2Rad;
        float angleOffset = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector3[] lineStrip = new Vector3[3];
        lineStrip[0] = transform.position;
        lineStrip[1] = transform.position + new Vector3(Mathf.Cos(angleRad / 2 + angleOffset) * distance,Mathf.Sin(angleRad / 2 + angleOffset) * distance);
        lineStrip[2] = transform.position + new Vector3(Mathf.Cos(-angleRad / 2 + angleOffset) * distance,Mathf.Sin(-angleRad / 2 + angleOffset) * distance);

        Gizmos.DrawLineStrip(lineStrip,true);
    }
}
