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
    private UIModeChange uiModeChange;
    [SerializeField] private bool isCollectable;

    private void Awake()
    {
        if(!door || isCollectable)
        {
            gameObject.SetActive(false);
        }
        uiModeChange = GameObject.Find("GameController").GetComponent<UIModeChange>();
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
            if (isCollectable)
            {
                uiModeChange.collectUpgrade();
            }
        }
    }
}
