using UnityEngine;
using UnityEngine.Playables;


public class StartCutScene : MonoBehaviour
{
    [SerializeField]PlayableDirector director;

    private void Awake()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        director.Play();
    }
}
