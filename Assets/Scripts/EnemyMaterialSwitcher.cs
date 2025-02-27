using UnityEngine;


public class EnemyMaterialSwitcher : MonoBehaviour
{
    enum MaterialTypes
    {
        Concrete,
        Dirt,
        Rubber,
        Metal
    }

    [SerializeField] private MaterialTypes enemyMaterialType;
    BoxCollider2D boxCollider;
    BoxCollider2D enemy;
    Rigidbody2D enemyRB;
    BoxCollider2D enemyCollider;

    LayerMask layers;

    void Start()
    {

        layers = new LayerMask();
        layers = 0b0110011;

        boxCollider = GetComponent<BoxCollider2D>();
        enemy = GameObject.Find("Guard").GetComponent<BoxCollider2D>();
        enemyRB = GameObject.Find("Guard").GetComponent<Rigidbody2D>();
        enemyCollider = GameObject.Find("Guard").GetComponent<BoxCollider2D>();

        // Takes the tag assigned to the game object and converts that string into an enum if it's one of the materials.
        if (tag == "Concrete" || tag == "Dirt" || tag == "Rubber" || tag == "Metal")
        {
            enemyMaterialType = (MaterialTypes)System.Enum.Parse(typeof(MaterialTypes), tag);
        }

        //Set Switch Group's active State to "Concrete"
        AkSoundEngine.SetSwitch("Guard_Footstep_Material", "Concrete", this.gameObject);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider == enemy)
        {
            Vector2 topRightRayStart = enemyRB.position + enemyCollider.offset + new Vector2(enemyCollider.size.x * 0.8f / 2f,
                                                                                               enemyCollider.size.y / 2f);
            Vector2 bottomRightRayStart = enemyRB.position + enemyCollider.offset + new Vector2(enemyCollider.size.x * 0.8f / 2f,
                                                                                                  -enemyCollider.size.y * 0.8f / 2f);
            Vector2 topLeftRayStart = enemyRB.position + enemyCollider.offset + new Vector2(-enemyCollider.size.x * 0.8f / 2f,
                                                                                               enemyCollider.size.y / 2f);
            Vector2 bottomLeftRayStart = enemyRB.position + enemyCollider.offset + new Vector2(-enemyCollider.size.x * 0.8f / 2f,
                                                                                                 -enemyCollider.size.y * 0.8f/ 2f);
            
            RaycastHit2D topRightRay = Physics2D.Raycast(topRightRayStart, Vector2.right, 0.4f, layers);
            RaycastHit2D bottomRightRay = Physics2D.Raycast(bottomRightRayStart, Vector2.right, 0.4f, layers);
            RaycastHit2D topLeftRay = Physics2D.Raycast(topLeftRayStart, Vector2.left, 0.4f, layers);
            RaycastHit2D bottomLeftRay = Physics2D.Raycast(bottomLeftRayStart, Vector2.left, 0.4f, layers);

            //Fucked up evil if statement dont care
            if ((topLeftRay.collider == null && bottomLeftRay.collider == null && topRightRay.collider == null && bottomRightRay.collider == null)
                 || (topLeftRay.collider != boxCollider && bottomLeftRay.collider != boxCollider && topRightRay.collider != boxCollider && bottomRightRay.collider != boxCollider))
            {
                //Change material to materialType

                if (enemyMaterialType == MaterialTypes.Concrete)
                {
                        //Set Switch Group's active State to "Concrete"
                        AkSoundEngine.SetSwitch("Guard_Footstep_Material", "Concrete", this.gameObject);
                        Debug.Log("ConcreteGuard");
                }

                if (enemyMaterialType == MaterialTypes.Dirt)
                {
                        //Set Switch Group's active State to "Dirt"
                        AkSoundEngine.SetSwitch("Guard_Footstep_Material", "Dirt", this.gameObject);
                        Debug.Log("DirtGuard");
                }

                if (enemyMaterialType == MaterialTypes.Rubber)
                {
                        //Set Switch Group's active State to "Rubber"
                        AkSoundEngine.SetSwitch("Guard_Footstep_Material", "Rubber", this.gameObject);
                        Debug.Log("RubberGuard");
                }

                if (enemyMaterialType == MaterialTypes.Metal)
                {
                        //Set Switch Group's active State to "Metal"
                        AkSoundEngine.SetSwitch("Guard_Footstep_Material", "Metal", this.gameObject);
                        Debug.Log("MetalGuard");
                }
            }
        }
    }
}
