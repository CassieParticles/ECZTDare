using UnityEngine;

public class CloakUnlock : MonoBehaviour
{
    private void UnlockCloak()
    {
        FindAnyObjectByType<MovementScript>().cloakUnlocked = true;
    }

    private void Awake()
    {
        GetComponent<ConsoleHackable>().AddConsoleListener(UnlockCloak);
    }
}
