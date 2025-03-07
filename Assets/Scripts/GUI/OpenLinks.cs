using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLinks : MonoBehaviour
{
  public void OpenItch()
    {
        Application.OpenURL("https://cosmic-coda.itch.io/");
    }

    public void OpenBlueSky()
    {
        Application.OpenURL("https://bsky.app/profile/cosmiccoda.bsky.social");
    }
}
