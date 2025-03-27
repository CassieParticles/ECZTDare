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
    [SerializeField] private LockableDoor[] doors;
    private UIModeChange uiModeChange;
    [SerializeField] private bool isCollectable;

    private void Awake()
    {
        if(doors.Length==0 || isCollectable)
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
                    foreach(LockableDoor door in doors)
                    {
                        door.Lock();
                    }
                    break;
                case Action.Unlock:
                    foreach (LockableDoor door in doors)
                    {
                        door.Unlock();
                    }
                    break;
                case Action.Toggle:
                    foreach (LockableDoor door in doors)
                    {
                        door.ToggleState();
                    }
                    break;
            }
            if (isCollectable)
            {
                uiModeChange.CollectUpgrade();
            }
        }
    }
}
