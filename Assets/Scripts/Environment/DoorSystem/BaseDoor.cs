using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDoor : MonoBehaviour
{
    public abstract void Lock();
    public abstract void Unlock();
    public abstract void ToggleState();
}
