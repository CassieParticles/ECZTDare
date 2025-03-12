using Unity.VisualScripting;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    private enum Action
    {
        Lock,
        Unlock,
        Toggle
    };

    [SerializeField] private Action action = Action.Unlock;
    [SerializeField] private LockableDoor door;

    private void Awake()
    {
        if(!door)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            switch (action)
            {
                case Action.Lock:
                    door.Lock();
                    break;
                case Action.Unlock:
                    door.Unlock();
                    break;
                case Action.Toggle:
                    door.ToggleState();
                    break;
            }

        }
    }
}
