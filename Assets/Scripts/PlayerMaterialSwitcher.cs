using UnityEngine;


public class PlayerMaterialSwitcher : MonoBehaviour
{
    enum MaterialTypes
    {
        Concrete,
        Dirt,
        Rubber,
        Metal
    }

    [SerializeField] private MaterialTypes materialType;
    BoxCollider2D boxCollider;
    BoxCollider2D player;
    Rigidbody2D playerRB;
    BoxCollider2D playerCollider;

    LayerMask layers;

    void Start()
    {

        layers = new LayerMask();
        layers = 0b0110011;

        boxCollider = GetComponent<BoxCollider2D>();
        player = GameObject.Find("Player").GetComponent<BoxCollider2D>();
        playerRB = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        playerCollider = GameObject.Find("Player").GetComponent<BoxCollider2D>();

        // Takes the tag assigned to the game object and converts that string into an enum if it's one of the materials.
        if (tag == "Concrete" || tag == "Dirt" || tag == "Rubber" || tag == "Metal")
        {
            materialType = (MaterialTypes)System.Enum.Parse(typeof(MaterialTypes), tag);
        }

        //Set Switch Group's active State to "Concrete"
        AkSoundEngine.SetSwitch("Player_Footstep_Material", "Concrete", player.gameObject);
        AkSoundEngine.SetSwitch("Player_Jump_Material", "Concrete", player.gameObject);
        AkSoundEngine.SetSwitch("Player_Land_Material", "Concrete", player.gameObject);
        AkSoundEngine.SetSwitch("Player_Slide_Material", "Concrete", player.gameObject);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider == player)
        {
            Vector2 topRightRayStart = playerRB.position + playerCollider.offset + new Vector2(playerCollider.size.x * 0.8f / 2f,
                                                                                               playerCollider.size.y / 2f);
            Vector2 bottomRightRayStart = playerRB.position + playerCollider.offset + new Vector2(playerCollider.size.x * 0.8f / 2f,
                                                                                                  -playerCollider.size.y * 0.8f / 2f);
            Vector2 topLeftRayStart = playerRB.position + playerCollider.offset + new Vector2(-playerCollider.size.x * 0.8f / 2f,
                                                                                               playerCollider.size.y / 2f);
            Vector2 bottomLeftRayStart = playerRB.position + playerCollider.offset + new Vector2(-playerCollider.size.x * 0.8f / 2f,
                                                                                                 -playerCollider.size.y * 0.8f/ 2f);
            
            RaycastHit2D topRightRay = Physics2D.Raycast(topRightRayStart, Vector2.right, 0.4f, layers);
            RaycastHit2D bottomRightRay = Physics2D.Raycast(bottomRightRayStart, Vector2.right, 0.4f, layers);
            RaycastHit2D topLeftRay = Physics2D.Raycast(topLeftRayStart, Vector2.left, 0.4f, layers);
            RaycastHit2D bottomLeftRay = Physics2D.Raycast(bottomLeftRayStart, Vector2.left, 0.4f, layers);

            //Fucked up evil if statement dont care
            if ((topLeftRay.collider == null && bottomLeftRay.collider == null && topRightRay.collider == null && bottomRightRay.collider == null)
                 || (topLeftRay.collider != boxCollider && bottomLeftRay.collider != boxCollider && topRightRay.collider != boxCollider && bottomRightRay.collider != boxCollider))
            {
                //Change material to materialType

                if (materialType == MaterialTypes.Concrete)
                {
                        //Set Switch Group's active State to "Concrete"
                        AkSoundEngine.SetSwitch("Player_Footstep_Material", "Concrete", player.gameObject);
                        AkSoundEngine.SetSwitch("Player_Jump_Material", "Concrete", player.gameObject);
                        AkSoundEngine.SetSwitch("Player_Land_Material", "Concrete", player.gameObject);
                        AkSoundEngine.SetSwitch("Player_Slide_Material", "Concrete", player.gameObject);
                        //AkSoundEngine.SetSwitch("Guard_Footstep_Material", "Concrete", guard.gameObject);
                }

                if (materialType == MaterialTypes.Dirt)
                {
                        //Set Switch Group's active State to "Dirt"
                        AkSoundEngine.SetSwitch("Player_Footstep_Material", "Dirt", player.gameObject);
                        AkSoundEngine.SetSwitch("Player_Jump_Material", "Dirt", player.gameObject);
                        AkSoundEngine.SetSwitch("Player_Land_Material", "Dirt", player.gameObject);
                        AkSoundEngine.SetSwitch("Player_Slide_Material", "Dirt", player.gameObject);
                        //AkSoundEngine.SetSwitch("Guard_Footstep_Material", "Dirt", guard.gameObject);
                }

                if (materialType == MaterialTypes.Rubber)
                {
                        //Set Switch Group's active State to "Rubber"
                        AkSoundEngine.SetSwitch("Player_Footstep_Material", "Rubber", player.gameObject);
                        AkSoundEngine.SetSwitch("Player_Jump_Material", "Rubber", player.gameObject);
                        AkSoundEngine.SetSwitch("Player_Land_Material", "Rubber", player.gameObject);
                        AkSoundEngine.SetSwitch("Player_Slide_Material", "Rubber", player.gameObject);
                        //AkSoundEngine.SetSwitch("Guard_Footstep_Material", "Rubber", guard.gameObject);
                }

                if (materialType == MaterialTypes.Metal)
                {
                        //Set Switch Group's active State to "Metal"
                        AkSoundEngine.SetSwitch("Player_Footstep_Material", "Metal", player.gameObject);
                        AkSoundEngine.SetSwitch("Player_Jump_Material", "Metal", player.gameObject);
                        AkSoundEngine.SetSwitch("Player_Land_Material", "Metal", player.gameObject);
                        AkSoundEngine.SetSwitch("Player_Slide_Material", "Metal", player.gameObject);
                        //AkSoundEngine.SetSwitch("Guard_Footstep_Material", "Metal", guard.gameObject);
                }
            }
        }
    }
}
