using UnityEngine;

public class CloakUnlock : MonoBehaviour
{
    private void UnlockCloak()
    {
        FindAnyObjectByType<MovementScript>().cloakUnlocked = true;
        FindAnyObjectByType<BatteryBar>().SetCloakBar();
    }

    private void Awake()
    {
        GetComponent<ConsoleHackable>().AddConsoleListener(UnlockCloak);
    }
}
